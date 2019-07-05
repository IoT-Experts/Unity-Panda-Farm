using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CodeStage.AntiCheat.ObscuredTypes;
public class ControllerButtonMap : MonoBehaviour
{
    public static ControllerButtonMap controllerButtonMap;
    public GameObject LayerParent;
    public GameObject LayerSpin;
    public GameObject LayerMore;
    public GameObject LayerLoadLevel;
    public GameObject FadeScene;
    public Button btnPlay;
    public Button _btnShop;
    public Button ButtonSpin;
    public Button btnMore;
    public Button btnCloseMore;
    public Text txtTotalCoin, txtTotalBasket;
    int totalBasket;
    float totalCoin;

    Animator AnimLayerSpin;
    Animator AnimLayerLoadLevel;

    void Start()
    {
        LayerMore.SetActive(false);
        controllerButtonMap = this;
        AnimLayerLoadLevel = LayerLoadLevel.GetComponent<Animator>();
        btnPlay.onClick.AddListener(() => ChangeLevel());
        _btnShop.onClick.AddListener(() => btnShop());
        ButtonSpin.onClick.AddListener(() => ButtonSpinClick());
        btnMore.onClick.AddListener(() => ButtonMoreGameClick());
        btnCloseMore.onClick.AddListener(() => ButtonCloseMoreClick());

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
        totalBasket = Data.GetData(Data.keyGio);
        totalCoin = (float)Data.GetData(Data.keyCoin);
        if (totalCoin / 1000000 > 1)
        {
            txtTotalCoin.text = (totalCoin / 1000000).ToString() + "m";
        }
        else
        {
            txtTotalCoin.text = (totalCoin / 1000).ToString() + "k";
        }
        txtTotalBasket.text = totalBasket.ToString();
    }

    public void btnShop()
    {
        iTween.PunchScale(_btnShop.gameObject, new Vector3(0.5f, 0.5f), 0.5f);
        StartCoroutine(ShowLayerShop());
    }


    void ButtonSpinClick()
    {
        iTween.PunchScale(ButtonSpin.gameObject, new Vector3(0.5f, 0.5f), 0.5f);
        LayerSpin.SetActive(true);
    }
    IEnumerator ShowLayerShop()
    {
        yield return new WaitForSeconds(0.3f);
        GameObject layerShop = Instantiate(Resources.Load<GameObject>("Prefabs/Shop/LayerShop")) as GameObject;        
        layerShop.transform.SetParent(LayerParent.transform, false);
        layerShop.transform.localPosition = LayerParent.transform.localPosition;
    }
    public void btnChooseLevel()
    {
        AnimLayerLoadLevel.SetTrigger("LayerLoadLevelShow");
    }

    public void btnCloseLayer()
    {
        AnimLayerLoadLevel.SetTrigger("LayerLoadLevelHide");
    }

    public void ChangeLevel()
    {
        iTween.PunchScale(btnPlay.gameObject, new Vector3(0.5f, 0.5f), 0.5f);
        StartCoroutine(LoadNewLevel());
        FadeScene.GetComponent<Animator>().Play("FadeSceneClose");
        if (ObscuredPrefs.GetInt("levelmax")!=0)
        {
            int level = ObscuredPrefs.GetInt("level");
            int levelMax = ObscuredPrefs.GetInt("levelmax");
            if (level<levelMax+1)
            {
                admodads.showfulladmob();
                admodads.RequestInterstitial();
            }           
        }
        admodads.DestroyBanner();
    }

    IEnumerator LoadNewLevel()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("GamePlay");

    }
    void ButtonCloseMoreClick()
    {
        iTween.PunchScale(btnCloseMore.gameObject, new Vector3(0.5f, 0.5f), 0.5f);
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.ButtonClick);
        LayerMore.SetActive(false);
        LayerMore.transform.parent.GetComponent<LayerMoreGame>().RemoveData();
    }
    private void ButtonMoreGameClick()
    {
        iTween.PunchScale(btnMore.gameObject, new Vector3(0.5f, 0.5f), 0.5f);

        MusicControll.musicControll.MakeSound(MusicControll.musicControll.ButtonClick);
        LayerMore.SetActive(true);
        LayerMore.transform.parent.GetComponent<LayerMoreGame>().BindingData();
    }

}
