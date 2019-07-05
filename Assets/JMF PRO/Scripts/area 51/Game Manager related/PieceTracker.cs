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

public class PieceTracker : MonoBehaviour
{

    static GameManager gm { get { return JMFUtils.gm; } }
    [HideInInspector]
    public int[] arrayRef = new int[2]; // a tracker to keep note on which board this piece belongs too..
     static bool isBeingDragged = false;
    static int[] dragOrigin;

    // drag call...
    void OnMouseEnter()
    {
        if (isBeingDragged && dragOrigin != arrayRef)
        {
            if (gm.iBoard(dragOrigin).allNeighbourBoards.Contains(gm.iBoard(arrayRef)))
            { // a neighbour board
                gm.draggedFromHere(dragOrigin, arrayRef); // make a drag call...
            }
            isBeingDragged = false; // completes the drag status...
        }
    }

    // initiate the drag sequence from this position
    void OnMouseDown()
    {
        dragOrigin = arrayRef; // set the drag origin
        isBeingDragged = true;
    }

    // function to cancel the drag once user lets go...s
    void OnMouseUp()
    {
        isBeingDragged = false; // cancels the drag
    }

    void OnMouseUpAsButton()
    { // mouse click
        //Debug.Log("b");
        JMFRelay.onPieceClick(arrayRef[0], arrayRef[1]);
    }
}
