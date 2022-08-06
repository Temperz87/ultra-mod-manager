using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using BepInEx;

namespace UKMM.Loader
{
    public static class UKModManager
    {
        public static List<ModInformation> foundMods = new List<ModInformation>();
        public static List<ModInformation> allLoadedMods = new List<ModInformation>();
        internal static bool AllowCyberGrindSubmission = true;
        private static bool initialized = false;
        private static Dictionary<ModInformation, GameObject> modObjects = new Dictionary<ModInformation, GameObject>();

        internal static void InitializeManager()
        {
            if (!initialized)
            {
                Debug.Log("Beginning UKModManager");
                initialized = true;
                CollectAssemblies();
                LoadOnStart();
            }
        }

        private static void CollectAssemblies()
        {
            DirectoryInfo modsDirectory = new DirectoryInfo(Environment.CurrentDirectory + @"\BepInEx\UKMM Mods\");
            if (modsDirectory.Exists)
                foreach (FileInfo info in modsDirectory.GetFiles("*.dll", SearchOption.AllDirectories))
                    LoadFromAssembly(info);
            else
                Directory.CreateDirectory(Environment.CurrentDirectory + @"\BepInEx\UKMM Mods\");
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

        /* For whatever reason, this function does not work
        internal static BepInPlugin GetBepinMetaData(Type t)
        {
            object[] customAttributes = t.GetCustomAttributes(typeof(BaseUnityPlugin), true);
            if (customAttributes.Length == 0)
            {
                throw new NullReferenceException("Could not find the metadata (BepInPlugin) to BaseUnityPlugin " + t.FullName);
            }
            return (BepInPlugin)customAttributes[0];
        }
        */

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
                    Debug.Log("Loaded mod " + info.modName);
                    return;
                }
                if (!info.mod.IsSubclassOf(typeof(UKMod)))
                    throw new ArgumentException("LoadMod(Type mod) was called using a type that did not inherit from UKMod or BaseUnityPlugin, type name is " + info.mod.Name);
                GameObject.DontDestroyOnLoad(modObject);
                modObject.SetActive(false);
                newMod = modObject.AddComponent(info.mod) as UKMod;
                allLoadedMods.Add(info);
                modObjects.Add(info, modObject);
                UKPlugin metaData = UKModManager.GetUKMetaData(info.mod);
                if (!metaData.allowCyberGrindSubmission)
                    AllowCyberGrindSubmission = false;
                modObject.SetActive(true);
                newMod.OnModLoaded();
                Debug.Log("Loaded mod " + info.modName);
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

        public static void UnloadMod(ModInformation information)
        {
            if (modObjects.ContainsKey(information) && information.supportsUnloading)
            {
                Debug.Log("trying to unload mod " + information.modName + " and unloading supported is " + information.supportsUnloading);
                GameObject modObject = modObjects[information];
                UKMod mod = modObject.GetComponent<UKMod>();
                mod.OnModUnloaded.Invoke();
                mod.OnModUnload();
                modObjects.Remove(information);
                allLoadedMods.Remove(information);
                GameObject.Destroy(modObject);
            }
        }

        public static ModInformation[] GetLoadedMods()
        {
            return allLoadedMods.ToArray().Clone() as ModInformation[];
        }
    }
}