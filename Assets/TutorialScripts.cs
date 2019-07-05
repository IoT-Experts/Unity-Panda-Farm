using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;
public class TutorialScripts : MonoBehaviour
{

    // Use this for initialization
    public Sprite[] Sprites;
    public Button ButtonClose;
    public Image image;
    //int count = 0;

    void Start()
    {
        ButtonClose.onClick.AddListener(() => ButtonCloseClick());
        string level = "Level" + ObscuredPrefs.GetInt("level");
      
        if (level == "Level1" || level == "Level2" || level == "Level3" || level == "Level4")
        {
           // ObscuredPrefs.SetInt(level, 0);
            if (ObscuredPrefs.GetInt(level) == 0)
            {
                gameObject.GetComponent<Animator>().Play("tutorialShow");
                switch (level)
                {
                    case "Level1":
                        Debug.Log("level1");
                        image.sprite = Sprites[0];
                        break;
                    case "Level2":
                        Debug.Log("level2");
                        image.sprite = Sprites[1];
                        break;
                    case "Level3":
                        Debug.Log("level3");
                        image.sprite = Sprites[2];
                        break;
                    case "Level4":
                        Debug.Log("level4");
                        image.sprite = Sprites[3];
                        break;
                    default:
                        break;
                }
            }
        }

    }

    // Update is called once per frame

    void ButtonCloseClick()
    {
        iTween.PunchScale(ButtonClose.gameObject, new Vector3(0.3f, 0.3f), 0.3f);
        gameObject.GetComponent<Animator>().Play("tutorialHide");
        ObscuredPrefs.SetInt("Level"+ObscuredPrefs.GetInt("level"), 1);

    }
}
