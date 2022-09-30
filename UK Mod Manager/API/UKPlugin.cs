using System;

namespace UMM
{
    public class UKPlugin : Attribute
    {
        public string name;
        public string version;
        public string description;
        public bool allowCyberGrindSubmission;
        public bool unloadingSupported;

        public UKPlugin(string name, string version, string description, bool allowCyberGrindSubmission, bool supportsUnloading)
        {
            this.name = name;
            this.version = version;
            this.description = description;
            this.allowCyberGrindSubmission = allowCyberGrindSubmission;
            this.unloadingSupported = supportsUnloading;
        }
    }
}