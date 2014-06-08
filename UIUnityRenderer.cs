using UnityEngine;
using System.Collections;

/// <summary>
/// NGUIでRenderer,ParticleSystemを扱うコンポーネント
/// </summary>
public class UIUnityRenderer : UIWidget
{
	// 指定されているMaterialの値を直接変更します（NGUIのレイアウトにより変更点が都度発生するので注意）
	public bool allowSharedMaterial = false;

	[HideInInspector][SerializeField] Renderer mRenderer;

	[HideInInspector][SerializeField] int renderQueue = -1;

	[HideInInspector][SerializeField] Material[] mMats;

	public Renderer cachedRenderer {
		get {
			if (mRenderer == null) {
				mRenderer = renderer;
			}
			return mRenderer;
		}
	}

	/// <summary>
	/// Material used by Renderer.
	/// </summary>
	public override Material material {
		get {
			if (ExistSharedMaterial0 () == false) {
				Debug.LogError ("Renderer or Material is not found.");
				return null;
			}
			if (allowSharedMaterial == false) {
				if (CheckMaterial (mMats) == false) {
					mMats = new Material[cachedRenderer.sharedMaterials.Length];
					for (int i = 0; i < cachedRenderer.sharedMaterials.Length; i++) {
						mMats [i] = new Material (cachedRenderer.sharedMaterials [i]);
						mMats [i].name = mMats [i].name + " (Copy)";
					}
				}
				if (CheckMaterial (mMats)) {
					if (Application.isPlaying) {
						if (cachedRenderer.materials != mMats) {
							cachedRenderer.materials = mMats;
						}
					}
				}
				// NGUIには0番目で登録する
				return mMats [0];
			} else {
				// SharedMaterial index0をNGUIに登録する
				return cachedRenderer.sharedMaterials [0];
			}
		}
		set {
			throw new System.NotImplementedException (GetType () + " has no material setter");
		}
	}

	/// <summary>
	/// Shader used by Renderer material.
	/// </summary>
	public override Shader shader {
		get {
			if (allowSharedMaterial == false) {
				if (CheckMaterial (mMats)) {
					return mMats [0].shader;
				}
			} else {
				if (ExistSharedMaterial0 ()) {
					return cachedRenderer.sharedMaterials [0].shader;
				}
			}
			return null;
		}
		set {
			throw new System.NotImplementedException (GetType () + " has no shader setter");
		}
	}

	/// <summary>
	/// SharedMaterialに一つでもマテリアルが存在するかチェック
	/// </summary>
	private bool ExistSharedMaterial0 ()
	{
		if (cachedRenderer != null && CheckMaterial (cachedRenderer.sharedMaterials)) {
			return true;
		}
		return false;
	}

	/// <summary>
	/// マテリアルが存在するかチェック
	/// </summary>
	private bool CheckMaterial (Material[] mats)
	{
		if (mats != null && mats.Length > 0) {
			for (int i = 0; i < mats.Length; i++) {
				if (mats [i] == null) {
					return false;
				}
			}
			return true;
		}
		return false;
	}

	protected override void OnDisable ()
	{	
		if (mMats != null) {
			for (int i = 0; i < mMats.Length; i++) {
				DestroyImmediate (mMats [i]);
				mMats [i] = null;
			}
			mMats = null;
		}
		base.OnDisable ();
	}

	protected override void OnUpdate ()
	{
		base.OnUpdate ();
		if (allowSharedMaterial == false) {
			if (CheckMaterial (mMats) && this.drawCall != null) {
				renderQueue = drawCall.finalRenderQueue;
				for (int i = 0; i < mMats.Length; i++) {
					if (mMats [i].renderQueue != renderQueue) {
						mMats [i].renderQueue = renderQueue;
					}
				}
			}
		} else {
			if (ExistSharedMaterial0 () && drawCall != null) {
				renderQueue = drawCall.finalRenderQueue;
				for (int i = 0; i < cachedRenderer.sharedMaterials.Length; i++) {
					cachedRenderer.sharedMaterials [i].renderQueue = renderQueue;
				}
			}
		}
	}

	/// <summary>
	/// Dammy Mesh
	/// </summary>
	public override void OnFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{

		verts.Add (new Vector3 (10000f, 10000f));
		verts.Add (new Vector3 (10000f, 10000f));
		verts.Add (new Vector3 (10000f, 10000f));
		verts.Add (new Vector3 (10000f, 10000f));

		uvs.Add (new Vector2 (0f, 0f));
		uvs.Add (new Vector2 (0f, 1f));
		uvs.Add (new Vector2 (1f, 1f));
		uvs.Add (new Vector2 (1f, 0f));

		cols.Add (color);
		cols.Add (color);
		cols.Add (color);
		cols.Add (color);
	}
}
