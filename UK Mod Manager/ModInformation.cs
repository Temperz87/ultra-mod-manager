using System;
using UMM.Loader;
using BepInEx;

namespace UMM
{
    public class ModInformation : IComparable<ModInformation>
    {
        public ModType modType;
        public Type mod;
        public string GUID;
        public string modName;
        public string modDescription;
        public Version modVersion;
        public bool supportsUnloading;
        public bool loadOnStart;
        public bool loaded;
        public Dependency[] dependencies;

        public ModInformation(Type mod, ModType modType)
        {
            this.modType = modType;
            this.mod = mod;

            // TODO: Read mod name from a manifest file
            if (modType == ModType.BepInPlugin)
            {
                BepInPlugin metaData = UltraModManager.GetBepinMetaData(mod);
                GUID = metaData.GUID;
                modName = metaData.Name;
                modVersion = metaData.Version;
                modDescription = "Mod unloading and descriptions are not supported by BepInEx plugins.";
                dependencies = UltraModManager.GetBepinDependencies(mod);
            }
            else if (modType == ModType.UKMod)
            {
                UKPlugin metaData = UltraModManager.GetUKMetaData(mod);
                GUID = metaData.GUID;
                modName = metaData.name;
                modDescription = metaData.description;
                modVersion = Version.Parse(metaData.version);
                supportsUnloading = metaData.unloadingSupported;
                dependencies = UltraModManager.GetUKModDependencies(mod);
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
                loaded = true;
                UltraModManager.LoadMod(this);
            }
        }

        public void UnLoadThisMod()
        {
            if (loaded && supportsUnloading)
            {
                loaded = false;
                UltraModManager.UnloadMod(this);
            }
        }

        public enum ModType
        {
            UKMod,
            BepInPlugin
        }
    }

    public class Dependency
    {
        public string GUID;
        public Version MinimumVersion;
    }
}