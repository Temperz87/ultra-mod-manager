using System;
using UMM.Loader;
using BepInEx;
using UnityEngine;

namespace UMM
{
    public class ModInformation : IComparable<ModInformation>
    {
        public ModType modType { get; }
        public Type mod { get; }
        public string modName { get; }
        public string modDescription { get; }
        public string modVersion { get; }
        public bool supportsUnloading { get; }
        public bool loadOnStart { get; internal set; }
        public bool loaded { get; private set; }

        public ModInformation(Type mod, ModType modType)
        {
            this.modType = modType;
            this.mod = mod;

            // TODO: Read mod name from a manifest file
            if (modType == ModType.BepInPlugin)
            {
                BepInPlugin metaData = UltraModManager.GetBepinMetaData(mod);
                modName = metaData.Name;
                modVersion = metaData.Version.ToString();
                modDescription = "Mod unloading and descriptions are not supported by BepInEx plugins.";
            }
            else if (modType == ModType.UKMod)
            {
                UKPlugin metaData = UltraModManager.GetUKMetaData(mod);
                modName = metaData.name;
                modDescription = metaData.description;
                modVersion = metaData.version;
                supportsUnloading = metaData.unloadingSupported;
            }
        }

        public void Clicked()
        {
            if (!loaded)
                LoadThisMod();
            else
                UnLoadThisMod();
        }

        public int CompareTo(ModInformation other)
        {
            return String.Compare(modName, other.modName);
        }

        public bool LoadThisMod()
        {
            if (!loaded)
            {
                try
                {
                    UltraModManager.LoadMod(this);
                    loaded = true;
                }
                catch (Exception e)
                {
                    Plugin.logger.LogMessage("Caught exception while trying to load mod " + modName);
                    Plugin.logger.LogMessage(e.ToString());
                    loaded = false;
                }
            }
            return loaded;
        }

        public void UnLoadThisMod()
        {
            if (loaded && supportsUnloading)
            {
                UltraModManager.UnloadMod(this);
                loaded = false; 
            }
        }

        public enum ModType
        {
            UKMod,
            BepInPlugin
        }
    }
}
