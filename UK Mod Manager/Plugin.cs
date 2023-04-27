using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;

namespace UMM.Loader
{
    [BepInPlugin("UMM", "umm.mainManager", VersionHandler.VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static bool initialized = false;
        internal static Plugin instance;
        internal static ManualLogSource logger;

        private void Start()
        {
            if (!initialized)
            {
                instance = this;
                logger = Logger;
                logger.LogMessage("UMM initializing!");
                try
                {
                    new Harmony("umm.mainManager").PatchAll();

                    UKAPI.Initialize();
                    StartCoroutine(VersionHandler.CheckVersion());
                    initialized = true;
                }
                catch (ArgumentException e)
                {
                    logger.LogError("UMM failed to initialize");
                    logger.LogError(e.Message);
                    logger.LogError(e.StackTrace);
                }
            }
        }

        public void Update()
        {
            UKAPI.Update();
        }

        private void OnApplicationQuit()
        {
            UKAPI.SaveFileHandler.DumpFile();
        }
    }
}
