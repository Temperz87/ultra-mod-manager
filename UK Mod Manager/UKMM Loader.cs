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
        private static bool initialized = false;

        internal static void InitializeManager()
        {
            if (!initialized)
            {
                Debug.Log("Beginning UKModManager");
                initialized = true;
                CollectAssemblies();
            }
        }

        private static void CollectAssemblies()
        {
            foreach (FileInfo info in new DirectoryInfo(Environment.CurrentDirectory + @"\BepInEx\UKMM Mods\").GetFiles("*.dll", SearchOption.AllDirectories))
                LoadFromAssembly(info);
        }

        public static void LoadFromAssembly(FileInfo fInfo)
        {
            Assembly ass = Assembly.LoadFile(fInfo.FullName);
            foreach (Type type in ass.GetTypes())
            {
                if (type.IsSubclassOf(typeof(UKMod)))
                {
                    foundMods.Add(new ModInformation(type, ModInformation.ModType.UKMod));
                }
                else if (type.IsSubclassOf(typeof(BaseUnityPlugin)))
                {
                    foundMods.Add(new ModInformation(type, ModInformation.ModType.BepInPlugin));
                }
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

        public static void LoadMod(Type mod)
        {
            if (mod.IsSubclassOf(typeof(BaseUnityPlugin)))
            {
                GameObject newBepinModObject = GameObject.Instantiate(new GameObject());
                GameObject.DontDestroyOnLoad(newBepinModObject);
                newBepinModObject.SetActive(false);
                newBepinModObject.AddComponent(mod);
                allLoadedMods.Add(new ModInformation(mod, ModInformation.ModType.BepInPlugin));
                newBepinModObject.SetActive(true);
                return;
            }
            if (!mod.IsSubclassOf(typeof(UKMod)))
                throw new ArgumentException("LoadMod(Type mod) was called using a type that did not inherit from UKMod or BaseUnityPlugin, type name is " + mod.Name);
            GameObject newModObject = GameObject.Instantiate(new GameObject());
            GameObject.DontDestroyOnLoad(newModObject);
            newModObject.SetActive(false);
            UKMod newMod = newModObject.AddComponent(mod) as UKMod;
            allLoadedMods.Add(new ModInformation(mod, ModInformation.ModType.UKMod));
            newModObject.SetActive(true);
            newMod.OnModLoaded();
        }


        public static ModInformation[] GetLoadedMods()
        {
            return allLoadedMods.ToArray().Clone() as ModInformation[];
        }
    }
}