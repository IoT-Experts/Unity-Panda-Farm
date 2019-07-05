using UnityEngine;
using System.Collections;

public class MoveLoadingControll : MonoBehaviour
{
    public GameObject Loading;
    public GameObject star1, star2, star3;
    bool checkStar1, checkStar2, checkStar3;
    public Sprite starComplete;
    int a1, a2, a3;
    float rotation0 = 16f;
    float rotation1 = 54f;
    float rotation2 = 110f;
    float rotation3 = 165f;
    float b1, b2, b3, b4;
    public GameManager gm { get { return JMFUtils.gm; } }
    public WinningConditions wc { get { return JMFUtils.wc; } }
    // Use this for initialization
    void Start()
    {
        Loading.transform.eulerAngles = new Vector3(0, 0, rotation1);
        a1 = wc.scoreToReach;
        a2 = wc.scoreMilestone2 - wc.scoreToReach;
        a3 = wc.scoreMilestone3 - wc.scoreMilestone2;
       // a4 = wc.scoreMilestone3;
        b1 = rotation1 - rotation0;
        b2 = rotation2 - rotation1;
        b3 = rotation3 - rotation2;
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.score <= a1)
        {
            float c = gm.score * (b1 / a1);
            Loading.transform.eulerAngles = new Vector3(0, 0, rotation0 + c);
        }
        if (gm.score > wc.scoreToReach && gm.score < wc.scoreMilestone2)
        {
            float d = gm.score - wc.scoreToReach;
            float c = d * (b2 / a2);
            Loading.transform.eulerAngles = new Vector3(0, 0, c + rotation1);
        }

        if (gm.score > wc.scoreMilestone2 && gm.score <= wc.scoreMilestone3)
        {
            float d = gm.score - wc.scoreMilestone2;
            float c = d * (b3 / a3);
            Loading.transform.eulerAngles = new Vector3(0, 0, c+rotation2);
        }

        if (gm.score >= wc.scoreToReach && !checkStar1 && gm.score < wc.scoreMilestone2)
        {
            star1.GetComponent<SpriteRenderer>().sprite = starComplete;
            iTween.PunchScale(star1.gameObject, new Vector3(0.5f, 0.5f), 0.5f);
            checkStar1 = true;
        }
        if (gm.score >= wc.scoreMilestone2 && !checkStar2 && gm.score < wc.scoreMilestone3)
        {
            iTween.PunchScale(star2.gameObject, new Vector3(0.5f, 0.5f), 0.5f);
            checkStar2 = true;
            star2.GetComponent<SpriteRenderer>().sprite = starComplete;
        }
        if (gm.score >= wc.scoreMilestone3 && !checkStar3)
        {
            iTween.PunchScale(star3.gameObject, new Vector3(0.5f, 0.5f), 0.5f);
            checkStar3 = true;
            star3.GetComponent<SpriteRenderer>().sprite = starComplete;
        }
    }
}
