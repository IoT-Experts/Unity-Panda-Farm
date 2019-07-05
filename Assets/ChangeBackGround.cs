using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class ChangeBackGround : MonoBehaviour
{
    public List<Sprite> ListBackground;
    // Use this for initialization
    void Start()
    {
        int a = Random.Range(0, ListBackground.Count);
        gameObject.GetComponent<Image>().sprite = ListBackground[a];
    }

}
