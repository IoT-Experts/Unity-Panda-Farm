using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("JMF/Pieces/BombPiece")]
public class BombPiece : PieceDefinition
{

    public override bool performPower(int[] arrayRef)
    {
        doBombPower(arrayRef, 1); // match T line type power ( destroys surrounding pieces 3x3 area)
        return false;
    }


    public override bool powerMatched(int[] posA, int[] posB, bool execute, GamePiece thisGp, GamePiece otherGp)
    {
        if (otherGp.pd is VerticalPiece || otherGp.pd is HorizontalPiece)
        {
            if (execute) StartCoroutine(doPowerMergeX(posA, posB, 1));
            return true;
        }
        if (otherGp.pd is BombPiece)
        {
            if (execute) StartCoroutine(doPowerMergeXX(posA, posB, 2, thisGp, otherGp));
            return true;
        }
        return false;
    }

    public override bool matchConditions(int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY, List<Board> linkedCubesTRBL, List<Board> linkedCubesTLBR)
    {
        int chainMatch = 0; // how many chain are making a cross...
        if (linkedCubesX.Count > 1) chainMatch++;
        if (linkedCubesY.Count > 1) chainMatch++;
        if (linkedCubesTLBR.Count > 1) chainMatch++;
        if (linkedCubesTRBL.Count > 1) chainMatch++;
        if (chainMatch > 1)
        { // + or T or L-type match special pieces
            gm.board[xPos, yPos].convertToSpecial(this); // makes the cube a special piece
            gm.board[xPos, yPos].panelHit();

            //lock the piece for just created power piece
            gm.lockJustCreated(xPos, yPos, 0.3f);
            GP_TrayBasket trayBasket = FindObjectOfType<GP_TrayBasket>();
            StartCoroutine(trayBasket.AddBasket(2, gm.board[xPos, yPos].position, 0));
            return true;
        }
        return false;
    }

    // POWER DEFINITION
    //

    // customisable bomb power with radius setting
    public void doBombPower(int[] arrayRef, int radius)
    {
        gm.animScript.doAnim(animType.BOMB, arrayRef[0], arrayRef[1]); // perform anim
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.bombSoundFx);


