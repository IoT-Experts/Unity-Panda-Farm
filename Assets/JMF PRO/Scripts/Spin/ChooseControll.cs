using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ChooseControll : MonoBehaviour
{
    public GameObject EffectItem;
    public Image ItemSpinComplete;
    bool check;
    // Use this for initialization
    void Start()
    {
        EffectItem.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (!check)
        {
            check = true;
            EffectItem.SetActive(true);
            Sprite sprite = coll.GetComponent<Image>().sprite;
            AddItem(coll.name, sprite);
            StartCoroutine(DeActiveEffect());

        }
    }

    IEnumerator DeActiveEffect()
    {
        yield return new WaitForSeconds(4f);
        EffectItem.SetActive(false);
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        check = false;
    }
    void AddItem(string nameItem, Sprite sprite)
    {
        ItemSpinComplete.GetComponent<Image>().sprite = sprite;
        switch (nameItem)
        {
            case "Item1":
                Data.UpdateData(Data.keyBinhXit, 1);
                break;
            case "Item2":
                Data.UpdateData(Data.keyBinhThuoc, 1);
                break;
            case "Item3":
                Data.UpdateData(Data.keyGio, 1);
                break;
            case "Item4":
                Data.UpdateData(Data.keyCoin, 500);
                break;
            case "Item5":
                Data.UpdateData(Data.keyBanTay, 1);
                break;
            case "Item6":
                Data.UpdateData(Data.keyCoin, 2000);
                break;
            case "Item7":
                Data.UpdateData(Data.keyBua, 1);
                break;
            case "Item8":
                Data.UpdateData(Data.keyCoin, 3000);
                break;
            default:
                break;
        }
    }



}
