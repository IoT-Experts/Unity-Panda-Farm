using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("JMF/Pieces/HorizontalPiece")]
public class HorizontalPiece : PieceDefinition
{

    public override bool performPower(int[] arrayRef)
    {
        doHorizontalPower(arrayRef); // match 4 line type power Horizontal line
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
        if (linkedCubesY.Count > 2)
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

    void doHorizontalPower(int[] arrayRef)
    {

        float delay = 0f;
        float delayIncreament = 0.1f; // the delay of each piece being destroyed.

        gm.animScript.doAnim(animType.ARROWH, arrayRef[0], arrayRef[1]); // perform anim
        //gm.audioScript.arrowSoundFx.play(); // play arrow sound fx
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.horizontal);

        // the left of this board...
        foreach (Board _board in gm.iBoard(arrayRef).getAllBoardInDirection(BoardDirection.Left))
        {
            gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
            delay += delayIncreament;
        }
        delay = 0f; // reset the delay
        // the right of this board...
        foreach (Board _board in gm.iBoard(arrayRef).getAllBoardInDirection(BoardDirection.Right))
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
       // gm.audioScript.arrowSoundFx.play(); // play arrow sound fx
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.horizontal);

        doHorizontalPower(posB); // perform basic power...

        gm.animScript.doAnim(animType.ARROWV, posB); // visuals for the v-power
        // then perform more power on top of basic power
        // the top of this board...
        foreach (Board _board in gm.iBoard(posB).getAllBoardInDirection(BoardDirection.Top))
        {
            gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
            delay += delayIncreament;
        }
        delay = 0f; // reset the delay
        // the bottom of this board...
        foreach (Board _board in gm.iBoard(posB).getAllBoardInDirection(BoardDirection.Bottom))
        {
            gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
            delay += delayIncreament;
        }
    }
}
