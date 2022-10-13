using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace UMM.Loader
{
    public static class UltraModManager
    {
        public static List<ModInformation> foundMods = new List<ModInformation>();
        public static List<ModInformation> allLoadedMods = new List<ModInformation>();
        public static bool outdated { get; internal set; } = false;
        public static string newLoaderVersion { get; internal set; } = "";
        private static bool initialized = false;
        private static Dictionary<ModInformation, GameObject> modObjects = new Dictionary<ModInformation, GameObject>();

        internal static void InitializeManager()
        {
            if (!initialized)
            {
                Debug.Log("Beginning UltraModManager");
                initialized = true;
                CollectAssemblies();
                LoadOnStart();
            }
        }

        private static void CollectAssemblies()
        {
            DirectoryInfo modsDirectory = new DirectoryInfo(Environment.CurrentDirectory + @"\BepInEx\UMM Mods\");
            if (modsDirectory.Exists)
                foreach (FileInfo info in modsDirectory.GetFiles("*.dll", SearchOption.AllDirectories))
                    LoadFromAssembly(info);
            else
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\BepInEx\UMM Mods\");
            Debug.Log("Found " + foundMods.Count + " mods that can be loaded.");
        }

        private static void LoadOnStart()
        {
            int loadedMods = 0;
            foreach (ModInformation info in foundMods)
            {
                if (info.loadOnStart)
                {
                    info.LoadThisMod();
                    loadedMods++;
                }
            }
            Debug.Log("Loaded " + loadedMods + " mods on start");
        }

        public static void LoadFromAssembly(FileInfo fInfo)
        {
            DirectoryInfo dInfo = new DirectoryInfo(fInfo.DirectoryName + "\\dependencies");
            if (dInfo.Exists) // this solution is a hack i am well aware
            {
                foreach (FileInfo info in dInfo.GetFiles("*.dll", SearchOption.AllDirectories))
                    Assembly.LoadFile(info.FullName);
            }
            try
            {
                Assembly ass = Assembly.LoadFile(fInfo.FullName);
                foreach (Type type in ass.GetTypes())
                {
                    ModInformation info;
                    if (type.IsSubclassOf(typeof(UKMod)))
                        info = new ModInformation(type, ModInformation.ModType.UKMod);
                    else if (type.IsSubclassOf(typeof(BaseUnityPlugin)))
                        info = new ModInformation(type, ModInformation.ModType.BepInPlugin);
                    else
                        continue;
                    Debug.Log("Adding mod info " + fInfo.FullName + " " + type.Name);
                    foundMods.Add(info);
                    object retrievedData = UKAPI.SaveFileHandler.RetrieveModData(info.modName, "LoadOnStart");
                    if (retrievedData != null && bool.Parse(retrievedData.ToString()))
                        info.loadOnStart = true;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Caught exception while trying to load assembly " + fInfo.FullName);
                Debug.Log(e.ToString());
                return;
            }
        }

        internal static BepInPlugin GetBepinMetaData(Type t)
        {
            object[] customAttributes = t.GetCustomAttributes(typeof(BepInPlugin), true);
            if (customAttributes.Length == 0)
            {
                throw new NullReferenceException("Could not find the metadata (BepInPlugin) to BaseUnityPlugin " + t.FullName);
            }
            return (BepInPlugin)customAttributes[0];
        }

        internal static UKPlugin GetUKMetaData(Type t)
        {
            object[] customAttributes = t.GetCustomAttributes(typeof(UKPlugin), true);
            if (customAttributes.Length == 0)
            {
                throw new NullReferenceException("Could not find the metadata (UKPlugin) to UKMod " + t.FullName);
            }
            return (UKPlugin)customAttributes[0];
        }

        public static void LoadMod(ModInformation info)
        {
            GameObject modObject = GameObject.Instantiate(new GameObject());
            UKMod newMod = null;
            try
            {
                Debug.Log("Trying to load mod " + info.modName);
                if (info.mod.IsSubclassOf(typeof(BaseUnityPlugin)))
                {
                    GameObject.DontDestroyOnLoad(modObject);
                    modObject.SetActive(false);
                    modObject.AddComponent(info.mod);
                    allLoadedMods.Add(info);
                    modObject.SetActive(true);
                    Debug.Log("Loaded BepInExPlugin " + info.modName);
                    return;
                }
                if (!info.mod.IsSubclassOf(typeof(UKMod)))
                    throw new ArgumentException("LoadMod(Type mod) was called using a type that did not inherit from UKMod or BaseUnityPlugin, type name is " + info.mod.Name);
                GameObject.DontDestroyOnLoad(modObject);
                modObject.SetActive(false);
                newMod = modObject.AddComponent(info.mod) as UKMod;
                allLoadedMods.Add(info);
                modObjects.Add(info, modObject);
                UKPlugin metaData = UltraModManager.GetUKMetaData(info.mod);
                if (!metaData.allowCyberGrindSubmission)
                    UKAPI.DisableCyberGrindSubmission(info.modName);
                modObject.SetActive(true);
                newMod.OnModLoaded();
                Debug.Log("Loaded UKMod " + info.modName);
            }
            catch (Exception e)
            {
                Debug.LogError("Caught exception while trying to load modinformation " + info.modName);
                Debug.LogException(e);
                if (modObject != null)
                {
                    if (newMod != null)
                        newMod.OnModUnload();
                    GameObject.Destroy(modObject); // I don't know if this is a good thing to do, if not please scream at me to remove it
                }
            }
        }

        public static void UnloadMod(ModInformation info)
        {
            if (modObjects.ContainsKey(info) && info.supportsUnloading)
            {
                Debug.Log("trying to unload mod " + info.modName);
                GameObject modObject = modObjects[info];
                UKMod mod = modObject.GetComponent<UKMod>();
                mod.OnModUnloaded.Invoke();
                mod.OnModUnload();
                modObjects.Remove(info);
                allLoadedMods.Remove(info);
                GameObject.Destroy(modObject);
                if (!UltraModManager.GetUKMetaData(info.mod).allowCyberGrindSubmission)
                    UKAPI.RemoveDisableCyberGrindReason(info.modName);
                Debug.Log("successfully unloaded mod " + info.modName);
            }
        }
    }
}