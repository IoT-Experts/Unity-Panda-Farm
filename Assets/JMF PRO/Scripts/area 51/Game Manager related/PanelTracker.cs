using UnityEngine;
using System.Collections;


/// <summary> ##################################
/// 
/// NOTICE :
/// This script is just a simple delegate to announce to GameManager
/// on which piece is being dragged and dragged towards which piece.
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################

public class PanelTracker : MonoBehaviour
{

    [HideInInspector]
    public GameManager gm { get { return JMFUtils.gm; } }
    [HideInInspector]
    public int[] arrayRef = new int[2]; // a tracker to keep note on which board this piece belongs too..
    int x { get { return arrayRef[0]; } } // easy reference for arrayRef[0]
    int y { get { return arrayRef[1]; } } // easy reference for arrayRef[1]
    public int count;

    void OnEnable()
    {
        BoxCollider bc = gameObject.AddComponent<BoxCollider>();
        JMFUtils.autoScale(gameObject);
        bc.center = new Vector3(0, 0, 10); // send the collider to the back, so it wont interfere with PieceTracker
    }

    void OnMouseUpAsButton()
    {
        JMFRelay.onPanelClick(x, y);
    }

    void OnMouseDown()
    {
        GP_TrayItem trayItem = FindObjectOfType<GP_TrayItem>();
        if (!trayItem.showShop)
        {
            if (trayItem.buaClicked)
            {
                StartCoroutine(trayItem.BuaPlay(gm.board[x, y].piece.position));
                StartCoroutine(DestroyObject(trayItem));
                trayItem.buaClicked = false;                
                Data.RemoveData(Data.keyBua, 1);
                trayItem.GetDataItem();
            }
            if (trayItem.bantayClicked)
            {
                trayItem.countInput++;

                if (trayItem.countInput == 1)
                {
                    StartCoroutine(trayItem.BanTayPlay(gm.board[x, y].piece.position));
                    trayItem.pos1[0] = x;
                    trayItem.pos1[1] = y;
                }
                if (trayItem.countInput == 2)
                {
                    if (x == trayItem.pos1[0] + 1 && y == trayItem.pos1[1] || x == trayItem.pos1[0] - 1 && y == trayItem.pos1[1] || x == trayItem.pos1[0] && y == trayItem.pos1[1] + 1 || x == trayItem.pos1[0] && y == trayItem.pos1[1] - 1)
                    {
                        StartCoroutine(trayItem.BanTayPlay(gm.board[x, y].piece.position));
                        trayItem.pos2[0] = x;
                        trayItem.pos2[1] = y;
                        StartCoroutine(gm.switchPositions(trayItem.pos1[0], trayItem.pos1[1], trayItem.pos2[0], trayItem.pos2[1]));
                        trayItem.countInput = 0;
                        Data.RemoveData(Data.keyBanTay, 1);
                        trayItem.GetDataItem();
                    }
                    else
                    {
                        trayItem.countInput = 1;
                    }
                }
            }
            if (trayItem.binhxitClicked)
            {
                if (gm.board[x, y].piece.thisPiece.tag == "fruitbom")
                {
                    gm.board[x, y].piece.thisPiece.transform.GetChild(0).gameObject.SetActive(false);
                    gm.board[x, y].piece.thisPiece.transform.GetChild(1).gameObject.SetActive(false);
                    gm.board[x, y].piece.thisPiece.GetComponent<GP_PieceScripts>().enabled = false;
                    Data.RemoveData(Data.keyBinhXit, 1);
                    trayItem.GetDataItem();
                    trayItem.AllButtonNormal();
                }
            }

            if (trayItem.binhthuocClicked)
            {
                Board board = gm.board[x, y];
                MusicControll.musicControll.MakeSound(MusicControll.musicControll.convertingSpecialFx);

                gm.animScript.doAnim(animType.CONVERTSPEC, board.arrayRef[0], board.arrayRef[1]);
                GameObject pm = gm.pieceManager;
                board.convertToSpecial(pm.GetComponent<SpecialFive>(), 0);
                gm.board[x, y].panelHit();
                gm.lockJustCreated(x, y, 0.3f);
                Data.RemoveData(Data.keyBinhThuoc, 1);
                trayItem.GetDataItem();
                trayItem.AllButtonNormal();
            }
        }
    }

    IEnumerator DestroyObject(GP_TrayItem trayItem)
    {
        yield return new WaitForSeconds(1f);
        Object.Destroy(gm.board[x, y].piece.thisPiece);
        gm.board[x, y].piece = null;
        gm.board[x, y].isBeingDelayed = false;
        gm.board[x, y].isFalling = false;
        trayItem.AllButtonNormal();
    }

    IEnumerator ScaleObject(GameObject a, Vector3 scalecurrent, float time, bool loop)
    {
        iTween.Defaults.easeType = iTween.EaseType.linear;
        while (loop)
        {
            iTween.ScaleTo(a, scalecurrent * 1.3f, time);
            yield return new WaitForSeconds(time);
            iTween.ScaleTo(a, scalecurrent, time);
            yield return new WaitForSeconds(time);
        }
    }
}
