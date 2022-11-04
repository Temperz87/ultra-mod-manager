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
        public static List<ModInfo> foundMods = new List<ModInfo>();
        public static List<ModInfo> allLoadedMods = new List<ModInfo>();
        public static bool outdated { get; internal set; } = false;
        public static string newLoaderVersion { get; internal set; } = "";
        private static bool initialized = false;
        private static Dictionary<ModInfo, GameObject> modObjects = new Dictionary<ModInfo, GameObject>();

        internal static void InitializeManager()
        {
            if (!initialized)
            {
                Plugin.logger.LogMessage("Beginning UltraModManager");
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
            Plugin.logger.LogInfo("Found " + foundMods.Count + " mods that can be loaded.");
        }

        private static void LoadOnStart()
        {
            int loadedMods = 0;
            foreach (ModInfo info in foundMods)
            {
                if (info.LoadOnStart)
                {
                    info.LoadThisMod();
                    loadedMods++;
                }
            }
            Plugin.logger.LogInfo("Loaded " + loadedMods + " mods on start");
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
                    ModInfo info;
                    if (type.IsSubclassOf(typeof(UKMod)))
                        info = new ModInfo(type, ModInfo.ModType.UKMod);
                    else if (type.IsSubclassOf(typeof(BaseUnityPlugin)))
                        info = new ModInfo(type, ModInfo.ModType.BepInPlugin);
                    else
                        continue;
                    Plugin.logger.LogInfo("Adding mod info " + fInfo.FullName + " " + type.Name);
                    foundMods.Add(info);
                    object retrievedData = UKAPI.SaveFileHandler.RetrieveModData("LoadOnStart", info.Name);
                    if (retrievedData != null && bool.Parse(retrievedData.ToString()))
                        info.LoadOnStart = true;
                }
            }
            catch (Exception e)
            {
                Plugin.logger.LogError("Caught exception while trying to load assembly " + fInfo.FullName + ": " + e.ToString());
                return;
            }
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

        internal static ModMetaData GetUKMetaData(Type t)
        {
            object[] customAttributes = t.GetCustomAttributes(typeof(ModMetaData), true);
            if (customAttributes.Length == 0)
            {
                throw new Exception("Could not find the metadata (UKPlugin) to UKMod " + t.FullName);
            }
            return (ModMetaData)customAttributes[0];
        }

        public static void LoadMod(ModInfo info)
        {
            GameObject modObject = GameObject.Instantiate(new GameObject());
            UKMod newMod = null;
            try
            {
                Plugin.logger.LogInfo("Trying to load mod " + info.Name);
                if (info.MainClass.IsSubclassOf(typeof(BaseUnityPlugin)))
                {
                    GameObject.DontDestroyOnLoad(modObject);
                    modObject.SetActive(false);
                    modObject.AddComponent(info.MainClass);
                    allLoadedMods.Add(info);
                    modObject.SetActive(true);
                    Plugin.logger.LogInfo("Loaded BepInExPlugin " + info.Name);
                    return;
                }
                if (!info.MainClass.IsSubclassOf(typeof(UKMod)))
                    throw new ArgumentException("LoadMod was called using a type that did not inherit from UKMod or BaseUnityPlugin, type name is " + info.MainClass.Name);
                GameObject.DontDestroyOnLoad(modObject);
                modObject.SetActive(false);
                newMod = modObject.AddComponent(info.MainClass) as UKMod;
                allLoadedMods.Add(info);
                modObjects.Add(info, modObject);
                ModMetaData metaData = UltraModManager.GetUKMetaData(info.MainClass);
                if (!metaData.AllowCybergrindSubmission)
                    UKAPI.DisableCyberGrindSubmission(info.Name);
                modObject.SetActive(true);
                newMod.OnModLoaded();
                Plugin.logger.LogInfo("Loaded UKMod " + info.Name);
            }
            catch (Exception e)
            {
                Plugin.logger.LogError("Caught exception while trying to load modinformation " + info.Name + ": " + e.ToString());
                if (modObject != null)
                {
                    if (newMod != null)
                        newMod.OnModUnload();
                    GameObject.Destroy(modObject); // I don't know if this is a good thing to do, if not please scream at me to remove it
                }
            }
        }

        public static void UnloadMod(ModInfo info)
        {
            if (modObjects.ContainsKey(info) && info.CanBeUnloaded)
            {
                Plugin.logger.LogInfo("Trying to unload mod " + info.Name);
                GameObject modObject = modObjects[info];
                UKMod mod = modObject.GetComponent<UKMod>();
                mod.OnModUnloaded.Invoke();
                mod.OnModUnload();
                modObjects.Remove(info);
                allLoadedMods.Remove(info);
                GameObject.Destroy(modObject);
                if (!UltraModManager.GetUKMetaData(info.MainClass).AllowCybergrindSubmission)
                    UKAPI.RemoveDisableCyberGrindReason(info.Name);
                Plugin.logger.LogInfo("Successfully unloaded mod " + info.Name);
            }
        }
    }
}