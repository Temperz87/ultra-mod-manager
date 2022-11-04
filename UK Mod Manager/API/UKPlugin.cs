using System;

namespace UMM
{
    public class UKPlugin : Attribute
    {
        public string Name { get; }
        public string Version { get; }
        public string Description { get; }
        public bool AllowCybergrindSubmission { get; }
        public bool CanBeUnloaded { get; }

        public UKPlugin(string name, string version, string description, bool allowCyberGrindSubmission, bool supportsUnloading)
        {
            this.Name = name;
            this.Version = version;
            this.Description = description;
            this.AllowCybergrindSubmission = allowCyberGrindSubmission;
            this.CanBeUnloaded = supportsUnloading;
        }
    }
}
