using System;
using BepInEx;
using UMM.Loader;

namespace UMM
{
    public class ModMetaData : Attribute
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public bool AllowCybergrindSubmission { get; }
        public bool CanBeDisabled { get; }

        public ModMetaData(string name, string version, string description, bool allowCyberGrindSubmission, bool supportsDisabling)
        {
            this.Name = name;
            this.Version = version;
            this.Description = description;
            this.AllowCybergrindSubmission = allowCyberGrindSubmission;
            this.CanBeDisabled = supportsDisabling;
        }

        public static ModMetaData GetFromType(ModType type, Type modClass) {
            // TODO: Read mod name from a manifest file
            if(type == ModType.BepInPlugin) {
                BepInPlugin metaData = UltraModManager.GetBepinMetaData(modClass);
                return new ModMetaData(metaData.Name, metaData.Version.ToString(3), "", true, false);
            } else if(type == ModType.UKMod) {
                ModMetaData metaData = UltraModManager.GetUKMetaData(modClass);
                return metaData;
            }
            throw new ArgumentException($"Unknown mod type {type}.", nameof(type));
        }
    }
}
