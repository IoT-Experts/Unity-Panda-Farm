using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("JMF/Pieces/TreasurePiece")]
public class TreasurePiece : PieceDefinition
{
    WinningConditions wc { get { return JMFUtils.wc; } }

    public override PieceDefinition chanceToSpawnThis(int x, int y)
    {
        if (wc.canSpawnTreasure())
        {
            return this;
        }
        return null;
    }

    public override void onPieceCreated(GamePiece gp)
    {
        wc.treasureList.Add(gp);

    }

    public override void onPieceDestroyed(GamePiece gp)
    {
        //   particleEmitter; 
        // play audio visuals
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.treasureCollectedFx);

        gm.animScript.doAnim(animType.TREASURECOLLECTED, gp.master.arrayRef[0], gp.master.arrayRef[1]); // instantiate this anim
    }

    public override int skinToUseDuringSpawn(int x, int y)
    {
        wc.treasuresSpawned++; // a treasure is spawned... increment the counter
        // use the treasureCount; or loop skins if exceed the skin number
        return (wc.treasuresSpawned % (skin.Length));
    }

    public override bool performPower(int[] arrayRef)
    {
        // no power to perform
        return false;
    }

    public override bool powerMatched(int posX1, int posY1, int posX2, int posY2, bool execute,
                                      PieceDefinition thisPd, PieceDefinition otherPd)
    {
        // no power to perform
        return false;
    }

    public override bool matchConditions(int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY, List<Board> linkedCubesTRBL, List<Board> linkedCubesTLBR)
    {
        return false;
    }
}
