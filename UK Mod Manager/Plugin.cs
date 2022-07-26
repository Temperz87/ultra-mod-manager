using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using BepInEx;
using HarmonyLib;

namespace UKMM.Loader
{
    [BepInPlugin("UKMM", "UKMM", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private static bool initialized = false;

        public void Start()
        {
            if (!initialized)
            {
                new Harmony("tempy.ukmm").PatchAll();
                StartCoroutine(UKAPI.InitializeAPI());
                initialized = true;
            }
        }

        public void OnApplicationQuit()
        {
            UKAPI.SaveFileHandler.DumpFile();
        }
    }
}
