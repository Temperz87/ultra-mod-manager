using System;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace UMM
{
    public abstract class UKMod : MonoBehaviour
    {
        public event Action ModDisabled;
        public string ModDirectoryPath { get; internal set; }
        public ModInfo Info { get; internal set; }
        // maybe include logo???

        /// <summary>
        /// Runs once when the mod gets loaded
        /// </summary>
        protected internal virtual void OnModEnabled() { 
            
        }

        /// <summary>
        /// Runs once when the mod gets unloaded
        /// </summary>
        protected internal virtual void OnModDisabled() {
            ModDisabled?.Invoke();
        }

        /// <summary>
        /// Sets persistent mod data to a save file
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="value">Value to Set as a string</param>
        public void SetPersistentModData(string key, string value)
        {
            UKAPI.SaveFileHandler.SetModData(Info.Metadata.Name, key, value);
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
            return UKAPI.SaveFileHandler.RetrieveModData(key, Info.Metadata.Name);
        }

        /// <summary>
        /// Gets persistent mod data from a save file as an int, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        public int RetrieveIntPersistentModData(string key)
        {
            return int.Parse(RetrieveStringPersistentModData(key, Info.Metadata.Name));
        }

        /// <summary>
        /// Gets persistent mod data from a save file as a boolean, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        public bool RetrieveBooleanPersistentModData(string key)
        {
            return bool.Parse(RetrieveStringPersistentModData(key, Info.Metadata.Name));
        }

        /// <summary>
        /// Gets persistent mod data from a save file as a float, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        public float RetrieveFloatPersistentModData(string key)
        {
            return float.Parse(RetrieveStringPersistentModData(key, Info.Metadata.Name));
        }

        /// <summary>
        /// Gets persistent mod data from a specific mod from a save file as a string
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static string RetrieveStringPersistentModData(string key, string modName)
        {
            return UKAPI.SaveFileHandler.RetrieveModData(key, modName);
        }

        /// <summary>
        /// Gets persistent mod data from a specific mod from a save file as an int, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static int RetrieveIntPersistentModData(string key, string modName)
        {
            return int.Parse(RetrieveStringPersistentModData(key, modName));
        }

        /// <summary>
        /// Gets persistent mod data from a specific mod from a save file as a boolean, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static bool RetrieveBooleanPersistentModData(string key, string modName)
        {
            return bool.Parse(RetrieveStringPersistentModData(key, modName));
        }

        /// <summary>
        /// Gets persistent mod data from a specific mod from a save file as a float, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static float RetrieveFloatPersistentModData(string key, string modName)
        {
            return float.Parse(RetrieveStringPersistentModData(key, modName));
        }

        /// <summary>
        /// Gets persistent mod data shared across all mods from a save file
        /// </summary>
        /// <param name="key">Name of value</param>
        public static string RetrieveStringPersistentUniversalModData(string key)
        {
            return UKAPI.SaveFileHandler.RetrieveModData(key, "allPersistentModData");
        }

        /// <summary>
        /// Gets persistent mod data shared across all mods from a save file as an int, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static int RetrieveIntPersistentUniversalModData(string key)
        {
            return int.Parse(RetrieveStringPersistentUniversalModData(key));
        }

        /// <summary>
        /// Gets persistent mod data shared across all mods from a save file as a boolean, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static bool RetrieveBooleanPersistentUniversalModData(string key)
        {
            return bool.Parse(RetrieveStringPersistentUniversalModData(key));
        }

        /// <summary>
        /// Gets persistent mod data shared across all mods from a save file as a float, note that this method just parses a string
        /// </summary>
        /// <param name="key">Name of value</param>
        public static float RetrieveFloatPersistentUniversalModData(string key)
        {
            return float.Parse(RetrieveStringPersistentUniversalModData(key));
        }

        /// <summary>
        /// Removes persistent mod data from given a key
        /// </summary>
        /// <param name="key">The name of the value you want to remove</param>
        public void RemovePersistentModData(string key)
        {
            UKAPI.SaveFileHandler.RemoveModData(Info.Metadata.Name, key);
        }

        /// <summary>
        /// Removes persistent mod data from a specified mod given a key
        /// </summary>
        /// <param name="key">The name of the value you want to remove</param>
        /// <param name="modName">Name of mod to remove data from</param>
        public void RemovePersistentModData(string key, string modName)
        {
            UKAPI.SaveFileHandler.RemoveModData(modName, key);
        }

        /// <summary>
        /// Removes persistent mod data shared across all mods from a given key
        /// </summary>
        /// <param name="key">The name of the value you want to remove</param>
        public static void RemovePersistentUniversalModData(string key)
        {
            UKAPI.SaveFileHandler.RemoveModData("allPersistentModData", key);
        }

        /// <summary>
        /// Ensures persistent mod data exists from a given key
        /// <param name="key">The name of the value you want to ensure exists</param>
        public bool PersistentModDataExists(string key)
        {
            return UKAPI.SaveFileHandler.EnsureModData(Info. Metadata.Name, key);
        }

        /// <summary>
        /// Ensures persistent mod data exists from a given key and a mod name
        /// </summary>
        /// <param name="key">Name of value you want ot ensure exists</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public static bool PersistentModDataExists(string key, string modName)
        {
            return UKAPI.SaveFileHandler.EnsureModData(modName, key);
        }

        /// <summary>
        /// Ensures persistent mod data exists shared across all mods from a given key
        /// </summary>
        /// <param name="key">The name of the value you want to ensure exists</param>
        public static bool UniversalModDataExists(string key)
        {
            return UKAPI.SaveFileHandler.EnsureModData("allPersistentModData", key);
        }
    }
}