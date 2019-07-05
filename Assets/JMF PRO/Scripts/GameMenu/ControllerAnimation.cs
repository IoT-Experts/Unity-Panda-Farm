using UnityEngine;
using System.Collections;

public class ControllerAnimation : MonoBehaviour
{

    public Animator anim;
    // Use this for initialization
    void Start()
    {
        InvokeRepeating("PlayFarmerAnimation", 0, Random.Range(4, 6));
    }

    // Update is called once per frame
    void Update()
    {

    }       

    //Play Farmer Animation
    public void PlayFarmerAnimation()
    {
        anim.SetTrigger("anim2");
    }
}