        // all the surrounding neighbour boards...
        foreach (Board _board in gm.getBoardsFromDistance(arrayRef, 1, radius))
        {
            gm.destroyInTime(_board.arrayRef, 0.1f, scorePerCube);
        }
    }

    // power merge ability code
    IEnumerator doPowerMergeXX(int[] posA, int[] posB, int radius, GamePiece thisGp, GamePiece otherGp)
    {
        gm.mergePieces(posA, posB, true); // merge effect
        yield return new WaitForSeconds(gm.gemSwitchSpeed); // wait for merge effect

        gm.destroyInTimeMarked(posA, 2.1f, scorePerCube);
        gm.destroyInTimeMarked(posB, 2.1f, scorePerCube);

        // visual effect for a time bomb
        Vector3 newSize = Vector3.Scale(thisGp.thisPiece.transform.localScale, new Vector3(1.45f, 1.45f, 1f));
        LeanTween.scale(thisGp.thisPiece, newSize, 0.5f).setLoopPingPong();
        LeanTween.scale(otherGp.thisPiece, newSize, 0.5f).setLoopPingPong();
        thisGp.thisPiece.GetComponent<PieceTracker>().enabled = false;
        otherGp.thisPiece.GetComponent<PieceTracker>().enabled = false;

        doBombPower(posA, radius); // normal big blast w/0 arrows
        doBombPower(posB, radius); // normal big blast w/0 arrows

        yield return new WaitForSeconds(2f);
        doBombPower(thisGp.master.arrayRef, 1); // normal blast
        doBombPower(otherGp.master.arrayRef, 1); // normal blast
    }

    // power merge ability code
    IEnumerator doPowerMergeX(int[] posA, int[] posB, int radius)
    {

        gm.mergePieces(posA, posB, false); // merge effect
        yield return new WaitForSeconds(gm.gemSwitchSpeed); // wait for merge effect

        gm.destroyInTimeMarked(posA, 0.1f, scorePerCube);
        gm.destroyInTimeMarked(posB, 0.1f, scorePerCube);

        doBombPower(posB, radius); // do bomb power with specified radius

        // arrow power...
        float delay = 0f; // the delay variable we are using...
        float delayIncreament = 0.1f; // the delay of each piece being destroyed.
        //gm.audioScript.arrowSoundFx.play(); // play arrow sound fx
        //MusicControll.musicControll.Horizontal();
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.horizontal);

        //gm.animScript.doAnim(animType.ARROWV,posB); // perform anim
        gm.animScript.doAnim(animType.BOMBX3, posB);
        bool destroyThis = false; // variable to help skip the first board in the list

        // the top of this board...
        foreach (Board _board in gm.iBoard(posB).getAllBoardInDirection(BoardDirection.Top))
        {
            if (destroyThis) gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
            destroyThis = true;
            delay += delayIncreament;
        }
        delay = 0f; // reset the delay
        destroyThis = false; // help to skip the first board in the list...
        // the bottom of this board...
        foreach (Board _board in gm.iBoard(posB).getAllBoardInDirection(BoardDirection.Bottom))
        {
            if (destroyThis) gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
            destroyThis = true;
            delay += delayIncreament;
        }


        delay = 0f; // reset the delay
        destroyThis = false; // help to skip the first board in the list...
        // the Left of this board...
        foreach (Board _board in gm.iBoard(posB).getAllBoardInDirection(BoardDirection.Left))
        {
            if (destroyThis) gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
            destroyThis = true;
            delay += delayIncreament;
        }
        delay = 0f; // reset the delay
        destroyThis = false; // help to skip the first board in the list...
        // the Right of this board...
        foreach (Board _board in gm.iBoard(posB).getAllBoardInDirection(BoardDirection.Right))
        {
            if (destroyThis) gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
            destroyThis = true;
            delay += delayIncreament;
        }

        if (gm.iBoard(posB).top != null)
        {
            //gm.animScript.doAnim(animType.ARROWH, gm.iBoard(posB).top.arrayRef); // perform anim
            delay = 0f; // reset the delay
            destroyThis = false; // help to skip the first board in the list...
            // up+1 & left
            foreach (Board _board in gm.iBoard(posB).top.getAllBoardInDirection(BoardDirection.Left))
            {
                if (destroyThis) gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
                destroyThis = true;
                delay += delayIncreament;
            }
            delay = 0f; // reset the delay
            destroyThis = false; // help to skip the first board in the list...
            // up+1 & right
            foreach (Board _board in gm.iBoard(posB).top.getAllBoardInDirection(BoardDirection.Right))
            {
                if (destroyThis) gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
                destroyThis = true;
                delay += delayIncreament;
            }
        }

        if (gm.iBoard(posB).bottom != null)
        {
            //gm.animScript.doAnim(animType.ARROWH, gm.iBoard(posB).bottom.arrayRef); // perform anim
            delay = 0f; // reset the delay
            destroyThis = false; // help to skip the first board in the list...
            // down+1 & left
            foreach (Board _board in gm.iBoard(posB).bottom.getAllBoardInDirection(BoardDirection.Left))
            {
                if (destroyThis) gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
                destroyThis = true;
                delay += delayIncreament;
            }
            delay = 0f; // reset the delay
            destroyThis = false; // help to skip the first board in the list...
            // down+1 & right
            foreach (Board _board in gm.iBoard(posB).bottom.getAllBoardInDirection(BoardDirection.Right))
            {
                if (destroyThis) gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
                destroyThis = true;
                delay += delayIncreament;
            }
        }

        if (gm.iBoard(posB).left != null)
        {
            //gm.animScript.doAnim(animType.ARROWV, gm.iBoard(posB).left.arrayRef); // perform anim
            delay = 0f; // reset the delay
            destroyThis = false; // help to skip the first board in the list...
            // left+1 & up
            foreach (Board _board in gm.iBoard(posB).left.getAllBoardInDirection(BoardDirection.Top))
            {
                if (destroyThis) gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
                destroyThis = true;
                delay += delayIncreament;
            }
            delay = 0f; // reset the delay
            destroyThis = false; // help to skip the first board in the list...
            // left+1 & down
            foreach (Board _board in gm.iBoard(posB).left.getAllBoardInDirection(BoardDirection.Bottom))
            {
                if (destroyThis) gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
                destroyThis = true;
                delay += delayIncreament;
            }
        }

        if (gm.iBoard(posB).right != null)
        {
            //gm.animScript.doAnim(animType.ARROWV, gm.iBoard(posB).right.arrayRef); // perform anim
            delay = 0f; // reset the delay
            destroyThis = false; // help to skip the first board in the list...
            // right+1 & up
            foreach (Board _board in gm.iBoard(posB).right.getAllBoardInDirection(BoardDirection.Top))
            {
                if (destroyThis) gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
                destroyThis = true;
                delay += delayIncreament;
            }
            delay = 0f; // reset the delay
            destroyThis = false; // help to skip the first board in the list...
            // right+1 & down
            foreach (Board _board in gm.iBoard(posB).right.getAllBoardInDirection(BoardDirection.Bottom))
            {
                if (destroyThis) gm.destroyInTime(_board.arrayRef, delay, scorePerCube);
                destroyThis = true;
                delay += delayIncreament;
            }
        }
    }
}
