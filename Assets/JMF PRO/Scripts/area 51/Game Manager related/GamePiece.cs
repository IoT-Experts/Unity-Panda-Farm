using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> ##################################
/// 
/// NOTICE :
/// This script is the GamePiece class used by the "Board" script.
/// It controls the visual representation as well as the defining character of a piece.
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################

public class GamePiece
{ // the game pieces as individual pieces on the board

    const string piecePoolName = JMFUtils.piecePoolName;
    public int slotNum; // definition of the skin array to use and piece type
    public GameObject thisPiece; // GUI object reference
    public Board master; // reference of this script
    public int extraEffectID = 0; // the ID for extra effect tweening called by Board.cs
    public bool markedForDestroy = false; // state to signify that it is waiting to be destroyed
    public bool justCreated = false;  // for match create power piece so that it doesnt get destroyed instantly
    public PieceDefinition pd;
    public Vector3 position;

    [HideInInspector]
    public GameManager gm { get { return JMFUtils.gm; } }
    public WinningConditions wc { get { return JMFUtils.wc; } }
    public GamePiece(PieceDefinition newPd, Board newMaster, int type, Vector3 newPosition)
    {
        pd = newPd;
        //Debug.Log(pd);
        master = newMaster;
        slotNum = type;
        position = newPosition;
    }

    public void init()
    {
        dressMe();
    }

    // visual representation of the game piece to the player
    public void dressMe()
    {
        destroyCall(1);
        if (pd == null)
        {
            return; // no piece definition... quit
        }
        if (pd is TreasurePiece)
        {
            Debug.Log("sinh con sau");
            int a = Random.Range(0, 100);
            Debug.Log("b=" + a);
            if (a % 2 == 0)
            {
                if (wc.countSpwanTreasure1 > 0)
                {
                    thisPiece = (GameObject)Object.Instantiate(pd.getSkin(0));
                    wc.countSpwanTreasure1--;
                    Debug.Log("sinh sau1");
                }
                else
                {
                    thisPiece = (GameObject)Object.Instantiate(pd.getSkin(1));
                    wc.countSpwanTreasure2--;
                    Debug.Log("sinh sau2");
                }
            }
            else
            {
                if (wc.countSpwanTreasure2 > 0)
                {
                    thisPiece = (GameObject)Object.Instantiate(pd.getSkin(1));
                    wc.countSpwanTreasure2--;
                    Debug.Log("sinh sau2");
                }
                else
                {
                    thisPiece = (GameObject)Object.Instantiate(pd.getSkin(0));
                    wc.countSpwanTreasure1--;
                    Debug.Log("sinh sau2");
                }


            }
        }
        else
        {
            thisPiece = (GameObject)Object.Instantiate(pd.getSkin(slotNum));
        }


        if (pd is HorizontalPiece || pd is VerticalPiece || pd is SpecialFive || pd is BombPiece)
        {
            gm.animScript.doAnim(animType.CONVERTSPEC, master.arrayRef);
        }
        pd.onPieceCreated(this); // piece is created, call the onCreate (if any)
        thisPiece.transform.parent = master.gm.gameObject.transform; // re-parent the object to the gameManager panel
        thisPiece.transform.position = position;

        if (thisPiece.GetComponent<CircleCollider2D>() == null)
        {
            thisPiece.AddComponent<CircleCollider2D>(); // add a box collider if not present
        }

        JMFUtils.autoScalePadded(thisPiece); // auto scaling feature
        pd.extraPiecePositioning(thisPiece);

        // prefab properties to sync-up ( don't forget the PieceTracker script )
        PieceTracker pt = thisPiece.GetComponent<PieceTracker>();
        if (pt == null)
        { // if the prefab doesnt have the script, add it dynamically...
            pt = thisPiece.AddComponent<PieceTracker>();
        }

        thisPiece.AddComponent<GP_PieceScripts>();
        pt.arrayRef = master.arrayRef;
    }

    // for external scripts to call, destroys the game piece with validation checks
    public void destroy(int i)
    {
        if (pd != null)
        {
            if (!pd.isSpecial)
            { // not a special piece... it is a colored piece
                master.gm.matchCount[slotNum]++; // increase the type count that is destroyed.
            }
            pd.onPieceDestroyed(this); // call the piece type onDestroy function (if any)
            pd = null; // null the piece attribute here
            master.gm.animScript.doAnim(animType.GLOBALDESTROY, master.arrayRef[0], master.arrayRef[1]);
        }
        destroyCall(i);
    }

    // method to remove the piece as we  dont need it anymore...
    // no validation or checks performed.. just remove it and reset!
    public void removePiece(int i)
    {
        pd = null; // null the piece attribute here
        destroyCall(i);
    }
    // for board reset when no more moves
    public void resetMe(PieceDefinition pieceType, int skinNum)
    {
        if (pd.ignoreReset)
        { // non-resettable piece
            return;
        }
        pd = pieceType;
        Debug.Log(pd.name);
        slotNum = skinNum;
        dressMe();
    }

    // converts this piece to a special piece
    public void specialMe(PieceDefinition specialType)
    {
        pd = specialType;
        dressMe();
    }

    // generic destroy function - all destroy call refers to this one function. Intended for Pooling friendly
    public void destroyCall(int i)
    {
        // NON- POOL MANAGER
        if (thisPiece != null)

            switch (i)
            {
                case 0:
                    DestroyPiece(thisPiece);
                    break;
                case 1:
                    Object.Destroy(thisPiece);
                    break;
            }
    }

    void DestroyPiece(GameObject a)
    {
        a.tag = "animQua";
        if (!GP_TrayBasket.checkMoveBasket)
        {
            GP_TrayBasket.checkMoveBasket = true;
        }
        a.transform.position = new Vector3(a.transform.position.x, a.transform.position.y, -5);
        a.GetComponent<CircleCollider2D>().isTrigger = true;
        Hashtable hh = new Hashtable();
        hh.Add("time", 1.5f);
        hh.Add("rotation", new Vector3(0, 0, Random.Range(180, 600)));
        iTween.RotateTo(a, hh);
        if (a.GetComponent<Rigidbody2D>() == null)
        {
            a.AddComponent<Rigidbody2D>();
        }
        a.GetComponent<Rigidbody2D>().gravityScale = Random.Range(6f, 6.4f);
    }
}