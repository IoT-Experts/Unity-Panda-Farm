using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class GM_ButtonController : MonoBehaviour {

    public enum TypeButton
    {
        btnPlay
    }

    public TypeButton typeButton;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown()
    {
        switch (typeButton)
        {
            case TypeButton.btnPlay:
                SceneManager.LoadScene("GameMap");
                break;
            default:
                break;
        }
     
    }
}
