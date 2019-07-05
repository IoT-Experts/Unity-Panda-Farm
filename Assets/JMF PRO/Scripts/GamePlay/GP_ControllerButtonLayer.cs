using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class GP_ControllerButtonLayer : MonoBehaviour
{
    Animator anim;
    public enum TypeButton
    {
        btnNext,
        btnRetry,
        btnCloseLayer
    }

    public TypeButton typeButton;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void OnMouseDown()
    {
        switch (typeButton)
        {
            case TypeButton.btnNext:
               
                anim.Play("btnNextClick");
                StartCoroutine(NextLevel());
                break;
            case TypeButton.btnRetry:
                StartCoroutine(RetryLevel());
                anim.Play("btnNextClick");               
                break;
            case TypeButton.btnCloseLayer:
                break;
        }
    }

    IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(0.5f);
        //Application.LoadLevel(1);
        SceneManager.LoadScene(1);
    }

    IEnumerator RetryLevel()
    {
        yield return new WaitForSeconds(0.5f);
        //Application.LoadLevel(Application.loadedLevelName);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
