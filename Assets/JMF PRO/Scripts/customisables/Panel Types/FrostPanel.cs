using UnityEngine;
using System.Collections;

[AddComponentMenu("JMF/Panels/FrostPanel")]
public class FrostPanel : PanelDefinition {

	
	// for external scripts to call, if splash damage hits correct panel type, perform the hit
	public override bool splashDamage(BoardPanel bp){
		bp.durability--;
		playAudioVisuals(bp);
		return true;
	}
	
	// function to check if pieces can fall into this board box
	public override bool allowsGravity(BoardPanel bp){
		return false;
	}
	
	// if the piece here can be used to form a match
	public override bool isMatchable(BoardPanel bp){
		return false;
	}
	
	// if the piece here can be switched around
	public override bool isSwitchable(BoardPanel bp){
		return false;
	}
	
	// if the piece here (if any) can be destroyed
	public override bool isDestructible(BoardPanel bp){
		return false;
	}
	
	// function to check if pieces can be stolen from this box by gravity
	public override bool isStealable(BoardPanel bp){
		return false;
	}
	
	// function to check if this board needs to be filled by gravity
	public override bool isFillable(BoardPanel bp){
		return true;
	}
	
	// function to check if this board is a solid panel
	// ( AKA piece, NO ENTRY!! ROADBLOCK~!- IMPORTANT, not the same of allowsGravity()~!
	// this function determines if pieces will landslide it's neighbouring piece to fill bottom blocks)
	public override bool isSolid(BoardPanel bp){
		return true;
	}

	// function to play the audio visuals of this panel
	public override void playAudioVisuals(BoardPanel bp){
        //MusicControll.musicControll.IcePanelHitFx();
		bp.master.gm.animScript.doAnim(animType.ICEHIT, bp.master.arrayRef[0], bp.master.arrayRef[1] );
	}
}
