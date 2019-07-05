using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
[AddComponentMenu("JMF/Pieces/SpecialFive")]
public class SpecialFive : PieceDefinition
{

    public override bool performPower(int[] arrayRef)
    {
        StartCoroutine(specialFiveColored(arrayRef, gm.iBoard(arrayRef), Random.Range(0, gm.NumOfActiveType), 2f, true));
        return false;
    }

    public override bool powerMatched(int[] posA, int[] posB, bool execute, GamePiece thisGp, GamePiece otherGp)
    {
        if (otherGp.pd is NormalPiece)
        {
            if (execute) StartCoroutine(specialMergeFive(posA, posB, thisGp, otherGp, 2f)); // do a power merge power
            return true;
        }
        if (otherGp.pd is VerticalPiece || otherGp.pd is HorizontalPiece)
        {
            if (execute) StartCoroutine(doPowerMergeFiveVH(posA, posB, thisGp, otherGp, 1.5f)); // do a power merge power
            return true;
        }
        if (otherGp.pd is BombPiece)
        {
            if (execute) StartCoroutine(doPowerMergeFiveX(posA, posB, thisGp, otherGp)); // do a power merge power
            return true;
        }
        if (otherGp.pd is SpecialFive)
        {
            if (execute) StartCoroutine(doPowerMergeFiveFive(posA, posB, thisGp, otherGp)); // do a power merge power
            return true;
        }
        return false;
    }

    public override bool matchConditions(int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY, List<Board> linkedCubesTRBL, List<Board> linkedCubesTLBR)
    {
        if (linkedCubesX.Count > 3 || linkedCubesY.Count > 3 ||
            linkedCubesTLBR.Count > 3 || linkedCubesTRBL.Count > 3)
        { // 5 match special pieces
            gm.board[xPos, yPos].convertToSpecial(this, 0); // makes the cube a special piece
            gm.board[xPos, yPos].panelHit();

            //lock the piece for just created power piece
            gm.lockJustCreated(xPos, yPos, 0.3f);
            GP_TrayBasket trayBasket = FindObjectOfType<GP_TrayBasket>();
            StartCoroutine(trayBasket.AddBasket(5, gm.board[xPos, yPos].position, 0));
            return true;
        }
        return false;
    }

    public override void extraPiecePositioning(GameObject thisPiece)
    {
        thisPiece.transform.localPosition += new Vector3(0, 0, -1 * thisPiece.transform.localScale.z);
    }

    // Does the power merge colored. match 5 type power ( destroys specified param color )
    IEnumerator specialFiveColored(int[] arrayRef, Board thisBd, int slotNum, float delay, bool visuals)
    {
        if (visuals)
        { // if play visual and sound effect
            MusicControll.musicControll.MakeSound(MusicControll.musicControll.matchFiveSoundFx);

            gm.animScript.doAnim(animType.RAINBOW, arrayRef); // visual fx animation
        }
        //		thisBd.isFalling = true; // do not let it fall :)

        float delayPerPiece = 1.5f;
        yield return new WaitForSeconds(delay);

        foreach (Board board in gm.board)
        { // destroys the selected color in each board
            if (board.isFilled && !board.pd.isSpecial && board.piece.slotNum == slotNum && board.piece != null)
            {
                yield return new WaitForSeconds(0.05f);
                GameObject a = gm.animScript.InstanceFiveEffect(arrayRef);
                gm.destroyInTime(board, delayPerPiece, scorePerCube);
                Hashtable hh = new Hashtable();
                hh.Add("time", 1.5f);
                hh.Add("position", board.piece.position);
                iTween.MoveTo(a, hh);
            }
        }
        gm.destroyInTimeMarked(thisBd, delay, scorePerCube); // locks this piece & destroys after x seconds
    }

    // Does the power merge colored. match 5 type power ( destroys specified param color )
    IEnumerator specialMergeFive(int[] posA, int[] posB, GamePiece thisGp, GamePiece otherGp, float delay)
    {
        gm.mergePieces(posA, posB, false); // merge effect
        yield return new WaitForSeconds(gm.gemSwitchSpeed); // wait for merge effect
        StartCoroutine(specialFiveColored(posB, thisGp.master, otherGp.slotNum, 1f, true)); // calls the colored function
        thisGp.thisPiece.transform.localScale = new Vector3(0, 0);
    }

    // power merge ability code
    IEnumerator doPowerMergeFiveVH(int[] posA, int[] posB, GamePiece thisGp, GamePiece otherGp, float delay)
    {
        int slotNum = otherGp.slotNum;
        gm.mergePieces(posA, posB, true); // merge effect
        yield return new WaitForSeconds(gm.gemSwitchSpeed); // wait for merge effect
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.specialMatchSoundFx);

