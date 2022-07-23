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


namespace UKMM
{
    public static class UKAPI
    {
        private static AssetBundle commonBundle;

        public static IEnumerator LoadCommonBundle()
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(Environment.CurrentDirectory + "\\ULTRAKILL_Data\\StreamingAssets\\common");
            yield return request;
            int attempts = 1;
            while (request.assetBundle == null)
            {
                if (attempts >= 5)
                {
                    Debug.Log("UKMM: Could not load common asset bundle");
                    yield break;
                }
                request = AssetBundle.LoadFromFileAsync(Environment.CurrentDirectory + "\\ULTRAKILL_Data\\StreamingAssets\\common");
                attempts++;
            }
            commonBundle = request.assetBundle;
        }

        /// <summary>
        /// Tries to create a Ultrakill asset load request from ULTRAKILL_Data/StreamingAssets/common, note that this request has to be awaited
        /// </summary>
        /// <param name="Name">Name of the asset to load, you MUST include the extensions (e.g. prefab)</param>
        /// <returns></returns>
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
        /// <param name="Name">Name of the asset to load, you MUST include the extensions (e.g. prefab)</param>
        /// <returns></returns>
        public static object LoadCommonAsset(string name)
        {
            if (commonBundle == null)
            {
                Debug.LogError("UKMM: Could not load asset " + name + " due to the common asset bundle not being loaded.");
                return null;
            }
            return commonBundle.LoadAsset(name);
        }
    }
}
