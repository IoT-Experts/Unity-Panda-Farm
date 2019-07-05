using UnityEngine;
using System.Collections;

public class HUDPopUp : MonoBehaviour
{

    TextMesh myLabel;
    Vector3 showSize;
    Vector3 hideSize = Vector3.zero;
    Vector3 hideposition;
    // Use this for initialization
    void Awake()
    {
        myLabel = GetComponent<TextMesh>();
        myLabel.text = "123456";       
        showSize = new Vector3(0.1f, 0.1f, 0.1f);
        transform.localScale = hideSize; // initially hidden

    }

    // called by external script (GameManager) to display the ScoreHUD
    public void display(int score)
    {
        myLabel.text = "+" + score.ToString();
        gameObject.GetComponent<Animator>().Play("HubScorePopUp");
        StopCoroutine("showMe");
        StartCoroutine("showMe");
    }

    // show the score with timing
    public IEnumerator showMe()
    {
        transform.localScale = showSize; // show it (makes it pop-out big)
        yield return new WaitForSeconds(0.7f); // wait for time
        transform.localScale = hideSize; // end with nothing
        //gameObject.transform.localPosition = hideposition;
    }
} 
