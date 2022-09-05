# UltraModManager
This mod manager is currently in Alpha, and is subject to make mod breaking changes, but is currently released for feedback. Also, this mod manager will be merged with [cygrind](https://github.com/cygrind) once it's ready, and this repo will be archived.
# Creating a mod
First you will need to download an installation of UMM (an installation tutorial can be found [here](https://youtu.be/meNiXcbPh_s)), and then you will need to create a new Class Library (.NET Framework) in Visual Studio (preferably using C# version 4.7.2). After that, reference UMM.dll, which should now be in ULTRAKILL\plugins\UMM\. The Mod loader will look for a class that inherits UKMod, and has the attribute UKPlugin, like so.
![Obscenely big picture](https://user-images.githubusercontent.com/58375877/183227327-4396fe56-3004-45ba-9b4d-fbc28556784f.png) <br>
Documentation for the API will come later.
