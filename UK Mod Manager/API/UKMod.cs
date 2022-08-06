using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace UKMM
{
    public abstract class UKMod : MonoBehaviour
    {
        public UnityEvent OnModUnloaded = new UnityEvent();
        public UKPlugin metaData;
        // maybe include logo???

        public UKMod()
        {
            Type type = this.GetType();
            Debug.Log("Mod found, type is " + type.Name);
            object[] customAttributes = type.GetCustomAttributes(typeof(UKPlugin), false);
            if (customAttributes.Length == 0)
            {
                throw new NullReferenceException("Could not find the metadata (UKPlugin) to UKMod " + type.Name);
            }
            metaData = (UKPlugin)customAttributes[0];
        }

        /// <summary>
        /// Runs once when the mod gets loaded
        /// </summary>
        public virtual void OnModLoaded() { }

        /// <summary>
        /// Runs once when the mod gets unloaded
        /// </summary>
        public virtual void OnModUnload() { }

        /// <summary>
        /// Sets persistent mod data to a save file
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="value">Value to Set as a string</param>
        public void SetPersistentModData(string key, string value)
        {
            UKAPI.SaveFileHandler.SetModData(metaData.name, key, value);
        }

        /// <summary>
        /// Sets persistent mod data to a specific mod to a save file
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="value">Value to Set as a string</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static void SetPersistentModData(string key, string value, string modName)
        {
            UKAPI.SaveFileHandler.SetModData(modName, key, value);
        }

        /// <summary>
        /// Sets persistent mod data shared across all mods to a save file
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="value">Value to Set as a string</param>
        public static void SetPersistentUniversalModData(string key, string value)
        {
            UKAPI.SaveFileHandler.SetModData("allPersistentModData", key, value);
        }

        /// <summary>
        /// Gets persistent mod data from a save file as a string
        /// </summary>
        /// <param name="key">Name of value</param>
        public string RetrieveStringPersistentModData(string key)
        {
            return UKAPI.SaveFileHandler.RetrieveModData(metaData.name, key);
        }

        /// <summary>
        /// Gets persistent mod data from a save file as an int, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        public int RetrieveIntPersistentModData(string key)
        {
            return int.Parse(RetrieveStringPersistentModData(metaData.name, key));
        }

        /// <summary>
        /// Gets persistent mod data from a save file as a boolean, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        public bool RetrieveBooleanPersistentModData(string key)
        {
            return bool.Parse(RetrieveStringPersistentModData(metaData.name, key));
        }

        /// <summary>
        /// Gets persistent mod data from a save file as a float, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        public float RetrieveFloatPersistentModData(string key)
        {
            return float.Parse(RetrieveStringPersistentModData(metaData.name, key));
        }

        /// <summary>
        /// Gets persistent mod data from a specific mod from a save file as a string
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static string RetrieveStringPersistentModData(string key, string modName)
        {
            return UKAPI.SaveFileHandler.RetrieveModData(modName, key);
        }

        /// <summary>
        /// Gets persistent mod data from a specific mod from a save file as an int, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static int RetrieveIntPersistentModData(string key, string modName)
        {
            return int.Parse(RetrieveStringPersistentModData(modName, key));
        }

        /// <summary>
        /// Gets persistent mod data from a specific mod from a save file as a boolean, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static bool RetrieveBooleanPersistentModData(string key, string modName)
        {
            return bool.Parse(RetrieveStringPersistentModData(modName, key));
        }

        /// <summary>
        /// Gets persistent mod data from a specific mod from a save file as a float, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static float RetrieveFloatPersistentModData(string key, string modName)
        {
            return float.Parse(RetrieveStringPersistentModData(modName, key));
        }

        /// <summary>
        /// Gets persistent mod data shared across all mods from a save file
        /// </summary>
        /// <param name="key">Name of value</param>
        public static string RetrieveStringPersistentUniversalModData(string key)
        {
            return UKAPI.SaveFileHandler.RetrieveModData("allPersistentModData", key);
        }

        /// <summary>
        /// Gets persistent mod data from a specific mod from a save file as an int, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static int RetrieveIntPersistentUniversalModData(string key)
        {
            return int.Parse(RetrieveStringPersistentUniversalModData(key));
        }

        /// <summary>
        /// Gets persistent mod data from a specific mod from a save file as a boolean, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static bool RetrieveBooleanPersistentUniversalModData(string key)
        {
            return bool.Parse(RetrieveStringPersistentUniversalModData(key));
        }

        /// <summary>
        /// Gets persistent mod data from a specific mod from a save file as a float, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static float RetrieveFloatPersistentUniversalModData(string key)
        {
            return float.Parse(RetrieveStringPersistentUniversalModData(key));
        }
    }
}