# Asset Finder v1.14
Tool that lists assets in scene and allows you to see which gameobjects are referencing them.

The Asset Finder Window is available at Window/Asset Finder.

Asset Finder allows you to find materials, textures and meshes that are being used by renderers in the scene. Note that this tool does not support references from custom components.
You may filter your search in different ways and locate objects using a certain asset.

![AssetFinder Window](https://raw.githubusercontent.com/AdVdTools/AssetFinder/master/asset_finder.png)

## Upper toolbar:
- Asset Type selctor (1-5 keys): Search materials, textures, meshes or audio and animation clips.
- Select All: Select objects in the filter. Hold Ctrl/Cmd to add objects to the current selection.
- Options (right click in window): 
  - Sort by Name. Wheter the search is sorted alphabetically.
  - Ignore Inactive. Search in objects active in hierachy only.
  - Search in Selection Only. Search in selected objects and their children only.
  - Show Detail. Show details about the objects shown. Click on detail to switch the info shown.
- Search Mode:
  - Find Assets. Search assets referenced from GameObjects.
  - Find Users (F key). Search GameObjects using a material, texture or mesh.
  
## Filter options:
- Shader name. Search materials, or textures used by materials, whose shader name starts as in this field.
- Material property name. Search textures in a single texture property of the materials found.
- Mesh components. Select the mesh components in which you want to search (MeshFilters, MeshColliders or SkinnedMeshRenderers).
- Asset Filter (Find Users). In Find Users mode, select an asset to find its users.
- Filter. Filter objects that contain a certain string in the name.


## Search result controls:
- Right click items in the list to highlight their position in hierarchy or the project.
- Left click any item to select it. Hold Shift to select several items in a row. Hold Ctrl/Cmd to add or remove items from selection.
- Double click (or F/Intro keys) on an asset to find its users.
- Double click (or F/Intro keys) on a GameObject to focus the scene view on it.
- Use the up/down arrow keys to move on the list.
- Use the Find button (a magnifying glass) next to assets to find its users.
- You can also drag and drop assets on the window to find its users.


## Notes:
 - Only materials in Renderers are found.
 - Only textures in found materials are found.
 - Only meshes in selected mesh components are found.
 - Shader list in shader filter shows load shaders only. Unity will load them if they are used.
 - Property list in material property filter shows texture properties of the current shader or the most common texture property names.
 - Search results may not update automatically on some changes. Focus the window again to make sure the search is refreshed.
 - Textures in UI elements or SpriteRenderers are not supported yet.
 - Only audio clips in AudioSources are found.
 - Only animation clips in Animators are found. Legacy Animation component is not supported.
 - Unity's Find Reference In Scene option may allow you find users in places this tool doesn't reach.
 
 
