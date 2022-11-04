using System;
using UMM;
using UnityEngine;

namespace UltraModManager.Tests {
	[ModMetaData(
		name: "UMM Tester",
		version: "1.0.0",
		description: "Tests functions of UMM.",
		allowCyberGrindSubmission: true,
		supportsDisabling: true
	)]
	public class UMMTester : UKMod {
		protected override void OnModEnabled() {
			base.OnModEnabled();
			Debug.Log("Tester Mod Enabled");

			// test asset bundle loading
			try {

				AudioClip clip = UKAPI.LoadCommonAsset<AudioClip>("assets/sounds/minos prime/mp_prepare2.wav");

				var obj = new GameObject("minos the pinos");
				var audSrc = obj.AddComponent<AudioSource>();
				audSrc.clip = clip;
				audSrc.Play();

			} catch(Exception e) {
				Debug.LogError($"Asset bundle test failed: {e}");
			}

			// test cybergrind disable
			try {

				UKAPI.DisableCyberGrindSubmission(nameof(UMMTester));

				Assert.IsFalse(nameof(UKAPI.CanSubmitCybergrindScore), UKAPI.CanSubmitCybergrindScore);

				UKAPI.RemoveDisableCyberGrindReason(nameof(UMMTester));

			} catch(Exception e) {
				Debug.LogError($"Cybergrind disable test failed: {e}");
			}

			// test persistent data
			try {

				SetPersistentModData("TestString", "Hello, World!");
				SetPersistentModData("TestNumber", "420");

				Assert.IsEqual("TestString", RetrieveStringPersistentModData("TestString"), "Hello, World!");
				Assert.IsEqual("TestNumber", RetrieveIntPersistentModData("TestNumber"), 420);

			} catch(Exception e) {
				Debug.LogError($"Persistent data test failed: {e}");
			}

			// test info
			try {

				Assert.IsEqual("Info.Metadata.Name", Info.Metadata.Name, "UMM Tester");
				Assert.IsEqual("Info.Metadata.Version", Info.Metadata.Version, "1.0.0");
				Assert.IsEqual("Info.Metadata.Description", Info.Metadata.Description, "Tests functions of UMM.");
				Assert.IsEqual("Info.Metadata.CanBeDisabled", Info.Metadata.CanBeDisabled, true);
				Assert.IsEqual("Info.Metadata.AllowCybergrindSubmission", Info.Metadata.AllowCybergrindSubmission, true);

			} catch(Exception e) {
				Debug.LogError($"Info test failed: {e}");
			}
		}

		protected override void OnModDisabled() {
			base.OnModDisabled();
			Debug.Log("Tester Mod Disabled");
		}
	}
	static class Assert {
		public static void IsFalse(string name, bool val) {
			if(!val) throw new Exception($"{name} should have been false, but it is true.");
		}
		public static void IsEqual(string name, object val, object shouldBe) {
			if(val != shouldBe) throw new Exception($"{name} should have been {shouldBe}, but it is {val}.");
		}
	}
}
