#pragma strict

var spriteMaterial : Material; //inspector variable, the material to be used for the sprite.
var animations : Font[]; //inspector variable, array of font animations.

private enum anims { Idle, Run, Stop, Turn, Crouch, UnCrouch, Jump, Land, Fall}
private enum LoopBehaviour { Loop=0, PingPong=1, OnceAndHold=2, OnceAndChange=3 }

//animation class
class AnimtionClass {
	@System.NonSerialized
	var TM : TextMesh; //text mesh component
	@System.NonSerialized
	var TMR : Renderer; //renderer component
	@System.NonSerialized
	var anim = anims.Idle; //currently playing animation: int index or enum value
	@System.NonSerialized
	var frame = 0; //current frame in animtion
	@System.NonSerialized
	var frameStep = 1; //step is 1 for playing forward, -1 for playing in reverse (pingpong)
	var frameRate = 10.0; //frame per second
	@System.NonSerialized
	var lastAnimFrameTime = 0.0; //last time the frame was changed
}

//controller movement class
class MovementClass {
	var maxSpeed = 2.0; //Max move speed
	var jumpPower = 8.0; //Max move speed
	var gravity = 9.8; //gravity. or weight 
	@System.NonSerialized
	var currentSpeed = 0.0; //current move speed
	@System.NonSerialized
	var currentVerticalSpeed = 0.0; //current vertical speed (jump or fall)
	@System.NonSerialized
	var targetSpeed = 0.0; //desired speed to lerp to
	@System.NonSerialized
	var currentDirection = Vector3.right; //facing direction
	@System.NonSerialized
	var moving = false; //moving or not moving status
	@System.NonSerialized
	var crouching = false; //crouching or not crouching status
	@System.NonSerialized
	var jumping = false; //jumping or not jumping status
	@System.NonSerialized
	var falling = false; //falling or not falling status
	@System.NonSerialized
	var timeLastGrounded = 0.0; //last time the character was known to be grounded
	@System.NonSerialized
	var collisionFlags : CollisionFlags; //collision flags 
}

//user input toggles. This is useful of checking things like double jump
class InputClass {
	@System.NonSerialized
	var jumpPressed = false; //jump flag
	@System.NonSerialized
	var jumpReleased = true; //This toggle stop the character bounching if you hold jump
	@System.NonSerialized
	var crouchPressed = false; //crouch toggle
	@System.NonSerialized
	var movePressed = false; //move toggle
	@System.NonSerialized
	var moveChanged = false; //flag for changing left to right
	@System.NonSerialized
	var v = 0.0; //move toggle
	@System.NonSerialized
	var h = 0.0; //move toggle
}

private var controller : CharacterController;
var movement : MovementClass;
var animate : AnimtionClass;
var buttons : InputClass;

private var debugstr = ""; //debug string for updating GUI with infos

function Start () {
	controller = GetComponent (CharacterController) as CharacterController;
	animate.TMR = GetComponent(Renderer);
	//Assign the sprite material to the renderer.
	animate.TMR.material = spriteMaterial;
	
	//Assign the sprite material to every animtion, Textmesh will change material when a new font is assigned.
	for (var i = 0; i<animations.Length; i++){
		animations[i].material = spriteMaterial;
	}
	animate.TM = GetComponent(TextMesh);
	animate.TM.font = animations[animate.anim];
}

