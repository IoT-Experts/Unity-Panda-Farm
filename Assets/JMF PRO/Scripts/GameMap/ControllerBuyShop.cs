using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControllerBuyShop : MonoBehaviour
{

    public GameObject ScrollViewCoin;
    public GameObject ScrollViewItem;
    public Button btnCoin;
    public Button btnItem;
    Animator btnCoinAnim;
    Animator btnItemAnim;
    // Use this for initialization
    void Start()
    {
        btnCoinAnim = btnCoin.GetComponent<Animator>();
        btnItemAnim = btnItem.GetComponent<Animator>();
        ScrollViewCoin.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void btnBuyCoin()
    {
        ScrollViewCoin.SetActive(true);
        ScrollViewItem.SetActive(false);
        btnItemAnim.SetTrigger("anim1");
        btnCoinAnim.SetTrigger("anim2");
    }

    public void btnBuyItem()
    {
        ScrollViewCoin.SetActive(false);
        ScrollViewItem.SetActive(true);
        btnItemAnim.SetTrigger("anim2");
        btnCoinAnim.SetTrigger("anim1");
    }
}
