using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace UKMM
{
    public abstract class UKMod : MonoBehaviour
    {
        public UnityEvent OnModUnloaded = new UnityEvent();
        public UKPlugin metaData;
        // maybe include logo???

        public UKMod()
        {
            Type type = this.GetType();
            Debug.Log("Mod found, type is " + type.Name);
            object[] customAttributes = type.GetCustomAttributes(typeof(UKPlugin), false);
            if (customAttributes.Length == 0)
            {
                throw new NullReferenceException("Could not find the metadata (UKPlugin) to UKMod " + type.Name);
            }
            metaData = (UKPlugin)customAttributes[0];
        }

        public virtual void OnModLoaded() { }
        public virtual void OnModUnload() { }
    }
}