using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
//using SimpleJSON;
using Newtonsoft.Json;
using UnityEngine.UI;

public class GP_ConfigData : MonoBehaviour
{
    // Use this for initialization
    public GameObject[] typeMission;
    public Sprite[] lstSprite;
    public Sprite[] lstSpriteBug;
    [HideInInspector]
    public List<Text> lstTextMesh;

    public GameObject _objectSau;
    public GameObject _parentSau;

    public TextMesh txtText;


    public Text scoreTarget1;
    public Text scoreTarget2;

    public TextMesh txtTime;
    public GameManager gm { get { return JMFUtils.gm; } }
    public WinningConditions wc { get { return JMFUtils.wc; } }
    void Start()
    {

    }

    public void LoadJsonData(string text, GameManager gm, PanelDefinition[] panelScripts)
    {
        int count = 0;
        var targetGame = JsonConvert.DeserializeObject<GP_ClassData>(text);

        for (int y = 0; y < gm.boardHeight; y++)
        {
            for (int x = 0; x < gm.boardWidth; x++)
            {

                int num = targetGame.num[count];
                //int pStrength = tarGetGame.data[count].pStrength;
                gm.board[x, y].panel = new BoardPanel(panelScripts[num], -1, gm.board[x, y]);
                count++;
            }
        }

        //time
        //if (targetGame.targetTime)
        //{
        wc.isTimerGame = targetGame.targetTime;
        wc.TimeGiven = targetGame.timeGiven;
        //}

        //move
        //if (targetGame.targetMove)
        //{
        wc.isMaxMovesGame = targetGame.targetMove;
        wc.allowedMoves = targetGame.move;
        //}

        //score
        wc.isScoreGame = wc.scoreRequiredWin = wc.scoreEndsGame = true;
        wc.scoreToReach = targetGame.score1;
        wc.scoreMilestone2 = targetGame.score2;
        wc.scoreMilestone3 = targetGame.score3;
        scoreTarget1.text = scoreTarget2.text = wc.scoreToReach.ToString();

        //typeFruit;  
        wc.isGetTypesGame = wc.typesRequiredWin = wc.typeEndsGame = targetGame.targetFruit;
        if (wc.isGetTypesGame)
        {
            typeMission[0].SetActive(false);
            typeMission[1].SetActive(true);
            int b = targetGame.lstMissionFruitAmout.Count;
            SetupTypefruit(b, 116, text, 0);
        }

        //treasureGame
        wc.isTreasureGame = wc.treasureEndsGame = wc.treasureRequiredWin = targetGame.targetBug;

        if (wc.isTreasureGame)
        {
            int num1 = targetGame.soluongSau[0];
            wc.numOfTreasures1 = wc.countSpwanTreasure1 = num1;
            int num2 = targetGame.soluongSau[1];
            wc.numOfTreasures2 = wc.countSpwanTreasure2 = num2;
            typeMission[0].SetActive(false);
            typeMission[1].SetActive(true);
            SetupTypefruit(2, 116, text, 1);
        }
        //    //// isBom    
        wc.isFruitBom = targetGame.targetBom;
        if (wc.isFruitBom)
        {
            wc.MaxBom = targetGame.soluongBom;
        }

        if (wc.isScoreGame && !wc.isGetTypesGame && !wc.isTreasureGame)
        {
            typeMission[0].SetActive(true);
            typeMission[1].SetActive(false);
        }
    }

    void SetupTypefruit(float maxType, float sizeObject, string data, int typeFruit)
    {
        var a = JsonConvert.DeserializeObject<GP_ClassData>(data);
        for (int i = 0; i < maxType; i++)
        {
            GameObject c = Instantiate(_objectSau) as GameObject;
            Text t = c.transform.GetChild(1).GetComponent<Text>();
            //c.transform.parent = _parentSau.transform;
           // c.transform.localScale = new Vector3(1, 1);
            c.transform.SetParent(_parentSau.transform,false);
            switch (typeFruit)
            {
                case 0:
                    string name = a.lstMissionFruitAmout[i].name;
                    int amout = a.lstMissionFruitAmout[i].amount;
                    t.text = amout.ToString();
                    foreach (var item in lstSprite)
                    {
                        if (item.name.Equals(name))
                        {
                            c.transform.GetChild(0).GetComponent<Image>().sprite = item;
                            c.name = name;
                            wc.ListQuaMission.Add(c);
                            wc.ListTextMesh.Add(t);
                            wc.lstSoluongquaComplete.Add(t.text);
                            break;
                        }
                    }
                    break;
                case 1:
                    c.transform.GetChild(0).GetComponent<Image>().sprite = lstSpriteBug[i];
                    int amout1 = a.soluongSau[i];
                    c.transform.GetChild(1).transform.localScale *= 1.5f;
                    t.text = amout1.ToString();
                    wc.ListQuaMission.Add(c);
                    wc.ListTextMesh.Add(t);
                    break;
            }
        }
    }
}