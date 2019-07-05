using UnityEngine;
using System.Collections;

/// <summary> ##################################
/// 
/// NOTICE :
/// This script is a simple info display stating no more moves.
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################

public class NoMoreMoves : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        // make it pop-out~!
        Vector3 newSize = Vector3.Scale(gameObject.transform.localScale, new Vector3(2f, 2f, 1f));
        //LeanTween.scale( gameObject , newSize ,0.5f, new object[]{"ease",LeanTweenType.easeOutBounce});
        LeanTween.scale(gameObject, newSize, 0.5f);
    }

}
