# UltraModManager
This mod manager is currently in Alpha, and is subject to make mod breaking changes, but is currently released for feedback. Also, this mod manager will be merged with [cygrind](https://github.com/cygrind) once it's ready, and this repo will be archived.
# Creating a mod
First you will need to download an installation of UMM (an installation tutorial can be found [here](https://youtu.be/meNiXcbPh_s)), and then you will need to create a new Class Library (.NET Framework) in Visual Studio/Rider. After that, reference UMM.dll, which should now be in ULTRAKILL\plugins\UMM\. The Mod loader will look for a class that inherits `UKMod`, and has the attribute `ModMetaData`, like so.
```cs
[ModMetaData(
  name:        "MyAwesomeMod",
  version:     "1.0.0",
  description: "Really cool mod",
  
  allowCyberGrindSubmission: true,
  supportsUnloading: true
)]
public sealed class Mod : UKMod {
  protected internal override void OnModEnabled() {
    // mod was enabled
  }
}
```
Documentation for the API will can be found at the [Wiki](https://github.com/Temperz87/ultra-mod-manager/wiki).
