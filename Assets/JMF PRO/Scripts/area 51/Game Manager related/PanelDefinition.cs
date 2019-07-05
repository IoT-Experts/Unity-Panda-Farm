using UnityEngine;
using System.Collections;

public abstract class PanelDefinition : MonoBehaviour {

	public bool isInFront = false;
	public bool hasStartingPiece = true;
	public bool hasDefaultPanel = true;
	public bool hasNoSkin = false;
	public int defaultStrength = 0;
    public bool hasDefaultPanel2 = false;

	public GameObject[] skin; // how the panel will look like

	// for external scripts to call, will indicate that the panel got hit
	public virtual bool gotHit(BoardPanel bp){
		playAudioVisuals(bp); // play audio visual for selected panels
		bp.durability--;
		return true;
	}

	// called by Board during GameManager game-start phase
	// different from Start() as that is unity start, not neccessarily the game is set-up yet
	public virtual void onGameStart(Board board){
		// do nothing....
	}

	// optional onCreate function to define extra behaviours
	public virtual void onPanelCreate(BoardPanel bp){
		// do nothing...
	}
	// optional onDestroy function to define extra behaviours
	public virtual void onPanelDestroy(BoardPanel bp){
		// do nothing...
	}
	// optional onPanelClicked function when panel is clicked
	public virtual void onPanelClicked(BoardPanel bp){
		// do nothing...
	}
	// optional onPlayerMove called by GameManager when player makes the next move
	public virtual void onPlayerMove(BoardPanel bp){
		// do nothing...
	}
	// optional onBoardStabilize called by GameManager when board stabilize and gets a suggestion
	public virtual void onBoardStabilize(BoardPanel bp) {
		// do nothing...
	}
	
	// for external scripts to call, if splash damage hits correct panel type, perform the hit
	public virtual bool splashDamage(BoardPanel bp){
		// do nothing...
		return false; // default behaviour
	}
	
	// function to check if pieces can fall into this board box
	public abstract bool allowsGravity(BoardPanel bp);
	
	// if the piece here can be used to form a match
	public abstract bool isMatchable(BoardPanel bp);
	
	// if the piece here can be switched around
	public abstract bool isSwitchable(BoardPanel bp);
	
	// if the piece here (if any) can be destroyed
	public abstract bool isDestructible(BoardPanel bp);
	
	// function to check if pieces can be stolen from this box by gravity
	public abstract bool isStealable(BoardPanel bp);
	
	// function to for resetBoard() to know which panel can be resetted
	public abstract bool isFillable(BoardPanel bp);
	
	// function to check if this board is a solid panel
	// determines if neighbour pieces will landslide; true = landslide / false = does not landslide
	public abstract bool isSolid(BoardPanel bp);
	
	// function to play the audio visuals of this panel
	public abstract void playAudioVisuals(BoardPanel bp);

}
