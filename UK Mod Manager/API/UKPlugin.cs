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
        internal bool usingManifest { get; } // Used to see if we should check for a manifest.json

        public UKPlugin(string GUID, bool allowCyberGrindSubmission, bool supportsUnloading)
        {
            this.GUID = GUID;
            this.allowCyberGrindSubmission = allowCyberGrindSubmission;
            this.unloadingSupported = supportsUnloading;
            this.usingManifest = true;
        }

        [Obsolete("With ThunderStore, UMM can now use the manifest.json to get the name, version, and description")]
        public UKPlugin(string GUID, string name, string version, string description, bool allowCyberGrindSubmission, bool supportsUnloading)
        {
            this.GUID = GUID;
            this.name = name;
            this.version = version;
            this.description = description;
            this.allowCyberGrindSubmission = allowCyberGrindSubmission;
            this.unloadingSupported = supportsUnloading;
        }

        [Obsolete("Please specify a GUID when making your mod, however since one wasn't specified it will be your mods name.")]
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
