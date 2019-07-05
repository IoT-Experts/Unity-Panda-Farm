using UnityEngine;
using System.Collections;
using SimpleJSON;
using Newtonsoft.Json;
using CodeStage.AntiCheat.ObscuredTypes;
public class SetUpListQua : MonoBehaviour
{

    public GameObject piecesManager;
    public GameManager gm { get { return JMFUtils.gm; } }
    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Settup_ListQua()
    {
        //ObscuredPrefs.SetString("level", "Level1");
        int level = ObscuredPrefs.GetInt("level");
        TextAsset file = (TextAsset)Resources.Load("Level/Level" + level);
        var N = JsonConvert.DeserializeObject<GP_ClassData>(file.text);

        gm.NumOfActiveType = N.loaiqua.Count;

        //normal
        for (int i = 0; i < N.loaiqua.Count; i++)
        {
            string name = N.loaiqua[i];
            GameObject a = Resources.Load("Prefabs/Fruit/" + name, typeof(GameObject)) as GameObject;
            piecesManager.GetComponent<NormalPiece>().skin[i] = a;
        }

        //horizontal
        for (int i = 0; i < N.loaiqua.Count; i++)
        {
            string name = N.loaiqua[i];
            GameObject a = Resources.Load("Prefabs/Fruit/" + name + "_h", typeof(GameObject)) as GameObject;
            piecesManager.GetComponent<HorizontalPiece>().skin[i] = a;
        }

        //vertical
        for (int i = 0; i < N.loaiqua.Count; i++)
        {
            string name = N.loaiqua[i];
            GameObject a = Resources.Load("Prefabs/Fruit/" + name + "_v", typeof(GameObject)) as GameObject;
            piecesManager.GetComponent<VerticalPiece>().skin[i] = a;
        }


        //bom
        for (int i = 0; i < N.loaiqua.Count; i++)
        {
            string name = N.loaiqua[i];
            GameObject a = Resources.Load("Prefabs/Fruit/" + name + "_zpow", typeof(GameObject)) as GameObject;
            piecesManager.GetComponent<BombPiece>().skin[i] = a;
        }
    }
}
