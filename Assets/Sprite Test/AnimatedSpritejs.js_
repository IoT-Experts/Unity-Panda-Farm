#pragma strict

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


var spriteMaterial : Material;
var animations : Font[];

private var animNames = ["Idle", "Run", "Stop", "Turn", "Crouch", "UnCrouch", "Jump", "Land", "Fall"];
private enum theLoopBehaviour { Loop=0, PingPong=1, OnceAndHold=2, OnceAndChange=3 }

private var lastAnimFrameTime = 0.0;
private var frame = 0;
private var frameStep = 1;
private var anim = 0;

private var TM : TextMesh;
private var TMR : Renderer;

function Start () {
	TMR = GetComponent(Renderer);
	TMR.material = spriteMaterial;
	for (var i = 0; i<animations.Length; i++){
		animations[i].material = spriteMaterial;
	}
	TM = GetComponent(TextMesh);
	TM.font = animations[anim];
}

function animateSprite () {
	if(Time.time > lastAnimFrameTime + 0.1){
		lastAnimFrameTime = Time.time;
		frame = frame + frameStep;
		if(frame > animations[anim].characterInfo.Length-2 || (frameStep == -1 && frame < animations[anim].characterInfo[0].uv.y)){
			switch (animations[anim].characterInfo[0].index cast theLoopBehaviour){
				case theLoopBehaviour.Loop:
					frame = animations[anim].characterInfo[0].uv.y;
					break;
				case theLoopBehaviour.PingPong:
					frameStep *= -1;
					frame = frame + frameStep * 2;
					break;
				case theLoopBehaviour.OnceAndHold:
					frame = frame - frameStep;
					break;
				case theLoopBehaviour.OnceAndChange:
					changeAnim(parseInt(animations[anim].characterInfo[0].uv.x),parseInt(animations[anim].characterInfo[0].uv.y));
					break;
			}
		}
		var C:char = frame+33;
		TM.text = "" + C;
	}
}

function changeAnim (a:int, f:int){
	anim = a;
	TM.font = animations[anim];
	frame = f;
	var C:char = frame+33;
	TM.text = "" + C;
	lastAnimFrameTime = Time.time;
	frameStep = 1;
}

function Update () {
	animateSprite ();
}

function OnGUI () {
	anim = GUI.SelectionGrid(Rect(10,10,270,60),anim,animNames,3);
	if(GUI.changed){
		changeAnim(anim, 0);
	}
}