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
        /// Adds persistent mod data to a save file
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="value">Value to add as a string</param>
        public void AddPersistentModData(string key, string value)
        {
            UKAPI.SaveFileHandler.AddModData(metaData.name, key, value);
        }

        /// <summary>
        /// Adds persistent mod data to a specific mod to a save file
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="value">Value to add as a string</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public void AddPersistentModData(string key, string value, string modName)
        {
            UKAPI.SaveFileHandler.AddModData(modName, key, value);
        }

        /// <summary>
        /// Adds persistent mod data shared across all mods to a save file
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="value">Value to add as a string</param>
        public void AddPersistentUniversalModData(string key, string value)
        {
            UKAPI.SaveFileHandler.AddModData("allPersistentModData", key, value);
        }

        /// <summary>
        /// Gets persistent mod data from a save file
        /// </summary>
        /// <param name="key">Name of value</param>
        public object RetrievePersistentModData(string key)
        {
            return UKAPI.SaveFileHandler.RetrieveModData(metaData.name, key);
        }

        /// <summary>
        /// Gets persistent mod data from a specific mod from a save file
        /// </summary>
        /// <param name="key">Name of value</param>
        /// <param name="modName">Name of mod to get the data from</param>
        public object RetrievePersistentModData(string key, string modName)
        {
            return UKAPI.SaveFileHandler.RetrieveModData(modName, key);
        }

        /// <summary>
        /// Gets persistent mod data shared across all mods from a save file
        /// </summary>
        /// <param name="key">Name of value</param>
        public object RetrievePersistentUniversalModData(string key)
        {
            return UKAPI.SaveFileHandler.RetrieveModData("allPersistentModData", key);
        }
    }
}