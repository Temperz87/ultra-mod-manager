using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UKMM
{
    public class UKPlugin : Attribute
    {
        public string name;
        public string version;
        public string description;
        public bool unloadingSupported;

        public UKPlugin(string name, string version, string description, bool supportsUnloading)
        {
            this.name = name;
            this.version = version;
            this.description = description;
            this.unloadingSupported = supportsUnloading;
        }
    }
}