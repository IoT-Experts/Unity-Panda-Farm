using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class GP_ControllerButtonMenu : MonoBehaviour
{
    public Sprite[] Sounds;
    public Sprite[] Music;
    public GameObject FadeScene;
    public enum TypeButton
    {
        btnMenu, btnSound, btnMusic, btnBack, btnRate
    }

    public TypeButton typeButton;
    int count;

    void Start()
    {
        switch (typeButton)
        {
            case TypeButton.btnMenu:
                break;
            case TypeButton.btnSound:             
                //if (MusicControll.musicControll.isMusicOn)
                //{
                //    gameObject.GetComponent<SpriteRenderer>().sprite = Sounds[0];
                //}
                //else
                //{
                //    gameObject.GetComponent<SpriteRenderer>().sprite = Sounds[1];
                //}

                break;
            case TypeButton.btnMusic:
                //if (MusicControll.musicControll.isMusicOn)
                //{
                //    gameObject.GetComponent<SpriteRenderer>().sprite = Music[0];
                //}
                //else
                //{
                //    gameObject.GetComponent<SpriteRenderer>().sprite = Music[1];
                //}
                break;
            case TypeButton.btnBack:
                break;
            case TypeButton.btnRate:
                break;
            default:
                break;
        }
    }
    void OnMouseDown()
    {
        iTween.PunchScale(gameObject, new Vector3(0.2f, 0.2f), 0.5f);
        switch (typeButton)
        {
            case TypeButton.btnMenu:
                if (count == 0)
                {
                    GameObject.Find("BgMenuGPlay").GetComponent<Animator>().SetTrigger("anim2");
                    count = 1;
                }
                else
                {
                    GameObject.Find("BgMenuGPlay").GetComponent<Animator>().SetTrigger("anim1");
                    count = 0;
                }

                break;
            case TypeButton.btnSound:
                if (MusicControll.musicControll.isSoundOn)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = Sounds[1];
                    MusicControll.musicControll.isSoundOn = false;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = Sounds[0];
                    MusicControll.musicControll.isSoundOn = true;
                }

                break;
            case TypeButton.btnMusic:
                GameObject music = GameObject.FindGameObjectWithTag("music");
                if (MusicControll.musicControll.isMusicOn)
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = Music[1];
                    MusicControll.musicControll.isMusicOn = false;
                    music.GetComponent<AudioSource>().Pause();
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sprite = Music[0];
                    MusicControll.musicControll.isMusicOn = true;
                    music.GetComponent<AudioSource>().Play();

                }
                break;
            case TypeButton.btnBack:
                FadeScene.GetComponent<Animator>().Play("FadeSceneClose");
                StartCoroutine(LoadHomeMenu());
                break;

            case TypeButton.btnRate:
                Application.OpenURL("https://play.google.com/store/apps/details?id=com.fruit.fram.puzzle.panda");
                break;
            default:
                break;
        }
    }


    IEnumerator LoadHomeMenu()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("GameMenu");

    }

}
