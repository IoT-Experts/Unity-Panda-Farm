using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("JMF/Pieces/SpecialSix")]
public class SpecialSix : PieceDefinition
{

    public override bool performPower(int[] arrayRef)
    {
        doPower6Match(arrayRef); // match 6 type power ( clears the entire board )
        return false;
    }

    public override bool powerMatched(int posX1, int posY1, int posX2, int posY2, bool execute,
                                      PieceDefinition thisPd, PieceDefinition otherPd)
    {
        if (otherPd.isDestructible)
        {
            if (execute) StartCoroutine(doPower6Merge(posX1, posY1, posX2, posY2)); // match 6 type power ( clears the entire board )
            return true;
        }
        return false;
    }

    public override bool matchConditions(int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY, List<Board> linkedCubesTRBL, List<Board> linkedCubesTLBR)
    {
        if (linkedCubesX.Count > 4 || linkedCubesY.Count > 4 ||
            linkedCubesTLBR.Count > 4 || linkedCubesTRBL.Count > 4)
        { // 5 match special pieces
            gm.board[xPos, yPos].convertToSpecial(this, 0); // makes the cube a special piece
            gm.board[xPos, yPos].panelHit();

            //lock the piece for just created power piece
            gm.lockJustCreated(xPos, yPos, 0.3f);
            return true;
        }
        return false;
    }

    //
    // POWER DEFINITION
    //

    // match 6 type power ( clears the entire board )
    public void doPower6Match(int[] pos)
    {
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.bombSoundFx);
        float delayPerPiece = 0.05f;
        int mScore = 50; // the score you want to give per destroyed box in this range
        gm.animScript.doAnim(animType.BOMB, pos[0], pos[1]); // visual fx animation

        // destroy the special 6 piece to avoid re-occurence loop
        gm.destroyInTimeMarked(pos[0], pos[1], 0, mScore);

        for (int x = 0; x < gm.boardWidth; x++)
        {
            for (int y = 0; y < gm.boardHeight; y++)
            {
                // code below fans out the destruction with the bomb being the epicentre
                if ((pos[0] - x) >= 0 && (pos[1] - y) >= 0)
                {
                    gm.destroyInTime(pos[0] - x, pos[1] - y, delayPerPiece * (x + y), mScore);
                }
                if ((pos[0] + x) < gm.boardWidth && (pos[1] + y) < gm.boardHeight)
                {
                    gm.destroyInTime(pos[0] + x, pos[1] + y, delayPerPiece * (x + y), mScore);
                }
                if ((pos[0] - x) >= 0 && (pos[1] + y) < gm.boardHeight)
                {
                    gm.destroyInTime(pos[0] - x, pos[1] + y, delayPerPiece * (x + y), mScore);
                }
                if ((pos[0] + x) < gm.boardWidth && (pos[1] - y) >= 0)
                {
                    gm.destroyInTime(pos[0] + x, pos[1] - y, delayPerPiece * (x + y), mScore);
                }
            }
        }
    }

    public IEnumerator doPower6Merge(int posX1, int posY1, int posX2, int posY2)
    {
        gm.mergePieces(posX1, posY1, posX2, posY2, false); // for visual effect mostly
        yield return new WaitForSeconds(gm.gemSwitchSpeed);
        doPower6Match(new int[2] { posX2, posY2 });
    }
}
