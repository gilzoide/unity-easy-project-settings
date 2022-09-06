# Easy Project Settings
Easily create custom Project Settings by adding [ProjectSettingsAttribute](Runtime/ProjectSettingsAttribute.cs) to your `ScriptableObject` subclass!


## Features
- Simple to use: just add [ProjectSettingsAttribute](Runtime/ProjectSettingsAttribute.cs) to your `ScriptableObject` subclass and it will automatically appear at the Project Settings window
- Project settings can be used in built players if asset paths are relative to the `Assets` folder
- Project settings can be loaded by code in the Unity editor independent of the asset path
- Project settings can be loaded by code in built players if asset paths are relative to a `Resources` folder
- Supports any asset paths, including paths relative to `ProjectSettings` folder
- Supports your custom editors, no extra code required


## How to install
Install via [Unity Package Manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html)
  using the following URL:

```
https://github.com/gilzoide/unity-easy-project-settings.git#1.0.0
```