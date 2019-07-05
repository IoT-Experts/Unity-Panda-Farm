using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GP_ClassData
{
    public float timeGiven { get; set; }
    public int move { get; set; }
    public int score1 { get; set; }
    public int score2 { get; set; }
    public int score3 { get; set; }
    public List<MissionType> lstMissionFruitAmout { get; set; }

    public bool targetTime { get; set; }
    public bool targetMove { get; set; }
    public bool taregtScore { get; set; }
    public bool targetFruit { get; set; }
    public bool targetBug { get; set; }
    public bool targetBom { get; set; }

    //}
    public List<string> loaiqua { get; set; }

    public List<int> soluongSau { get; set; }

    public int soluongBom { get; set; }

    public List<int> num { get; set; }

    public class MissionType
    {
        public string name { get; set; }
        public int amount { get; set; }
    }
}
