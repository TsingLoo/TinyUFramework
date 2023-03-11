# TinyUFramework
 A tiny UI framework for unity project

## Package(Plugin) Required：

- [DOTween](http://dotween.demigiant.com/)
- [LitJson](https://github.com/LitJSON/litjson)



## UI

*PanelManager* can refresh and update the UI panels info saved in **UIJson.json**. It is also the main entrance of operations about Panel.

To initialize it, you should ensure the correct setting of paths, this could be found in *PanelManager.cs*:

```csharp
readonly string panelPrefabPath = Application.dataPath + @"/Bundles/Resources/Prefabs/UI/Panel/";
readonly string jsonPath = Application.dataPath + @"/Bundles/Json/UIJson.json";
```

![Correct Path](http://images.tsingloo.com/image-20230311212438698.png)
