using System;

namespace UMM
{
    public class UKDependency : Attribute
    {
        public string GUID;
        public Version MinimumVersion;

        public UKDependency(string GUID, string MinimumVersion)
        {
            this.GUID = GUID;
            this.MinimumVersion = Version.Parse(MinimumVersion);
        }
    }
}
