using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("JMF/Panels/PortalA")]
public class PortalA : PanelDefinition {

	public List<Board> boardA = new List<Board>(); // the board A - entry path reference
	public List<Board> boardB = new List<Board>(); // the board B - exit path reference

	GameManager gm{get{return JMFUtils.gm;}}


	void Awake(){
		boardA.Clear(); // clears any old references before scene start
		boardB.Clear(); // clears any old references before scene start
	}

	// called by Board during GameManager game-start phase
	// different from Start() as that is unity start, not neccessarily the game is set-up yet
	public override void onGameStart(Board board){
		for( int x = 0; x < board.gm.boardWidth; x++){
			for( int y = 0; y < board.gm.boardHeight; y++){
				if(board.gm.board[x,y].panel.pnd is PortalB){ // find the exit pair
					if(board.gm.board[x,y].panel.durability == board.panel.durability){ // matching exit pair
						boardA.Add(board); // save the entry pair reference
						boardB.Add(board.gm.board[x,y]); // save the exit pair reference
					}
				}
			}
		}
	}

	// optional onCreate function to define extra behaviours
	public override void onPanelCreate(BoardPanel bp){
		// add swirl tweening...
		LeanTween.rotateAround(bp.backPanel,Vector3.back,359f,3.0f);
	}
	// optional onDestroy function to define extra behaviours
	// not the same as being hit... this is when the panel is destroyed completely and changing types
	public override void onPanelDestroy(BoardPanel bp){
		// default does nothing...
	}
	// optional onPlayerMove called by GameManager when player makes the next move
	public override void onPlayerMove(BoardPanel bp){
		// default does nothing...
	}
	// optional onBoardStabilize called by GameManager when board stabilize and gets a suggestion
	public override void onBoardStabilize(BoardPanel bp) {
		// default does nothing...
	}

	// for external scripts to call, will indicate that the panel got hit
	public override bool gotHit(BoardPanel bp){
		return false; // do nothing~!
	}

	// for external scripts to call, if splash damage hits correct panel type, perform the hit
	public override bool splashDamage(BoardPanel bp){
		return false; // do nothing...
	}
	
	// function to check if pieces can fall into this board box
	// ( AKA piece want to come in? Welcome~! )
	public override bool allowsGravity(BoardPanel bp){
		int listNum = boardA.IndexOf(bp.master);
		if(listNum >= 0 ){
			int[] arrayRef = getExitPath(boardB[listNum]);
			if(arrayRef[0] >= 0 && arrayRef[0] < gm.boardWidth &&
			   arrayRef[1] >= 0 && arrayRef[1] < gm.boardHeight) { // within bounds
				if( boardA[listNum].isFilled && !boardB[listNum].isFilled
				   && !boardA[listNum].isFalling){ // if there is a piece ready to teleport
					boardB[listNum].piece = boardA[listNum].piece; // moves the piece in memory
					boardB[listNum].piece.master = boardB[listNum]; // sync the master data
					boardA[listNum].piece = null;

					// moves the piece visually instantly
					boardB[listNum].piece.thisPiece.transform.position = boardB[listNum].position;
					// cancels any tweening still running
					LeanTween.cancel(boardB[listNum].piece.thisPiece); // mostly its the after-effect drop

					boardB[listNum].isFalling = false; // reset board status ( as pre-caution )
					gm.dropPieces(arrayRef[0],arrayRef[1]); // start the gravity check on the other side
				}

				if(gm.countUnfilled(boardB[listNum].arrayRef[0],boardB[listNum].arrayRef[1],true) > 0
				   && !boardB[listNum].isFilled){
					return true; // still has boxes to fill, allow more in
				}
			}
		}
		return false;
	}
	
	// if the piece here can be used to form a match - self explanatory?
	public override bool isMatchable(BoardPanel bp){
		// your logic here ( if needed )
		return false;
	}
	
	// if the piece here can be switched around - self explanatory?
	public override bool isSwitchable(BoardPanel bp){
		// your logic here ( if needed )
		return false;
	}
	
	// if the piece here (if any) can be destroyed / Matched
	public override bool isDestructible(BoardPanel bp){
		// your logic here ( if needed )
		return true;
	}
	
	// function to check if pieces can be stolen from this box by gravity 
	// ( AKA piece leaving the box when gravity calls )
	public override bool isStealable(BoardPanel bp){
		// your logic here ( if needed )
		return false;
	}
	
	// function to for resetBoard() to know which panel can be resetted
	public override bool isFillable(BoardPanel bp){
		return false;
	}
	
	// function to check if this board is a solid panel
	// IMPORTANT, not the same of allowsGravity()~!
	// this function determines if pieces will landslide it's neighbouring piece to fill bottom blocks)
	public override bool isSolid(BoardPanel bp){
		// your logic here ( if needed )
		return true;
	}

	// function to play the audio visuals of this panel
	public override void playAudioVisuals (BoardPanel bp){
		// define your audio visual call here...
		// e.g. >
//		master.gm.audioScript.playSound(PlayFx.YOUR DEFINED AUDIO);
//		master.gm.animScript.doAnim(animType.YOUR DEFINED ANIM, master.arrayRef[0], master.arrayRef[1] );
	}
	

	int[] getExitPath(Board board){
		int[] newRef = new int[] {board.arrayRef[0], board.arrayRef[1] }; // make a copy of the array position

		// compensate for gravity
		switch(gm.currentGravity){
		case Gravity.LEFT:
			newRef[0] = newRef[0] - 1;
			break;
		case Gravity.RIGHT:
			newRef[0] = newRef[0] + 1;
			break;
		case Gravity.DOWN:
			newRef[1] = newRef[1] - 1;
			break;
		case Gravity.UP:
			newRef[1] = newRef[1] + 1;
			break;
		}
		return newRef;
	}

}
