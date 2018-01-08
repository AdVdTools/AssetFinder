/*
 * Created by Angel David on 23/05/2016.
 */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AdVd
{
	public partial class AssetFinderWindow : EditorWindow
	{
		static int textureDetail=0;//Size
		static int meshDetail=1;//Verts
		static int audioDetail=1;//Duration
		static int animDetail=1;//Duration


		private abstract class ObjectDetail
		{
			public Object obj;

			public abstract void GUI(AssetFinderWindow assetFinder);
		}

		private class AssetDetail : ObjectDetail
		{
			public AssetDetail(Object obj)//, GUIContent content)
			{
				this.obj = obj;
			}
			public override void GUI(AssetFinderWindow assetFinder){
				if (obj is Material){
					Shader sh = (obj as Material).shader;
					EditorGUILayout.SelectableLabel(sh.name,
						GUILayout.Width(assetFinder.position.width*0.25f),
						GUILayout.Height(EditorGUIUtility.singleLineHeight));
				}
				else if (obj is Texture){
					Texture tex = obj as Texture;
					string detail = "";
					if (textureDetail==0) detail = tex.width+"x"+tex.height;
					else if (textureDetail==1) detail = tex.wrapMode.ToString();
					else if (textureDetail==2) detail = tex.filterMode.ToString();
					if (GUILayout.Button(detail, EditorStyles.label,
						GUILayout.Width(75f), GUILayout.Height(EditorGUIUtility.singleLineHeight))){
						textureDetail = (textureDetail+1)%3;
					}
				}
				else if (obj is Mesh){
					Mesh mesh = obj as Mesh;
					string detail = meshDetail==0 ? "Tris: "+(mesh.triangles.Length/3) : "Verts: "+mesh.vertexCount;
					if (GUILayout.Button(detail, EditorStyles.label,
						GUILayout.Width(75f), GUILayout.Height(EditorGUIUtility.singleLineHeight))){
						meshDetail = 1 - meshDetail;
					}
				}
				else if (obj is AudioClip){
					AudioClip audio = obj as AudioClip;
					string detail = audioDetail==0 ? audio.frequency+" Hz" : MMSSFFFTimeFormat(audio.length);
					string tooltip = audioDetail==0 ? "" : "Clip length";
					if (GUILayout.Button(new GUIContent(detail, tooltip), EditorStyles.label,
						GUILayout.Width(75f), GUILayout.Height(EditorGUIUtility.singleLineHeight))){
						audioDetail = 1 - audioDetail;
					}
				}
				else if (obj is AnimationClip){
					AnimationClip anim = obj as AnimationClip;// Legacy Animation is currently disabled
					string detail = animDetail==0 ? (anim.legacy?anim.wrapMode+" (L)":anim.frameRate +" FPS") : anim.length.ToString("0.000");
					string tooltip = animDetail==0 ? "" : "Clip length";
					if (GUILayout.Button(new GUIContent(detail, tooltip), EditorStyles.label,
						GUILayout.Width(60f), GUILayout.Height(EditorGUIUtility.singleLineHeight))){
						animDetail = 1 - animDetail;
					}
				}
			}

			static string MMSSFFFTimeFormat(float time){
				return ((int)time/60).ToString("00")+":"+(time%60).ToString("00.000");
			}
		}

		private class ComponentDetail : ObjectDetail
		{
			private class DetailButton
			{
				public GUIContent content;
				public delegate void OnClick();
				public OnClick onClick = delegate() {};
				public DetailButton(GUIContent content)
				{
					this.content=new GUIContent(content);//Copy GUIContent to avoid weird tooltip behaviour
				}

				public DetailButton(GUIContent content, OnClick onClickAction) : this(content)
				{ 
					this.onClick=onClickAction;
				}
			}

			static GUIStyle iconDetailStyle;
			DetailButton[] detailButtons;

			static public ComponentDetail CreateDetail(Object obj, AssetFinderWindow assetFinder)
			{
				GameObject go = obj as GameObject;
				List<DetailButton> detailButtonList = new List<DetailButton>();
				if (assetFinder.assetType == MATERIALS){
					foreach(Renderer renderer in go.GetComponents<Renderer>()){
						foreach(Material mat in renderer.sharedMaterials){
							if (mat == assetFinder.filterMaterial){
								GUIContent guiContent = EditorGUIUtility.ObjectContent(renderer, typeof(Renderer));
								guiContent.tooltip = renderer.GetType().Name;
								detailButtonList.Add(new DetailButton(guiContent));
								break;
							}
						}
					}
				}
				else if (assetFinder.assetType == TEXTURES){
					foreach (Renderer renderer in go.GetComponents<Renderer>()) {
						foreach (Material mat in renderer.sharedMaterials) {
							if (mat != null) {
								Material material = mat;//Second reference to material that won't change and will be used by the delegate.
								Shader s = material.shader;
								if (!s.name.StartsWith(assetFinder.shaderName)) continue;
								int propCount = ShaderUtil.GetPropertyCount(s);
								for (int i = 0; i < propCount; i++) {
									if (ShaderUtil.GetPropertyType(s, i) == ShaderUtil.ShaderPropertyType.TexEnv) {
										string propName = ShaderUtil.GetPropertyName(s, i);
										if ((assetFinder.materialPropertyName.Equals("") || assetFinder.materialPropertyName.Equals(propName))
											&& material.HasProperty(propName)) {
											Texture texture = material.GetTexture(propName);
											if (texture == assetFinder.filterTexture) {
												GUIContent guiContent = EditorGUIUtility.ObjectContent(renderer, typeof(Renderer));
												guiContent.tooltip = material.name + "(" + propName + ")";
												detailButtonList.Add(new DetailButton(EditorGUIUtility.ObjectContent(renderer, typeof(Renderer)),
													delegate() {
														EditorGUIUtility.PingObject(material);
													}
												));
												break;
											}
										}
									}
								}
							}
						}
					}
				}
				else if (assetFinder.assetType == MESHES){
					MeshFilter mf = go.GetComponent<MeshFilter>();
					if (mf !=null && mf.sharedMesh == assetFinder.filterMesh){
						GUIContent guiContent = EditorGUIUtility.ObjectContent(mf, typeof(MeshFilter));
						guiContent.tooltip = mf.GetType().Name;
						detailButtonList.Add(new DetailButton(guiContent));
					} 
					SkinnedMeshRenderer smr = go.GetComponent<SkinnedMeshRenderer>();
					if (smr !=null && smr.sharedMesh == assetFinder.filterMesh){
						GUIContent guiContent = EditorGUIUtility.ObjectContent(smr, typeof(SkinnedMeshRenderer));
						guiContent.tooltip = smr.GetType().Name;
						detailButtonList.Add(new DetailButton(guiContent));
					} 
					foreach(MeshCollider mc in go.GetComponents<MeshCollider>()){
						if (mc.sharedMesh == assetFinder.filterMesh){
							GUIContent guiContent = EditorGUIUtility.ObjectContent(mc, typeof(MeshCollider));
							guiContent.tooltip = mc.GetType().Name;
							detailButtonList.Add(new DetailButton(guiContent));
						} 
					}
				}
				return new ComponentDetail(go, detailButtonList.ToArray());
			}

			private ComponentDetail(GameObject gameObject, DetailButton[] detailButtons)
			{
				if (iconDetailStyle == null) // Init style
				{
					//iconDetailStyle = new GUIStyle(EditorStyles.label);//May fail
					iconDetailStyle = new GUIStyle();
					iconDetailStyle.padding = new RectOffset(1,1,1,1);
					iconDetailStyle.margin = new RectOffset(2,2,2,2);
					iconDetailStyle.imagePosition = ImagePosition.ImageOnly;
				}

				this.obj = gameObject;
				this.detailButtons = detailButtons;//EditorGUIUtility.ObjectContent(
				//if ? set tooltip
			}

			public override void GUI(AssetFinderWindow assetFinder)
			{
				foreach (DetailButton db in detailButtons)
				{
					if (GUILayout.Button(db.content, iconDetailStyle,
						GUILayout.Width(EditorGUIUtility.singleLineHeight),
						GUILayout.Height(EditorGUIUtility.singleLineHeight))){
						db.onClick();
					}
				}
//				EditorGUILayout.LabelField(content, iconDetailStyle,
//					GUILayout.Width(EditorGUIUtility.singleLineHeight));
			}
		}
	}

	//TODO custom filters through reflection:
	// create and save(serialize) conditions (Component->MeshRenderer, Material.field[sharedMaterial]->materialFilter
	// combine conditions (&& ||)
}
