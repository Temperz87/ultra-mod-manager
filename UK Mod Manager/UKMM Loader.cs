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
            foreach (FileInfo info in new DirectoryInfo(Environment.CurrentDirectory + @"\BepInEx\UKMM Mods\").GetFiles("*.dll", SearchOption.AllDirectories))
                LoadFromAssembly(info);
        }

        private static void LoadOnStart()
        {
            foreach (ModInformation info in foundMods)
                if (info.loadOnStart)
                    info.LoadThisMod();
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
                Debug.Log("Adding info " + fInfo.FullName + " " + type.Name);
                foundMods.Add(info);   
                object retrievedData = UKAPI.SaveFileHandler.RetrieveModData(info.modName, "LoadOnStart");
                if (retrievedData != null && bool.Parse(retrievedData.ToString()))
                    info.loadOnStart = true;
            }
        }

        internal static BepInPlugin GetBepinMetaData(Type t)
        {
            object[] customAttributes = t.GetCustomAttributes(typeof(BaseUnityPlugin), true);
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
            if (info.mod.IsSubclassOf(typeof(BaseUnityPlugin)))
            {
                GameObject newBepinModObject = GameObject.Instantiate(new GameObject());
                GameObject.DontDestroyOnLoad(newBepinModObject);
                newBepinModObject.SetActive(false);
                newBepinModObject.AddComponent(info.mod);
                allLoadedMods.Add(info);
                //modObjects.Add(bInfo, newBepinModObject);
                newBepinModObject.SetActive(true);
                return;
            }
            if (!info.mod.IsSubclassOf(typeof(UKMod)))
                throw new ArgumentException("LoadMod(Type mod) was called using a type that did not inherit from UKMod or BaseUnityPlugin, type name is " + info.mod.Name);
            GameObject newModObject = GameObject.Instantiate(new GameObject());
            GameObject.DontDestroyOnLoad(newModObject);
            newModObject.SetActive(false);
            UKMod newMod = newModObject.AddComponent(info.mod) as UKMod;
            allLoadedMods.Add(info);
            modObjects.Add(info, newModObject);
            UKPlugin metaData = UKModManager.GetUKMetaData(info.mod);
            if (!metaData.allowCyberGrindSubmission)
                AllowCyberGrindSubmission = false;
            newModObject.SetActive(true);
            newMod.OnModLoaded();
        }

        public static void UnloadMod(ModInformation information)
        {
            Debug.Log("information is " + information.modName + " and unloading supported is " + information.supportsUnloading);
            if (modObjects.ContainsKey(information) && information.supportsUnloading)
            {
                GameObject modObject = modObjects[information];
                UKMod mod = modObject.GetComponent<UKMod>();
                mod.OnModUnloaded.Invoke();
                mod.OnModUnload();
                modObjects.Remove(information);
                allLoadedMods.Remove(information);
                GameObject.Destroy(modObject);

                UKAPI.EnableCyberGrindSubmission(); // This call will only enbale cyber grind submissions if no other loaded mods do
            }
        }

        public static ModInformation[] GetLoadedMods()
        {
            return allLoadedMods.ToArray().Clone() as ModInformation[];
        }
    }
}