function animateSprite () {
	//if enough time has elapsed since the last frame change
	if(Time.time > animate.lastAnimFrameTime + (1.0/(animate.frameRate * (animations[animate.anim].characterInfo[0].uv.width + 1)))){
		animate.lastAnimFrameTime = Time.time;
		animate.frame = animate.frame + animate.frameStep; //increment frame
		if(animate.frame > animations[animate.anim].characterInfo.Length-2 || (animate.frameStep == -1 && animate.frame < animations[animate.anim].characterInfo[0].uv.y)){ //if the frame has incremented past the length of the animation, or backwards in the case of pingpong
			switch (animations[animate.anim].characterInfo[0].index cast LoopBehaviour){ //choose behaviour
				case LoopBehaviour.Loop: //if its a loop, go back to the start of the loop
					animate.frame = animations[animate.anim].characterInfo[0].uv.y;
					break;
				case LoopBehaviour.PingPong: //its its a pingpong, swap the framestep
					animate.frameStep *= -1;
					animate.frame = animate.frame + animate.frameStep * 2;
					break;
				case LoopBehaviour.OnceAndHold: //if its a hold, put the frame back to the last
					animate.frame = animate.frame - animate.frameStep;
					break;
				case LoopBehaviour.OnceAndChange: //if its a change, call changeAnim with the new anim and starting frame
					changeAnim(parseInt(animations[animate.anim].characterInfo[0].uv.x),parseInt(animations[animate.anim].characterInfo[0].uv.y));
					break;
			}
		}
		debugstr = "frame:" + animate.frame + " in anim:" + animate.anim;
		var C:char = animate.frame+33; //change the frame into a char
		animate.TM.text = "" + C; //assign the char into the TextMesh
	}
}

function changeAnim (a:int, f:int){ //animation and frame number in
	animate.anim = a;
	animate.TM.font = animations[animate.anim];
	animate.frame = f;
	var C:char = animate.frame+33;
	animate.TM.text = "" + C;
	animate.lastAnimFrameTime = Time.time;
	animate.frameStep = 1;
	debugstr = "frame:" + animate.frame + " in anim:" + animate.anim;
}

