using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UKMM.Loader;
using Newtonsoft.Json;

namespace UKMM
{
    public static class UKAPI
    {
        public static bool triedLoadingBundle = false;
        private static AssetBundle commonBundle;

        internal static IEnumerator InitializeAPI()
        {
            if (triedLoadingBundle)
                yield break;
            SaveFileHandler.LoadData();
            Debug.Log("UKMM: Trying to load common bundle from " + Environment.CurrentDirectory + "\\ULTRAKILL_Data\\StreamingAssets\\common");
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(Environment.CurrentDirectory + "\\ULTRAKILL_Data\\StreamingAssets\\common");
            yield return request;
            int attempts = 1;
            while (request.assetBundle == null)
            {
                if (attempts >= 5)
                {
                    Debug.Log("UKMM: Could not load common asset bundle");
                    triedLoadingBundle = true;
                    yield break;
                }
                request = AssetBundle.LoadFromFileAsync(Environment.CurrentDirectory + "\\ULTRAKILL_Data\\StreamingAssets\\common");
                attempts++;
            }

            Debug.Log("Loaded common bundle");
            commonBundle = request.assetBundle;
            triedLoadingBundle = true;
            UKModManager.InitializeManager();
        }

        /// <summary>
        /// Tries to create a Ultrakill asset load request from ULTRAKILL_Data/StreamingAssets/common, note that this request has to be awaited
        /// </summary>
        /// <param name="name">Name of the asset to load, you MUST include the extensions (e.g. prefab)</param>
        /// <returns>A new asset bundle request for your item</returns>
        public static AssetBundleRequest LoadCommonAssetAsync(string name)
        {
            if (commonBundle == null)
            {
                Debug.LogError("UKMM: Could not load asset " + name + " due to the common asset bundle not being loaded.");
                return null;
            }
            return commonBundle.LoadAssetAsync(name);
        }

        /// <summary>
        /// Tries to load an Ultrakill asset from ULTRAKILL_Data/StreamingAssets/common
        /// </summary>
        /// <param name="name">Name of the asset to load, you MUST include the extensions (e.g. prefab)</param>
        /// <returns>The asset as an object if found, otherwise returns null</returns>
        public static object LoadCommonAsset(string name)
        {
            if (commonBundle == null)
            {
                Debug.LogError("UKMM: Could not load asset " + name + " due to the common asset bundle not being loaded.");
                return null;
            }
            return commonBundle.LoadAsset(name);
        }

        /// <summary>
        /// Gets all mod information, loaded or not
        /// </summary>
        /// <returns>Returns an array of all found mods</returns>
        public static ModInformation[] GetAllModInformation()
        {
            return UKModManager.foundMods.ToArray().Clone() as ModInformation[];
        }

        /// <summary>
        /// Gets all mod loaded mod information
        /// </summary>
        /// <returns>Returns an array of all loaded mods</returns>
        public static ModInformation[] GetAllLoadedModInformation()
        {
            return UKModManager.allLoadedMods.ToArray().Clone() as ModInformation[];
        }

        internal static class SaveFileHandler
        {
            private static Dictionary<string, Dictionary<string, string>> savedData = new Dictionary<string, Dictionary<string, string>>();
            private static string path = "";

            internal static void LoadData()
            {
                path = Assembly.GetExecutingAssembly().Location;
                path = path.Substring(0, path.LastIndexOf("\\")) + "\\persistent mod data.json";
                Debug.Log("Trying to load save file from " + path);
                FileInfo fInfo = new FileInfo(path);
                if (fInfo.Exists)
                {
                    using (StreamReader jFile = fInfo.OpenText())
                    {
                        savedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jFile.ReadToEnd());
                        jFile.Close();
                    }
                }
                else
                {
                    Debug.Log("Couldn't find a save file, making one now");
                    fInfo.Create();
                }
            }

            internal static void DumpFile()
            {
                FileInfo fInfo = new FileInfo(path);
                using (StreamReader jFile = fInfo.OpenText())
                {
                    Dictionary<string, Dictionary<string, string>> jValues = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jFile.ReadToEnd());
                    jFile.Close();
                    File.WriteAllText(fInfo.FullName, JsonConvert.SerializeObject(jValues));
                }
            }

            /// <summary>
            /// Gets presistent mod data from a key and modname 
            /// </summary>
            /// <param name="modName">The name of the mod to retrieve data from</param>
            /// <param name="key">The value you want</param>
            /// <returns>The mod data if found, otherwise null</returns>
            public static object RetrieveModData(string modName, string key)
            {
                if (savedData.ContainsKey(modName))
                {
                    if (savedData[modName].ContainsKey(key))
                        return savedData[modName][key];
                }
                AddModData(modName, key, ""); 
                return null;
            }
            /// <summary>
            /// Adds persistent mod data from a key and mod name
            /// </summary>
            /// <param name="modName">The name of the mod to add data to</param>
            /// <param name="key">The key for the data</param>
            /// <param name="value">The data you want as a string, note you can only add strings</param>
            public static void AddModData(string modName, string key, string value)
            {
                if (!savedData.ContainsKey(modName))
                {
                    Dictionary<string, string> newDict = new Dictionary<string, string>();
                    newDict.Add(key, value);
                    savedData.Add(modName, newDict);
                }
                else if (!savedData[modName].ContainsKey(key))
                    savedData[modName].Add(key, value);
                else
                    savedData[modName][key] = value;
            }
        }
    }
}
