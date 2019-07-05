using UnityEngine;
using System.Collections;

[AddComponentMenu("JMF/Panels/EmptyPanel")]
public class EmptyPanel : PanelDefinition {

	// for external scripts to call, will indicate that the panel got hit
	public override bool gotHit(BoardPanel bp){
		return false;
	}

	// function to check if pieces can fall into this board box
	public override bool allowsGravity(BoardPanel bp){
		int emptyCount = bp.master.gm.countBlockedUnfilled(bp.master.arrayRef[0],bp.master.arrayRef[1],true);
		if(emptyCount > 0){
			return true;
		}
		return false;
	}
	
	// if the piece here can be used to form a match
	public override bool isMatchable(BoardPanel bp){
		return true;
	}
	
	// if the piece here can be switched around
	public override bool isSwitchable(BoardPanel bp){
		return false;
	}
	
	// if the piece here (if any) can be destroyed
	public override bool isDestructible(BoardPanel bp){
		return true;
	}
	
	// function to check if pieces can be stolen from this box by gravity
	public override bool isStealable(BoardPanel bp){
		return true;
	}
	
	// function to check if this board needs to be filled by gravity
	public override bool isFillable(BoardPanel bp){
		return false;
	}
	
	// function to check if this board is a solid panel
	// ( AKA piece, NO ENTRY!! ROADBLOCK~!- IMPORTANT, not the same of allowsGravity()~!
	// this function determines if pieces will landslide it's neighbouring piece to fill bottom blocks)
	public override bool isSolid (BoardPanel bp){
		return false;
	}

	// function to play the audio visuals of this panel
	public override void playAudioVisuals(BoardPanel bp){
		// nothing...
	}
}
