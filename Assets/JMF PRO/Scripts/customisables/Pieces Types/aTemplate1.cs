using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * PieceDefinition template class
 * ==========================
 * 
 * use this as a guide template to create your own custom piece
 * 
 */

/// <summary>
/// 
/// BELOW ARE THE FUNCTIONS THAT IS USEFUL
/// 
/// ==============
/// WARNING, wrap these in StartCoroutine( "x" );
/// gm.destroyInTime()    <--- this is a function that accepts 4 properties. 
///                       1. the x position.
///                       2. the y position.
///                       3. the delay before the piece is destroyed ( in float value )
///                       4. the score added once the piece is destroyed.
/// gm.destroyInTimeMarked()  <--- same as destroyInTime(), but pieces here will be marked and other
///                                destroy calls will not affect it. (i.e., another bomb will not pre-maturely
///                                destroy the piece.)
/// ===============
/// 
/// gm.mergePieces()   <--- just a visual affect to swap / move the piece and merge them visually
///                         accepts 5 variables :-
///                         var 1 & 2 = the X and the Y position of the 1st board
///                         var 3 & 5 = the X and the Y position of the 2nd board
///                         var 5 = is a bool that determines if both pieces are swapping or a one sided merge visual
/// 
/// yield return new WaitForSeconds(gm.gemSwitchSpeed);  <--- use this to wait for the visual effect above...
/// 
/// 
/// gm.boardWidth / gm.boardHeight    <--- the width and height of the current board
/// gm.board[x,y]    <--- use this to reference the board if you needed more board properties
/// 
/// gm.lockJustCreated(posX1,posY1,0f)   <--- use this to lock the pieces so that it will not be destroyed by
///                                           a chain explosion so quickly.
///                                           var 1 & var 2 = the board array position [x,y]
///                                           var 3 = the delay before it is unlocked
/// 
/// gm.board[x,y].piece.thisPiece.      <--- technique to allow/disallow users to drag the piece after special powers
///     GetComponent<PieceTracker>().enabled
/// 
/// gm.audioScript.playSound(PlayFx.fxtype) <--- to play sound effects defined in AudioPlayer script
/// gm.animScript.doAnim(animType,x,y) <--- to play your desired anim defined in CustomAnimations script
/// 
/// </summary>

public class aTemplate1 : PieceDefinition {

	public override bool performPower(int[] arrayRef){
		/* your function call here - define your own power...
		 * e.g. doMyPower(arrayRef);
		 * 
		 * arrayRef is the board position of the piece..
		 * arrayRef[0] = x position; arrayRef[1] = y position.
		 * use it to call gm.board[x,y]
		 * 
		 */

		// return calls...
		return false; // default call - will destroy the piece immediately after this ( AKA after calling the power )
//		return true; // Only if you do not wish the custom piece to be destroyed instantly
		/*
		 * IMPORTANT : 
		 * you must call gm.destroyInTimeMarked(x,y, delay, mScore);
		 * either here or in your power function to manually destroy the piece
		 * ( that is if you returned false; )
		 * 
		*/ 
	}
	
	public override bool powerMatched(int[] posA, int[] posB, bool execute, GamePiece thisGp, GamePiece otherGp){
		/* 
		 * posA = the board position of the selected swap piece
		 * posB = the board position of the swap partner piece
		 * 
		 * I used "if (execute)" to check if it is a powerMatch call or a suggesting piece call
		 * 
		 * thisGp refers to the this GamePiece - if you need it
		 * otherGp refers to the partner GamePiece it is trying to Merge with...
		 */

//		if(otherGp.pd is *another PieceDefinition*){
//			if(execute) StartCoroutine( doPowerMerge(posA,posB,thisGp,otherGp));
			// NOTE : check other PieceDefinition scripts for sample doPowerMerge() behaviours
		//		return true; // if there is powerMatching...it will NOT continue to perform normal match
//		}
		return false; // if there is NO powerMatching... it will continue to perform normal match
	}
	
