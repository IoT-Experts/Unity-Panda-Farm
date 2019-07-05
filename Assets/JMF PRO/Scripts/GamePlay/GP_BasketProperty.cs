
using UnityEngine;
using System.Collections;

public class GP_BasketProperty : MonoBehaviour
{
    public int index;

    TextMesh txtCount;

    Animator anim;

    int count = 6;

    WinningConditions winning;

    void Start()
    {
        txtCount = transform.GetComponentInChildren<TextMesh>();
        anim = transform.GetComponentInChildren<Animator>();
        winning = FindObjectOfType<WinningConditions>();
    }

    public void RemoveCountBasket(int i)
    {
        if (count > 0)
        {
            count -= i;
            anim.SetTrigger("scale");
            txtCount.text = count.ToString();
        }
        if (count <= 0)
        {
            GP_TrayBasket trayBasket = FindObjectOfType<GP_TrayBasket>();
            trayBasket.basketCurrent[index].GetComponent<GP_Basket1>().active = false;
            GP_TrayBasket.SoluongBasket--;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "animQua")
        {
            string name = coll.name.Substring(0, coll.name.Length - 7);
            winning.RemoveFruit(name);
            RemoveCountBasket(1);
            Destroy(coll.gameObject);
        }
    }

    public void SetActive()
    {
        gameObject.SetActive(true);
    }

}
