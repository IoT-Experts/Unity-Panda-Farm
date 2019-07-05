using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("JMF/Pieces/VerticalPiece")]
public class VerticalPiece : PieceDefinition
{

    public override bool performPower(int[] arrayRef)
    {
        doVerticalPower(arrayRef); // match 4 line type power Vertical line
        return false;
    }

    public override bool powerMatched(int[] posA, int[] posB, bool execute, GamePiece thisGp, GamePiece otherGp)
    {
        if (otherGp.pd is VerticalPiece || otherGp.pd is HorizontalPiece)
        {
            if (execute) StartCoroutine(doPowerMergeHV(posA, posB, thisGp.master, otherGp.master)); // do a power merge power

            return true; // <--- has power merge
        }
        return false; // <--- no power merge
    }

    public override bool matchConditions(int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY, List<Board> linkedCubesTRBL, List<Board> linkedCubesTLBR)
    {
        if (linkedCubesX.Count > 2 || linkedCubesTLBR.Count > 2 || linkedCubesTRBL.Count > 2)
        { // 4 match in a row
            gm.board[xPos, yPos].convertToSpecial(this); // makes the cube a special piece
            gm.board[xPos, yPos].panelHit();

            //lock the piece for just created power piece
            gm.lockJustCreated(xPos, yPos, 0.3f);
            GP_TrayBasket trayBasket = FindObjectOfType<GP_TrayBasket>();
            StartCoroutine(trayBasket.AddBasket(1, gm.board[xPos, yPos].position,0));
            return true;
        }
        return false;
    }

    //
    // POWER DEFINITION
    //

    // match 4 line type power Vertical line
    void doVerticalPower(int[] arrayRef)
    {
        float delay = 0f;
        float delayIncreament = 0.1f; // the delay of each piece being destroyed.
        gm.animScript.doAnim(animType.ARROWV, arrayRef[0], arrayRef[1]); // perform anim
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.horizontal);

        // the top of this board...
        foreach (Board _board in gm.iBoard(arrayRef).getAllBoardInDirection(BoardDirection.Top))
        {
            gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
            delay += delayIncreament;
        }
        delay = 0f; // reset the delay
        // the bottom of this board...
        foreach (Board _board in gm.iBoard(arrayRef).getAllBoardInDirection(BoardDirection.Bottom))
        {
            gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
            delay += delayIncreament;
        }
    }

    // power merge ability code
    IEnumerator doPowerMergeHV(int[] posA, int[] posB, Board thisBd, Board otherBd)
    {
        gm.mergePieces(posA, posB, false); // merge effect
        yield return new WaitForSeconds(gm.gemSwitchSpeed); // wait for merge effect

        gm.destroyInTimeMarked(thisBd, 0.1f, scorePerCube);
        gm.destroyInTimeMarked(otherBd, 0.1f, scorePerCube);

        float delay = 0f; // the delay variable we are using...
        float delayIncreament = 0.1f; // the delay of each piece being destroyed.
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.horizontal);
        doVerticalPower(posB); // perform basic power...
        // then perform more power on top of basic
        gm.animScript.doAnim(animType.ARROWH, posB); // perform anim
        delay = 0f; // reset the delay
        // the Left of this board...
        foreach (Board _board in gm.iBoard(posB).getAllBoardInDirection(BoardDirection.Left))
        {
            gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
            delay += delayIncreament;
        }
        delay = 0f; // reset the delay
        // the Right of this board...
        foreach (Board _board in gm.iBoard(posB).getAllBoardInDirection(BoardDirection.Right))
        {
            gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
            delay += delayIncreament;
        }
    }
}