	public override bool matchConditions (int xPos, int yPos, List<Board> linkedCubesX, List<Board> linkedCubesY, List<Board> linkedCubesTRBL, List<Board> linkedCubesTLBR){

		/*
		 * xPos / yPos = board position - use as gm.board[xPos,yPos]
		 * linkedCubesX = matching piece horizontally - use linkedCubesX.Count to get the amount
		 * linkedCubesX = matching piece vertically - use linkedCubesY.Count to get the amount
		 * linkedCubesTRBL = matching piece from TopRight > BottomLeft - use linkedCubesTRBL.Count to get the amount
		 * linkedCubesTLBR = matching piece from TopLeft > BottomRight - use linkedCubesTLBR.Count to get the amount
		 * 
		 */
//
//		if ( your criteria here ) {
//
//			
//			gm.board[xPos,yPos].convertToSpecial(this, *an int* ); // makes the cube a special piece
//			/* "an int" is the num array of skin you wish to use.
//			 * if it is not a special piece and you want to follow back the color, use
//			 * gm.board[xPos,yPos].convertToSpecial(this);
//			 */ 
//
//			
//			gm.board[xPos,yPos].panelHit(); // hit the panel durability - ONLY IF YOU ARE NOT DESTROYING THE PIECE
//		    // else,
//			gm.board[xPos,yPos].destroyBox();
//			
//			//lock the piece for just created power piece
//			StartCoroutine(gm.lockJustCreated(xPos,yPos,0.3f)); // only if you dont want a premature trigger
//			return true; // return true if you did something... it will destroy the rest of the linked match
//		}
		return false; // default return false ... nothing will happen
	}

	
	// ============================================================
	//  Virtual functions that users can override
	//  or leave it as default behaviours
	//
	// P.S.> you can delete this entire section / any specific function if you are not changing anything...
	// ============================================================

	// called by Board during GameManager game-start phase
	// different from Start() as that is unity start, not neccessarily the game is set-up yet
	public override void onGameStart(Board board){
		// do nothing....
	}
	
	// called by GamePiece during creation of a type
	public override void onPieceCreated(GamePiece gp){
		// default does nothing...
		// your own code here if you need
	}
	
	// called by GamePiece during destruction of a type
	public override void onPieceDestroyed(GamePiece gp){
		// default do nothing...
		// your own code here if you need
	}

	// called by PieceTracker->JMFRelay during piece clicked
	public override void onPieceClicked (GamePiece gp){
		// default do nothing...
		// your own code here if you need
	}

	// called by GameManager when player makes the next move
	public override void onPlayerMove(Board board) {
		// default do nothing...
		// your own code here if you need
	}
	
	// called by GameManager when board stabilize and gets a suggestion
	public override void onBoardStabilize(Board board) {
		// default do nothing...
		// your own code here if you need
		// think of this as something like "on the next turn"...
	}
	// Optional piece splash function when a piece is destroyed
	public override void splashDamage(Board board){
		// default do nothing...
		// your own code here if you need
		// splash when a match is formed...
	}
	
	// for external script to call (mainly GamePiece.cs) to call which skin to use
	public override GameObject getSkin(int num){
		return skin[num]; // default behaviour will use back the same color skin
	}

	// different from getSkin() ... this is for piece to specify 
	// the skin to use during spawning new piece ** when you use chanceToSpawnThis()
	public override int skinToUseDuringSpawn(int x, int y){
		// ** x / y is the board position being called for spawning...
		return 0; // default behaviour when called, return skin 0
	}
	
	// when spawning a new piece by gravity, chance to spawn this type...
	public override PieceDefinition chanceToSpawnThis(int x, int y){
		// ** x / y is the board position being called for spawning...

		return null; // default does nothing... will create a normal piece instead
		/*
		 * else, 
		 * if( your criteria here...)
		 * return this; // spawns this piece if conditions are met...
		 * WARNING - beware of returning true all the time, then it will only spawn this piece!
		 */
	}
	

	
	// user can further specify the position of the object on top of the default if needed
	public override void extraPiecePositioning(GameObject thisPiece){
		// default is no extra positioning

		// else, in front of the normal board position
		// thisPiece.transform.localPosition += new Vector3(0,0,-1*thisPiece.transform.localScale.z);

		// else, behind the normal board position
		// thisPiece.transform.localPosition += new Vector3(0,0,1*thisPiece.transform.localScale.z);
	}
	
