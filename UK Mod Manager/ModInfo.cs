using System;
using UMM.Loader;
using BepInEx;

namespace UMM
{
    public class ModInfo : IComparable<ModInfo>
    {
        public ModType Type { get; }
        public Type MainClass { get; } 
        public ModMetaData Metadata { get; }
        public bool EnableOnStart { get; internal set; }
        public bool IsEnabled { get; private set; }

        public ModInfo(Type mod, ModType modType)
        {
            this.Type = modType;
            this.MainClass = mod;

            Metadata = ModMetaData.GetFromType(modType, mod);
        }

        public void ToggleEnabled()
        {
            if (!IsEnabled)
                Enable();
            else
                Disable();
        }

        public int CompareTo(ModInfo other)
        {
            return String.Compare(Metadata.Name, other.Metadata.Name);
        }

        public void Enable()
        {
            if (!IsEnabled)
            {
                UltraModManager.LoadMod(this);
                IsEnabled = true;
            }
        }

        public void Disable()
        {
            if (IsEnabled && Metadata.CanBeDisabled)
            {
                UltraModManager.DisableMod(this);
                IsEnabled = false; 
            }
        }
    }
}