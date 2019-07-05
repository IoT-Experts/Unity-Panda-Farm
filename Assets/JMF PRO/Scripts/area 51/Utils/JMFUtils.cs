
/// <summary>
/// JMF utils. use as a helper class for various static function calls
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class JMFUtils {

	public const string gmPanelName = "GameManagerPanel";
	public const string panelPoolName = "Panels";
	public const string piecePoolName = "Pieces";
	public const string particlePoolName = "Particles";
	public static GameManager gm; // updated by GameManager -> Awake()
	public static WinningConditions wc;  // updated by GameManager -> Awake()
	public static VisualManager vm;  // updated by GameManager -> Awake()
	public static bool isPooling {get{return gm.usingPoolManager;}}

	// -----------------------------------------------------------------------------------------

	
	/// <summary>
	/// 
	/// the codes below are mine ... 
	/// COPYRIGHT kurayami88
	/// 
	/// </summary>


	// look for an object bounds
	public static Bounds findObjectBounds(GameObject obj){
		// includes all mesh types (filter; renderer; skinnedRenderer)
		Renderer ren = obj.GetComponent<Renderer>();
		if(ren == null){
			ren = obj.GetComponentInChildren<Renderer>();
		}
		if(ren != null){
			return ren.bounds;
		}
		Debug.LogError("Your prefab" + obj.ToString() + "needs a mesh to scale!!!");
		return new Bounds(Vector3.zero,Vector3.zero); // fail safe
	}
	
	// auto scale objects to fit into a board box size
	public static void autoScale(GameObject obj){
		autoScaleRatio(obj,1f); // default ratio of 1
	}
	
	// auto scale objects to fit into a board box size - with padding!
	public static void autoScalePadded(GameObject obj){
		autoScaleRatio(obj,gm.boxPadding); // default ratio of 1
	}
	
	public static void autoScaleHexagon(GameObject obj){
		autoScaleRatio(obj,1.156f); // 1.156f is the hexagon's scale
	}
	
	// auto scale objects to fit into a board box size with ratio
	public static void autoScaleRatio(GameObject obj, float ratio){
		obj.transform.localScale = Vector3.one; // resets the scale first
		
		// auto scaling feature
		Bounds bounds = findObjectBounds(obj);
		float val = (gm.size* (1-(gm.spacingPercentage/100.0f)) * ratio) / // get the bigger size to keep ratio
			Mathf.Clamp( Mathf.Max(bounds.size.x,bounds.size.y),0.0000001f,float.MaxValue);
		obj.transform.localScale = new Vector3 (val, val, val ); // the final scale value
		
		// adjust the box collider if present...
		BoxCollider bc = obj.GetComponent<BoxCollider>();
		if ( bc != null){
			float maxSize = Mathf.Max( new float[] {bounds.size.x,bounds.size.y,bounds.size.z} );
			bc.size = new Vector3(maxSize, maxSize, bounds.size.z + 0.01f);
			bc.center = Vector3.zero;
		}
	}
}
