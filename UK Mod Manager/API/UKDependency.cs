using System;

namespace UMM
{
    public class UKDependency : Attribute
    {
        public string GUID { get; private set; }
        public Version MinimumVersion { get; private set; }

        public UKDependency(string GUID, string MinimumVersion)
        {
            this.GUID = GUID;
            this.MinimumVersion = Version.Parse(MinimumVersion);
        }
    }
}
