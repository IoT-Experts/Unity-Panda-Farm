using UnityEngine;
using System.Collections;

public class GP_BugScripts : MonoBehaviour
{
    Animator anim;
    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        InvokeRepeating("RandomAnim", 0, Random.Range(5, 10));
    }

    void RandomAnim()
    {
        anim.SetTrigger("anim1");
    }   
}