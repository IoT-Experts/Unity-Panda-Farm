using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * PieceDefinition main class
 * ==========================
 * 
 * it is the holder as well as the definition of all its children.
 * There are functions all children must implement as well as certain functions
 * that users can change to suit their piece properly.
 * 
 * 
 */


public abstract class PieceDefinition : MonoBehaviour {

	public bool createdByMatch = true;
	public bool isSpecial = false;
	public bool isDestructible = true;
	public bool allowGravity = true;
	public bool landslideEffect = false;
	public bool ignoreReset = false;
	public int scorePerCube = 0;
	public GameObject[] skin; // how the piece will look like
	[HideInInspector] public GameManager gm {get{return JMFUtils.gm;}}

	//
	// All child class must specify these characteristics
	//

	public abstract bool matchConditions(int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY,
	                                     List<Board> linkedCubesTRBL, List<Board> linkedCubesTLBR);


	//
	//  Virtual functions that users can override
	//  or leave it as default behaviours
	//

	// power call for pieces
	public virtual bool performPower(int[] arrayRef){
		return false; // default is nothing..
	}

	// old powerMatched function..
	public virtual bool powerMatched(int posX1, int posY1, int posX2, int posY2, bool execute,
	                                 PieceDefinition pdMain, PieceDefinition pdSub){
		return false; // default is nothing..
	}

	// OVERLOADED new powerMatched function..
	public virtual bool powerMatched (int[] posA, int[] posB, bool execute, GamePiece thisGp, GamePiece otherGp) {
		return false; // default is nothing..
	}

	// called by Board during GameManager game-start phase
	// different from Start() as that is unity start, not neccessarily the game is set-up yet
	public virtual void onGameStart(Board board){
		// do nothing....
	}

	// called by GamePiece during creation of a type
	public virtual void onPieceCreated(GamePiece gp){
		// do nothing....
	}

	// called by GamePiece during destruction of a type
	public virtual void onPieceDestroyed(GamePiece gp){
		// do nothing...
	}

	// called by PieceTracker->JMFRelay during piece clicked
	public virtual void onPieceClicked(GamePiece gp){
		// do nothing...
	}

	// called by GameManager when player makes the next move
	public virtual void onPlayerMove(Board board) {
		// do nothing...
	}

	// called by GameManager when board stabilize and gets a suggestion
	public virtual void onBoardStabilize(Board board) {
		// do nothing...
	}
	// Optional piece splash function when a piece is destroyed
	public virtual void splashDamage(Board board){
		// do nothing...
	}

	// for external script to call (mainly GamePiece.cs) to call which skin to use
	public virtual GameObject getSkin(int num){
		return skin[num];
	}

	// when spawning a new piece by gravity, chance to spawn a type defined...
	public virtual PieceDefinition chanceToSpawnThis(int x, int y){
		// ** x / y is the board position being called for spawning...
		return null; // default does nothing... will create a normal piece instead
	}

	// different from getSkin() ... this is for piece to specify 
	// the skin to use during spawning new piece ** when you use chanceToSpawnThis()
	public virtual int skinToUseDuringSpawn(int x, int y){
		// ** x / y is the board position being called for spawning...
		return 0;
	}

	// user can further specify the position of the object on top of the default if needed
	public virtual void extraPiecePositioning(GameObject thisPiece){
		// default is no extra positioning
	}

	// the default check pattern for match types... checks across and top-down for similar pieces
	public virtual void checkPattern(int xPos, int yPos, int checkNum){
		if(!gm.board[xPos,yPos].isFilled){
			return; // null piece... quit checking this position
		} 
		if(!gm.pieceTypes[checkNum].createdByMatch ||
		   gm.board[xPos,yPos].piece.pd.isSpecial ||
		   !gm.board[xPos,yPos].canBeMatched() ){
			return; // not a piece that allows matching
		}

		// variables to keep track of the match potentials
		List<Board> linkedCubesX = new List<Board>(); // collections of linked cubes for rows
		List<Board> linkedCubesY = new List<Board>(); // collections of linked cubes for columns
		List<Board> linkedCubesDiagTRBL = new List<Board>(); // collections of linked cubes for diagonal TRBL
		List<Board> linkedCubesDiagTLBR = new List<Board>(); // collections of linked cubes for diagonal TLBR

		int matchingRows = 0;
		int matchingCols = 0;
        //int matchingDiagTRBL = 0;
        //int matchingDiagTLBR = 0;

		int slotNum = gm.board[xPos,yPos].piece.slotNum;

		// step 1 : check up/down (universal to both square & hex board
		foreach(Board _board in gm.board[xPos,yPos].getAllBoardInDirection(BoardDirection.Top)){
			if ( _board.canBeMatched() && _board.piece.slotNum == slotNum) {
				linkedCubesY.Add(_board); // remember this board box
				matchingCols++; // increase linked counter
			} else {
				break; // exit loop as no more match this side...
			}
		}

		foreach(Board _board in gm.board[xPos,yPos].getAllBoardInDirection(BoardDirection.Bottom)){
			if ( _board.canBeMatched() && _board.piece.slotNum == slotNum) {
				linkedCubesY.Add(_board); // remember this board box
				matchingCols++; // increase linked counter
			} else {
				break; // exit loop as no more match this side...
			}
		}

		if ( matchingCols < 2 ) { // means less than 2 similar cube in the row
			linkedCubesY.Clear(); // forget the marked boxes
		}

		// step 2 : board type specific check
        foreach (Board _board in gm.board[xPos, yPos].getAllBoardInDirection(BoardDirection.Left))
        {
            if (_board.canBeMatched() && _board.piece.slotNum == slotNum)
            {
                linkedCubesX.Add(_board); // remember this board box
                matchingRows++; // increase linked counter
            }
            else
            {
                break; // exit loop as no more match this side...
            }
        }

        foreach (Board _board in gm.board[xPos, yPos].getAllBoardInDirection(BoardDirection.Right))
        {
            if (_board.canBeMatched() && _board.piece.slotNum == slotNum)
            {
                linkedCubesX.Add(_board); // remember this board box
                matchingRows++; // increase linked counter
            }
            else
            {
                break; // exit loop as no more match this side...
            }
        }

        if (matchingRows < 2)
        { // means less than 2 similar cube in the row
            linkedCubesX.Clear(); // forget the marked boxes
        }		
		//
		// step 3 : finalize results
		//
		gm.validateMatch(checkNum, xPos, yPos, linkedCubesX,linkedCubesY, linkedCubesDiagTRBL, linkedCubesDiagTLBR);
	}
}
