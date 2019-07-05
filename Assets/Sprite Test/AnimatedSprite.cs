#pragma warning disable 0618

/*
	In order to implement your own alternative to AnimatedSprite, and have it work
	in the FontEditor window, you need to change some of the parameters that are
	used in Editor/FontEditor.js


	Replace all occurrances of  'AnimatedSprite'  with the class that you write to replace it
	Your replacement class needs to have a Material variable,
	Replace all occurrances of  '.spriteMaterial'  with the Material variable that you use
	Your replacement class also needs to have a Font[] array,
	Replace all occurrances of  '.animations'  with the Font[] variable that you use



	Note: The FontEditor and AnimatedSprite script repurposes the first character definition in each
	Font(animation) to store the properties of Loop Behaviour, Start Frame, FPS Multiplier, etc

	Note 2: The frame variable and characterInfo index always need to be increadsed by 33,
	characters with an index below 33 have non-rendering behaviours, like newline, tab, and space
	*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimatedSprite : MonoBehaviour {
	public Material spriteMaterial;
	public Font[] animations;

	string[] animNames = new string[] { "Idle", "Run", "Stop", "Turn", "Crouch", "UnCrouch", "Jump", "Land", "Fall" };
	private enum theLoopBehaviour { Loop = 0, PingPong = 1, OnceAndHold = 2, OnceAndChange = 3 }

	float lastAnimFrameTime = 0;
	int frame = 0, frameStep = 1, anim = 0;

	TextMesh TM;
	Renderer TMR;

	void Start () {
		TMR = GetComponent<Renderer>();
		TMR.material = spriteMaterial;
		for (int i = 0; i < animations.Length; i++) {
			animations[i].material = spriteMaterial;
		}
		TM = GetComponent<TextMesh>();
		TM.font = animations[anim];
	}

	void animateSprite () {
		if (Time.time > lastAnimFrameTime + 0.1f) {
			lastAnimFrameTime = Time.time;
			frame = frame + frameStep;
			if (frame > animations[anim].characterInfo.Length - 2 || (frameStep == -1 && frame < animations[anim].characterInfo[0].uv.y)) {
				switch ((theLoopBehaviour) animations[anim].characterInfo[0].index) {
					case theLoopBehaviour.Loop:
						frame = (int) animations[anim].characterInfo[0].uv.y;
						break;
					case theLoopBehaviour.PingPong:
						frameStep *= -1;
						frame = frame + frameStep * 2;
						break;
					case theLoopBehaviour.OnceAndHold:
						frame = frame - frameStep;
						break;
					case theLoopBehaviour.OnceAndChange:
						changeAnim((int) animations[anim].characterInfo[0].uv.x, (int) animations[anim].characterInfo[0].uv.y);
						break;
				}
			}
			char C = (char) (frame + 33);
			TM.text = "" + C;
		}
	}

	void changeAnim (int a, int f) {
		anim = a;
		TM.font = animations[anim];
		frame = f;
		char C = (char) (frame + 33);
		TM.text = "" + C;
		lastAnimFrameTime = Time.time;
		frameStep = 1;
	}

	void Update () {
		animateSprite();
	}

	void OnGUI () {
		anim = GUI.SelectionGrid(new Rect(10, 10, 270, 60), anim, animNames, 3);
		if (GUI.changed) {
			changeAnim(anim, 0);
		}
	}
}