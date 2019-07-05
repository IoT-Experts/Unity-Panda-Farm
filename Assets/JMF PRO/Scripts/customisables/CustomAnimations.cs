using UnityEngine;
using System.Collections;

/// <summary> ##################################
/// 
/// NOTICE :
/// This script is the animation/particles section.
/// "Powers" and in "PowerMerge" references animations from this script;
/// which in turn will generate the called animations.
///  
/// </summary> ##################################

public enum animType
{
    GLOBALDESTROY, NOMOREMOVES, ARROWH, ARROWV, ARROWVH, ARROWTX, STAR, RAINBOW, BOMB, BOMBX3,
    ARROWTRBL, ARROWTLBR, ROCKHIT, LOCKHIT, ICEHIT, SHADEHIT, CONVERTSPEC, TREASURECOLLECTED
};

public class CustomAnimations : MonoBehaviour
{

    public GameObject PieceDestroyEffect; // global piSece destroy effect
    public GameObject noMoreMoves; // no more moves effect
    public GameObject horizontalAnim;
    public GameObject verticalAnim;
    public GameObject TRBLAnim;
    public GameObject TLBRAnim;
    public GameObject starAnim;
    public GameObject rainbowAnim;
    public GameObject growingEffect;
    public GameObject bombAnim;
    public GameObject bombx3Anim;
    public GameObject rockAnim;
    public GameObject lockAnim;
    public GameObject iceAnim;
    public GameObject shadedAnim;
    public GameObject convertingAnim;
    public GameObject treasureCollectedAnim;

    GameManager gm;
    const string animPoolName = JMFUtils.particlePoolName;


    void Awake()
    {
        gm = GetComponent<GameManager>();
    }

    /*
     * NOTES :
     * 
     * Use "gm.board[x,y].position" to get the origin location of the caller
     * gm.boardWidth / gm.boardHeight    <--- the width and height of the current board
     * 
     * ---------------------------
     * 
     * IMPORTANT ~!!
     * 
     * Pool Manager version of the script has an auto-despawn function
     * located in the "Lifespan.cs" script found in area 51/GUI Related/
     * 
     * 
     */

    // External scripts will call this function
    // From here, CustomAnimations script will select the appropriate anim to use.

    // OVERLOADED FUNCTION for doAnim
    public void doAnim(animType animType, int[] arrayRef)
    {
        doAnim(animType, arrayRef[0], arrayRef[1]); // call main function
    }

