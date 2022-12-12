using System;

namespace UMM
{
    public class UKPlugin : Attribute
    {
        public string GUID { get; }
        public string name { get; }
        public string version { get; }
        public string description { get; }
        public bool allowCyberGrindSubmission { get; }
        public bool unloadingSupported { get; }
        
        public UKPlugin(string GUID, string name, string version, string description, bool allowCyberGrindSubmission, bool supportsUnloading)
        {
            this.GUID = GUID;
            this.name = name;
            this.version = version;
            this.description = description;
            this.allowCyberGrindSubmission = allowCyberGrindSubmission;
            this.unloadingSupported = supportsUnloading;
        }

        public UKPlugin(string name, string version, string description, bool allowCyberGrindSubmission, bool supportsUnloading)
        {
            this.GUID = name;
            this.name = name;
            this.version = version;
            this.description = description;
            this.allowCyberGrindSubmission = allowCyberGrindSubmission;
            this.unloadingSupported = supportsUnloading;
        }
    }
}
