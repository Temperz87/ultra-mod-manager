using System;
using UMM.Loader;
using BepInEx;

namespace UMM
{
    public class ModInfo : IComparable<ModInfo>
    {
        public ModType Type { get; }
        public Type MainClass { get; }
        public string Name { get; }
        public string Description { get; }
        public string Version { get; }
        public bool CanBeUnloaded { get; }
        public bool LoadOnStart { get; internal set; }
        public bool IsLoaded { get; private set; }

        public ModInfo(Type mod, ModType modType)
        {
            this.Type = modType;
            this.MainClass = mod;

            // TODO: Read mod name from a manifest file
            if (modType == ModType.BepInPlugin)
            {
                BepInPlugin metaData = UltraModManager.GetBepinMetaData(mod);
                Name = metaData.Name;
                Version = metaData.Version.ToString();
                Description = "Mod unloading and descriptions are not supported by BepInEx plugins.";
            }
            else if (modType == ModType.UKMod)
            {
                ModMetaData metaData = UltraModManager.GetUKMetaData(mod);
                Name = metaData.Name;
                Description = metaData.Description;
                Version = metaData.Version;
                CanBeUnloaded = metaData.CanBeUnloaded;
            }
        }

        public void ToggleLoaded()
        {
            if (!IsLoaded)
                LoadThisMod();
            else
                UnLoadThisMod();
        }

        public int CompareTo(ModInfo other)
        {
            return String.Compare(Name, other.Name);
        }

        public void LoadThisMod()
        {
            if (!IsLoaded)
            {
                UltraModManager.LoadMod(this);
                IsLoaded = true;
            }
        }

        public void UnLoadThisMod()
        {
            if (IsLoaded && CanBeUnloaded)
            {
                UltraModManager.UnloadMod(this);
                IsLoaded = false; 
            }
        }

        public enum ModType
        {
            UKMod,
            BepInPlugin
        }
    }
}