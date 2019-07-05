using UnityEngine;
using System.Collections;

[AddComponentMenu("JMF/Panels/ShadedPanel")]
public class ShadedPanel : PanelDefinition {


	// function to check if pieces can fall into this board box
	public override bool allowsGravity(BoardPanel bp){
		return true;
	}
	
	// if the piece here can be used to form a match
	public override bool isMatchable(BoardPanel bp){
		return true;
	}
	
	// if the piece here can be switched around
	public override bool isSwitchable(BoardPanel bp){
		return true;
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
		return true;
	}
	
	// function to check if this board is a solid panel
	// ( AKA piece, NO ENTRY!! ROADBLOCK~!- IMPORTANT, not the same of allowsGravity()~!
	// this function determines if pieces will landslide it's neighbouring piece to fill bottom blocks)
	public override bool isSolid(BoardPanel bp){
		return false;
	}

	// function to play the audio visuals of this panel
	public override void playAudioVisuals(BoardPanel bp){
        //MusicControll.musicControll.ShadedPanelHitFx();
        bp.master.gm.animScript.doAnim(animType.SHADEHIT, bp.master.arrayRef[0], bp.master.arrayRef[1] );
	}
}
