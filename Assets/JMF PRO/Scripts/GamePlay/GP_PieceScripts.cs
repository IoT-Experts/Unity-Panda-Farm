using UnityEngine;
using System.Collections;
public class GP_PieceScripts : MonoBehaviour
{
    WinningConditions winning;
    TextMesh txtBom;
    public GameManager gm { get { return JMFUtils.gm; } }
    int b;
    int soluongBom;
    bool checkNo;
    void Start()
    {
        winning = FindObjectOfType<WinningConditions>();

        if (winning.isFruitBom)
        {
            int a = Random.Range(0, 15);
            if (winning.countBom < winning.MaxBom)
            {
                if (a == 7 && gameObject.transform.childCount > 1)
                {
                    txtBom = gameObject.transform.GetChild(1).GetChild(0).GetComponent<TextMesh>();
                    gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    winning.countBom++;
                    gameObject.tag = "fruitbom";
                    soluongBom = Random.Range(8, 13);
                    b = gm.moves;
                }
            }
        }
    }

    void Update()
    {
        if (txtBom != null && gm.gameState == GameState.GameActive && gameObject.tag == "fruitbom")
        {
            int a = soluongBom - (gm.moves - b);
            txtBom.text = (Mathf.Abs(a) + 1).ToString();
            if (Mathf.Abs(a) <= 2)
            {
                gameObject.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
            }
            if (Mathf.Abs(a) <= 0&& !checkNo)
            {
                checkNo = true;
                gameObject.transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                winning.checkGameoverbom = true;
                GameObject c = Instantiate(winning.SauBom, gameObject.transform.position, Quaternion.identity) as GameObject;
                Destroy(c, 1f);
            }
        }
    }
}