        List<GamePiece> toBeDestroyed = new List<GamePiece>(); // list of pieces to be destroyed
        thisGp.thisPiece.transform.localScale = new Vector3(0, 0);
        foreach (Board board in gm.board)
        {
            if (board.isFilled && !board.pd.isSpecial && board.piece.slotNum == slotNum)
            {
                if (Random.Range(0, 2) == 0)
                {// convert the piece to this type (either vertical or horizontal)
                    yield return new WaitForSeconds(0.2f);
                    if (board.piece != null)
                    {
                        board.piece.specialMe(gm.pieceManager.GetComponent<HorizontalPiece>());
                    }
                }
                else
                {
                    yield return new WaitForSeconds(0.2f);
                    if (board.piece != null)
                    {
                        board.piece.specialMe(gm.pieceManager.GetComponent<VerticalPiece>());
                    }
                }
                gm.animScript.doAnim(animType.CONVERTSPEC, board.arrayRef);
                toBeDestroyed.Add(board.piece);
            }
        }
        gm.destroyInTimeMarked(thisGp, toBeDestroyed.Count * 0.2f, scorePerCube); // locks this piece & destroys after x seconds
        yield return new WaitForSeconds(delay);
        int index; // variable to be used
        while (toBeDestroyed.Count > 0)
        { // if there are still converted boards in the list...
            index = Random.Range(0, toBeDestroyed.Count); // randomly pick a board from the list
            if (toBeDestroyed[index] != null && (toBeDestroyed[index].pd is HorizontalPiece ||
                                                toBeDestroyed[index].pd is VerticalPiece))
            { // if it's the correct piece...
                toBeDestroyed[index].master.destroyBox(); // destroys the piece that was previously converted
                yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            }
            toBeDestroyed.RemoveAt(index); // remove from the list after processed...
        }
    }

    // power merge ability code
    IEnumerator doPowerMergeFiveX(int[] posA, int[] posB, GamePiece thisGp, GamePiece otherGp)
    {
        int slotNum = otherGp.slotNum;
        gm.mergePieces(posA, posB, true); // merge effect
        yield return new WaitForSeconds(gm.gemSwitchSpeed); // wait for merge effect
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.bombSoundFx);

        Vector3 newSize = Vector3.Scale(thisGp.thisPiece.transform.localScale, new Vector3(1.45f, 1.45f, 1f));
        LeanTween.scale(thisGp.thisPiece, newSize, 0.5f).setLoopPingPong();
        thisGp.thisPiece.GetComponent<PieceTracker>().enabled = false;
        StartCoroutine(specialFiveColored(thisGp.master.arrayRef, thisGp.master, slotNum, 0f, false)); // color specific rainbow bust
        gm.pieceManager.GetComponent<BombPiece>().doBombPower(otherGp.master.arrayRef, 2); // do the T match (big ver.) power!
        yield return new WaitForSeconds(2f); // wait for 2 secs
        int color = Random.Range(0, gm.NumOfActiveType); // choose a random color..
        while (color == slotNum)
        {
            color = Random.Range(0, gm.NumOfActiveType); // make sure it's not the same color as previous
        }
        gm.destroyInTimeMarked(thisGp, 4f, scorePerCube); // destroys only after the delay
        StartCoroutine(specialFiveColored(thisGp.master.arrayRef, thisGp.master, color, 2f, true)); // blows up another color...
    }

    // power merge ability code
    IEnumerator doPowerMergeFiveFive(int[] posA, int[] posB, GamePiece thisGp, GamePiece otherGp)
    {
        gm.mergePieces(posA, posB, false); // merge effect
        yield return new WaitForSeconds(gm.gemSwitchSpeed); // wait for merge effect
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.bombSoundFx);
        float delayPerPiece = 0.05f;
        gm.animScript.doAnim(animType.BOMB, posB); // visual fx animation

        // destroy the special 5 piece to avoid re-occurence loop


        for (int x = 0; x < gm.boardWidth; x++)
        {
            for (int y = 0; y < gm.boardHeight; y++)
            {
                // code below fans out the destruction with the bomb being the epicentre
                if ((posB[0] - x) >= 0 && (posB[1] - y) >= 0)
                {
                    gm.destroyInTime(posB[0] - x, posB[1] - y, delayPerPiece * (x + y), scorePerCube);
                }
                if ((posB[0] + x) < gm.boardWidth && (posB[1] + y) < gm.boardHeight)
                {
                    gm.destroyInTime(posB[0] + x, posB[1] + y, delayPerPiece * (x + y), scorePerCube);
                }
                if ((posB[0] - x) >= 0 && (posB[1] + y) < gm.boardHeight)
                {
                    gm.destroyInTime(posB[0] - x, posB[1] + y, delayPerPiece * (x + y), scorePerCube);
                }
                if ((posB[0] + x) < gm.boardWidth && (posB[1] - y) >= 0)
                {
                    gm.destroyInTime(posB[0] + x, posB[1] - y, delayPerPiece * (x + y), scorePerCube);
                }
            }
            gm.destroyInTimeMarked(posA, 0f, scorePerCube);
            // destroy the special 5 piece to avoid re-occurence loop
            gm.destroyInTimeMarked(posB, 0f, scorePerCube);
        }
    }
}
