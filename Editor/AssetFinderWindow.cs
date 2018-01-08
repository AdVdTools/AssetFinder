/*
 * Created by Angel David on 21/03/2016.
 */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AdVd
{
	public partial class AssetFinderWindow : EditorWindow
    {
        Texture2D searchIcon, cogIcon;

        int mode = 0;
        string[] modes = new string[] { "Find Assets", "Find Users" };
        const int FA_MODE = 0;
        const int FU_MODE = 1;

        int assetType = 0;
        string[] findAssetsToolbar = new string[] { "Materials", "Textures", "Meshes", "Audio Clips", "Animation Clips" };
        const int MATERIALS = 0;
        const int TEXTURES = 1;
		const int MESHES = 2;
		const int AUDIO_CLIPS = 3;
		const int ANIMATION_CLIPS = 4;

        [MenuItem("Window/Asset Finder")]
        static void Init()
        {
            AssetFinderWindow window = GetWindow<AssetFinderWindow>();
            window.titleContent = new GUIContent("AssetFinder");
            window.autoRepaintOnSceneChange = true;

            window.LoadResources();
            window.SetListDirty();

            window.Show();
        }

        void LoadResources()
        {
            searchIcon = EditorGUIUtility.Load("Assets/AssetFinder/Editor/search_icon.png") as Texture2D;
            cogIcon = EditorGUIUtility.Load("Assets/AssetFinder/Editor/cog_icon.png") as Texture2D;
        }

		Color SelectionColor{
			get {
				if (Application.HasProLicense()) return new Color32(112, 144, 160, 255);
				else return new Color32(0, 50, 230, 255);
			}
		}

        void OnEnable()
        {
            LoadResources();
            SetListDirty();

            if (Undo.undoRedoPerformed != this.SetListDirty)
            {
                Undo.undoRedoPerformed += this.SetListDirty;
            }
        }

        void OnDestroy()
        {
            Undo.undoRedoPerformed -= this.SetListDirty;
        }
        
		void OnFocus()
		{
			SetListDirty();
		}

		void OnLostFocus()
		{
			SetListDirty();
		}

        void OnSelectionChange()
        {
            SetListDirty();
        }

        void OnHierarchyChange()
        {
            SetListDirty();
        }

        void OnProjectChange()
        {
            LoadResources();
            SetListDirty();
        }
        
        void OnInspectorUpdate()
        {
            if (listIsDirty) FindAssets();
            Repaint();
        }

        bool listIsDirty = true;

        [SerializeField]
        string filter = "";
		bool searchMeshFilters = true, searchSkinnedMeshRenderers = true, searchMeshColliders;

        [SerializeField]
        string shaderName = "";
        const int MAIN_SHADERS_COUNT = 5;

        [SerializeField]
        string materialPropertyName = "";
        List<string> defaultPropertyList = new List<string>(
            new string[] { "_MainTex", "_BumpMap", "_Detail", "_ParallaxMap", "_FrontTex", "_BackTex", "_RightTex", "_LeftTex",
            "_UpTex", "_DownTex", "_Tex", "_Cube", "_Control", "_BumpSpecMap", "_DecalTex", "_GlossMap", "_MetallicGlossMap",
            "_OcclusionMap", "_EmissionMap", "DetailMask",  "_DetailAlbedoMap", "_DetailNormalMap", "_Illum", "_LightMap"});
        const int MAIN_DEFAULT_PROPERTIES_COUNT = 3;

        bool sortByName = true, searchInSelectionOnly, ignoreInactive, showDetail = true;

		List<Object> objList = new List<Object>();
        System.Type type;
		List<ObjectDetail> objDetailList = new List<ObjectDetail>();

        Vector2 scrollPos;

        [SerializeField]
        Material filterMaterial;
        [SerializeField]
        Texture filterTexture;
        [SerializeField]
		Mesh filterMesh;
		[SerializeField]
		AudioClip filterAudio;
		[SerializeField]
		AnimationClip filterAnim;

        Object lastObjClicked;
        double lastClickTime;
        const double doubleClickTime = 0.25f;

		Object lastObjSelected;
		Object firstObjSelected;

        void OnGUI()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            EditorGUIUtility.labelWidth *= 0.5f;
            EditorGUILayout.PrefixLabel("Asset Type:", EditorStyles.toolbarButton, EditorStyles.toolbarButton);
            assetType = EditorGUILayout.Popup(assetType, findAssetsToolbar, EditorStyles.toolbarPopup);
            EditorGUIUtility.labelWidth *= 2;

            EditorGUILayout.Space();

            if (GUILayout.Button(new GUIContent("Select All"), EditorStyles.toolbarButton))
            {
                SelectAllInFilter();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button(cogIcon ? new GUIContent(cogIcon, "Settings") : new GUIContent("...", "Settings"),
                EditorStyles.toolbarDropDown, GUILayout.Width(EditorGUIUtility.singleLineHeight * 2.0f), GUILayout.Height(EditorGUIUtility.singleLineHeight))) 
            {
                Vector2 winSize = position.size; //GUILayoutUtility.GetLastRect();
                Rect dropDownRect = new Rect(winSize.x-2.0f*EditorGUIUtility.singleLineHeight-EditorStyles.toolbar.padding.right,
                    2*EditorGUIUtility.singleLineHeight-1, winSize.x, -EditorGUIUtility.singleLineHeight);//.yMin = dropDownRect.yMax - 30;
                GetGenericMenu().DropDown(dropDownRect);
            }

            EditorGUILayout.EndHorizontal();

            mode = GUILayout.Toolbar(mode, modes);

            // AssetType Options
            if (assetType == MESHES)
            {
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(new GUIContent("Search in:"));
				searchMeshFilters = EditorGUILayout.ToggleLeft(new GUIContent("MeshFilters"), searchMeshFilters);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
				searchSkinnedMeshRenderers = EditorGUILayout.ToggleLeft(new GUIContent("SkinnedMeshRenderers"), searchSkinnedMeshRenderers);
				searchMeshColliders = EditorGUILayout.ToggleLeft(new GUIContent("MeshColliders"), searchMeshColliders);
                EditorGUILayout.EndHorizontal();
            }
            if (assetType == TEXTURES || (assetType == MATERIALS && mode == FA_MODE))
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                shaderName = EditorGUILayout.TextField(new GUIContent("Shader name"), shaderName);
                if (GUILayout.Button(new GUIContent(EditorStyles.foldout.onNormal.background, "Select Shader"), EditorStyles.label,
                    GUILayout.Width(EditorGUIUtility.singleLineHeight), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    EditorGUIUtility.editingTextField = false;
                    GenericMenu gm = GetSelectionMenu(GetShaderList(), MAIN_SHADERS_COUNT, SetShaderName);
                    gm.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();
            }
            if (assetType == TEXTURES)
            {
                EditorGUILayout.BeginHorizontal();
                materialPropertyName = EditorGUILayout.TextField(new GUIContent("Material property name"), materialPropertyName);
                if (GUILayout.Button(new GUIContent(EditorStyles.foldout.onNormal.background, "Select Name"), EditorStyles.label, 
                    GUILayout.Width(EditorGUIUtility.singleLineHeight), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                {
                    EditorGUIUtility.editingTextField = false;
                    Shader shader = Shader.Find(shaderName);
                    GenericMenu gm = GetSelectionMenu(GetShaderPropertiesList(shader), MAIN_DEFAULT_PROPERTIES_COUNT, SetMaterialPropertyName);
                    gm.ShowAsContext();
                }
                EditorGUILayout.EndHorizontal();
            }
            
            // AssetType Filter
            EditorGUILayout.Space();
            if (mode == FU_MODE)
            {
                if (assetType == MATERIALS)
                {
                    filterMaterial = EditorGUILayout.ObjectField(new GUIContent("Material"), filterMaterial, typeof(Material), false) as Material;
                }
                else if (assetType == TEXTURES)
                {
					filterTexture = EditorGUILayout.ObjectField(new GUIContent("Texture"), filterTexture, typeof(Texture), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Texture;
                }
                else if (assetType == MESHES)
                {
					filterMesh = EditorGUILayout.ObjectField(new GUIContent("Mesh"), filterMesh, typeof(Mesh), false) as Mesh;
				}
				else if (assetType == AUDIO_CLIPS)
				{
					filterAudio = EditorGUILayout.ObjectField(new GUIContent("Audio Clip"), filterAudio, typeof(AudioClip), false) as AudioClip;
				}
				else if (assetType == ANIMATION_CLIPS)
				{
					filterAnim = EditorGUILayout.ObjectField(new GUIContent("Animation Clip"), filterAnim, typeof(AnimationClip), false) as AnimationClip;
				}
            }
            filter = EditorGUILayout.TextField(new GUIContent("Filter"), filter);

			// Object List
			Rect listRect = EditorGUILayout.BeginVertical();
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(false));
			if (objList != null)
            {
                // Styles used to draw the list elements.
                GUIStyle objListStyle = new GUIStyle(EditorStyles.textField);
                objListStyle.imagePosition = ImagePosition.ImageLeft;
                objListStyle.active = EditorStyles.textField.focused;
                GUIStyle objListStyleSelected = new GUIStyle(objListStyle);
				if (focusedWindow==this){
					objListStyleSelected.normal.background = objListStyleSelected.focused.background;
				}
				objListStyleSelected.normal.textColor = SelectionColor;
                //objListStyle.onNormal.background = EditorStyles.textField.focused.background;
                //objListStyle.onNormal.textColor = new Color32(0, 50, 230, 255);

                List<Object> selection = new List<Object>(Selection.objects);
				for (int objIndex = 0;objIndex < objList.Count;objIndex++)
                {
					Object obj = objList[objIndex];
                    EditorGUILayout.BeginHorizontal();

                    GUIContent objContent = new GUIContent(EditorGUIUtility.ObjectContent(obj, type));//Copy GUIContent to avoid weird tooltip behavior
                    objContent.tooltip = "Hold ctrl/cmd to add objects to selection.";
                    //if (obj is Texture) objContent.image = EditorGUIUtility.FindTexture("Texture Icon");//Prevents the GUI from breaking due to preview icons
                    
                    bool selected = selection.Contains(obj);
                    if (GUILayout.Button(objContent, selected ? objListStyleSelected : objListStyle,
                        GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.MinWidth(0)))
                    {
						if(Event.current.button == 0)
						{
							if (Event.current.shift)
							{
								if (firstObjSelected == null){
									firstObjSelected = lastObjSelected = obj;
									selection.Clear();
									selection.Add(obj);
								}
								else {
									lastObjSelected = obj;
									int firstIndex = objList.IndexOf(firstObjSelected);
									selection.Clear();
									selection.AddRange(GetObjRange(firstIndex, objIndex));
								}
							}
							else if (EditorGUI.actionKey)
							{
								if (!selected) selection.Add(obj);
								else selection.Remove(obj);
								lastObjSelected=obj;
							}
							else { 
								double time = EditorApplication.timeSinceStartup;
								if (time-lastClickTime<doubleClickTime && lastObjClicked == obj) // Double Click
								{

									if (mode == FA_MODE) FindUsers(lastObjSelected);
									else if (mode == FU_MODE && lastObjSelected!=null) SceneView.FrameLastActiveSceneView();
								}
								else // Single Left Click
								{
									selection.Clear();
									selection.Add(obj);
									firstObjSelected=lastObjSelected=obj;
								}
								lastClickTime = time;
								lastObjClicked = obj;
							}
						}
						else if(Event.current.button == 1) // Right Click
						{
							EditorGUIUtility.PingObject(obj);
						}
						GUI.FocusControl("");
                    }
					if (showDetail && obj!=null && objIndex < objDetailList.Count)
					{
						objDetailList[objIndex].GUI(this);
						//ShowDetail(obj);
					}
                    if (mode == FA_MODE)
                    {
                        if (GUILayout.Button(searchIcon ? new GUIContent(searchIcon, "Find Users") : new GUIContent("F", "Find Users"),
                            EditorStyles.miniButtonRight, GUILayout.Width(EditorGUIUtility.singleLineHeight * 1.5f), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
                        {
							FindUsers(obj);
							GUI.FocusControl("");
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
                if (GUI.changed) Selection.objects = selection.ToArray();// Update the selection.
            }
			if (assetType == MESHES && !searchMeshColliders && !searchMeshFilters && !searchSkinnedMeshRenderers){
				EditorGUILayout.HelpBox("Select at least a component to search in.", MessageType.Warning, true);
			}
			EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

			// DragAndDrop operation
			if (Event.current.type == EventType.DragUpdated){
				Object[] objs = DragAndDrop.objectReferences;
				if (objs.Length>0){
					Object obj = objs[0];
					if (obj is Material || obj is Texture || obj is Mesh || obj is AudioClip || obj is AnimationClip){
						DragAndDrop.visualMode = DragAndDropVisualMode.Move;
					}
				}
			}
			else if (Event.current.type == EventType.DragPerform){
				DragAndDrop.AcceptDrag();
				Object[] objs = DragAndDrop.objectReferences;
				if (objs.Length>0){
					Object obj = objs[0];
					FindUsers(obj);
				}
			}

			//Handle Keys
			if (Event.current.type == EventType.KeyDown){
				KeyCode key = Event.current.keyCode;
				//Up/Down arrows
				if ((key == KeyCode.UpArrow || key == KeyCode.DownArrow) && objList.Count>0){
					if (Event.current.shift){
						int firstIndex, lastIndex;
						if (firstObjSelected==null) firstIndex = 0;
						else firstIndex = objList.IndexOf(firstObjSelected);
						if (lastObjSelected==null) lastIndex=firstIndex;
						else {
							lastIndex = objList.IndexOf(lastObjSelected);
							if (key==KeyCode.UpArrow && lastIndex>0) lastIndex--;
							else if (key==KeyCode.DownArrow && lastIndex<objList.Count-1) lastIndex++;
						}
						firstObjSelected = objList[firstIndex];
						lastObjSelected = objList[lastIndex];
						Selection.objects = GetObjRange(firstIndex, lastIndex);
					}
					else{
						int lastIndex;
						if (lastObjSelected==null) lastIndex=0;
						else {
							lastIndex = objList.IndexOf(lastObjSelected);
							if (key==KeyCode.UpArrow && lastIndex>0) lastIndex--;
							else if (key==KeyCode.DownArrow && lastIndex<objList.Count-1) lastIndex++;
						}
						firstObjSelected=lastObjSelected=objList[lastIndex];
						Selection.objects = new Object[]{ lastObjSelected };
					}
				}
				//Right/Left arrows
				if (key == KeyCode.RightArrow) mode = FU_MODE;
				else if (key == KeyCode.LeftArrow) mode = FA_MODE;

				//Number keys
				if (key == KeyCode.Alpha1 || key == KeyCode.Keypad1) assetType = MATERIALS;
				if (key == KeyCode.Alpha2 || key == KeyCode.Keypad2) assetType = TEXTURES;
				if (key == KeyCode.Alpha3 || key == KeyCode.Keypad3) assetType = MESHES;
				if (key == KeyCode.Alpha4 || key == KeyCode.Keypad4) assetType = AUDIO_CLIPS;
				if (key == KeyCode.Alpha5 || key == KeyCode.Keypad5) assetType = ANIMATION_CLIPS;

				//Enter/F key
				if (key == KeyCode.F || key == KeyCode.Return || key == KeyCode.KeypadEnter){
					if (mode == FA_MODE) FindUsers(lastObjSelected);
					else if (mode == FU_MODE && lastObjSelected!=null) SceneView.FrameLastActiveSceneView();
				}
				//Backspace key
				if (key == KeyCode.Backspace) mode = FA_MODE;
				Event.current.Use();//Does this prevent beep sound?
				GUI.FocusControl("");
				SetListDirty();
			}
				
            if (GUI.changed)
            {
                SetListDirty();
            }

			if (Event.current.type == EventType.MouseDown && Event.current.button == 0)//Left Click
            {
				if (!listRect.Contains(Event.current.mousePosition)){
					Selection.objects = new Object[0];
					GUI.FocusControl("");
				}
                SetListDirty();
            }

			if (Event.current.type == EventType.ContextClick)//Right Click
			{
				GetGenericMenu(true).ShowAsContext();
			}

            //if (EditorWindow.focusedWindow != this)
            //{
            //    Vector2 size = position.size;
            //    GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
            //    boxStyle.alignment = TextAnchor.MiddleCenter;
            //    GUI.Box(new Rect(Vector2.zero, size), "Click to refresh the list", boxStyle);
            //}
        }

        void SelectAllInFilter()
        {
            if (EditorGUI.actionKey)
            {
                List<Object> selection = new List<Object>(Selection.objects);
                selection.AddRange(objList);
                Selection.objects = selection.ToArray();
            }
            else
            {
                Selection.objects = objList.ToArray();
            }
        }

		Object[] GetObjRange(int firstIndex, int lastIndex){
			if (firstIndex>lastIndex){
				int aux = firstIndex;
				firstIndex = lastIndex;
				lastIndex = aux;
			}
			Object[] sel = new Object[lastIndex-firstIndex+1];
			for(int i = 0, o = firstIndex; o <= lastIndex; o++, i++) sel[i] = objList[o];
			return sel;
		}

		void FindUsers(Object obj){
			if (obj is Material){
				filterMaterial = obj as Material;
				assetType = MATERIALS;
			}
			else if (obj is Texture){
				filterTexture = obj as Texture;
				assetType = TEXTURES;
			}
			else if (obj is Mesh){
				filterMesh = obj as Mesh;
				assetType = MESHES;
			}
			else if (obj is AudioClip){
				filterAudio = obj as AudioClip;
				assetType = AUDIO_CLIPS;
			}
			else if (obj is AnimationClip){
				filterAnim = obj as AnimationClip;
				assetType = ANIMATION_CLIPS;
			}
			else return;
			mode = FU_MODE;
			SetListDirty();
		}

        void ClearFilters()
        {
            filter = "";
            materialPropertyName = "";
            shaderName = "";
            filterMaterial = null;
            filterTexture = null;
            filterMesh = null;
			filterAudio = null;
			filterAnim = null;
        }

		void ToggleSortByName()
		{ 
			sortByName=!sortByName;
            SetListDirty();
		}

        void ToggleIgnoreInactive()
        {
            ignoreInactive = !ignoreInactive;
            SetListDirty();
        }

        void ToggleSearchInSelectionOnly()
        {
            searchInSelectionOnly = !searchInSelectionOnly;
            SetListDirty();
        }

		void ToggleShowDetail()
		{
			showDetail = !showDetail;
			Repaint();
		}


        GenericMenu GetGenericMenu(bool context = false)
		{
			GenericMenu gm = new GenericMenu();
            gm.AddItem(new GUIContent("Sort by Name"), sortByName, ToggleSortByName);
			gm.AddItem(new GUIContent("Ignore Inactive"), ignoreInactive, ToggleIgnoreInactive);
			gm.AddItem(new GUIContent("Search in Selection Only"), searchInSelectionOnly, ToggleSearchInSelectionOnly);
			gm.AddItem(new GUIContent("Show Detail"), showDetail, ToggleShowDetail);
            if (context)
            {
                gm.AddSeparator("");
                gm.AddItem(new GUIContent("Clear Filters"), false, ClearFilters);
                gm.AddItem(new GUIContent("Select All"), false, SelectAllInFilter);
            }
            return gm;
		}

        GenericMenu GetSelectionMenu(List<string> options, int mainOptionsCount, GenericMenu.MenuFunction2 assignFunction)
        {
            if (mainOptionsCount > options.Count) mainOptionsCount = options.Count;
            GenericMenu gm = new GenericMenu();
            List<string> rootList = new List<string>(mainOptionsCount);
            for (int i = 0; i < options.Count; i++)
            {
                int rootEnd = options[i].IndexOf('/');
                string root = rootEnd == -1 ? options[i] : options[i].Substring(0, rootEnd);
                if (rootList.Contains(root))
                {
                    gm.AddItem(new GUIContent(options[i]), false, assignFunction, options[i]);
                }
                else
                {
                    if (rootList.Count < mainOptionsCount)
                    {
                        rootList.Add(root);
                        gm.AddItem(new GUIContent(options[i]), false, assignFunction, options[i]);
                    }
                    else
                    {
                        gm.AddItem(new GUIContent("More/" + options[i]), false, assignFunction, options[i]);
                    }
                }
            }
            if (options.Count == 0) gm.AddDisabledItem(new GUIContent("Empty List"));
            gm.AddSeparator("");
            gm.AddItem(new GUIContent("Clear"), false, assignFunction, "");
            return gm;
        }

        void SetMaterialPropertyName(object mpn)
        {
            if (mpn is string) materialPropertyName = (string)mpn;
        }

        void SetShaderName(object sn)
        {
            if (sn is string) shaderName = (string)sn;
        }

        List<string> GetShaderList()
        {
            Shader[] shaders = Resources.FindObjectsOfTypeAll<Shader>();
            List<string> shaderList = new List<string>();
            foreach (Shader s in shaders)
            {
                if ((s.hideFlags & HideFlags.HideInInspector) == HideFlags.None && !s.name.StartsWith("Hidden"))
                {
                    shaderList.Add(s.name);
                }
            }
            return shaderList;
        }

        List<string> GetShaderPropertiesList(Shader shader)
        {
            if (shader == null) return defaultPropertyList;
            List<string> propertyList = new List<string>();
            int propCount = ShaderUtil.GetPropertyCount(shader);
            for (int i = 0; i < propCount; i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    string propName = ShaderUtil.GetPropertyName(shader, i);
                    propertyList.Add(propName);
                }
            }
            return propertyList;
        }

        bool Filter(Object obj)
        {
			return obj.name.ToLowerInvariant().Contains(filter.ToLowerInvariant());
        }
        
        void SetListDirty()
        {
            listIsDirty = true;
            Repaint();
        }

        T[] FindObjects<T>() where T : Object
        {
            if (searchInSelectionOnly)
            {
                return System.Array.ConvertAll<Object, T>(Selection.GetFiltered(typeof(T), SelectionMode.Deep), obj => (T)obj);
            }
            else
            {
                return System.Array.FindAll<T>(Resources.FindObjectsOfTypeAll<T>(), obj => IsInScene(obj));
                //return GameObject.FindObjectsOfType<T>();
            }
        }

        bool IsInScene(Object obj)
        {
            return (obj.hideFlags & (HideFlags.HideAndDontSave | HideFlags.HideInInspector)) == HideFlags.None;
        }

        void FindAssets()
        {
			objList.Clear();
            if (mode == FA_MODE)
            {
                if (assetType == MATERIALS)
                {
                    foreach (Renderer renderer in FindObjects<Renderer>()) 
                    {
                        if (ignoreInactive && !renderer.gameObject.activeInHierarchy) continue;
                        foreach (Material material in renderer.sharedMaterials)
                        {
                            if (material==null || !material.shader.name.StartsWith(shaderName)) continue;
                            if (!objList.Contains(material) && Filter(material)) objList.Add(material);
                        }
                    }

                    type = typeof(Material);
                }
                else if (assetType == TEXTURES)
                {
                    foreach (Renderer renderer in FindObjects<Renderer>())
                    {
                        if (ignoreInactive && !renderer.gameObject.activeInHierarchy) continue;
                        foreach (Material material in renderer.sharedMaterials)
                        {
                            if (material != null){
                                Shader s = material.shader;
                                if (!s.name.StartsWith(shaderName)) continue;
                                int propCount = ShaderUtil.GetPropertyCount(s);
                                for(int i = 0; i < propCount; i++)
                                {
                                    if (ShaderUtil.GetPropertyType(s, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                                    {
                                        string propName = ShaderUtil.GetPropertyName(s, i);
                                        if ((materialPropertyName.Equals("") || materialPropertyName.Equals(propName)) && material.HasProperty(propName))
                                        {
                                            Texture texture = material.GetTexture(propName);
                                            if (texture != null && !objList.Contains(texture) && Filter(texture)) objList.Add(texture);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    type = typeof(Texture);
                }
                else if (assetType == MESHES)
                {
                    if (searchMeshFilters)
                    {
                        foreach (MeshFilter mf in FindObjects<MeshFilter>())
                        {
                            if (ignoreInactive && !mf.gameObject.activeInHierarchy) continue;
                            Mesh mesh = mf.sharedMesh;
                            if (mesh != null && !objList.Contains(mesh) && Filter(mesh)) objList.Add(mesh);
                        }
					}
					if (searchSkinnedMeshRenderers)
					{
						foreach (SkinnedMeshRenderer smr in FindObjects<SkinnedMeshRenderer>())
						{
							if (ignoreInactive && !smr.gameObject.activeInHierarchy) continue;
							Mesh mesh = smr.sharedMesh;
							if (mesh != null && !objList.Contains(mesh) && Filter(mesh)) objList.Add(mesh);
						}
					}
                    if (searchMeshColliders)
                    {
                        foreach (MeshCollider mc in FindObjects<MeshCollider>())
                        {
                            if (ignoreInactive && !mc.gameObject.activeInHierarchy) continue;
                            Mesh mesh = mc.sharedMesh;
                            if (mesh != null && !objList.Contains(mesh) && Filter(mesh)) objList.Add(mesh);
                        }
                    }

                    type = typeof(Mesh);
                }
				else if (assetType == AUDIO_CLIPS)
				{
					foreach (AudioSource audioSource in FindObjects<AudioSource>()){
						if (ignoreInactive && !audioSource.gameObject.activeInHierarchy) continue;
						AudioClip audio = audioSource.clip;
						if (audio!=null && !objList.Contains(audio) && Filter(audio)) objList.Add(audio);
					}

					type = typeof(AudioClip);
				}
				else if (assetType == ANIMATION_CLIPS)
				{
					foreach (Animator animator in FindObjects<Animator>()){
						if (ignoreInactive && !animator.gameObject.activeInHierarchy) continue;
                        if (animator.runtimeAnimatorController == null) continue;
						foreach (AnimationClip anim in animator.runtimeAnimatorController.animationClips){
							if (anim!=null && !objList.Contains(anim) && Filter(anim))
							{
								objList.Add(anim);
							}
						}
					}
//					foreach (Animation animation in FindObjects<Animation>()){
//						if (ignoreInactive && !animation.gameObject.activeInHierarchy) continue;
//						foreach (AnimationState animState in animation){//Updates on PlayMode only
//							AnimationClip anim = animState.clip;//Check null state
//							if (anim!=null && !objList.Contains(anim) && Filter(anim))
//							{
//								objList.Add(anim);
//							}
//						}
//					}
					type = typeof(AnimationClip);
				}
            }
            else if (mode == FU_MODE)
            {
                if (assetType == MATERIALS)
                {
                    if (filterMaterial != null)
                    {
                        foreach (Renderer renderer in FindObjects<Renderer>())
                        {
							GameObject go = renderer.gameObject;
                            if (ignoreInactive && !go.activeInHierarchy) continue;
                            foreach (Material material in renderer.sharedMaterials)
                            {
                                //if (material == null || !material.shader.name.StartsWith(shaderName)) continue; //Ignore shader filter, it's redundant.
								if (material == filterMaterial && Filter(go))
                                {
                                    objList.Add(go);
                                    break;
                                }
                            }
                        }
                    }

					type = typeof(GameObject);
                }
                else if (assetType == TEXTURES)
                {
                    if (filterTexture != null)
                    {
                        foreach (Renderer renderer in FindObjects<Renderer>())
						{
							GameObject go = renderer.gameObject;
                            if (ignoreInactive && !go.activeInHierarchy) continue;
                            foreach (Material material in renderer.sharedMaterials)
                            {
                                if (material != null)
                                {
                                    Shader s = material.shader;
                                    if (!s.name.StartsWith(shaderName)) continue;
                                    int propCount = ShaderUtil.GetPropertyCount(s);
                                    for (int i = 0; i < propCount; i++)
                                    {
                                        if (ShaderUtil.GetPropertyType(s, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                                        {
                                            string propName = ShaderUtil.GetPropertyName(s, i);
                                            if ((materialPropertyName.Equals("") || materialPropertyName.Equals(propName)) && material.HasProperty(propName))
                                            {
                                                Texture texture = material.GetTexture(propName);
												if (texture == filterTexture && !objList.Contains(go) && Filter(go))
                                                {
                                                    objList.Add(go);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

					type = typeof(GameObject);
                }
                else if (assetType == MESHES)
                {
                    if (filterMesh != null)
                    {
                        if (searchMeshFilters)
                        {
                            foreach (MeshFilter mf in FindObjects<MeshFilter>())
							{
								GameObject go = mf.gameObject;
                                if (ignoreInactive && !go.activeInHierarchy) continue;
                                Mesh mesh = mf.sharedMesh;
                                if (mesh == filterMesh && !objList.Contains(go) && Filter(go))
                                {
                                    objList.Add(go);
                                }
                            }
                        }
						if (searchSkinnedMeshRenderers)
						{
							foreach (SkinnedMeshRenderer smr in FindObjects<SkinnedMeshRenderer>())
							{
								GameObject go = smr.gameObject;
								if (ignoreInactive && !go.activeInHierarchy) continue;
								Mesh mesh = smr.sharedMesh;
								if (mesh == filterMesh && !objList.Contains(go) && Filter(go))
								{
									objList.Add(go);
								}
							}
						}
						if (searchMeshColliders)
						{
							foreach (MeshCollider mc in FindObjects<MeshCollider>())
							{
								GameObject go = mc.gameObject;
								if (ignoreInactive && !go.activeInHierarchy) continue;
								Mesh mesh = mc.sharedMesh;
								if (mesh == filterMesh && !objList.Contains(go) && Filter(go))
								{
									objList.Add(go);
								}
							}
						}
                    }

					type = typeof(GameObject);
				}
				else if (assetType == AUDIO_CLIPS)
				{
					foreach (AudioSource audioSource in FindObjects<AudioSource>())
					{
						GameObject go = audioSource.gameObject;
						if (ignoreInactive && !go.activeInHierarchy) continue;
						AudioClip audio = audioSource.clip;
						if (audio == filterAudio && !objList.Contains(go) && Filter(go))
						{
							objList.Add(go);
						}
					}
					type = typeof(GameObject);

				}
				else if (assetType == ANIMATION_CLIPS)
				{
					foreach (Animator animator in FindObjects<Animator>())
					{
						GameObject go = animator.gameObject;
						if (ignoreInactive && !go.activeInHierarchy) continue;
                        if (animator.runtimeAnimatorController == null) continue;
						foreach (AnimationClip anim in animator.runtimeAnimatorController.animationClips){
							if (anim == filterAnim && !objList.Contains(go) && Filter(go))
							{
								objList.Add(go);
							}
						}
					}
//					foreach (Animation animation in FindObjects<Animation>())
//					{
//						GameObject go = animation.gameObject;
//						if (ignoreInactive && !go.activeInHierarchy) continue;
//						foreach (AnimationState animState in animation){//Updates in playmode only
//							AnimationClip anim = animState.clip;
//							if (anim == filterAnim && !objList.Contains(go) && Filter(go))
//							{
//								objList.Add(go);
//							}
//						}
//					}
					type = typeof(GameObject);
				}
            }

            if (sortByName) objList.Sort((obj1, obj2) => obj1.name.CompareTo(obj2.name));

			objDetailList = new List<ObjectDetail>(objList.Count);
			foreach(Object obj in objList)
			{
				ObjectDetail detail;
				if (obj is GameObject)
				{
					detail = ComponentDetail.CreateDetail(obj, this);
				}
				else 
				{
					detail = new AssetDetail(obj);
				}
				objDetailList.Add(detail);
			}

            listIsDirty = false;

			//Fix selection info
			if (firstObjSelected!=null && (!objList.Contains(firstObjSelected) ||
				!ArrayUtility.Contains<Object>(Selection.objects, firstObjSelected))){
				firstObjSelected = null; lastObjSelected = null;
			}
			if (lastObjSelected!=null && (!objList.Contains(lastObjSelected) ||
				!ArrayUtility.Contains<Object>(Selection.objects, lastObjSelected))){
				lastObjSelected = firstObjSelected;
			}
			if (firstObjSelected == null || lastObjSelected == null){
				firstObjSelected = objList.Find(obj => ArrayUtility.Contains<Object>(Selection.objects, obj));
				lastObjSelected = objList.FindLast(obj => ArrayUtility.Contains<Object>(Selection.objects, obj));
			}
        }

		//TODO Event.current.Use();
    }
}