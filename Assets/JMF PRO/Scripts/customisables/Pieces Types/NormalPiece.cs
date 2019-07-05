using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("JMF/Pieces/NormalPiece")]
public class NormalPiece : PieceDefinition {

	public bool weightedSpawn = false;
	[Range(0,100)]
	public List<int> weights = new List<int>(9);
	int totalWeight = 0; // variable to hold the total weights
	int selected = 0; // a variable to store the selected random range for weights
	int addedWeight = 0; // a variable to store the cumulative added weight for calculations

	void Start(){
		// run once weighted calculation...
		totalWeight = 0; // reset the value first...
		for(int z = 0; z < gm.NumOfActiveType; z++){ // adds all available skin based on active type
			if(z < weights.Count){ // ensure we have allocated weights and add to the list
				totalWeight += weights[z];
			}
		}
	}

	public override PieceDefinition chanceToSpawnThis (int x, int y)
	{
		if(weightedSpawn) return this; // if enabled, use assigned weights
		return null; // else, random behaviour
	}

	public override int skinToUseDuringSpawn (int x, int y)
	{
		selected = Random.Range(1,totalWeight+1); // the selected weight by random
		addedWeight = 0; // resets the value first...
		for(int z = 0; z < weights.Count; z++){
			addedWeight+= weights[z];
			if(weights[z] > 0 && addedWeight > selected){
				return z; // found the skin we want to use based on the selected weight
			}
		}
		return 0; // failsafe ...
	}

	public override bool performPower(int[] arrayRef){
		// no power to perform
		return false;
	}

	public override bool powerMatched(int posX1, int posY1, int posX2, int posY2, bool execute,
	                                  PieceDefinition thisPd, PieceDefinition otherPd){
		// no power to perform
		return false;
	}

	public override bool matchConditions (int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY, List<Board> linkedCubesTRBL, List<Board> linkedCubesTLBR){
		if ( linkedCubesX.Count > 1 || linkedCubesY.Count > 1
		    || linkedCubesTLBR.Count > 1 || linkedCubesTRBL.Count > 1) { // 3 matching pieces

			gm.board[xPos,yPos].destroyBox(); // nothing special...
			return true;
		}
		return false;
	}
}
