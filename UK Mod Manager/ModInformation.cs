using System;
using UMM.Loader;
using BepInEx;

namespace UMM
{
    public class ModInformation : IComparable<ModInformation>
    {
        public ModType modType;
        public Type mod;
        public string modName;
        public string modDescription;
        public string modVersion;
        public bool supportsUnloading;
        public bool loadOnStart;
        public bool loaded;

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

        public void LoadThisMod()
        {
            if (!loaded)
            {
                UltraModManager.LoadMod(this);
                loaded = true;
            }
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