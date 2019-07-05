using UnityEngine;
using System.Collections;

/// <summary> ##################################
/// 
/// NOTICE :
/// This script is the combo text + animation
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################


public class ComboPopUp : MonoBehaviour {
	
	Vector3 oriScale, newSize;
	TextMesh combotxt;
	

	// Use this for initialization
	void Start () {
		combotxt = gameObject.GetComponent<TextMesh>();
		
		oriScale = gameObject.transform.localScale;
		gameObject.transform.localScale = Vector3.zero; // initialy not shown
		newSize = Vector3.Scale(oriScale,new Vector3(1.5f,1.5f,1.5f));
	}
	
	// called by GameManager script
	public IEnumerator displayCombo(int num){
		
		gameObject.transform.localScale = Vector3.zero; // start from nothing
		combotxt.text = "Combo\n"+num.ToString();
		
		// animate it (makes it pop-out big)
		LeanTween.scale( gameObject, newSize ,0.5f);
		yield return new WaitForSeconds(0.5f);
		LeanTween.scale( gameObject, oriScale ,0.5f);
		
		yield return new WaitForSeconds(1f);
		gameObject.transform.localScale = Vector3.zero; // end with nothing
	}
}
