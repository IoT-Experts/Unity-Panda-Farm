using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GP_TrayBasket : MonoBehaviour
{
    public GameManager gm { get { return JMFUtils.gm; } }

    public List<GameObject> basketCurrent;
    GameObject[,] lstBasketMove = new GameObject[9, 1];

    public GameObject basket, basketInstance, basket3;

    public TextMesh txtTotalBasket;

    public static bool checkMoveBasket;
    public static int totalBaket;
    public static int SoluongBasket = 9;
    GameObject gameManager;
    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("gamemanager");
        totalBaket = Data.GetData(Data.keyGio);

        txtTotalBasket.text = totalBaket.ToString();
        for (int i = 0; i < basketCurrent.Count; i++)
        {
            GameObject a = TaoBasket(basketCurrent[i].transform.position) as GameObject;
            lstBasketMove[i, 0] = a;
        }
    }

    GameObject TaoBasket(Vector3 position)
    {
        GameObject a = Instantiate(basket, position, Quaternion.identity) as GameObject;
        return a;
    }

    IEnumerator UpdatePosition()
    {
        yield return new WaitForSeconds(0.5f);
        int x = 0;
        List<GameObject> abc = new List<GameObject>();
        for (int i = 0; i < lstBasketMove.GetLength(0); i++)
        {
            if (lstBasketMove[i, 0] != null)
            {
                abc.Add(lstBasketMove[i, 0]);
            }
        }
        switch (abc.Count)
        {
            case 1:
                x = 4;
                break;
            case 2:
                x = 3;
                break;
            case 3:
                x = 3;
                break;
            case 4:
                x = 3;
                break;
            case 5:
                x = 2;
                break;
            case 6:
                x = 1;
                break;
            case 7:
                x = 1;
                break;
            case 8:
                x = 0;
                break;
        }

        for (int i = 0; i < lstBasketMove.GetLength(0); i++)
        {
            lstBasketMove[i, 0] = null;
        }
        for (int i = 0; i < abc.Count; i++)
        {
            lstBasketMove[i + x, 0] = abc[i];
            Hashtable hh = new Hashtable();
            hh.Add("time", 0.3f);
            hh.Add("position", basketCurrent[i + x].transform.position);
            iTween.MoveTo(abc[i], hh);
        }
        abc.Clear();
    }



    IEnumerator InstanceBasket()
    {
        for (int i = 0; i < lstBasketMove.GetLength(0); i++)
        {
            if (lstBasketMove[i, 0] == null && totalBaket > 0)
            {
                totalBaket--;
                //PlayerPrefs.SetInt("totalgio", totalBaket);
                Data.RemoveData(Data.keyGio, 1);
                SoluongBasket++;
                GameObject a = TaoBasket(basketInstance.transform.position);
                lstBasketMove[i, 0] = a;
                yield return new WaitForSeconds(0.4f);
                Hashtable hh = new Hashtable();
                hh.Add("time", 0.5f);
                hh.Add("position", basketCurrent[i].transform.position);
                iTween.MoveTo(a, hh);
            }
        }
    }

    public IEnumerator AddBasket(int amount, Vector3 position, int type)
    {
        for (int i = 0; i < amount; i++)
        {
            yield return new WaitForSeconds(0.3f);
            totalBaket++;
            GameObject a = (GameObject)Instantiate(basket3, position, Quaternion.identity);
            Hashtable hh = new Hashtable();
            hh.Add("time", 0.5f);
            hh.Add("position", basketInstance.transform.position);
            iTween.MoveTo(a, hh);
            Destroy(a, 0.5f);
            yield return new WaitForSeconds(0.3f);
            iTween.PunchScale(basketInstance, new Vector3(65f, 65f), 0.7f);
            txtTotalBasket.text = totalBaket.ToString();
            Data.UpdateData(Data.keyGio, 1);
            MusicControll.musicControll.MakeSound(MusicControll.musicControll.AddBasket);
        }
    }

    void Update()
    {
        txtTotalBasket.text = totalBaket.ToString();
        if (gm.canMove && SoluongBasket < 9 && totalBaket > 0 && GameObject.FindWithTag("animQua") == null && checkMoveBasket)
        {
            StartCoroutine(InstanceBasket());
        }
        if (gm.canMove && totalBaket == 0 && GameObject.FindWithTag("animQua") == null && checkMoveBasket)
        {
            StartCoroutine(UpdatePosition());
        }
        if (totalBaket == 0 && SoluongBasket == 0)
        {
            gameManager.GetComponent<WinningConditions>().checkgio = true;
        }
    }
}
