using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UMM.HarmonyPatches;
using UnityEngine;
using UnityEngine.Networking;
using static UMM.ModInformation;

namespace UMM.Loader
{
    public static class UltraModManager
    {
        public static DirectoryInfo modsDirectory = new DirectoryInfo(Path.Combine(BepInEx.Paths.BepInExRootPath, "UMM Mods"));
        public static Dictionary<string, ModInformation> foundMods = new Dictionary<string, ModInformation>();
        public static Dictionary<string, ModInformation> allLoadedMods = new Dictionary<string, ModInformation>();
        public static bool outdated { get; internal set; } = false;
        public static bool devBuild { get; internal set; } = false;
        public static string newLoaderVersion { get; internal set; } = "";

        private static bool initialized = false;
        private static Dictionary<ModInformation, GameObject> modObjects = new Dictionary<ModInformation, GameObject>();
        private static Dictionary<string, ModProfile> allProfiles = new Dictionary<string, ModProfile>();
        private static ModProfile currentProfile = null;

        internal static void InitializeManager()
        {
            if (!initialized)
            {
                Plugin.logger.LogMessage("Beginning UltraModManager");
                initialized = true;
                LoadMods();
                LoadOnStart();
            }
        }

        private static void LoadMods()
        {
            if (modsDirectory.Exists)
            {
                Plugin.logger.LogMessage("Collecting Assemblies");
                Dictionary<Assembly, FileInfo> allAssemblies = new Dictionary<Assembly, FileInfo>();
                foreach (FileInfo info in modsDirectory.GetFiles("*.dll", SearchOption.AllDirectories))
                {
                    try
                    {
                        Assembly ass = LoadAssembly(info);
                        if (ass != null)
                        {
                            allAssemblies.Add(ass, info);
                        }
                    }
                    catch (TypeLoadException e)
                    {
                        Plugin.logger.LogWarning("Couldn't load " + info.FullName + " possibly due to a missing assembly, will try to reload later:\n" + e.ToString());
                    }
                }

                Plugin.logger.LogMessage("Getting mod types ");
                foreach (Assembly ass in allAssemblies.Keys)
                    GetTypesFromAssembly(ass, allAssemblies[ass]);
            }
            else
                modsDirectory.Create();

            Plugin.logger.LogInfo("Found " + foundMods.Count + " mods that can be loaded.");
        }

        private static void LoadOnStart()
        {
            int loadedMods = 0;
            foreach (ModInformation info in foundMods.Values)
            {
                if (info.loadOnStart)
                {
                    info.LoadThisMod();
                    if (info.loaded)
                        loadedMods++;
                }
            }
            if (loadedMods > 0)
                Plugin.logger.LogInfo("Loaded " + loadedMods + " mods on start");
        }

        /// <summary>
        /// Tries to load an assembly
        /// </summary>
        /// <param name="fInfo"></param>
        /// <exception cref="TypeLoadException"></exception>
        internal static Assembly LoadAssembly(FileInfo fInfo)
        {
            DirectoryInfo dInfo = new DirectoryInfo(fInfo.DirectoryName + Path.DirectorySeparatorChar + "dependencies");
            if (dInfo.Exists) // this solution is a hack i am well aware
            {
                foreach (FileInfo info in dInfo.GetFiles("*.dll", SearchOption.AllDirectories))
                    Assembly.LoadFrom(info.FullName);
            }


            Assembly ass = null;
            Plugin.logger.LogInfo("Trying to load assembly " + fInfo.FullName);
            try
            {
                ass = Assembly.LoadFrom(fInfo.FullName);
            }
            catch (FileNotFoundException e)
            {
                Plugin.logger.LogWarning("Couldn't find file " + fInfo.FullName);
                return null;
            }
            catch (FileLoadException e)
            {
                throw e;
            }

            return ass;
        }

