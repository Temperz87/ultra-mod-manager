using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using BepInEx;
using HarmonyLib;

namespace UMM.Loader
{
    [BepInPlugin("UMM", "UMM", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private static bool initialized = false;

        public void Start()
        {
            if (!initialized)
            {
                Debug.Log("UMM initializing!");
                new Harmony("umm.mainManager").PatchAll();
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
