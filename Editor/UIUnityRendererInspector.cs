using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor (typeof(UIUnityRenderer), true)]
public class UIUnityRendererInspector : UIWidgetInspector
{
	UIUnityRenderer mRenderer;
	bool foldout = true;

	protected override void OnEnable ()
	{
		base.OnEnable ();
		mRenderer = target as UIUnityRenderer;
	}

	protected override bool ShouldDrawProperties ()
	{
		SerializedProperty sp = serializedObject.FindProperty ("renderQueue");

		if (sp != null) {
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("RenderQueue", sp.intValue.ToString ());
			EditorGUILayout.EndHorizontal ();
		}

		mRenderer.allowSharedMaterial = EditorGUILayout.Toggle ("Use Shared Material", mRenderer.allowSharedMaterial);

		foldout = EditorGUILayout.Foldout (foldout, "Materials");
		if (foldout) {
			sp = serializedObject.FindProperty ("mMats");
			if (sp != null) {
				for (int i = 0; i < sp.arraySize; i++) {
					NGUIEditorTools.DrawProperty ("Material" + i, sp.GetArrayElementAtIndex (i));
				}
			}
		}
		return true;
	}

}