        /// <summary>
        /// Tries to load all mods from an assembly
        /// </summary>
        /// <param name="ass">Assembly containing mod files</param>
        /// <param name="fInfo">Assembly path</param>
        /// <exception cref="TypeLoadException"></exception>
        internal static void GetTypesFromAssembly(Assembly ass, FileInfo fInfo)
        {
            Type[] assemblyTypes = null;

            try
            {
                assemblyTypes = ass.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                Plugin.logger.LogWarning("Caught ReflectionTypeLoadException, so this mod may break");
                assemblyTypes = e.Types.Where(t => t != null).ToArray();
            }
            catch (Exception e)
            {
                Plugin.logger.LogWarning("Unhandled exception while loading " + fInfo.FullName + "\n" + e.ToString());
                return;
            }

            foreach (Type type in assemblyTypes)
            {
                ModInformation info;
                try
                {
                    if (type.IsSubclassOf(typeof(UKMod)))
                        info = new ModInformation(type, ModInformation.ModType.UKMod, fInfo.DirectoryName);
                    else if (type.IsSubclassOf(typeof(BaseUnityPlugin)))
                        info = new ModInformation(type, ModInformation.ModType.BepInPlugin, fInfo.DirectoryName);
                    else
                        continue;
                }
                catch (FileNotFoundException e)
                {
                    throw new TypeLoadException(e.Message);
                }
                FileInfo iconInfo = new FileInfo(Path.Combine(fInfo.DirectoryName + Path.DirectorySeparatorChar + "icon.png"));
                if (iconInfo.Exists)
                    Plugin.instance.StartCoroutine(GetModImage(iconInfo, info));
                foundMods.Add(info.GUID, info);
                object retrievedData = UKAPI.SaveFileHandler.RetrieveModData("LoadOnStart", info.modName);
                if (retrievedData != null && bool.Parse(retrievedData.ToString()))
                    info.loadOnStart = true;
            }
        }
        internal static IEnumerator GetModImage(FileInfo imageURL, ModInformation info)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL.FullName))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError)
                {
                    Plugin.logger.LogError("Couldn't load preview image " + imageURL + " for mod " + info.modName);
                    Plugin.logger.LogError(www.error);
                }
                else
                {
                    info.previewIcon = DownloadHandlerTexture.GetContent(www);
                    Plugin.logger.LogInfo("Loaded preview image for mod " + info.modName);
                }
            }
            yield break;
        }

        internal static BepInPlugin GetBepinMetaData(Type t)
        {
            object[] customAttributes = t.GetCustomAttributes(typeof(BepInPlugin), true);
            if (customAttributes.Length == 0)
            {
                throw new Exception("Could not find the metadata (BepInPlugin) to BaseUnityPlugin " + t.FullName);
            }
            return (BepInPlugin)customAttributes[0];
        }

        internal static UKPlugin GetUKMetaData(Type t)
        {
            object[] customAttributes = t.GetCustomAttributes(typeof(UKPlugin), true);
            if (customAttributes.Length == 0)
            {
                throw new Exception("Could not find the metadata (UKPlugin) to UKMod " + t.FullName);
            }
            return (UKPlugin)customAttributes[0];
        }

        internal static List<Dependency> GetBepinDependencies(Type t)
        {
            object[] customAttributes = t.GetCustomAttributes(typeof(BepInDependency), true);
            List<Dependency> dependencies = new List<Dependency>();
            foreach (BepInDependency attribute in customAttributes)
            {
                dependencies.Add(new Dependency() { GUID = attribute.DependencyGUID, MinimumVersion = attribute.MinimumVersion });
            }
            if (dependencies.Count > 0)
                Plugin.logger.LogInfo("Found " + dependencies.Count + " BepinDependencies");
            return dependencies;
        }

        internal static List<Dependency> GetUKModDependencies(Type t)
        {
            UKDependency[] customAttributes = (UKDependency[])t.GetCustomAttributes(typeof(UKDependency), true);
            List<Dependency> dependencies = new List<Dependency>();
            foreach (UKDependency attribute in customAttributes)
            {
                dependencies.Add(new Dependency() { GUID = attribute.GUID, MinimumVersion = attribute.MinimumVersion });
            }
            if (dependencies.Count > 0)
                Plugin.logger.LogInfo("Found " + dependencies.Count + " UKModDependencies");
            return dependencies;
        }

        public static void LoadMod(ModInformation info)
        {
            if (allLoadedMods.ContainsKey(info.GUID))
                return;
            if (info.dependencies != null && info.dependencies.Count > 0)
            {
                Plugin.logger.LogInfo("Found dependencies for mod " + info.modName + ", trying to to load them now");
                foreach (Dependency dependency in info.dependencies)
                {
                    if (foundMods.ContainsKey(dependency.GUID))
                    {
                        if (foundMods[dependency.GUID].modVersion >= dependency.MinimumVersion)
                        {
                            if (!foundMods[dependency.GUID].LoadThisMod())
                            {
                                Plugin.logger.LogError("Couldn't load dependency " + dependency.GUID + " for mod " + info.modName);
                                return;
                            }
                            Inject_ModsButton.ReportModStateChanged(foundMods[dependency.GUID]);
                        }
                        else
                        {
                            try
                            {
                                info.UnLoadThisMod();
                            }
                            finally
                            {
                                Plugin.logger.LogWarning($"Required dependency ({foundMods[dependency.GUID].modName}, version {foundMods[dependency.GUID].modVersion}) did not meet version requirements of {info.modName} (minimum version {dependency.MinimumVersion})");
                            }
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            info.UnLoadThisMod();
                        }
                        finally
                        {
                            Plugin.logger.LogWarning($"Required dependency ({dependency.GUID}) of {info.modName} not found.");
                        }
                        return;
                    }
                }
            }
            GameObject modObject = GameObject.Instantiate(new GameObject());
            UKMod newMod = null;
            try
            {
                Plugin.logger.LogMessage("Trying to load mod " + info.modName);
                if (info.mod.IsSubclassOf(typeof(BaseUnityPlugin)))
                {
                    GameObject.DontDestroyOnLoad(modObject);
                    modObject.SetActive(false);
                    modObject.AddComponent(info.mod);
                    allLoadedMods.Add(info.GUID, info);
                    modObject.SetActive(true);
                    Plugin.logger.LogMessage("Loaded BepInExPlugin " + info.modName);
                    return;
                }
                if (!info.mod.IsSubclassOf(typeof(UKMod)))
                    throw new ArgumentException("LoadMod was called using a type that did not inherit from UKMod or BaseUnityPlugin, type name is " + info.mod.Name);
                GameObject.DontDestroyOnLoad(modObject);
                modObject.SetActive(false);
                newMod = modObject.AddComponent(info.mod) as UKMod;
                allLoadedMods.Add(info.GUID, info);
                modObjects.Add(info, modObject);
                UKPlugin metaData = UltraModManager.GetUKMetaData(info.mod);
                if (!metaData.allowCyberGrindSubmission)
                    UKAPI.DisableCyberGrindSubmission(info.modName);
                modObject.SetActive(true);
                newMod.OnModLoaded();
                Plugin.logger.LogMessage("Loaded UKMod " + info.modName);
            }
            catch (Exception e)
            {
                Plugin.logger.LogError("Caught exception while trying to load modinformation " + info.modName);
                Plugin.logger.LogError(e);
                if (allLoadedMods.ContainsKey(info.GUID))
                    allLoadedMods.Remove(info.GUID);
                info.ForceLoadState(false);
                Inject_ModsButton.ReportModStateChanged(info);
                if (modObject != null)
                {
                    if (newMod != null && newMod.metaData.unloadingSupported)
                    {
                        try
                        {
                            newMod.OnModUnload();
                        }
                        catch (Exception)
                        {
                            // Lovely it threw an exception twice :P
                        }
                    }
                    GameObject.Destroy(modObject); // I don't know if this is a good thing to do, if not please scream at me to remove it
                }
            }
        }

        public static void UnloadMod(ModInformation info)
        {
            if (modObjects.ContainsKey(info) && info.supportsUnloading)
            {
                Plugin.logger.LogInfo("Trying to unload mod " + info.modName);
                GameObject modObject = modObjects[info];
                UKMod mod = modObject.GetComponent<UKMod>();
                mod.OnModUnloaded.Invoke();
                mod.OnModUnload();
                modObjects.Remove(info);
                allLoadedMods.Remove(info.GUID);
                GameObject.Destroy(modObject);
                if (!UltraModManager.GetUKMetaData(info.mod).allowCyberGrindSubmission)
                    UKAPI.RemoveDisableCyberGrindReason(info.modName);
                Plugin.logger.LogInfo("Successfully unloaded mod " + info.modName);
            }
        }

        public static void LoadModProfiles()
        {
            string modProfiles = UKAPI.SaveFileHandler.RetrieveModData("ModProfiles", "UMM");
            while (modProfiles.IndexOf(";;") != -1)
            {
                ModProfile newProfile = new ModProfile(modProfiles.Substring(0, modProfiles.IndexOf(";;")));
                allProfiles.Add(newProfile.name, newProfile);
                modProfiles = modProfiles.Substring(modProfiles.IndexOf(";;") + 2);
            }
            string currentProfileRetrieved = UKAPI.SaveFileHandler.RetrieveModData("CurrentModProfile", "UMM");
            if (allProfiles.ContainsKey(currentProfileRetrieved))
                currentProfile = allProfiles[currentProfileRetrieved];
        }

        public static void DumpModProfiles()
        {
            string modProfiles = "";
            foreach (ModProfile profile in allProfiles.Values)
            {
                modProfiles += profile?.ToString();
            }
            UKAPI.SaveFileHandler.SetModData("UMM", "ModProfiles", modProfiles);
            if (currentProfile != null)
                UKAPI.SaveFileHandler.SetModData("UMM", "CurrentModProfile", currentProfile.name);
        }
    }
}
