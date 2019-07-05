using UnityEngine;
using System.Collections;

/// <summary> ##################################
/// 
/// NOTICE :
/// This script is a simple timer to kill the object when the animation is completed.
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################

public class Lifespan : MonoBehaviour {

	void OnEnable(){
		ParticleSystem psys = this.GetComponent<ParticleSystem>();
        Destroy(gameObject, psys.startLifetime + psys.duration);
	}
}