	// the default check pattern for match types... checks across and top-down for similar pieces
	public override void checkPattern(int xPos, int yPos, int checkNum){

		base.checkPattern(xPos,yPos,checkNum); // default call...

		// BELOW IS THE DEFAULT CHECK PATTERN .... for your reference...
		// the base call above already calls the default as shown here.
		//
		// xPos/yPos = board position calling the check
		// checkNum = the pieceDefinition type being called for the check criteria - you do not need to modify this
		// checkNum is passed to gm.validateMatch(checkNum, xPos, yPos, linkedCubesX,linkedCubesY);

//		if(gm.board[xPos,yPos].piece == null || gm.board[xPos,yPos].piece.pd == null){
//			return; // null piece... quit checking this position
//		} 
//		if(!gm.board[xPos,yPos].piece.pd.createdByMatch){
//			return; // not a piece that allows matching
//		}
//		
//		// variables to keep track of the match potentials
//		List<Board> linkedCubesX = new List<Board>(); // collections of linked cubes for rows
//		List<Board> linkedCubesY = new List<Board>(); // collections of linked cubes for columns
//		int matchingRows = 0;
//		int matchingCols = 0;
//		int slotNum = gm.board[xPos,yPos].piece.slotNum;
//		
//		// step 1 : check rows
//		//
//		for (int x = (xPos-1) ; x >= 0; x--) { //check the left side of the cube ( required for T shape matches)
//			if ( gm.board[x,yPos].canBeMatched() && gm.board[x,yPos].piece.slotNum == slotNum) {
//				linkedCubesX.Add(gm.board[x,yPos]); // remember this board box
//				matchingRows++; // increase linked counter
//			} else {
//				break; // exit loop as no more match this side...
//			}
//		}
//		for (int x = (xPos+1) ; x < gm.boardWidth ; x++) { //check the right side of the cube 
//			if ( gm.board[x,yPos].canBeMatched() && gm.board[x,yPos].piece.slotNum == slotNum) {
//				linkedCubesX.Add(gm.board[x,yPos]); // remember this board box
//				matchingRows++; // increase linked counter
//			} else {
//				break; // exit loop as no more match this side...
//			}
//		}
//		if ( matchingRows < 2 ) { // means less than 2 similar cube in the row
//			linkedCubesX.Clear(); // forget the marked boxes
//		}
//		
//		//
//		// step 2 : check columns
//		//
//		for (int y = (yPos-1) ; y >= 0; y--) { //check the bottom side of the cube ( required for T shape matches)
//			if ( gm.board[xPos,y].canBeMatched() && gm.board[xPos,y].piece.slotNum == slotNum) {
//				linkedCubesY.Add(gm.board[xPos,y]); // remember this board box
//				matchingCols++; // increase linked counter
//			} else {
//				break; // exit loop as no more match this side...
//			}
//		}
//		for (int y = (yPos+1) ; y < gm.boardHeight ; y++) { //check the top side of the cube 
//			if ( gm.board[xPos,y].canBeMatched() && gm.board[xPos,y].piece.slotNum == slotNum) {
//				linkedCubesY.Add(gm.board[xPos,y]); // remember this board box
//				matchingCols++; // increase linked counter
//			} else {
//				break; // exit loop as no more match this side...
//			}
//		}
//		if ( matchingCols < 2 ) { // means less than 2 similar cube in the Column
//			linkedCubesY.Clear(); // forget the marked boxes
//		}
//		
//		//
//		// step 3 : finalize results
//		//
//		
//		gm.validateMatch(checkNum, xPos, yPos, linkedCubesX,linkedCubesY);
	}
}