function Update () {
	//first handle input
	buttons.h = Input.GetAxisRaw ("Horizontal"); //Get input on h axis
	buttons.v = Input.GetAxisRaw ("Vertical"); //Get input on v axis
	
	if(Mathf.Abs(buttons.h) > 0.1){ //if h input axis buttons are pressed
		buttons.movePressed = true;
		if(movement.currentDirection != Vector3 (buttons.h, 0, 0)){ //if currentDirection is not the input direction
			if(!movement.crouching){ //cant turn on the stop while crouching
				movement.currentDirection = Vector3 (buttons.h, 0, 0); //Set direction and
				buttons.moveChanged = true; //change move flag
			}
		}
	} else {
		buttons.movePressed = false;
	}
	
	if(buttons.v > 0.1){ //...if jump is pressed...
		if(!movement.jumping && !movement.falling && buttons.jumpReleased){ //if not currently in a jump or fall
			buttons.jumpPressed = true;
			buttons.crouchPressed = false;
			buttons.jumpReleased = false;
		}
	} else if(buttons.v < -0.1){ //...or if crouch is pressed...
		if(controller.isGrounded){ //...while grounded
			buttons.crouchPressed = true;
			buttons.jumpPressed = false;
		}
	} else { //neither jump or crouch pressed
		buttons.crouchPressed = false;
		buttons.jumpReleased = true;
		buttons.jumpPressed = false;
	}
	
	//Now translate input flags into movement flags
	if(buttons.movePressed){ //if move buttons are pressed
		if(!movement.moving && !movement.crouching){ //if not currently moving or crouching
			movement.moving = true; //set moving
			if(!movement.jumping && !movement.falling){ //if not mid-jump or fall
				changeAnim(anims.Run, 0); //play run anim, unless...
			}
		}
		if(buttons.moveChanged){ //...if there was a change of direction
			transform.localScale.x = (buttons.h > 0)?1:-1; //flip the sprite, this could be acheived with rotation if scale is unwanted
			buttons.moveChanged = false;
			if(!movement.jumping && !movement.falling){ //if not mid-jump or fall
				changeAnim(anims.Turn, 0); //start the turn animation
			}
		}
	} else { //h input axis has no input (user stopped pressing)
		if(movement.moving){
			movement.moving = false; //stop moving
			if(!movement.jumping && !movement.falling){ //if not mid-jump
				changeAnim(anims.Stop, 0); //play the stop anim
			}
		}
	}
	
	
	
	if(controller.isGrounded){ //if the character is grounded...
		movement.timeLastGrounded = Time.time;
		movement.currentVerticalSpeed = -0.5; //Stops character from becoming airborn breifly when moving down a slope, but not stairs
		if(movement.falling){ //if he was falling
			movement.falling = false; //unset jumping
			if(movement.moving){ //and either
				changeAnim(anims.Run,15); //play the run anim
			} else {
				changeAnim(anims.Land,0); //or play the landing anim
			}
		}
		if(buttons.jumpPressed){ //...if jump is pressed...
			if(!movement.jumping){ //...and not currently jumping...
				movement.jumping = true; //set jumping
				movement.currentVerticalSpeed = movement.jumpPower; //add jump power to the vertical speed
				changeAnim(anims.Jump,0); //play the jump anim
				buttons.jumpPressed = false; //dont allow the user to hold jump
			}
		} else if(buttons.crouchPressed){ //...or if crouch is pressed...
			if(!movement.crouching){ //...and not currently crouching...
				movement.crouching = true; //set crouching
				movement.moving = false; //unset moving, you cant move while crouching
				changeAnim(anims.Crouch, 0); //play the crouch anim
			}
		} else { //...otherwise not pressing crouch or jump, while grounded
			if(movement.crouching){ //if he was crouching
				movement.crouching = false; //unset crouch
				changeAnim(anims.UnCrouch,0); //and play uncrouch anim
			}
		}
	} else { //character is in the air
		if(Time.time > movement.timeLastGrounded + 0.15){ //this stops changing to falling for only small drops eg. stairs
			if(!movement.jumping && !movement.falling){ //if its not airborn because of jumping, or falling after a jump, must be falling off a platform
				changeAnim(anims.Fall,0); //start falling
				movement.falling = true;
				movement.timeLastGrounded = Time.time;
			}
		}
	}
	
	movement.currentVerticalSpeed -= movement.gravity * Time.deltaTime;
	
	if(movement.currentVerticalSpeed < 1){ //if the jump nears its apex
		if(movement.jumping){ //while jumping
			changeAnim(anims.Fall,0); //start falling
			movement.jumping = false;
			movement.falling = true;
		}
	}
	
	if(movement.moving){ //if the character is moving after all input is accounted for
		movement.targetSpeed = Mathf.Min (Mathf.Abs(buttons.h), 1.0); //Set target speed on the 0->1 scale
	} else {
		movement.targetSpeed = 0; //0 if not moving
	}
	movement.targetSpeed *= movement.maxSpeed; //Translate into 0->movement.maxSpeed
	movement.currentSpeed = Mathf.Lerp (movement.currentSpeed, movement.targetSpeed, 0.1); //Lerp current speed
	var currentMovementOffset = (movement.currentDirection * movement.currentSpeed) + Vector3 (0.0, movement.currentVerticalSpeed, 0.0); //Movement offset for this update
	currentMovementOffset *= Time.deltaTime; //Not framerate dependant
	movement.collisionFlags = controller.Move (currentMovementOffset); //Apply move, and get collisionflags
	
	animateSprite (); //Call the actual sprite animation
	
	
	if(Input.GetKey(KeyCode.B)){ //for 'de-bugging' animation frames hold B to slow-mo
		Time.timeScale = 0.2;
	} else {
		Time.timeScale = 1.0;
	}
}

function OnGUI () {
	GUILayout.Label(debugstr);
	GUI.color = movement.moving?Color.green:Color.red;
	GUILayout.Label("moving");
	GUI.color = movement.jumping?Color.green:Color.red;
	GUILayout.Label("jumping");
	GUI.color = movement.crouching?Color.green:Color.red;
	GUILayout.Label("crouching");
	GUI.color = movement.falling?Color.green:Color.red;
	GUILayout.Label("falling");
	GUI.color = buttons.movePressed?Color.green:Color.red;
	GUILayout.Label("pressing move");
	GUI.color = buttons.jumpPressed?Color.green:Color.red;
	GUILayout.Label("pressing jump");
	GUI.color = buttons.crouchPressed?Color.green:Color.red;
	GUILayout.Label("pressing crouch");
	GUI.color = controller.isGrounded?Color.red:Color.green;
	GUILayout.Label("Airborn");
	
	GUI.color = Color.white;
	GUILayout.Label("ground time:" + movement.timeLastGrounded);
	GUILayout.Label("time:" + Time.time);
	GUILayout.Label("direction:" + movement.currentDirection);
	GUILayout.Label("Input:" + Vector3(buttons.h, buttons.v, 0));
}
