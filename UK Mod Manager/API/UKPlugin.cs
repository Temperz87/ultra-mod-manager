using System;

namespace UMM
{
    public class UKPlugin : Attribute
    {
        public string GUID;
        public string name;
        public string version;
        public string description;
        public bool allowCyberGrindSubmission;
        public bool unloadingSupported;

        public UKPlugin(string GUID, string name, string version, string description, bool allowCyberGrindSubmission, bool supportsUnloading)
        {
            this.GUID = GUID;
            this.name = name;
            this.version = version;
            this.description = description;
            this.allowCyberGrindSubmission = allowCyberGrindSubmission;
            this.unloadingSupported = supportsUnloading;
        }
    }
}
