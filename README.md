# UK Mod Manager
This mod manager is currently in Alpha, and is subject to make mod breaking changes, but is currently released for feedback.
<h2>Creating a mod using UKAPI</h2>
First you will need to download an installation of UKMM from [here](https://youtu.be/meNiXcbPh_s), then you will need to create a new Class Library (.NET Framework) in Visual Studio (preferably using C# version 4.7.2). After that, reference UKMM.dll, which should now be in ULTRAKILL\plugins\UKMM\. The Mod loader will look for a class that inherits UKMod, and has the attribute UKPlugin, like so.
![Untitled](https://user-images.githubusercontent.com/58375877/183227327-4396fe56-3004-45ba-9b4d-fbc28556784f.png) <br>
Documentation will come later, however note the OnModLoaded method.
