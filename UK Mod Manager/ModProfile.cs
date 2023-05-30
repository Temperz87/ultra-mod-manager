using BepInEx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UMM;
using UnityEngine;
using UnityEngine.Networking;

namespace UMM.Loader
{
    [Obsolete(Plugin.UKMOD_DEPRECATION_MESSAGE)]
    public class ModProfile
    {
        public string name;
        public List<ModInformation> allMods = new List<ModInformation>();

        public ModProfile(string profileString)
        {
            this.name = profileString.Substring(0, profileString.IndexOf(":"));
            profileString = profileString.Substring(profileString.IndexOf(":") + 1);
            foreach (string mod in profileString.Split(';'))
            {
                if (UltraModManager.allLoadedMods.ContainsKey(mod))
                    allMods.Add(UltraModManager.allLoadedMods[mod]);
                else
                    Plugin.logger.LogWarning("Mod with GUID: " + mod + " was not found in the loaded mods list!"); // copilot wrote this entire line, including the warning, my job is being replaced :(
            }
        }

        public override string ToString()
        {
            string result = name + ":";
            foreach (ModInformation info in allMods)
                result += info.GUID + ";";
            result += ";";
            return result;
        }
    }
}
