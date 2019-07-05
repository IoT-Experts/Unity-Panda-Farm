using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class VisualManager : MonoBehaviour
{

    [HideInInspector]
    public GameManager gm { get { return JMFUtils.gm; } }
    public GameObject defaultSquareBackPanel;
    public GameObject defaultSquareBackPanel2;
    public GameObject defaultHexBackPanel;
    public bool displayScoreHUD = false;
    public GameObject scoreHUD;
    public Text scoreTxtObject2; // reference to the text score counter
    public Text scoreTxtObject1;
    public TextMesh movesTxtObject; // reference to the text moves counter
    public GameObject comboTxtObject; // reference to the text combo combo

    // Use this for initialization
    void Start()
    {

        SyncReferences();
        StartCoroutine(GUIUpdate()); // widgets update
    }

    void SyncReferences()
    {
        if (comboTxtObject != null)
        {
            gm.comboScript = comboTxtObject.GetComponent<ComboPopUp>(); // find and assign the combo script
        }
    }

    IEnumerator GUIUpdate()
    {
        while (JMFUtils.gm.gameState != GameState.GameOver)
        { // while is not gameOver... update the GUIs
            txtUpdate();
            yield return new WaitForSeconds(gm.gameUpdateSpeed);
        }
    }

    // to output the score to the text label
    void txtUpdate()
    {
        scoreTxtObject1.text = gm.score.ToString();
        scoreTxtObject2.text = gm.score.ToString();
        if (movesTxtObject != null)
        {
            movesTxtObject.text = gm.moves.ToString();
        }
    }
}