    public void doAnim(animType animType, int x, int y)
    {
        switch (animType)
        {
            case animType.GLOBALDESTROY:
                if (PieceDestroyEffect)
                {
                   Instantiate(PieceDestroyEffect, gm.board[x, y].position, Quaternion.identity);               
                }
                break;
            case animType.NOMOREMOVES:
                if (noMoreMoves)
                {
                    Instantiate(noMoreMoves);
                }
                break;
            case animType.ARROWH:
                if (horizontalAnim != null)
                {
                  GameObject a=  Instantiate(horizontalAnim, gm.board[x, y].position, Quaternion.identity) as  GameObject;
                  Destroy(a, 2f);
                }
                break;
            case animType.ARROWV:
                if (verticalAnim != null)
                {
                 GameObject a=   Instantiate(verticalAnim, gm.board[x, y].position, Quaternion.identity) as GameObject;
                    Destroy(a, 2f);

                }
                break;
            case animType.ARROWVH:
                if (verticalAnim != null && horizontalAnim != null)
                { // animation effect
                    Instantiate(verticalAnim, gm.board[x, y].position, Quaternion.identity);
                    Instantiate(horizontalAnim, gm.board[x, y].position, Quaternion.identity);
                }
                break;
            case animType.ARROWTX: // is when match-4 power combine with match-T
                if (verticalAnim != null && horizontalAnim != null)
                { // animation effect
                    Instantiate(verticalAnim, gm.board[x, y].position, Quaternion.identity);
                    Instantiate(horizontalAnim, gm.board[x, y].position, Quaternion.identity);
                    if (x + 1 < gm.boardWidth)
                    {
                        Instantiate(verticalAnim, gm.board[x + 1, y].position, Quaternion.identity);
                    }
                    if (x - 1 >= 0)
                    {
                        Instantiate(verticalAnim, gm.board[x - 1, y].position, Quaternion.identity);
                    }
                    if (y + 1 < gm.boardHeight)
                    {
                        Instantiate(horizontalAnim, gm.board[x, y + 1].position, Quaternion.identity);
                    }
                    if (y - 1 >= 0)
                    {
                        Instantiate(horizontalAnim, gm.board[x, y - 1].position, Quaternion.identity);
                    }
                }
                break;
            case animType.ARROWTLBR: // is when match-4 power TopLeft <> BottomRight
                if (TLBRAnim != null)
                { // animation effect
                    Instantiate(TLBRAnim, gm.board[x, y].position, Quaternion.identity);
                }
                break;
            case animType.ARROWTRBL: // is when match-4 power TopLeft <> BottomRight
                if (TRBLAnim != null)
                { // animation effect
                    Instantiate(TRBLAnim, gm.board[x, y].position, Quaternion.identity);
                }
                break;
            case animType.STAR:
                if (starAnim != null)
                {
                    Instantiate(starAnim, gm.board[x, y].position, Quaternion.identity);
                }
                break;
            case animType.RAINBOW:
                if (rainbowAnim != null)
                {
                    GameObject a = Instantiate(growingEffect, gm.board[x, y].position, Quaternion.identity) as GameObject;
                    Destroy(a, 2);
                }
                break;
            case animType.BOMB:
                if (bombAnim != null)
                {
                    GameObject a = Instantiate(bombAnim, gm.board[x, y].position, Quaternion.identity) as GameObject;
                    Destroy(a, 0.5f);
                }
                break;
            case animType.LOCKHIT:
                if (lockAnim != null)
                {
                    Instantiate(lockAnim,new Vector3( gm.board[x, y].position.x,gm.board[x, y].position.y,-10), Quaternion.identity);
                }
                break;
            case animType.ROCKHIT:
                if (rockAnim != null)
                {
                    Instantiate(rockAnim, gm.board[x, y].position, Quaternion.identity);
                }
                break;
            case animType.ICEHIT:
                if (iceAnim != null)
                {
                    Instantiate(iceAnim, new Vector3(gm.board[x, y].position.x, gm.board[x, y].position.y, -10), Quaternion.identity);
                }
                break;
            case animType.SHADEHIT:
                if (shadedAnim != null)
                {
                    Instantiate(shadedAnim, gm.board[x, y].position, Quaternion.identity);
                }
                break;
            case animType.CONVERTSPEC:
                if (convertingAnim != null)
                {
                    GameObject a = Instantiate(convertingAnim, new Vector3(gm.board[x, y].position.x, gm.board[x, y].position.y, -5), Quaternion.identity) as GameObject;
                    Destroy(a, 2f);
                }
                break;
            case animType.TREASURECOLLECTED:
                if (treasureCollectedAnim != null)
                {
                    Instantiate(treasureCollectedAnim, gm.board[x, y].position, Quaternion.identity);
                }
                break;
            case animType.BOMBX3:
                if (bombx3Anim != null)
                {
                    GameObject a = Instantiate(bombx3Anim, gm.board[x, y].position, Quaternion.identity) as GameObject;
                    Destroy(a, 1f);
                }
                break;
        }
    }
    public GameObject InstanceFiveEffect(int[] arrayRef)
    {
        int x = arrayRef[0];
        int y = arrayRef[1];
        GameObject a = Instantiate(rainbowAnim, new Vector3(gm.board[x, y].position.x, gm.board[x, y].position.y, -22), Quaternion.identity) as GameObject;
        Destroy(a, 2);
        return a;
    }
}
