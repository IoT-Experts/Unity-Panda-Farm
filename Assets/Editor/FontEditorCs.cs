#pragma warning disable 0618

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System;


public class FontEditorCs : EditorWindow {

	bool SpriteEditor = false;
	Font[] FontList;
	string[] FontListNames;
	Font theFont;
	Texture2D theTex;
	GUISkin theGUISkin;
	GameObject SpritePreview, TextPreview;
	TextMesh SpritePreviewT, TextPreviewT;
	Renderer SpritePreviewR, TextPreviewR;
	Material TextPreviewM;
	bool UsePreview = true, RepeatSetting = false, SnapFound = false, PerCharacterOffset = false, PM = false, PMS = false;
	Color BGCol = new Color( 0.46f, 0.5f, 0.46f );
	Rect MenuBar = new Rect( 0, 0, 150, 512 ), SnapRect;
	List<SetRect> Rects, AutoRectList, UnsortAutoRectList;
	Vector2 MovingRectOffset = Vector2.zero, ResizingRectOffset = Vector2.zero;
	int Snap = 0;
	string[] Snaps = new string[] { "None", "Snap", "Horiz", "Vert" };
	int[,] TexAsInts;
	Color[] Colors = new Color[] { new Color( 1, 1, 1 ), new Color( 0.871f, 0.035f, 0.035f ), new Color( 0.055f, 0.431f, 0.063f ), new Color( 0.102f, 0.153f, 0.824f ), new Color( 0.91f, 0.902f, 0.224f ), new Color( 0.216f, 0.749f, 0.741f ), new Color( 0.549f, 0.086f, 0.745f ), new Color( 0.914f, 0.341f, 0.059f ), new Color( 0.278f, 0.914f, 0.059f ) };

	//Interface vars
	int ClickedRectInd = -1;
	float MovingAllSliderStart = 0;
	bool ClickedOnRect = false, MovingRect = false, ResizingRect = false, MovingSpriteCenter = false, MovingSlider = false, MovingAllToggle = false, MovingAllSpriteToggle = false;
	Vector2 MovingAllSpriteCenterStart, checkPos;

	//GUI vars
	bool MouseOffGUI = false, UsingEditor = false, UsingPacker = false, UsingMenu = true, ReFocusBlank = false, ReFocusChar = false, ResetPrompt = false, AutoPrompt = false, MenuPrompt = false;
	string DupeErrorString, OverlapErrorString, SkipErrorString;
	bool[] ErrorMsg = new bool[19];
	int FirstOverlapRect, SecondOverlapRect, FirstDupeRect, SecondDupeRect;
	enum Errors { NoMaterial, NoTexture, NoSpriteMaterial, NoTextMesh, NPOT, ReadWrite, Format, Spacebar, Size, SameFont, OverlapChar, OverlapSprite, DupeChar, DupeSprite, SkipSprite, NoFonts, MissedFont, ShareFont, UnevenLength }
	string[] SplitError;
	Texture2D[] MenuPics, OrientPics;
	float PromptOffset1 = 0, PromptOffset2 = 0, PromptOffset3 = 0, PromptOffset4 = 0, PromptOffset1Dest = 0, PromptOffset2Dest = 0, PromptOffset3Dest = 0, PromptOffset4Dest = 0;
	float SliderSize = 4; //Change this to get bigger sliders, no equivalent GUI control

	//Autoset vars
	List<char> CharRepeatList;
	bool AutoSetDialog = false, DialogStop = false;
	string AutoSetCharStr = "";
	int AutoSetIndex = 0;
	List<Vector2> Shape;
	int FloodfillPadding = 0; //increase this to pad Rects out in autoset and shrinkwrap shape finding, no equivalent GUI control

	//SmartSet vars
	bool SmartMode = false;
	Color SmartColor = Color.red;
	List<Vector2> ShapeSmartLine;

	//Sprite vars
	AnimatedSprite FontContainer;
	int RectFontIndex = 0, RectFontIndexOld = 0, SpriteIndex, SpriteIndexOld, DupeMissOffset = 0;
	bool SpriteSetDialog = false;
	List<Vector3> SpriteIndexRepeatList;
	enum defaultPivotPointEnum { PivotPixel = 0, BottomRight = 1, BottomCenter = 2, BottomLeft = 3, MiddleRight = 4, MiddleCenter = 5, MiddleLeft = 6, TopRight = 7, TopCenter = 8, TopLeft = 9 }
	enum SpritePropertiesLoopBehaviourEnum { Loop = 0, PingPong = 1, OnceAndHold = 2, OnceAndChange = 3 }
	Vector2[] defaultPivots = new Vector2[] { Vector2.zero, Vector2.zero, new Vector2( 0.5f, 0 ), new Vector2( 1, 0 ), new Vector2( 0, 0.5f ), new Vector2( 0.5f, 0.5f ), new Vector2( 1, 0.5f ), new Vector2( 0, 1 ), new Vector2( 0.5f, 1 ), new Vector2( 1, 1 ) };
	SpritePropertiesLoopBehaviourEnum SpritePropertiesLoopBehaviour = 0, SpritePropertiesLoopBehaviourOld = 0;
	defaultPivotPointEnum defaultPivotPoint = 0;
	int SpritePropertiesLoopStart = 0, SpritePropertiesLoopStartOld = 0, SpritePropertiesNextAnim = 0, SpritePropertiesNextAnimOld = 0, SpritePropertiesFontIndex = 0, SpritePropertiesFontIndexOld = 0;
	float SpritePropertiesAnimFPS = 0, SpritePropertiesAnimFPSOld = 0;

	//animating sprite in scene
	float lastAnimFrameTime = 0, frameTime = 0.1f;
	int SpriteFrame = 0, SpriteFrameOld = 0;
	bool AutoAnimate = true, AutoAnimateOld = true;

	//Font vars
	int CharCount, UVx, UVy, UVw, UVh, Orient, OrientOld, ChrL = 47;
	Rect UVRect;
	float Cx, Cy, WidBefore, WidBeforeOld, WidAfter, WidAfterOld;
	Vector2 CPos;
	string ChrStr = "";
	SerializedProperty p, p2;
	SerializedObject SO;

	//"drawOffset" vars ('camera' control)
	bool drawOffsetClick = false;
	Vector2 drawOffsetPos, drawOffsetPosDest;
	float drawOffsetScale = 1, drawOffsetScaleDest = 1, ShrunkX = 0, ShrunkY = 0;

	//Rect Packer vars
	DateTime TimeA;
	Font inFont, outFont;
	Font[] inFontList, outFontList;
	AnimatedSprite inFontContainer, outFontContainer;
	Texture2D inTex, outTex;
	bool FitToPOT = true, AllowNPOT = true, AllowRotationTest = true, AnchorSort = true;
	int inTexWidth = 512, inTexHeight = 512, PackBuffer = 1, NewLineIndex, LineIndex, PackMode, SortMode, PackSizeX, PackSizeY, MaxSizeX, MaxSizeY;
	List<PackRect> StartPackRectList, ResultPackRectList;
	PackRect swapPackRect, GetPackRect;
	string[] Packs = new string[] { "Simple Pack", "Switch back", "Partition", "Anchor" };
	string[] Sorts = new string[] { "Sort by height", "Sort by width", "Longest length", "Shortest length" };
	List<Vector2> Anchors;
	List<Rect> Partitions;

	[MenuItem( "Window/Font Editor/FontEditor" )]
	static void Init () {
		FontEditorCs window = (FontEditorCs) EditorWindow.GetWindow( typeof( FontEditorCs ) );
		window.Show();
		window.position = new Rect( 20, 80, 512 + 300, 512 );
	}


	//This class stores the start and result rect for the packer
	public class PackRect {
		public CharacterInfo CI;
		public int CIIndex, fontIndex;
		public Rect StartRect, ResultRect;
		public float Height;
		public bool SameOrient = true;

		public PackRect ( PackRect f ) {
			this.CI = f.CI;
			this.CIIndex = f.CIIndex;
			this.fontIndex = f.fontIndex;
			this.StartRect = f.StartRect;
			this.ResultRect = f.ResultRect;
			this.Height = f.Height;
			this.SameOrient = f.SameOrient;
		}
		public PackRect () {
			this.CI = new CharacterInfo();
			this.CIIndex = 0;
			this.fontIndex = 0;
			this.StartRect = new Rect( 0, 0, 0, 0 );
			this.ResultRect = new Rect( 0, 0, 0, 0 );
			this.Height = 0;
			this.SameOrient = true;
		}
	}

	public class SetRect {
		public Rect rect;
		public float aWidth, bWidth, vOffset;
		public bool Orient;
		public Vector2 spritePivot;
		public int fontIndex, CIIndex;

		public SetRect ( SetRect r ) {
			if ( r.Orient ) {
				this.rect = new Rect( r.rect.x, r.rect.y + r.rect.height, r.rect.width, r.rect.height );
			} else {
				this.rect = new Rect( r.rect.x + r.rect.width, r.rect.y, r.rect.width, r.rect.height );
			}
			this.aWidth = 0;
			this.bWidth = 0;
			this.Orient = r.Orient;
			this.vOffset = r.vOffset;
			this.spritePivot = r.spritePivot;
			this.fontIndex = r.fontIndex;
			this.CIIndex = -1;
		}

		public SetRect () {
			this.rect = new Rect( 0, 0, 0, 0 );
			this.aWidth = 0;
			this.bWidth = 0;
			this.Orient = false;
			this.vOffset = 0;
			this.spritePivot = Vector2.zero;
			this.fontIndex = 0;
			this.CIIndex = -1;
		}

	}


	//Awake
	void Awake () {
		//Load some images for the GUI
		theGUISkin = EditorGUIUtility.Load( "FontSetter/Font Setter Skin.guiskin" ) as GUISkin;
		OrientPics = new Texture2D[2];
		OrientPics[0] = EditorGUIUtility.Load( "FontSetter/GUIOrient_01.png" ) as Texture2D;
		OrientPics[1] = EditorGUIUtility.Load( "FontSetter/GUIOrient_02.png" ) as Texture2D;
		MenuPics = new Texture2D[3];
		MenuPics[0] = EditorGUIUtility.Load( "FontSetter/GUIBlankBar.png" ) as Texture2D;
		MenuPics[1] = EditorGUIUtility.Load( "FontSetter/GUILeftBar.png" ) as Texture2D;
		MenuPics[2] = EditorGUIUtility.Load( "FontSetter/GUIRightBar.png" ) as Texture2D;
		SpriteIndexRepeatList = new List<Vector3>();
		CharRepeatList = new List<char>();
		FontList = new Font[1];
		PM = EditorApplication.isPlaying;
	}


	//This animates a sprite during setter
	void Update () {
		if ( SpriteEditor && !SpriteSetDialog && FontContainer != null && UsingEditor && SpritePreview != null ) {
			if ( EditorApplication.timeSinceStartup > lastAnimFrameTime + frameTime && AutoAnimate ) {
				lastAnimFrameTime = (float) EditorApplication.timeSinceStartup;
				SpriteFrame++;
				if ( SpriteFrame > FontList[SpritePropertiesFontIndex].characterInfo.Length - 2 ) {
					if ( (int) SpritePropertiesLoopBehaviour == 0 ) {
						SpriteFrame = (int) FontList[SpritePropertiesFontIndex].characterInfo[0].uv.y;
					} else {
						SpriteFrame = 0;
					}
				}
				char C = (char) ( SpriteFrame + 33 );
				SpritePreviewT.text = "" + C;
			}
		}
	}


	//OnGUI
	void OnGUI () {
		//qwer 
		if ( UsingEditor && Rects == null ) {
			UsingEditor = false;
			UsingMenu = true;
			Debug.Log( Rects );
		}
#if UNITY_2_6 || UNITY_2_6_1 ||UNITY_3_0 ||UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
		Undo.CreateSnapshot();
		Undo.RegisterSnapshot();
#else
		//This might not be needed?
		//Undo.RecordObjects( FontList, "Font Changed" );
#endif
		wantsMouseMove = true;
		MenuBar = new Rect( 0, 0, 150, position.height );
		Event e = Event.current;

		//change gui focus to a blank control
		if ( ReFocusBlank ) {
			GUI.FocusControl( "" );
			ReFocusBlank = false;
		}

		//change gui focus to CharacterEntry
		if ( ReFocusChar ) {
			GUI.FocusControl( "CharacterEntry" );
			ReFocusChar = false;
		}

		//this disbles the editor when playmode is started - a bunch of vars are not preseved, leading to console being flooded with errors. better to just disable it
		if ( !PMS && PM != EditorApplication.isPlaying ) {
			if ( PM ) {
				PM = false;
			} else {
				position = new Rect( position.x, position.y - 5, 220, 150 );
				PMS = true;
				return;
			}
		}

		if ( PMS ) {
			GUI.skin = theGUISkin;
			GUI.BeginGroup( new Rect( position.width / 2 - 80, position.height / 2 - 50, 160, 80 ) );
			GUI.Box( new Rect( 0, 0, 160, 80 ), "" );
			GUI.Label( new Rect( 10, 4, 140, 80 ), "This extension doesn't support entering PlayMode, please close and re-open it's window" );
			GUI.EndGroup();
			return;
		}

		if ( UsingPacker ) {
			drawOffsetPos = Vector2.Lerp( drawOffsetPos, drawOffsetPosDest, 0.02f );
			//Rightmouse down, start drawOffset pan
			if ( e.type == EventType.MouseDown && ( e.button == 2 || e.button == 1 ) ) {
				drawOffsetClick = true;
			}

			//Right Mouseup from drawOffset move
			if ( e.type == EventType.MouseUp && ( e.button == 2 || e.button == 1 ) && drawOffsetClick ) {
				drawOffsetClick = false;
			}

			//Dragging with rightmouse, update drawOffset pan
			if ( e.type == EventType.MouseDrag && ( e.button == 2 || e.button == 1 ) && drawOffsetClick ) {
				drawOffsetPosDest = drawOffsetPosDest + Event.current.delta;
				drawOffsetPosDest = new Vector2( Mathf.Clamp( drawOffsetPosDest.x, -( outTex.width / 2 ), ( outTex.width / 2 ) ),
													Mathf.Clamp( drawOffsetPosDest.y, -( outTex.height / 2 ), ( outTex.height / 2 ) ) );
			}

		}


		MouseOffGUI = new Rect( MenuBar.width, 0, position.width - 300, position.height ).Contains( Event.current.mousePosition );
		if ( UsingEditor ) {
			ShrunkX = ( theTex.width - ( position.width - ( MenuBar.width * 2 ) ) ) / 2;
			ShrunkY = ( theTex.height - position.height ) / 2;
			drawOffsetPos = Vector2.Lerp( drawOffsetPos, drawOffsetPosDest, 0.02f );

			if ( e.type == EventType.ScrollWheel ) {
				drawOffsetPosDest /= drawOffsetScaleDest;
				drawOffsetScaleDest = Mathf.Clamp( Mathf.RoundToInt( ( drawOffsetScale + ( e.delta.y > 0 ? -0.5f : 0.5f ) ) * 2 ) / 2.0f, 0.5f, 10 );
				drawOffsetPosDest *= drawOffsetScaleDest;
			}
			drawOffsetScale = Mathf.Lerp( drawOffsetScale, drawOffsetScaleDest, 0.02f );



			//Leftmouse down
			if ( e.type == EventType.MouseDown && e.button == 0 && MouseOffGUI && !AutoSetDialog && !SpriteSetDialog ) {
				//register undo for any mousedown inside the gui

#if UNITY_2_6 || UNITY_2_6_1 ||UNITY_3_0 ||UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
				Undo.CreateSnapshot();
				Undo.RegisterSnapshot();
#else
				Undo.RecordObjects( FontList, "Font Changed" );
#endif

				//checkPos is the position of the mouse with respect to the bitmap
				checkPos = new Vector2( ( e.mousePosition.x - MenuBar.width + ShrunkX ) - drawOffsetPosDest.x + ( ( theTex.width / 2 ) * ( drawOffsetScale - 1 ) ), theTex.height - ( e.mousePosition.y + ShrunkY - drawOffsetPosDest.y ) + ( ( theTex.height / 2 ) * ( drawOffsetScale - 1 ) ) ) / drawOffsetScale;
				bool found = false;
				int foundIndex = -1;
				for ( int i = 0; i < Rects.Count; i++ ) {
					//Check for sprite center
					Rect SpriteCenterRect = new Rect( Rects[i].rect.x + Rects[i].spritePivot.x - ( 7.0f / drawOffsetScale ), Rects[i].rect.y + Rects[i].spritePivot.y - ( 7.0f / drawOffsetScale ), ( 16.0f / drawOffsetScale ), ( 16.0f / drawOffsetScale ) );
					if ( SpriteEditor && SpriteCenterRect.Contains( checkPos ) ) {
						found = true;
						MovingRectOffset = checkPos - new Vector2( SpriteCenterRect.x + ( 7.0f / drawOffsetScale ), SpriteCenterRect.y + ( 7.0f / drawOffsetScale ) );
						MovingSpriteCenter = true;
						MovingAllSpriteCenterStart = Rects[i].spritePivot;
						foundIndex = i;
						break;
					}

					//Check for offset slider
					Rect SliderRect;
					if ( Rects[i].Orient ) {
						SliderRect = new Rect( Rects[i].rect.xMax - Rects[i].vOffset - ( SliderSize / drawOffsetScale ), Rects[i].rect.y, ( SliderSize / drawOffsetScale ) * 2, Rects[i].rect.height );
					} else {
						SliderRect = new Rect( Rects[i].rect.x, Rects[i].rect.yMax - Rects[i].vOffset - ( SliderSize / drawOffsetScale ), Rects[i].rect.width, ( SliderSize / drawOffsetScale ) * 2 );
					}
					if ( !SpriteEditor && PerCharacterOffset && SliderRect.Contains( checkPos ) ) {
						found = true;
						MovingRectOffset = checkPos - new Vector2( SliderRect.x, SliderRect.y );
						MovingSlider = true;
						MovingAllSliderStart = Rects[i].vOffset;
						foundIndex = i;
						break;
					}

					//Check for Rects
					if ( Rects[i].rect.Contains( checkPos ) ) {
						found = true;
						Rect ResizerRect = new Rect( Rects[i].rect.xMax - 11.0f / drawOffsetScale, Rects[i].rect.y, 11.0f / drawOffsetScale, 11.0f / drawOffsetScale );
						if ( ResizerRect.Contains( checkPos ) ) {
							ResizingRect = true;
							ResizingRectOffset = checkPos - new Vector2( Rects[i].rect.x + Rects[i].rect.width, Rects[i].rect.y );
						} else {
							MovingRect = true;
							MovingRectOffset = checkPos - new Vector2( Rects[i].rect.x, Rects[i].rect.y );
						}
						foundIndex = i;
						break;
					}
				}

				if ( found ) {
					ClickedRectInd = foundIndex;
					GUI.FocusControl( "" );
					GetFontInfoToGUI( Rects[foundIndex].CIIndex, ( SpriteEditor ? Rects[foundIndex].fontIndex : 0 ) );
					ReFocusChar = true;
					ClickedOnRect = true;
				}
			}


			//Leftmouse HOLD, dragging something
			if ( e.type == EventType.MouseDrag && e.button == 0 ) {
				checkPos = new Vector2( ( e.mousePosition.x - MenuBar.width + ShrunkX ) - drawOffsetPosDest.x + ( ( theTex.width / 2 ) * ( drawOffsetScale - 1 ) ), theTex.height - ( e.mousePosition.y + ShrunkY - drawOffsetPosDest.y ) + ( ( theTex.height / 2 ) * ( drawOffsetScale - 1 ) ) ) / drawOffsetScale;

				if ( MovingSpriteCenter ) {//Moving a sprite center
					if ( MovingAllToggle ) {
						switch ( Snap ) {
							case 0:
								for ( int i = 0; i < Rects.Count; i++ ) {
									if ( MovingAllSpriteToggle && Rects[i].fontIndex != Rects[ClickedRectInd].fontIndex ) { continue; }
									Rects[i].spritePivot = Rects[i].spritePivot + new Vector2( ( checkPos.x - MovingRectOffset.x ) - Rects[ClickedRectInd].rect.x, ( checkPos.y - MovingRectOffset.y ) - Rects[ClickedRectInd].rect.y ) - MovingAllSpriteCenterStart;
									UpdateFont( i, Rects[i].CIIndex, Rects[i].fontIndex, false );
								}
								break;
							case 1:
								Rects[ClickedRectInd].spritePivot = new Vector2( ( checkPos.x - MovingRectOffset.x ) - Rects[ClickedRectInd].rect.x, ( checkPos.y - MovingRectOffset.y ) - Rects[ClickedRectInd].rect.y );
								Rects[ClickedRectInd].spritePivot.x = Mathf.Clamp( Mathf.RoundToInt( Rects[ClickedRectInd].spritePivot.x / ( Rects[ClickedRectInd].rect.width / 2 ) ) * ( Rects[ClickedRectInd].rect.width / 2 ), 0, Rects[ClickedRectInd].rect.width );
								Rects[ClickedRectInd].spritePivot.y = Mathf.Clamp( Mathf.RoundToInt( Rects[ClickedRectInd].spritePivot.y / ( Rects[ClickedRectInd].rect.height / 2 ) ) * ( Rects[ClickedRectInd].rect.height / 2 ), 0, Rects[ClickedRectInd].rect.height );
								Vector2 snapVector;
								snapVector.x = Rects[ClickedRectInd].spritePivot.x / Rects[ClickedRectInd].rect.width;
								snapVector.y = Rects[ClickedRectInd].spritePivot.y / Rects[ClickedRectInd].rect.height;
								for ( int i = 0; i < Rects.Count; i++ ) {
									if ( MovingAllSpriteToggle && Rects[i].fontIndex != Rects[ClickedRectInd].fontIndex ) { continue; }
									if ( i == ClickedRectInd ) { continue; }
									Rects[i].spritePivot = new Vector2( snapVector.x * Rects[i].rect.width, snapVector.y * Rects[i].rect.height );
									UpdateFont( i, Rects[i].CIIndex, Rects[i].fontIndex, false );
								}
								break;
							case 2:
								for ( int i = 0; i < Rects.Count; i++ ) {
									if ( MovingAllSpriteToggle && Rects[i].fontIndex != Rects[ClickedRectInd].fontIndex ) { continue; }
									Rects[i].spritePivot = Rects[i].spritePivot + new Vector2( ( checkPos.x - MovingRectOffset.x ) - Rects[ClickedRectInd].rect.x, Rects[ClickedRectInd].spritePivot.y ) - MovingAllSpriteCenterStart;
									Rects[i].spritePivot = RoundedV2( Rects[i].spritePivot );
									UpdateFont( i, Rects[i].CIIndex, Rects[i].fontIndex, false );
								}
								break;
							case 3:
								for ( int i = 0; i < Rects.Count; i++ ) {
									if ( MovingAllSpriteToggle && Rects[i].fontIndex != Rects[ClickedRectInd].fontIndex ) { continue; }
									Rects[i].spritePivot = Rects[i].spritePivot + new Vector2( Rects[ClickedRectInd].spritePivot.x, ( checkPos.y - MovingRectOffset.y ) - Rects[ClickedRectInd].rect.y ) - MovingAllSpriteCenterStart;
									Rects[i].spritePivot = RoundedV2( Rects[i].spritePivot );
									UpdateFont( i, Rects[i].CIIndex, Rects[i].fontIndex, false );
								}
								break;
						}
						MovingAllSpriteCenterStart = Rects[ClickedRectInd].spritePivot;
					} else {
						switch ( Snap ) {
							case 0:
								Rects[ClickedRectInd].spritePivot = new Vector2( ( checkPos.x - MovingRectOffset.x ) - Rects[ClickedRectInd].rect.x, ( checkPos.y - MovingRectOffset.y ) - Rects[ClickedRectInd].rect.y );
								break;
							case 1:
								Rects[ClickedRectInd].spritePivot = new Vector2( ( checkPos.x - MovingRectOffset.x ) - Rects[ClickedRectInd].rect.x, ( checkPos.y - MovingRectOffset.y ) - Rects[ClickedRectInd].rect.y );
								Rects[ClickedRectInd].spritePivot.x = Mathf.Clamp( Mathf.RoundToInt( Rects[ClickedRectInd].spritePivot.x / ( Rects[ClickedRectInd].rect.width / 2 ) ) * ( Rects[ClickedRectInd].rect.width / 2 ), 0, Rects[ClickedRectInd].rect.width );
								Rects[ClickedRectInd].spritePivot.y = Mathf.Clamp( Mathf.RoundToInt( Rects[ClickedRectInd].spritePivot.y / ( Rects[ClickedRectInd].rect.height / 2 ) ) * ( Rects[ClickedRectInd].rect.height / 2 ), 0, Rects[ClickedRectInd].rect.height );
								break;
							case 2:
								Rects[ClickedRectInd].spritePivot = new Vector2( ( checkPos.x - MovingRectOffset.x ) - Rects[ClickedRectInd].rect.x, Rects[ClickedRectInd].spritePivot.y );
								break;
							case 3:
								Rects[ClickedRectInd].spritePivot = new Vector2( Rects[ClickedRectInd].spritePivot.x, ( checkPos.y - MovingRectOffset.y ) - Rects[ClickedRectInd].rect.y );
								break;
						}
						if ( Snap != 1 ) { Rects[ClickedRectInd].spritePivot = RoundedV2( Rects[ClickedRectInd].spritePivot ); }
						UpdateFont( ClickedRectInd, Rects[ClickedRectInd].CIIndex, Rects[ClickedRectInd].fontIndex, true );
					}
				}

				if ( MovingSlider ) { //Moving an offset slider
					if ( MovingAllToggle ) {
						for ( int i = 0; i < Rects.Count; i++ ) {
							if ( Rects[ClickedRectInd].Orient ) {
								Rects[i].vOffset = Rects[i].vOffset + ( -( checkPos.x - Rects[ClickedRectInd].rect.xMax - MovingRectOffset.x + ( SliderSize / drawOffsetScale ) ) - MovingAllSliderStart );
							} else {
								Rects[i].vOffset = Rects[i].vOffset + ( -( checkPos.y - Rects[ClickedRectInd].rect.yMax - MovingRectOffset.y + ( SliderSize / drawOffsetScale ) ) - MovingAllSliderStart );
							}
							UpdateFont( i, Rects[i].CIIndex, Rects[i].fontIndex, false );
						}
						MovingAllSliderStart = Rects[ClickedRectInd].vOffset;
					} else {
						switch ( Snap ) {
							case 1: // SNAP
								SnapFound = false;
								for ( int i = 0; i < Rects.Count; i++ ) {
									if ( i == ClickedRectInd ) { continue; }
									if ( Rects[i].rect.Contains( checkPos ) ) {//Find a snap, set offset for this rect, to offset of that rect
										if ( Rects[ClickedRectInd].Orient ) {
											if ( Rects[i].Orient ) { //if Rect a is sideways, is rect b sideways too?
												Rects[ClickedRectInd].vOffset = Rects[i].vOffset - Rects[i].rect.xMax + Rects[ClickedRectInd].rect.xMax;
											} else {
												Rects[ClickedRectInd].vOffset = Rects[i].vOffset;
											}
										} else {
											if ( Rects[i].Orient ) { //if Rect a is upright, is rect b upright too?
												Rects[ClickedRectInd].vOffset = Rects[i].vOffset;
											} else {
												Rects[ClickedRectInd].vOffset = Rects[i].vOffset - Rects[i].rect.yMax + Rects[ClickedRectInd].rect.yMax;
											}
										}
										SnapFound = true;
										break;
									}
								}
								if ( !SnapFound ) {//Couldnt find a snap target, apply non-snap, depending on orient
									if ( Rects[ClickedRectInd].Orient ) {
										Rects[ClickedRectInd].vOffset = -( checkPos.x - Rects[ClickedRectInd].rect.xMax - MovingRectOffset.x + ( SliderSize / drawOffsetScale ) );
									} else {
										Rects[ClickedRectInd].vOffset = -( checkPos.y - Rects[ClickedRectInd].rect.yMax - MovingRectOffset.y + ( SliderSize / drawOffsetScale ) );
									}
								}
								break;
							default: // Other (no SNAP), depending on orient
								if ( Rects[ClickedRectInd].Orient ) {
									Rects[ClickedRectInd].vOffset = -( checkPos.x - Rects[ClickedRectInd].rect.xMax - MovingRectOffset.x + ( SliderSize / drawOffsetScale ) );
								} else {
									Rects[ClickedRectInd].vOffset = -( checkPos.y - Rects[ClickedRectInd].rect.yMax - MovingRectOffset.y + ( SliderSize / drawOffsetScale ) );
								}
								break;
						}
						UpdateFont( ClickedRectInd, Rects[ClickedRectInd].CIIndex, Rects[ClickedRectInd].fontIndex, true );
					}
				}

				if ( MovingRect || ResizingRect ) { //Moving or resizing a rect
					Rect OldRect;
					float OldMaxY = 0, newHeight = 0, newWidth = 0, newY = 0;
					if ( ResizingRect ) {
						checkPos -= ResizingRectOffset;
						OldRect = Rects[ClickedRectInd].rect;
						OldMaxY = OldRect.y + OldRect.height;
						newHeight = Mathf.Max( OldMaxY - checkPos.y, 2 );
						newWidth = Mathf.Max( checkPos.x - OldRect.x, 2 );
						newY = Mathf.Min( checkPos.y, OldMaxY - 2 );
					}
					switch ( Snap ) {
						case 0: //NO SNAP#
							if ( MovingRect ) { Rects[ClickedRectInd].rect = new Rect( checkPos.x - MovingRectOffset.x, checkPos.y - MovingRectOffset.y, Rects[ClickedRectInd].rect.width, Rects[ClickedRectInd].rect.height ); }
							if ( ResizingRect ) { Rects[ClickedRectInd].rect = new Rect( Rects[ClickedRectInd].rect.x, newY, newWidth, newHeight ); }
							break;
						case 1: // SNAP
							if ( MovingRect ) { Rects[ClickedRectInd].rect = SnapToPos( new Rect( checkPos.x - MovingRectOffset.x, checkPos.y - MovingRectOffset.y, Rects[ClickedRectInd].rect.width, Rects[ClickedRectInd].rect.height ), ClickedRectInd ); }
							if ( ResizingRect ) {
								if ( !SnapFound ) {
									Rects[ClickedRectInd].rect = SnapToSize( new Rect( Rects[ClickedRectInd].rect.x, newY, newWidth, newHeight ), ClickedRectInd );
								} else if ( !SnapRect.Contains( Event.current.mousePosition ) ) {
									SnapFound = false;
								}
							}
							break;
						case 2: // CONSTRAIN TO X
							if ( MovingRect ) { Rects[ClickedRectInd].rect = new Rect( checkPos.x - MovingRectOffset.x, Rects[ClickedRectInd].rect.y, Rects[ClickedRectInd].rect.width, Rects[ClickedRectInd].rect.height ); }
							if ( ResizingRect ) { Rects[ClickedRectInd].rect = new Rect( Rects[ClickedRectInd].rect.x, Rects[ClickedRectInd].rect.y, newWidth, Rects[ClickedRectInd].rect.height ); }
							break;
						case 3: // CONSTRAIN TO Y
							if ( MovingRect ) { Rects[ClickedRectInd].rect = new Rect( Rects[ClickedRectInd].rect.x, checkPos.y - MovingRectOffset.y, Rects[ClickedRectInd].rect.width, Rects[ClickedRectInd].rect.height ); }
							if ( ResizingRect ) { Rects[ClickedRectInd].rect = new Rect( Rects[ClickedRectInd].rect.x, newY, Rects[ClickedRectInd].rect.width, newHeight ); }
							break;
					}
					Rects[ClickedRectInd].rect = RoundedRect( Rects[ClickedRectInd].rect );
					UpdateFont( ClickedRectInd, Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ), true );
				}
			}




			//Leftmouse Up, END ANY MOVE OR RESIZE
			if ( e.rawType == EventType.MouseUp && e.button == 0 ) {
				CheckOverlapRects();
				MovingRect = false;
				ResizingRect = false;
				MovingSlider = false;
				MovingSpriteCenter = false;
			}

			//Rightmouse down, start drawOffset pan
			if ( e.type == EventType.MouseDown && ( e.button == 2 || e.button == 1 ) && MouseOffGUI ) {
				drawOffsetClick = true;
			}

			//Right Mouseup from drawOffset move
			if ( e.type == EventType.MouseUp && ( e.button == 2 || e.button == 1 ) && drawOffsetClick ) {
				drawOffsetClick = false;
			}

			//Dragging with rightmouse, update drawOffset pan
			if ( e.type == EventType.MouseDrag && ( e.button == 2 || e.button == 1 ) && drawOffsetClick ) {
				drawOffsetPosDest = drawOffsetPosDest + Event.current.delta;
				drawOffsetPosDest = new Vector2( Mathf.Clamp( drawOffsetPosDest.x, -( theTex.width / 2 ) * drawOffsetScale, ( theTex.width / 2 ) * drawOffsetScale ),
													Mathf.Clamp( drawOffsetPosDest.y, -( theTex.height / 2 ) * drawOffsetScale, ( theTex.height / 2 ) * drawOffsetScale ) );
			}
		}



		//Now GUI drawing, buttons, etc

		PromptOffset1 = Mathf.Lerp( PromptOffset1, PromptOffset1Dest, 0.01f );
		PromptOffset2 = Mathf.Lerp( PromptOffset2, PromptOffset2Dest, 0.01f );
		PromptOffset3 = Mathf.Lerp( PromptOffset3, PromptOffset3Dest, 0.01f );
		PromptOffset4 = Mathf.Lerp( PromptOffset4, PromptOffset4Dest, 0.01f );
		if ( PromptOffset1 < 1 && PromptOffset1Dest == 0 ) { MenuPrompt = false; }
		if ( PromptOffset2 < 1 && PromptOffset2Dest == 0 ) { ResetPrompt = false; }
		if ( PromptOffset3 < 1 && PromptOffset3Dest == 0 ) { AutoPrompt = false; }
		if ( PromptOffset4 < 1 && PromptOffset4Dest == 0 ) { DialogStop = false; }

		GUI.skin = theGUISkin;

		//Draw Main Menu
		if ( UsingMenu ) {
			GUI.Box( MenuBar, "" ); //////Left Box
			GUI.BeginGroup( new Rect( 4, 4, position.width - 4, MenuBar.height - 8 ) );//Clip The Menu pics
			GUI.DrawTexture( new Rect( 0, 0, MenuPics[1].width, MenuPics[1].height ), MenuPics[1] );
			GUI.EndGroup();
			GUI.Box( new Rect( position.width - MenuBar.width, 0, MenuBar.width, MenuBar.height ), "" );/////RightBox
			GUI.BeginGroup( new Rect( 4, 4, position.width - 4, MenuBar.height - 8 ) );//Clip The Menu pics
			GUI.DrawTexture( new Rect( position.width - 8 - MenuPics[2].width, 0, MenuPics[2].width, MenuPics[2].height ), MenuPics[2] );
			GUI.EndGroup();

			GUI.skin = null;
			GUI.BeginGroup( new Rect( position.width - 140, 10, 130, 200 ) ); // Get tex and font for packer
			EditorGUIUtility.LookLikeControls( 65, 40 );
			if ( SpriteEditor ) {
				inFontContainer = (AnimatedSprite) EditorGUI.ObjectField( new Rect( 0, 5, 130, 20 ), "Sprite In", inFontContainer, typeof( AnimatedSprite ), false );
				outFontContainer = (AnimatedSprite) EditorGUI.ObjectField( new Rect( 0, 25, 130, 20 ), "Sprite Out", outFontContainer, typeof( AnimatedSprite ), false );
			} else {
				inFont = (Font) EditorGUI.ObjectField( new Rect( 0, 5, 130, 20 ), "Font In", inFont, typeof( Font ), false );
				outFont = (Font) EditorGUI.ObjectField( new Rect( 0, 25, 130, 20 ), "Font Out", outFont, typeof( Font ), false );
			}
			GUI.skin = theGUISkin;
			GUI.enabled = ( ( !SpriteEditor && inFont != null && outFont != null && inFont != outFont ) ||
			 ( SpriteEditor && inFontContainer != null && outFontContainer != null && inFontContainer != outFontContainer ) );
			if ( GUI.Button( new Rect( 0, 55, 130, 30 ), SpriteEditor ? "Start SpritePacker" : "Start FontPacker" ) ) {
				Begin( SpriteEditor ? 3 : 2 );
			}
			GUI.EndGroup();

			GUI.enabled = true;
			GUI.skin = null;
			GUI.BeginGroup( new Rect( 10, 10, 130, 500 ) ); // Get tex and font for setter
			if ( SpriteEditor ) {
				EditorGUIUtility.LookLikeControls( 40, 80 );
				FontContainer = (AnimatedSprite) EditorGUI.ObjectField( new Rect( 0, 5, 130, 20 ), "Sprite", FontContainer, typeof( AnimatedSprite ), false );
			} else {
				EditorGUIUtility.LookLikeControls( 40, 80 );
				theFont = (Font) EditorGUI.ObjectField( new Rect( 0, 5, 130, 20 ), "Font", theFont, typeof( Font ), false );
			}
			GUI.skin = theGUISkin;
			GUI.enabled = ( ( !SpriteEditor && theFont != null ) || ( SpriteEditor && FontContainer != null ) );
			if ( GUI.Button( new Rect( 0, 55, 130, 30 ), SpriteEditor ? "Start SpriteSetter" : "Start FontSetter" ) ) {
				Begin( SpriteEditor ? 1 : 0 );
			}
			GUI.enabled = true;
			UsePreview = GUI.Toggle( new Rect( 0, 90, 130, 20 ), UsePreview, "Preview 3DText" );
			GUI.EndGroup();

			GUI.BeginGroup( new Rect( 4, position.height - 24, 130, 30 ) ); // Switch to SpriteEditor
			SpriteEditor = GUI.Toggle( new Rect( 0, 0, 130, 20 ), SpriteEditor, "Sprite Mode" );
			GUI.EndGroup();


		} //Enf of main menu


		//Draw FontSetter
		if ( UsingEditor ) {
			if ( e.type == EventType.ValidateCommand && e.commandName == "UndoRedoPerformed" ) {
				HarvestRects( 0, true );
			}
			GUI.skin = theGUISkin;
			GUI.Box( MenuBar, "" ); //////Left Box
			GUI.BeginGroup( new Rect( 4, 4, position.width - 4, MenuBar.height - 8 ) );//Clip The Menu pics
			GUI.DrawTexture( new Rect( 0, 0, MenuPics[1].width, MenuPics[1].height ), MenuPics[1] );
			GUI.EndGroup();
			GUI.Box( new Rect( position.width - MenuBar.width, 0, MenuBar.width, MenuBar.height ), "" );/////RightBox
			GUI.BeginGroup( new Rect( 4, 4, position.width - 4, MenuBar.height - 8 ) );//Clip The Menu pics
			GUI.DrawTexture( new Rect( position.width - 8 - MenuPics[0].width, 0, MenuPics[0].width, MenuPics[0].height ), MenuPics[0] );
			GUI.EndGroup();
			GUI.enabled = !AutoSetDialog && !SpriteSetDialog;

			GUI.BeginGroup( new Rect( position.width - 140, 10, 130, 80 ) ); //Undo and shrinkwrap buttons
			if ( GUI.Button( new Rect( 0, 0, 130, 30 ), "Undo" ) ) {
				Undo.PerformUndo();
			}
			GUI.enabled = !AutoSetDialog && !SpriteSetDialog && Rects.Count > 0;
			if ( GUI.Button( new Rect( 10, 40, 110, 25 ), "Shrinkwrap" ) ) {
				ShrinkWrap();
			}
			GUI.EndGroup();
			GUI.enabled = true;
			GUI.BeginGroup( new Rect( MenuBar.width, 0, position.width - 150, position.height ) );//Clip The Background Box
			GUI.color = Color.gray;
			GUI.skin = theGUISkin;
			GUI.Box( new Rect( 0, 0, position.width - 300, position.height ), "", "White" );
			GUI.color = Color.white;
			GUI.EndGroup();


			//Controls in sprite setter
			if ( SpriteEditor && !SpriteSetDialog ) {
				GUI.BeginGroup( new Rect( position.width - MenuBar.width, 100, MenuBar.width, 200 ) ); //Animation Properties group
				GUI.Box( new Rect( 0, 0, 150, (int) SpritePropertiesLoopBehaviour == 3 ? 160 : ( (int) SpritePropertiesLoopBehaviour == 2 ? 120 : 140 ) ), "" );
				GUI.Label( new Rect( 10, 2, 130, 20 ), "Animation Properties:" );
				EditorGUIUtility.LookLikeControls( 40, 60 );
				SpritePropertiesFontIndex = EditorGUI.Popup( new Rect( 10, 24, 130, 20 ), "Anim:", SpritePropertiesFontIndex, FontListNames );
				SpritePropertiesLoopBehaviour = (SpritePropertiesLoopBehaviourEnum) EditorGUI.EnumPopup( new Rect( 10, 44, 130, 20 ), "Loop:", SpritePropertiesLoopBehaviour );
				if ( (int) SpritePropertiesLoopBehaviour == 3 ) {
					EditorGUIUtility.LookLikeControls( 70, 60 );
					SpritePropertiesNextAnim = EditorGUI.Popup( new Rect( 10, 64, 130, 20 ), "Change to:", SpritePropertiesNextAnim, FontListNames );
				}
				if ( (int) SpritePropertiesLoopBehaviour == 0 || (int) SpritePropertiesLoopBehaviour == 1 || (int) SpritePropertiesLoopBehaviour == 3 ) {
					EditorGUIUtility.LookLikeControls( 105, 60 );
					SpritePropertiesLoopStart = Mathf.Clamp( EditorGUI.IntField( new Rect( 10, (int) SpritePropertiesLoopBehaviour == 3 ? 84 : 64, 130, 18 ), (int) SpritePropertiesLoopBehaviour == 3 ? "Starting at:" : "Loop start frame:", SpritePropertiesLoopStart ), 0, FontList[(int) SpritePropertiesLoopBehaviour == 3 ? SpritePropertiesNextAnim : SpritePropertiesFontIndex].characterInfo.Length - 2 );
				}
				EditorGUIUtility.LookLikeControls( 105, 60 );
				SpritePropertiesAnimFPS = EditorGUI.FloatField( new Rect( 10, (int) SpritePropertiesLoopBehaviour == 3 ? 102 : ( (int) SpritePropertiesLoopBehaviour == 2 ? 62 : 82 ), 130, 18 ), "Anim FPS Multip:", SpritePropertiesAnimFPS );
				AutoAnimate = GUI.Toggle( new Rect( 10, (int) SpritePropertiesLoopBehaviour == 3 ? 116 : ( (int) SpritePropertiesLoopBehaviour == 2 ? 76 : 96 ), 130, 20 ), AutoAnimate, "Auto-animate" );
				GUI.Label( new Rect( 10, (int) SpritePropertiesLoopBehaviour == 3 ? 134 : ( (int) SpritePropertiesLoopBehaviour == 2 ? 94 : 114 ), 30, 20 ), "" + SpriteFrame );
				SpriteFrame = (int) GUI.HorizontalSlider( new Rect( 28, (int) SpritePropertiesLoopBehaviour == 3 ? 140 : ( (int) SpritePropertiesLoopBehaviour == 2 ? 100 : 120 ), 112, 18 ), SpriteFrame, 0, FontList[SpritePropertiesFontIndex].characterInfo.Length - 2 );

				//if you switch auto on, go back to the start.
				if ( AutoAnimate != AutoAnimateOld ) {
					AutoAnimateOld = AutoAnimate;
					if ( AutoAnimate == true ) {
						SpriteFrame = 0;
					}
				}

				//choosing a new font needs to update the other fields
				if ( SpritePropertiesFontIndex != SpritePropertiesFontIndexOld ) {
					ReFocusBlank = true;
					SpritePropertiesFontIndexOld = SpritePropertiesFontIndex;
					SpritePropertiesLoopBehaviour = (SpritePropertiesLoopBehaviourEnum) FontList[SpritePropertiesFontIndex].characterInfo[0].index;
					SpritePropertiesLoopBehaviourOld = SpritePropertiesLoopBehaviour;
					SpritePropertiesNextAnim = (int) FontList[SpritePropertiesFontIndex].characterInfo[0].uv.x;
					SpritePropertiesNextAnimOld = SpritePropertiesNextAnim;
					SpritePropertiesLoopStart = (int) FontList[SpritePropertiesFontIndex].characterInfo[0].uv.y;
					SpritePropertiesLoopStartOld = SpritePropertiesLoopStart;
					SpritePropertiesAnimFPS = FontList[SpritePropertiesFontIndex].characterInfo[0].uv.width + 1;
					SpritePropertiesAnimFPSOld = SpritePropertiesAnimFPS;
					if ( SpritePreview != null ) {
						SpritePreviewT.font = FontList[SpritePropertiesFontIndex];
					}
					SpriteFrame = 0;
					ClickedRectInd = FindFrame();
					if ( ClickedRectInd != -1 ) {
						GetFontInfoToGUI( Rects[ClickedRectInd].CIIndex, Rects[ClickedRectInd].fontIndex );
					}

				}

				//changing anim properties needs to change the font.CI[0]
				if ( SpritePropertiesLoopBehaviour != SpritePropertiesLoopBehaviourOld ) {
					SpritePropertiesLoopBehaviourOld = SpritePropertiesLoopBehaviour;
					SO = new SerializedObject( FontList[SpritePropertiesFontIndex] );
					p = SO.FindProperty( "m_CharacterRects.Array" );
					p = p.GetArrayElementAtIndex( 0 );
					p = p.FindPropertyRelative( "index" );
					p.intValue = (int) SpritePropertiesLoopBehaviour;
					SO.ApplyModifiedProperties();
				}

				if ( SpritePropertiesNextAnim != SpritePropertiesNextAnimOld ) {
					SpritePropertiesNextAnimOld = SpritePropertiesNextAnim;
					SO = new SerializedObject( FontList[SpritePropertiesFontIndex] );
					p = SO.FindProperty( "m_CharacterRects.Array" );
					p = p.GetArrayElementAtIndex( 0 );
					p = p.FindPropertyRelative( "uv" );
					p.rectValue = new Rect( SpritePropertiesNextAnim, p.rectValue.y, p.rectValue.width, p.rectValue.height );
					SO.ApplyModifiedProperties();
				}

				if ( SpritePropertiesLoopStart != SpritePropertiesLoopStartOld ) {
					SpritePropertiesLoopStartOld = SpritePropertiesLoopStart;
					SO = new SerializedObject( FontList[SpritePropertiesFontIndex] );
					p = SO.FindProperty( "m_CharacterRects.Array" );
					p = p.GetArrayElementAtIndex( 0 );
					p = p.FindPropertyRelative( "uv" );
					p.rectValue = new Rect( p.rectValue.x, SpritePropertiesLoopStart, p.rectValue.width, p.rectValue.height );
					SO.ApplyModifiedProperties();
				}

				if ( SpritePropertiesAnimFPS != SpritePropertiesAnimFPSOld ) {
					SpritePropertiesAnimFPSOld = SpritePropertiesAnimFPS;
					SO = new SerializedObject( FontList[SpritePropertiesFontIndex] );
					p = SO.FindProperty( "m_CharacterRects.Array" );
					p = p.GetArrayElementAtIndex( 0 );
					p = p.FindPropertyRelative( "uv" );
					p.rectValue = new Rect( p.rectValue.x, p.rectValue.y, SpritePropertiesAnimFPS - 1, p.rectValue.height );
					SO.ApplyModifiedProperties();
				}

				if ( e.type == EventType.KeyDown && e.keyCode == KeyCode.Period && MouseOffGUI && !AutoAnimate ) {
					SpriteFrame++;
				}

				if ( e.type == EventType.KeyDown && e.keyCode == KeyCode.Comma && MouseOffGUI && !AutoAnimate ) {
					SpriteFrame--;
				}

				if ( SpriteFrame != SpriteFrameOld && !AutoAnimate && SpritePreview != null ) {
					if ( SpriteFrame < 0 ) { SpriteFrame = FontList[SpritePropertiesFontIndex].characterInfo.Length - 2; }
					if ( SpriteFrame > FontList[SpritePropertiesFontIndex].characterInfo.Length - 2 ) { SpriteFrame = 0; }
					SpriteFrameOld = SpriteFrame;
					char C = (char) ( SpriteFrame + 33 );
					SpritePreviewT.text = "" + C;
					ClickedRectInd = FindFrame();
					if ( ClickedRectInd != -1 ) {
						GetFontInfoToGUI( Rects[ClickedRectInd].CIIndex, Rects[ClickedRectInd].fontIndex );
					}
				}
				GUI.EndGroup();



			}

			GUI.enabled = ( !AutoSetDialog && !SpriteSetDialog && !AutoPrompt && !MenuPrompt );
			GUI.BeginGroup( new Rect( position.width - MenuBar.width + 10, position.height - 150 - PromptOffset1, MenuBar.width - 10, 150 ) ); //RESET Group
			GUI.BeginGroup( new Rect( 0, 30 - PromptOffset2, 130, 30 ) ); //RESET Group
			if ( ResetPrompt ) {
				if ( GUI.Button( new Rect( 0, 0, 60, 30 ), "Reset" ) ) {
					ResetFont( -1 );
					PromptOffset2Dest = 0;
					ResetPrompt = false;
				}
				if ( GUI.Button( new Rect( 70, 0, 60, 30 ), "Cancel" ) ) {
					PromptOffset2Dest = 0;
				}
			}
			GUI.EndGroup();
			if ( GUI.Button( new Rect( 0, 31, 130, 30 ), "Reset Font" ) ) {
				PromptOffset2Dest = 30;
				PromptOffset1Dest = 0;
				MenuPrompt = false;
				AutoPrompt = false;
				ResetPrompt = true;
			}
			GUI.EndGroup();

			GUI.enabled = ( !AutoSetDialog && !SpriteSetDialog && !ResetPrompt && !MenuPrompt );
			GUI.BeginGroup( new Rect( position.width - MenuBar.width + 10, position.height - 230 - PromptOffset1 - PromptOffset2, MenuBar.width - 10, 200 ) ); //AUTO Group
			GUI.BeginGroup( new Rect( 0, 71 - PromptOffset3, 130, PromptOffset3 ) ); //AUTO Group
			if ( AutoPrompt ) {
				if ( !SpriteEditor ) {
					if ( GUI.Button( new Rect( 0, 45, 60, 25 ), "Basic" ) ) {
						ResetFont( -1 );
						AutoSet();
						AutoPrompt = false;
					}
					if ( GUI.Button( new Rect( 0, 10, 60, 25 ), "Smart" ) ) {
						SmartMode = true;
						ResetFont( -1 );
						AutoSet();
						AutoPrompt = false;
					}
				} else {
					if ( GUI.Button( new Rect( 0, 0, 60, 28 ), "Auto!" ) ) {
						ResetFont( -1 );
						AutoSet();
						AutoPrompt = false;
					}

					if ( GUI.Button( new Rect( 0, 29, 60, 20 ), "Get Piv" ) ) {
						LocatePivots();
						if ( ClickedRectInd != -1 ) { GetFontInfoToGUI( Rects[ClickedRectInd].CIIndex, Rects[ClickedRectInd].fontIndex ); }
						PromptOffset3Dest = 0;
					}

					if ( GUI.Button( new Rect( 0, 50, 60, 20 ), "Draw Piv" ) ) {
						DrawSpritePivots();
					}
				}

				GUI.Label( new Rect( 63, 0, 150, 20 ), SpriteEditor ? "Pivot Color:" : "SmartColor:" );
				SmartColor = EditorGUI.ColorField( new Rect( 71, 20, 60, 20 ), SmartColor );

				if ( GUI.Button( new Rect( 70, 45, 60, 25 ), "Cancel" ) ) {
					PromptOffset3Dest = 0;
				}
			}
			GUI.EndGroup();
			if ( GUI.Button( new Rect( 0, 71, 130, 30 ), "Auto Set" ) ) {
				PromptOffset1Dest = 0;
				PromptOffset2Dest = 0;
				PromptOffset3Dest = 71;
				AutoPrompt = true;
				ResetPrompt = false;
				MenuPrompt = false;
			}
			GUI.EndGroup();
			GUI.enabled = true;

			GUI.BeginGroup( new Rect( position.width - MenuBar.width + 10, position.height - 80 - PromptOffset1, MenuBar.width, 150 ) ); //ResetCam Group
			if ( GUI.Button( new Rect( 0, 0, 130, 30 ), "Reset View" ) ) {
				drawOffsetPosDest = Vector2.zero;
				drawOffsetScaleDest = 1;
			}
			GUI.EndGroup();

			GUI.enabled = ( !AutoSetDialog && !SpriteSetDialog && !ResetPrompt && !AutoPrompt );
			GUI.BeginGroup( new Rect( position.width - MenuBar.width + 10, position.height - 70, MenuBar.width, 150 ) ); //Back to menu Group
			GUI.BeginGroup( new Rect( 0, 30 - PromptOffset1, 130, 150 ) ); //Back to menu Group
			if ( MenuPrompt ) {
				if ( GUI.Button( new Rect( 0, 0, 60, 30 ), "Return" ) ) {
					UsingEditor = false;
					UsingMenu = true;
					ResetError();
					MenuPrompt = false;
					PromptOffset1Dest = 0;
					position = new Rect( position.x, position.y - 5, 512, 512 );
				}
				if ( GUI.Button( new Rect( 70, 0, 60, 30 ), "Cancel" ) ) {
					PromptOffset1Dest = 0;
				}
			}
			GUI.EndGroup();
			if ( GUI.Button( new Rect( 0, 31, 130, 30 ), "Return to Menu" ) ) {
				PromptOffset1Dest = 30;
				PromptOffset2Dest = 0;
				MenuPrompt = true;
				AutoPrompt = false;
				ResetPrompt = false;
			}
			GUI.EndGroup();

			GUI.skin = theGUISkin;
			GUI.enabled = !AutoSetDialog && !SpriteSetDialog;
			GUI.BeginGroup( new Rect( MenuBar.width, 0, ( position.width - 300 ), position.height ) ); //clip the Tex
			GUI.color = BGCol; //Draw a box for the background.
			GUI.Box( new Rect( drawOffsetPos.x - ShrunkX - ( theTex.width / 2 * ( drawOffsetScale - 1 ) ) - 6, drawOffsetPos.y - ShrunkY - ( theTex.height / 2 * ( drawOffsetScale - 1 ) ) - 6, 12 + theTex.width * drawOffsetScale, 12 + theTex.height * drawOffsetScale ), "", "theTexBG" );
			GUI.color = Color.white;//Draw the fonts texture
			GUI.DrawTexture( new Rect( drawOffsetPos.x - ShrunkX - ( theTex.width / 2 * ( drawOffsetScale - 1 ) ), drawOffsetPos.y - ShrunkY - ( theTex.height / 2 * ( drawOffsetScale - 1 ) ), theTex.width * drawOffsetScale, theTex.height * drawOffsetScale ), theTex );
			GUI.EndGroup();

			GUI.BeginGroup( new Rect( MenuBar.width, 0, position.width - 300, position.height ) );//Clip The Rects
			for ( int i = 0; i < Rects.Count; i++ ) {
				if ( RectOnScreen( ConvertPixelsToScreen( Rects[i].rect ) ) ) {
					Rect ScreenRect = ConvertPixelsToScreen( Rects[i].rect );
					GUI.Box( ScreenRect, "", i == ClickedRectInd ? "EmptyBoxSelected" : "EmptyBox" ); //draw a rect for each character
					if ( PerCharacterOffset ) {
						Rect SliderBox;
						if ( Rects[i].Orient ) {
							SliderBox = new Rect( Rects[i].rect.xMax - Rects[i].vOffset - ( SliderSize / drawOffsetScale ), Rects[i].rect.y, ( SliderSize / drawOffsetScale ) * 2, Rects[i].rect.height );
						} else {
							SliderBox = new Rect( Rects[i].rect.x, Rects[i].rect.yMax - Rects[i].vOffset - ( SliderSize / drawOffsetScale ), Rects[i].rect.width, ( SliderSize / drawOffsetScale ) * 2 );
						}
						GUI.Box( ConvertPixelsToScreen( SliderBox ), "", "GuideLine" ); //draw the slider for each rect
					}
					if ( SpriteEditor ) {
						//draw the sprite pivot, and 2 labels for the sprite index
						GUI.Box( ConvertPixelsToScreen( new Rect( Rects[i].rect.x + Rects[i].spritePivot.x - ( 7.0f / drawOffsetScale ), Rects[i].rect.y + Rects[i].spritePivot.y - ( 7.0f / drawOffsetScale ), 16.0f / drawOffsetScale, 16.0f / drawOffsetScale ) ), "", i == ClickedRectInd ? "SpritePivotSelected" : "SpritePivot" );
						GUI.Label( new Rect( ScreenRect.x + 1, ScreenRect.y + 1, ScreenRect.width, ScreenRect.height ), "" + ( FontList[Rects[i].fontIndex].characterInfo[Rects[i].CIIndex].index - 33 ), "RectLabel2" );
						GUI.color = Colors[Rects[i].fontIndex % Colors.Length];
						GUI.Label( ScreenRect, "" + ( FontList[Rects[i].fontIndex].characterInfo[Rects[i].CIIndex].index - 33 ), "RectLabel" );
						GUI.color = Color.white;
					} else {
						//draw 2 labels for the character
						GUI.Label( new Rect( ScreenRect.x + 1, ScreenRect.y + 1, ScreenRect.width, ScreenRect.height ), "" + (char) ( FontList[0].characterInfo[Rects[i].CIIndex].index ), "RectLabel2" );
						GUI.Label( ScreenRect, "" + (char) FontList[0].characterInfo[Rects[i].CIIndex].index, "RectLabel" );
					}
				}
			}
			GUI.EndGroup();

			if ( AutoSetDialog || SpriteSetDialog ) {
				GUI.BeginGroup( new Rect( MenuBar.width, 0, position.width - 300, position.height ) );//Clip The Auto Rects
				GUI.enabled = true;
				GUI.Box( ConvertPixelsToScreen( AutoRectList[AutoSetIndex].rect ), "", "AutoBox" );
				GUI.EndGroup();
			}


			GUI.BeginGroup( new Rect( 10, 6, MenuBar.width - 10, 65 ) ); //Size Group
			GUI.Label( new Rect( 0, 0, 140, 40 ), SpriteEditor ? "Number of \nsprites:" : "Number of\ncharacters:" );
			GUI.enabled = false;
			CharCount = EditorGUI.IntField( new Rect( 80, 9, 40, 20 ), CharCount, "TextField" );
			GUI.enabled = !AutoSetDialog && !SpriteSetDialog;

			if ( GUI.Button( new Rect( 0, 42, 50, 20 ), "+1" ) ) {
#if UNITY_2_6 || UNITY_2_6_1 ||UNITY_3_0 ||UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
				Undo.CreateSnapshot();
				Undo.RegisterSnapshot();
#else
				Undo.RecordObjects( FontList, "Font Changed" );
#endif
				GUI.FocusControl( "" );
				SO = new SerializedObject( FontList[RectFontIndex] );
				p = SO.FindProperty( "m_CharacterRects.Array.size" );
				WidBefore = 0;
				WidBeforeOld = 0;
				WidAfter = 0;
				WidAfterOld = 0;
				Orient = 0;
				OrientOld = 0;
				SpriteIndex++;
				SpriteIndexOld = SpriteIndex;
				CharCount = CharCount + 1;
				p.intValue++;
				SO.ApplyModifiedProperties();
				SetRect newSetRect = new SetRect();
				if ( ClickedRectInd == -1 ) {
					newSetRect.rect = new Rect( 0, 0, 100, 100 );
				} else {
					newSetRect = new SetRect( Rects[ClickedRectInd] );
				}
				newSetRect.CIIndex = FontList[newSetRect.fontIndex].characterInfo.Length - 1;
				Rects.Add( newSetRect );
				ClickedRectInd = CharCount - 1;
				ClickedOnRect = true;
				char newChar = (char) ( ChrL + 1 );
				ChrStr = "" + newChar;
				ChrL++;
				UpdateFont( ClickedRectInd, Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ), true );
				if ( SpriteEditor ) {
					CheckSprites();
				} else {
					CheckDupeChars();
				}
				if ( Rects.Count == 1 ) { CameraOnRect( Rects[Rects.Count - 1].rect ); }
			}
			GUI.enabled = ClickedOnRect && !AutoSetDialog && !SpriteSetDialog && ClickedRectInd > -1;

			if ( e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete && ClickedRectInd != -1 && MouseOffGUI ) {
				RemoveRect();
			}

			if ( GUI.Button( new Rect( 60, 42, 70, 20 ), "Remove" ) ) {
#if UNITY_2_6 || UNITY_2_6_1 ||UNITY_3_0 ||UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5 || UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2
				Undo.CreateSnapshot();
				Undo.RegisterSnapshot();
#else
				Undo.RecordObjects( FontList, "Font Changed" );
#endif
				RemoveRect();
			}

			GUI.EndGroup(); // End Size Set group

			GUI.BeginGroup( new Rect( 0, 80, MenuBar.width, 200 ) ); //Rect properties group
			GUI.Box( new Rect( 0, 0, 150, SpriteEditor ? 192 : 142 ), "" );
			GUI.Label( new Rect( 10, 2, 140, 20 ), SpriteEditor ? "Sprite Properties:" : "Character Properties:" );
			GUI.Label( new Rect( 33, 23, 140, 20 ), SpriteEditor ? "" : "Character:" );
			GUI.SetNextControlName( "CharacterEntry" );
			if ( SpriteEditor ) {
				EditorGUIUtility.LookLikeControls( 83, 25 );
				SpriteIndex = Mathf.Max( EditorGUI.IntField( new Rect( 15, 23, 128, 20 ), "Sprite Index:", SpriteIndex, "TextField" ), 0 );
				if ( ClickedRectInd > -1 && ( SpriteIndex != SpriteIndexOld ) && !AutoSetDialog && !SpriteSetDialog ) {
					SpriteIndexOld = SpriteIndex;
					ChrL = SpriteIndex + 33;
					UpdateFont( ClickedRectInd, Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ), true );
					CheckSprites();
				}
			} else {
				ChrStr = EditorGUI.TextField( new Rect( 98, 23, 45, 20 ), "", ChrStr, "TextField" );
				if ( ChrStr.Length != 0 && ChrL != ChrStr[0] && !AutoSetDialog && !SpriteSetDialog && ClickedRectInd != -1 ) {
					ChrL = ChrStr[0];
					UpdateFont( ClickedRectInd, Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ), true );
					CheckDupeChars();
				}
			}

			EditorGUIUtility.LookLikeControls( 20, 25 );
			UVx = EditorGUI.IntField( new Rect( 10, 50, 65, 20 ), "X:", UVx, "TextField" );
			UVy = EditorGUI.IntField( new Rect( 78, 50, 65, 20 ), "Y:", UVy, "TextField" );
			UVw = EditorGUI.IntField( new Rect( 10, 70, 65, 20 ), "W:", UVw, "TextField" );
			UVh = EditorGUI.IntField( new Rect( 78, 70, 65, 20 ), "H:", UVh, "TextField" );

			if ( UVRect != new Rect( UVx, UVy, UVw, UVh ) && !AutoSetDialog && !SpriteSetDialog ) {
				UVRect = new Rect( UVx, UVy, UVw, UVh );
				Rects[ClickedRectInd].rect = UVRect;
				UpdateFont( ClickedRectInd, Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ), true );
			}

			if ( SpriteEditor ) {
				GUI.Label( new Rect( 12, 95, 100, 60 ), "Sprite Pivot:" );
				EditorGUIUtility.LookLikeControls( 20, 25 );
				Cx = EditorGUI.FloatField( new Rect( 10, 115, 65, 20 ), "X:", Cx, "TextField" );
				Cy = EditorGUI.FloatField( new Rect( 78, 115, 65, 20 ), "Y:", Cy, "TextField" );

				if ( CPos != new Vector2( Cx, Cy ) && !AutoSetDialog && !SpriteSetDialog ) {
					CPos = new Vector2( Cx, Cy );
					Rects[ClickedRectInd].spritePivot = CPos;
					UpdateFont( ClickedRectInd, Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ), true );
				}

				EditorGUIUtility.LookLikeControls( 40, 60 );
				RectFontIndex = EditorGUI.Popup( new Rect( 10, 145, 130, 20 ), "Font:", RectFontIndex, FontListNames );

				//change the font index outside of autoset
				if ( RectFontIndex != RectFontIndexOld && !AutoSetDialog && !SpriteSetDialog && Rects.Count > 1 ) {
					CharacterInfo CI = FontList[RectFontIndexOld].characterInfo[Rects[ClickedRectInd].CIIndex];
					SO = new SerializedObject( FontList[RectFontIndexOld] );
					p = SO.FindProperty( "m_CharacterRects.Array" );
					p.DeleteArrayElementAtIndex( Rects[ClickedRectInd].CIIndex ); //Delete the CI from the old font
					SO.ApplyModifiedProperties();
					for ( int i = 0; i < Rects.Count; i++ ) { //Shift down all the CIindex for all rects in this font
						if ( Rects[i].fontIndex == RectFontIndexOld && Rects[i].CIIndex > Rects[ClickedRectInd].CIIndex ) {
							Rects[i].CIIndex--;
						}
					}
					SO = new SerializedObject( FontList[RectFontIndex] );
					p = SO.FindProperty( "m_CharacterRects.Array.size" );
					p.intValue = FontList[RectFontIndex].characterInfo.Length + 1; //Expand the new font to accomodate the CI
					p = SO.FindProperty( "m_CharacterRects.Array" );
					p2 = p.GetArrayElementAtIndex( FontList[RectFontIndex].characterInfo.Length ); //Set all the values from the CI
					p2.Next( true ); //p2 is now CharIndex
					p2.intValue = CI.index;
					p2.Next( false ); //p2 is now UV Rect
					p2.rectValue = CI.uv;
					p2.Next( false ); //p2 is now Vert Rect
					p2.rectValue = CI.vert;
					p2.Next( false ); //p2 is now Width
					p2.floatValue = CI.width;
					p2.Next( false ); //p2 is now Flip bool
					p2.boolValue = CI.flipped;
					SO.ApplyModifiedProperties();

					Rects[ClickedRectInd].fontIndex = RectFontIndex;
					Rects[ClickedRectInd].CIIndex = FontList[RectFontIndex].characterInfo.Length - 1;
					UpdateFont( ClickedRectInd, Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ), true );
					RectFontIndexOld = RectFontIndex;
					CheckSprites();
				}

				//change the font index during autoset
				if ( RectFontIndex != RectFontIndexOld && !AutoSetDialog && SpriteSetDialog ) {
					SpriteIndex = FontList[RectFontIndex].characterInfo.Length - 1;
					RectFontIndexOld = RectFontIndex;
				}

				GUI.enabled = ErrorMsg[(int) Errors.DupeSprite] || ErrorMsg[(int) Errors.SkipSprite];
				if ( GUI.Button( new Rect( 50, 165, 90, 18 ), "Fix Indices" ) ) {
					int missing = CheckSprites( Rects[ClickedRectInd].fontIndex );
					while ( missing != -1 ) {
						for ( int i = 0; i < Rects.Count; i++ ) {
							if ( Rects[i].fontIndex == Rects[ClickedRectInd].fontIndex ) {
								if (
								( ErrorMsg[(int) Errors.DupeSprite] && missing != i && FontList[Rects[ClickedRectInd].fontIndex].characterInfo[Rects[i].CIIndex].index >= FontList[Rects[ClickedRectInd].fontIndex].characterInfo[Rects[missing].CIIndex].index ) ||
								( ErrorMsg[(int) Errors.SkipSprite] && FontList[Rects[ClickedRectInd].fontIndex].characterInfo[Rects[i].CIIndex].index - 33 > missing ) ) {
									SO = new SerializedObject( FontList[Rects[ClickedRectInd].fontIndex] );
									p = SO.FindProperty( "m_CharacterRects.Array" );
									p2 = p.GetArrayElementAtIndex( Rects[i].CIIndex );
									p2.Next( true ); //p2 is now CharIndex
									p2.intValue += DupeMissOffset;
									SO.ApplyModifiedProperties();
								}
							}
						}
						missing = CheckSprites( Rects[ClickedRectInd].fontIndex );
					}
					GetFontInfoToGUI( Rects[ClickedRectInd].CIIndex, Rects[ClickedRectInd].fontIndex );
				}
			} else {
				EditorGUIUtility.LookLikeControls( 88, 25 );
				WidBefore = EditorGUI.FloatField( new Rect( 10, 95, 133, 20 ), "Leading Width:", WidBefore, "TextField" );
				WidAfter = EditorGUI.FloatField( new Rect( 10, 115, 133, 20 ), "Trailing Width:", WidAfter, "TextField" );

				if ( WidBefore != WidBeforeOld && !AutoSetDialog && !SpriteSetDialog ) {
					WidBeforeOld = WidBefore;
					Rects[ClickedRectInd].bWidth = WidBefore;
					UpdateFont( ClickedRectInd, Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ), true );
				}

				if ( WidAfter != WidAfterOld && !AutoSetDialog && !SpriteSetDialog ) {
					WidAfterOld = WidAfter;
					Rects[ClickedRectInd].aWidth = WidAfter;
					UpdateFont( ClickedRectInd, Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ), true );
				}
			}
			GUI.EndGroup(); // End Rect properties group

			if ( !SpriteEditor ) {
				GUI.enabled = !PerCharacterOffset && !AutoSetDialog;
				if ( GUI.Button( new Rect( 10, 230, 130, 35 ), "Use Per-Character\nVertical Offsets" ) ) {
					for ( int i = 0; i < Rects.Count; i++ ) {
						SO = new SerializedObject( FontList[0] );
						p = SO.FindProperty( "m_CharacterRects" );
						p.Next( true );
						p2 = p.GetArrayElementAtIndex( i );
						p2.Next( true ); //p2 is now CharIndex
						p2.Next( false ); //p2 is now UV Rect
						p2.Next( false ); //p2 is now Vert Rect
						Rects[i].vOffset = p2.rectValue.y;
					}
					PerCharacterOffset = true;
				}
			}

			GUI.BeginGroup( new Rect( 10, 270, MenuBar.width - 10, 150 ) ); //Snaps Group
			GUI.Label( new Rect( 0, 0, 100, 20 ), "Snap:" );
			GUI.enabled = ClickedOnRect && !AutoSetDialog && !SpriteSetDialog && ClickedRectInd > -1;
			Snap = GUI.SelectionGrid( new Rect( 0, 20, 130, 40 ), Snap, Snaps, 2 );
			GUI.EndGroup();

			GUI.BeginGroup( new Rect( 0, 330, 150, 250 ) ); //Orientation buttons
			GUI.Label( new Rect( 10, 0, 130, 20 ), "Character Orientation:" );
			Orient = GUI.SelectionGrid( new Rect( 11, 20, 128, 64 ), Orient, OrientPics, 2, "BlankButton" );

			if ( Orient != OrientOld && !AutoSetDialog && !SpriteSetDialog ) {
				OrientOld = Orient;
				Rects[ClickedRectInd].Orient = ( Orient == 1 );
				UpdateFont( ClickedRectInd, Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ), true );
			}
			GUI.EndGroup();

			GUI.BeginGroup( new Rect( 0, 417, 150, 250 ) ); //interface toggles
			GUI.Label( new Rect( 30, 0, 130, 40 ), SpriteEditor ? "Move all\nsprite pivots" : "Move all\nvertical offsets" );
			MovingAllToggle = GUI.Toggle( new Rect( 10, 6, 130, 20 ), MovingAllToggle, "" );
			if ( MovingAllToggle && SpriteEditor ) {
				MovingAllSpriteToggle = GUI.Toggle( new Rect( 10, 32, 130, 20 ), MovingAllSpriteToggle, "Only this anim" );
			}
			GUI.EndGroup();

			GUI.enabled = true;

			GUI.BeginGroup( new Rect( 0, position.height - 40, MenuBar.width, 50 ) ); //BGCol Group
			GUI.Label( new Rect( 10, 0, 130, 40 ), "Background\ncolor:" );
			BGCol = EditorGUI.ColorField( new Rect( 90, 8, 50, 20 ), BGCol );
			GUI.EndGroup();

			if ( AutoSetDialog ) {//AutoSet Dialog
				GUI.BeginGroup( new Rect( position.width - 150, ( position.height / 2 ) - 120, 150, 200 ) ); //Clip AUTOSET Dialog
				GUI.Box( new Rect( 0, 0, 150, 135 + PromptOffset4 ), "" );
				GUI.Label( new Rect( 10, 10, 140, 20 ), "What character is this?" );
				if ( e.type == EventType.KeyDown && e.keyCode == KeyCode.Return && AutoSetCharStr.Length > 0 && GUI.GetNameOfFocusedControl() == "AutoText" ) {
					AddRect( false, 0 );
				}
				if ( e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape && GUI.GetNameOfFocusedControl() == "AutoText" ) {
					SkipRect();
				}
				GUI.SetNextControlName( "AutoText" );
				AutoSetCharStr = EditorGUI.TextField( new Rect( 55, 30, 40, 21 ), "", AutoSetCharStr, "TextField" );
				GUI.enabled = AutoSetCharStr.Length > 0;
				if ( GUI.Button( new Rect( 10, 55, 60, 30 ), "Next" ) ) {
					AddRect( false, 0 );
				}
				GUI.enabled = true;
				if ( GUI.Button( new Rect( 80, 55, 60, 30 ), "Skip" ) ) {
					SkipRect();
				}
				GUI.BeginGroup( new Rect( 10, 90 + PromptOffset4, 130, 30 ) ); //stop AUTOSET Dialog
				if ( DialogStop ) {
					if ( GUI.Button( new Rect( 0, 0, 60, 30 ), "Stop" ) ) {
						PromptOffset4Dest = 0;
						EndAutoSet();
						AutoRectList.Clear();
					}
					if ( GUI.Button( new Rect( 70, 0, 60, 30 ), "Cancel" ) ) {
						PromptOffset4Dest = 0;
					}
				}
				GUI.EndGroup();
				if ( GUI.Button( new Rect( 10, 90, 130, 30 ), "Stop Auto Set" ) ) {
					DialogStop = true;
					PromptOffset4Dest = 30;
				}
				if ( CharRepeatList.Count > 1 && AutoSetIndex == 0 ) {
					if ( GUI.Button( new Rect( 30, 135 + PromptOffset4, 90, 20 ), "Repeat Last" ) ) {
						RepeatSetting = true;
						for ( int i = 0; i < CharRepeatList.Count; i++ ) {
							if ( CharRepeatList[i] == 0 ) {
								AutoRectList.RemoveAt( AutoSetIndex );
								AutoSetCharStr = "";
							} else {
								AutoSetCharStr = "" + CharRepeatList[i];
								AddRect( false, 0 );
							}
						}
						RepeatSetting = false;
						EndAutoSet();
					}
				}
				GUI.FocusControl( "AutoText" );
				GUI.EndGroup();
			}

			if ( ErrorMsg[(int) Errors.Spacebar] ) {
				GUI.BeginGroup( new Rect( position.width - MenuBar.width, 100, MenuBar.width, 200 ) );//SPACEBAR Prompt
				GUI.Box( new Rect( 0, 0, 150, 61 ), "" );
				GUI.Label( new Rect( 10, 4, 135, 60 ), "Don't forget to add a Rect for the Spacebar character!" );
				if ( GUI.Button( new Rect( 96, 39, 50, 18 ), "OK" ) ) {
					ErrorMsg[(int) Errors.Spacebar] = false;
				}
				GUI.EndGroup();
			}

			if ( ErrorMsg[(int) Errors.SkipSprite] ) {
				GUI.BeginGroup( new Rect( position.width - MenuBar.width - ( ErrorMsg[(int) Errors.OverlapSprite] ? 323 : 169 ), position.height - 43, 170, 43 ) );//Skipped Sprite
				GUI.Box( new Rect( 0, 0, 170, 43 ), "" );
				SplitError = SkipErrorString.Split( '#' );
				GUI.color = Colors[int.Parse( SplitError[0] )];
				GUI.Label( new Rect( 6, 5, 160, 60 ), SplitError[1], "RectLabel" );
				GUI.color = Color.white;
				GUI.Label( new Rect( 10, 19, 160, 60 ), SplitError[2] );
				GUI.EndGroup();
			}

			if ( ErrorMsg[(int) Errors.DupeSprite] ) {
				GUI.BeginGroup( new Rect( position.width - MenuBar.width - ( ErrorMsg[(int) Errors.OverlapSprite] ? 308 : 154 ), position.height - 57, 170, 57 ) );//Duped Sprite
				GUI.Box( new Rect( 0, 0, 155, 57 ), "" );
				SplitError = DupeErrorString.Split( '#' );
				GUI.Label( new Rect( 10, 4, 150, 60 ), "" + SplitError[0] );
				GUI.color = Colors[int.Parse( SplitError[1] )];
				GUI.Label( new Rect( 8, 22, 150, 60 ), "" + SplitError[2], "RectLabel" );
				GUI.color = Color.white;
				GUI.Label( new Rect( 10, 34, 150, 60 ), "" + SplitError[3] );
				GUI.EndGroup();
			}

			if ( ErrorMsg[(int) Errors.OverlapSprite] ) {
				GUI.BeginGroup( new Rect( position.width - MenuBar.width - 154, position.height - 80, 170, 80 ) );//Overlapping Sprite
				GUI.Box( new Rect( 0, 0, 155, 80 ), "" );
				SplitError = OverlapErrorString.Split( '#' );
				GUI.Label( new Rect( 10, 3, 150, 30 ), "The Rect for" );
				GUI.color = Colors[int.Parse( SplitError[0] )];

				if ( GUI.Button( new Rect( 4, 22, 147, 18 ), SplitError[1] ) ) {
					ClickedRectInd = SecondOverlapRect;
					ReFocusBlank = true;
					CameraOnRect( Rects[ClickedRectInd].rect );
					GetFontInfoToGUI( Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ) );
				}

				GUI.color = Color.white;
				GUI.Label( new Rect( 10, 39, 150, 30 ), "overlaps with" );
				GUI.color = Colors[int.Parse( SplitError[2] )];

				if ( GUI.Button( new Rect( 4, 58, 147, 18 ), SplitError[3] ) ) {
					ClickedRectInd = FirstOverlapRect;
					ReFocusBlank = true;
					CameraOnRect( Rects[ClickedRectInd].rect );
					GetFontInfoToGUI( Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ) );
				}

				GUI.color = Color.white;
				GUI.EndGroup();
			}

			if ( ErrorMsg[(int) Errors.DupeChar] ) {
				GUI.BeginGroup( new Rect( position.width - MenuBar.width - 129, position.height - 60, 130, 60 ) );//Duped Character
				GUI.Box( new Rect( 0, 0, 130, 60 ), "" );
				GUI.Label( new Rect( 8, 2, 120, 60 ), DupeErrorString );
				if ( GUI.Button( new Rect( 4, 38, 60, 18 ), "First" ) ) {
					ClickedRectInd = FirstDupeRect;
					ReFocusBlank = true;
					CameraOnRect( Rects[ClickedRectInd].rect );
					GetFontInfoToGUI( Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ) );
				}
				if ( GUI.Button( new Rect( 66, 38, 60, 18 ), "Second" ) ) {
					ClickedRectInd = SecondDupeRect;
					ReFocusBlank = true;
					CameraOnRect( Rects[ClickedRectInd].rect );
					GetFontInfoToGUI( Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ) );
				}
				GUI.EndGroup();
			}

			if ( ErrorMsg[(int) Errors.OverlapChar] ) {
				GUI.BeginGroup( new Rect( position.width - MenuBar.width - ( ErrorMsg[(int) Errors.DupeChar] ? 228 : 99 ), position.height - 60, 130, 60 ) );//Overlapping Character Rects 
				SplitError = OverlapErrorString.Split( '#' );
				GUI.Box( new Rect( 0, 0, 100, 60 ), "" );
				GUI.Label( new Rect( 6, 4, 90, 60 ), SplitError[0] );
				if ( GUI.Button( new Rect( 4, 38, 45, 18 ), SplitError[1] ) ) {
					ClickedRectInd = FirstOverlapRect;
					ReFocusBlank = true;
					CameraOnRect( Rects[ClickedRectInd].rect );
					GetFontInfoToGUI( Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ) );
				}
				if ( GUI.Button( new Rect( 51, 38, 45, 18 ), SplitError[2] ) ) {
					ClickedRectInd = SecondOverlapRect;
					ReFocusBlank = true;
					CameraOnRect( Rects[ClickedRectInd].rect );
					GetFontInfoToGUI( Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ) );
				}
				GUI.EndGroup();
			}


			if ( SpriteSetDialog ) {//SpriteSet Dialog
				GUI.BeginGroup( new Rect( position.width - 150, position.height / 2 - 120, 167, 200 ) ); //Clip SpriteSet Dialog
				GUI.Box( new Rect( 0, 0, 150, 149 + PromptOffset4 ), "" );
				if ( GUI.Button( new Rect( 10, 72, 60, 30 ), "Next" ) ) {
					AddRect( true, RectFontIndex );
				}
				if ( e.type == EventType.KeyDown && e.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "SpriteIndInput" ) {
					AddRect( true, RectFontIndex );
				}
				if ( e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape && GUI.GetNameOfFocusedControl() == "SpriteIndInput" ) {
					SkipRect();
				}

				EditorGUIUtility.LookLikeControls( 40, 60 );
				RectFontIndex = EditorGUI.Popup( new Rect( 10, 10, 130, 20 ), "Font:", RectFontIndex, FontListNames );
				EditorGUIUtility.LookLikeControls( 80, 50 );
				GUI.SetNextControlName( "SpriteIndInput" );
				SpriteIndex = Mathf.Max( EditorGUI.IntField( new Rect( 10, 30, 130, 20 ), "Sprite Index:", SpriteIndex ), 0 );
				EditorGUIUtility.LookLikeControls( 40, 60 );
				defaultPivotPoint = (defaultPivotPointEnum) EditorGUI.EnumPopup( new Rect( 10, 53, 130, 20 ), "Pivot:", defaultPivotPoint );
				if ( GUI.Button( new Rect( 80, 72, 60, 30 ), "Skip" ) ) {
					SkipRect();
				}
				GUI.BeginGroup( new Rect( 10, 107 + PromptOffset4, 130, 30 ) ); //stop SpriteSet Dialog
				if ( DialogStop ) {
					if ( GUI.Button( new Rect( 0, 0, 60, 30 ), "Stop" ) ) {
						PromptOffset4Dest = 0;
						EndAutoSet();
						AutoRectList.Clear();
					}
					if ( GUI.Button( new Rect( 70, 0, 60, 30 ), "Cancel" ) ) {
						PromptOffset4Dest = 0;
					}
				}
				GUI.EndGroup();
				if ( GUI.Button( new Rect( 10, 107, 130, 30 ), "Stop Sprite Set" ) ) {
					DialogStop = true;
					PromptOffset4Dest = 30;
				}
				if ( SpriteIndexRepeatList.Count > 1 && AutoSetIndex == 0 ) {
					if ( GUI.Button( new Rect( 30, 149 + PromptOffset4, 90, 20 ), "Repeat Last" ) ) {
						RepeatSetting = true;
						for ( int i = 0; i < SpriteIndexRepeatList.Count; i++ ) {
							if ( SpriteIndexRepeatList[i].x == -1 ) {
								AutoRectList.RemoveAt( AutoSetIndex );
							} else {
								SpriteIndex = (int) SpriteIndexRepeatList[i].x;
								RectFontIndex = (int) SpriteIndexRepeatList[i].y;
								defaultPivotPoint = (defaultPivotPointEnum) ( SpriteIndexRepeatList[i].z );
								AddRect( true, RectFontIndex );
							}
						}
						RepeatSetting = false;
						EndAutoSet();
					}
				}
				GUI.FocusControl( "SpriteIndInput" );
				GUI.EndGroup();
			}
		} //End of font setter



		//Below here is drawing RectPacker GUI
		if ( UsingPacker ) {
			GUI.Box( new Rect( position.width - MenuBar.width, 0, MenuBar.width, MenuBar.height ), "" );/////RightBox
			GUI.BeginGroup( new Rect( 4, 4, position.width - 4, MenuBar.height - 8 ) );//Clip The Menu pics
			GUI.DrawTexture( new Rect( position.width - 8 - MenuPics[2].width, 0, MenuPics[2].width, MenuPics[2].height ), MenuPics[2] );
			GUI.EndGroup();
			GUI.BeginGroup( new Rect( position.width - MenuBar.width, position.height - 80 - PromptOffset1, MenuBar.width, 50 ) ); //BGCol Group
			GUI.Label( new Rect( 10, 0, 130, 40 ), "Background\ncolor:" );
			BGCol = EditorGUI.ColorField( new Rect( 90, 8, 50, 20 ), BGCol );
			GUI.EndGroup();

			GUI.BeginGroup( new Rect( position.width - MenuBar.width + 10, position.height - 70, MenuBar.width, 150 ) ); //Back to menu Group
			GUI.BeginGroup( new Rect( 0, 30 - PromptOffset1, 130, 200 ) ); //Back to menu Group
			if ( MenuPrompt ) {
				if ( GUI.Button( new Rect( 0, 0, 60, 30 ), "Return" ) ) {
					UsingPacker = false;
					UsingMenu = true;
					ResetError();
					MenuPrompt = false;
					PromptOffset1Dest = 0;
					position = new Rect( position.x, position.y - 5, 512, 512 );
				}
				if ( GUI.Button( new Rect( 70, 0, 60, 30 ), "Cancel" ) ) {
					PromptOffset1Dest = 0;
				}
			}
			GUI.EndGroup();
			if ( GUI.Button( new Rect( 0, 31, 130, 30 ), "Return to Menu" ) ) {
				PromptOffset1Dest = 30;
				MenuPrompt = true;
			}
			GUI.EndGroup();

			GUI.BeginGroup( new Rect( 0, 0, position.width - 150, position.height ) );//Clip The Background Box
			GUI.color = BGCol;
			if ( outTex ) {
				GUI.Box( new Rect( 4 + drawOffsetPos.x, 4 + drawOffsetPos.y, outTex.width + 12, outTex.height + 12 ), "", "theTexBG" );
			} else {
				GUI.Box( new Rect( 4, 4, 256, 256 ), "", "theTexBG" );
			}
			GUI.color = Color.white;
			GUI.EndGroup();

			if ( GUI.Button( new Rect( position.width - 140, 10, 130, 30 ), "Pack!" ) ) {
				ClearLog();
				BeginPack();
			}

			GUI.BeginGroup( new Rect( position.width - 140, 45, 130, 500 ) );
			GUI.Label( new Rect( 0, 0, 130, 20 ), "Sort Rects:" );
			SortMode = GUI.SelectionGrid( new Rect( 0, 20, 130, 75 ), SortMode, Sorts, 2 );
			GUI.Label( new Rect( 0, 110, 130, 22 ), "Packing Method:" );
			PackMode = GUI.SelectionGrid( new Rect( 0, 130, 130, 75 ), PackMode, Packs, 2 );
			AllowNPOT = GUI.Toggle( new Rect( 0, 220, 130, 20 ), AllowNPOT, "Allow NPOT result" );
			AllowRotationTest = GUI.Toggle( new Rect( 0, 240, 130, 20 ), AllowRotationTest, "Allow Rotation test" ); ;
			AnchorSort = GUI.Toggle( new Rect( 0, 260, 130, 20 ), AnchorSort, "Sort Anchor Mode" ); ;
			FitToPOT = GUI.Toggle( new Rect( 0, 280, 130, 20 ), FitToPOT, "Fit to POT Texture" );
			EditorGUIUtility.LookLikeControls( 80, 25 );
			PackBuffer = Mathf.Max( EditorGUI.IntField( new Rect( 0, 305, 130, 20 ), "Rect Buffer:", PackBuffer, "TextField" ), 0 );
			GUI.EndGroup();

			if ( outTex ) {
				GUI.BeginGroup( new Rect( 0, 0, position.width - 150, position.height ) );
				GUI.DrawTexture( new Rect( 10 + drawOffsetPos.x, 10 + drawOffsetPos.y, outTex.width, outTex.height ), outTex );
				GUI.EndGroup();
			}
		}


		if ( ErrorMsg[(int) Errors.NoMaterial] ) {
			GUI.BeginGroup( new Rect( position.width / 2 - 75, position.height / 2 - 30, 150, 150 ) ); //Error NoMaterial Group
			GUI.Box( new Rect( 0, 0, 150, 90 ), "" );
			GUI.Label( new Rect( 8, 4, 135, 150 ), "One of your Fonts has no material attatched. Add a material that uses the \"Textured Text Shader\" shader." );
			GUI.EndGroup();
		}

		if ( ErrorMsg[(int) Errors.NoTexture] ) {
			GUI.BeginGroup( new Rect( position.width / 2 - 75, position.height / 2 - 30, 150, 150 ) ); //Error NoMaterial Group
			GUI.Box( new Rect( 0, 0, 150, 60 ), "" );
			GUI.Label( new Rect( 8, 4, 135, 150 ), "One of your Fonts has a material with no texture map assigned." );
			GUI.EndGroup();
		}

		if ( ErrorMsg[(int) Errors.NoSpriteMaterial] ) {
			GUI.BeginGroup( new Rect( position.width / 2 - 65, position.height / 2 - 30, 130, 150 ) ); //Error No Sprite Material Group
			GUI.Box( new Rect( 0, 0, 130, 60 ), "" );
			GUI.Label( new Rect( 8, 4, 125, 150 ), "Your sprite has no material and/or texture assigned." );
			GUI.EndGroup();
		}

		if ( ErrorMsg[(int) Errors.NoTextMesh] ) {
			GUI.BeginGroup( new Rect( position.width / 2 - 71, position.height / 2 - 30, 142, 150 ) ); //Error No TextMesh/Renderer Group
			GUI.Box( new Rect( 0, 0, 142, 60 ), "" );
			GUI.Label( new Rect( 8, 4, 137, 150 ), "Your sprite has no TextMesh and/or no Renderer components." );
			GUI.EndGroup();
		}

		if ( ErrorMsg[(int) Errors.NPOT] ) {
			GUI.BeginGroup( new Rect( position.width / 2 - 100, position.height / 2 - 30, 200, 60 ) ); //Error NPOT Group
			GUI.Box( new Rect( 0, 0, 200, 60 ), "" );
			GUI.Label( new Rect( 6, 4, 190, 150 ), "Your Font Map is NPOT, Change the Import Settings property for NPOT maps to 'None'." );
			GUI.EndGroup();
		}

		if ( ErrorMsg[(int) Errors.ReadWrite] ) {
			GUI.BeginGroup( new Rect( position.width / 2 - 100, position.height / 2 - 30, 200, 60 ) ); //Error ReadWrite Group
			GUI.Box( new Rect( 0, 0, 200, 60 ), "" );
			GUI.Label( new Rect( 10, 4, 190, 60 ), "Please set Read/Write Enabled in the texture settings for the Font Map before running." );
			GUI.EndGroup();
		}

		if ( ErrorMsg[(int) Errors.Format] ) {
			GUI.BeginGroup( new Rect( position.width / 2 - 100, position.height / 2 - 40, 200, 80 ) ); //Error Format Group
			GUI.Box( new Rect( 0, 0, 200, 80 ), "" );
			GUI.Label( new Rect( 10, 4, 185, 80 ), "Please set the Font Map to a compatible format in its Texture Settings (eg. Compressed, Truecolor, RGBA32)." );
			GUI.EndGroup();
		}

		if ( ErrorMsg[(int) Errors.Size] ) {
			GUI.BeginGroup( new Rect( position.width / 2 - 75, position.height / 2 - 30, 200, 130 ) ); //Error Size Group
			GUI.Box( new Rect( 0, 0, 150, 80 ), "" );
			GUI.Label( new Rect( 8, 4, 135, 130 ), "Your texture Map is larger than the import size, change this in the Import Settings" );
			GUI.EndGroup();
		}

		if ( ErrorMsg[(int) Errors.SameFont] ) {
			GUI.BeginGroup( new Rect( position.width / 2 - 90, position.height / 2 - 30, 180, 130 ) ); //Error SameFont Group
			GUI.Box( new Rect( 0, 0, 180, 60 ), "" );
			GUI.Label( new Rect( 8, 4, 170, 60 ), "You have assigned a sprite that has the same font (animation) more than once." );
			GUI.EndGroup();
		}

		if ( ErrorMsg[(int) Errors.NoFonts] ) {
			GUI.BeginGroup( new Rect( position.width / 2 - 75, position.height / 2 - 30, 150, 130 ) ); //Error NoFonts Group
			GUI.Box( new Rect( 0, 0, 150, 60 ), "" );
			GUI.Label( new Rect( 8, 4, 130, 60 ), "You have assigned a sprite that contains no fonts (animations)." );
			GUI.EndGroup();
		}

		if ( ErrorMsg[(int) Errors.MissedFont] ) {
			GUI.BeginGroup( new Rect( position.width / 2 - 75, position.height / 2 - 30, 150, 130 ) ); //Error MissedFont Group
			GUI.Box( new Rect( 0, 0, 150, 60 ), "" );
			GUI.Label( new Rect( 8, 4, 130, 60 ), "You have assigned a sprite with a missing font (animation)." );
			GUI.EndGroup();
		}

		if ( ErrorMsg[(int) Errors.ShareFont] ) {
			GUI.BeginGroup( new Rect( position.width / 2 - 70, position.height / 2 - 30, 140, 130 ) ); //Error ShareFont Group
			GUI.Box( new Rect( 0, 0, 140, 60 ), "" );
			GUI.Label( new Rect( 8, 4, 130, 60 ), "The in-sprite and out-sprite share font (animation) objects." );
			GUI.EndGroup();
		}

		if ( ErrorMsg[(int) Errors.UnevenLength] ) {
			GUI.BeginGroup( new Rect( position.width / 2 - 85, position.height / 2 - 30, 170, 130 ) ); //Error UnevenLength Group
			GUI.Box( new Rect( 0, 0, 170, 60 ), "" );
			GUI.Label( new Rect( 8, 4, 160, 60 ), "The in-sprite and out-sprite have a different number of fonts (animations)." );
			GUI.EndGroup();
		}

		this.Repaint();
	}





	//Leave the main menu and go into pack or set
	void Begin ( int Mode ) {
		ResetError();
		//To ignore any errors, comment out the return; and x=true lines in each error check
		if ( Mode == 0 || Mode == 1 ) {//Checks for fontsetter || sprite setter
			if ( Mode == 1 ) { //Checks things related to sprites
				if ( !CheckFonts( FontContainer ) ) {
					return;
				}
				theTex = FontContainer.spriteMaterial.mainTexture as Texture2D;
			}

			if ( Mode == 0 && !theFont.material ) { //Checks if the font has material assigned
				ErrorMsg[(int) Errors.NoMaterial] = true;
				return;
			}

			if ( Mode == 0 ) { //Checks if the font material has a texture
				if ( !theFont.material.mainTexture ) {
					ErrorMsg[(int) Errors.NoTexture] = true;
					return;
				} else {
					theTex = theFont.material.mainTexture as Texture2D;
				}
			}

			if ( !CheckNPOT( theTex ) ) { //Checks the tex for NPOT size, and if it is, checks if 'none' is set.
				ErrorMsg[(int) Errors.NPOT] = true;
				return;
			}

			if ( Mode == 0 && CheckImportSize( theTex ) ) { //Checks if the importer max size can encompass the actual image
				ErrorMsg[(int) Errors.Size] = true;
				return;
			}

			SO = new SerializedObject( theTex );
			p = SO.FindProperty( "m_IsReadable" );
			if ( !p.boolValue ) { //Checks Read/Write
				ErrorMsg[(int) Errors.ReadWrite] = true;
				return;
			}

			if ( theTex.format != TextureFormat.ARGB32 && theTex.format != TextureFormat.RGBA32 &&
			theTex.format != TextureFormat.BGRA32 && theTex.format != TextureFormat.RGB24 &&
			theTex.format != TextureFormat.Alpha8 &&
			theTex.format != TextureFormat.DXT1 && theTex.format != TextureFormat.DXT5 ) { //Checks the tex format (best to use compressed or truecolor
				ErrorMsg[(int) Errors.Format] = true;
				return;
			}
		}

		if ( Mode == 2 || Mode == 3 ) {//checks for packer
			if ( Mode == 3 ) { //Sprite appropriateness checks
				if ( !CheckFonts( inFontContainer ) ) { return; }
				if ( !CheckFonts( outFontContainer ) ) { return; }
				if ( !CheckInOutContainers() ) { return; }
				inTex = inFontContainer.spriteMaterial.mainTexture as Texture2D;
			}

			if ( Mode == 2 && ( !inFont.material || !outFont.material ) ) { //Checks if the font has material assigned
				ErrorMsg[(int) Errors.NoMaterial] = true;
				return;
			}

			if ( Mode == 2 ) { //Checks if the material has a texture assigned
				if ( !inFont.material.mainTexture ) {
					ErrorMsg[(int) Errors.NoTexture] = true;
					return;
				} else {
					inTex = inFont.material.mainTexture as Texture2D;
				}
			}

			if ( !CheckNPOT( inTex ) ) { //Checks the tex for NPOT size, and if it is, checks if 'none' is set.
				ErrorMsg[(int) Errors.NPOT] = true;
				return;
			}

			if ( CheckImportSize( inTex ) ) { //Checks if the importer max size can encompass the actual image
				ErrorMsg[(int) Errors.Size] = true;
				return;
			}

			SO = new SerializedObject( inTex );
			p = SO.FindProperty( "m_IsReadable" );
			if ( !p.boolValue ) { //Checks Read/Write
				ErrorMsg[(int) Errors.ReadWrite] = true;
				return;
			}
			if ( inTex.format != TextureFormat.ARGB32 && inTex.format != TextureFormat.RGBA32 &&
			inTex.format != TextureFormat.BGRA32 && inTex.format != TextureFormat.RGB24 &&
			inTex.format != TextureFormat.Alpha8 &&
			inTex.format != TextureFormat.DXT1 && inTex.format != TextureFormat.DXT5 ) { //Checks the tex format (best to use compressed or truecolor
				ErrorMsg[(int) Errors.Format] = true;
				return;
			}
		}

		PromptOffset1 = 0;
		PromptOffset1Dest = 0;

		if ( Mode == 0 || Mode == 1 ) { // Load and initialise Setter
			position = new Rect( position.x, position.y - 5, Mathf.Max( theTex.width + 300 + 12, 587 ), Mathf.Max( theTex.height + 12, 512 + 12 ) );
			Shape = new List<Vector2>();
			ShapeSmartLine = new List<Vector2>();
			Rects = new List<SetRect>();
			AutoRectList = new List<SetRect>();
			UnsortAutoRectList = new List<SetRect>();
			FontList = new Font[( Mode == 1 ? FontContainer.animations.Length : 1 )];
			if ( Mode == 1 ) {
				FontListNames = new String[FontList.Length];
				for ( int i = 0; i < FontList.Length; i++ ) {
					FontList[i] = FontContainer.animations[i];
					FontListNames[i] = FontList[i].name;
					if ( FontList[i].characterInfo.Length < 1 ) {
						ResetFont( i );
					}
					FontList[i].material = FontContainer.spriteMaterial;
				}
				SpriteFrame = 0;
				SpritePropertiesFontIndex = 0;
				SpritePropertiesFontIndexOld = 1;
				ChrL = -1;
			} else {
				ChrL = 47;
				FontList[0] = theFont;
			}
			if ( Mode == 0 && UsePreview ) {
				LocatePreview( FontList[0] );
			}
			if ( Mode == 1 && UsePreview ) {
				LocatePreviewSprite( FontContainer.gameObject );
			}
			UsingEditor = true;
			UsingMenu = false;
			ClickedOnRect = false;
			ClickedRectInd = -1;
			CharCount = 0;
			Orient = 0;
			OrientOld = 0;
			drawOffsetPos = Vector2.zero;
			drawOffsetPosDest = Vector2.zero;
			HarvestRects( 0, false );
		} else { // Load and initiate Packer
			inFontList = new Font[( Mode == 3 ? inFontContainer.animations.Length : 1 )];
			outFontList = new Font[( Mode == 3 ? outFontContainer.animations.Length : 1 )];
			if ( Mode == 3 ) {
				for ( int i = 0; i < inFontList.Length; i++ ) {
					inFontList[i] = inFontContainer.animations[i];
					outFontList[i] = outFontContainer.animations[i];
				}
			} else {
				inFontList[0] = inFont;
				outFontList[0] = outFont;
			}
			if ( Mode == 2 && UsePreview ) {
				LocatePreview( outFontList[0] );
			}
			UsingPacker = true;
			UsingMenu = false;
			position = new Rect( position.x, position.y - 5, 512 + 180, position.height );
		}

	}



	//Updates Rects information in the fontsettings object
	void UpdateFont ( int RectIndex, int CIIndex, int fontInd, bool GotoGui ) {
		SO = new SerializedObject( FontList[fontInd] );
		p = SO.FindProperty( "m_CharacterRects.Array" );
		p2 = p.GetArrayElementAtIndex( CIIndex );
		p2.Next( true ); //p2 is now CharIndex
		if ( GotoGui ) { p2.intValue = ChrL; }
		p2.Next( false ); //p2 is now UV Rect
		Rect newRect = new Rect( Rects[RectIndex].rect.x / theTex.width, Rects[RectIndex].rect.y / theTex.height, Rects[RectIndex].rect.width / theTex.width, Rects[RectIndex].rect.height / theTex.height );
		if ( Rects[RectIndex].Orient ) {
			newRect.y = newRect.y + newRect.height;
			newRect.height = newRect.height * -1;
		}
		p2.rectValue = newRect;
		p2.Next( false ); //p2 is now Vert Rect
		if ( SpriteEditor ) {
			newRect = new Rect( -Rects[RectIndex].spritePivot.x, Rects[RectIndex].rect.height - Rects[RectIndex].spritePivot.y, Rects[RectIndex].rect.width, -Rects[RectIndex].rect.height );
		} else {
			newRect = new Rect( Rects[RectIndex].bWidth, Rects[RectIndex].vOffset, Rects[RectIndex].rect.width, -Rects[RectIndex].rect.height );
		}

		if ( Rects[RectIndex].Orient ) {
			if ( SpriteEditor ) {
				newRect.x = 1 - newRect.y;
				newRect.y = newRect.width - Rects[RectIndex].spritePivot.x;
			}
			newRect.width = Rects[RectIndex].rect.height;
			newRect.height = Rects[RectIndex].rect.width * -1;
		}
		p2.rectValue = newRect;
		p2.Next( false ); //p2 is now Width
		p2.floatValue = ( newRect.width + Rects[RectIndex].aWidth + Rects[RectIndex].bWidth );
		p2.Next( false ); //p2 is now Flip bool
		p2.boolValue = Rects[RectIndex].Orient;
		SO.ApplyModifiedProperties();
		if ( GotoGui ) GetFontInfoToGUI( CIIndex, fontInd );
	}


	//Gets Rect information from the fontsettings object for the currently selected rect
	void GetFontInfoToGUI ( int CInd, int f ) {
		SO = new SerializedObject( FontList[f] );
		p = SO.FindProperty( "m_CharacterRects.Array" );
		p2 = p.GetArrayElementAtIndex( CInd );
		p2.Next( true ); //p2 is now CharIndex
		char c = (char) p2.intValue;
		if ( c < 10 ) {
			ChrL = "a"[0];
			ChrStr = "a";
		} else {
			ChrL = c;
			ChrStr = "" + c;
		}
		if ( SpriteEditor ) {
			SpriteIndex = p2.intValue - 33;
			SpriteIndexOld = SpriteIndex;
		}
		p2.Next( false ); //p2 is now UV Rect
		UVx = (int) ( p2.rectValue.x * theTex.width );
		UVy = (int) ( p2.rectValue.y * theTex.height );
		UVw = (int) ( p2.rectValue.width * theTex.width );
		UVh = (int) ( p2.rectValue.height * theTex.height );
		UVRect = new Rect( UVx, UVy, UVw, UVh );
		p2.Next( false ); //p2 is now Vert Rect
		Rect vertRect = p2.rectValue;
		p2.Next( false ); //p2 is now Width
		WidBefore = vertRect.x;
		WidBeforeOld = WidBefore;
		WidAfter = p2.floatValue - vertRect.width - vertRect.x;
		if ( WidAfter >= -0.01 && WidAfter <= 0.01 ) {
			WidAfter = 0;
		}
		WidAfterOld = WidAfter;
		p2.Next( false ); //p2 is now Flip bool
		if ( p2.boolValue ) {
			Orient = 1;
			OrientOld = 1;
			Cx = -vertRect.height - vertRect.y;
			Cy = vertRect.width + vertRect.x - 1;
		} else {
			Orient = 0;
			OrientOld = 0;
			Cx = -vertRect.x;
			Cy = -vertRect.height - vertRect.y;
		}
		CPos = new Vector2( Cx, Cy );
		if ( SpriteEditor ) {
			RectFontIndex = f;
			RectFontIndexOld = f;
		}
	}


	//Gets Rect information from the fontsettings object for all rects, and stores in lists
	void HarvestRects ( int Cycle, bool Undoing ) {
		SO = new SerializedObject( FontList[Cycle] );
		p = SO.FindProperty( "m_CharacterRects.Array.size" );
		if ( p.intValue < 1 || ( SpriteEditor && p.intValue < 2 ) ) {
			return;
		}
		if ( Cycle == 0 ) {
			Rects.Clear();
			CharCount = 0;
		}
		int RectCount = p.intValue;
		p = SO.FindProperty( "m_CharacterRects.Array" );
		for ( int i = 0; i < RectCount; i++ ) {
			if ( SpriteEditor && i == 0 ) { continue; }
			SetRect newSetRect = new SetRect();
			p2 = p.GetArrayElementAtIndex( i );
			p2.Next( true ); //p2 is now CharIndex
			p2.Next( false ); //p2 is now UV Rect
			Rect newRect = new Rect( 0, 0, 0, 0 );
			newRect.x = p2.rectValue.x * theTex.width;
			newRect.y = p2.rectValue.y * theTex.height;
			newRect.width = p2.rectValue.width * theTex.width;
			newRect.height = p2.rectValue.height * theTex.height;
			if ( newRect.height < 0 ) {
				newRect.height *= -1;
				newRect.y -= newRect.height;
			}
			newSetRect.rect = newRect;
			p2.Next( false ); //p2 is now Vert Rect
			newRect = p2.rectValue;
			newSetRect.vOffset = p2.rectValue.y;
			p2.Next( false ); //p2 is now Width
			newSetRect.aWidth = p2.floatValue - newRect.width - newRect.x;
			newSetRect.bWidth = newRect.x;
			p2.Next( false ); //p2 is now Flip bool
			newSetRect.Orient = p2.boolValue;
			if ( p2.boolValue ) {
				newSetRect.spritePivot = new Vector2( -newRect.height - newRect.y, newRect.width + newRect.x - 1 );
			} else {
				newSetRect.spritePivot = new Vector2( -newRect.x, -newRect.height - newRect.y );
			}
			newSetRect.CIIndex = i;
			newSetRect.fontIndex = Cycle;
			Rects.Add( newSetRect );
		}
		CharCount += SpriteEditor ? RectCount - 1 : RectCount;
		ClickedOnRect = true;
		if ( !Undoing ) {
			ClickedRectInd = 0;
		} else {
			if ( ClickedRectInd > Rects.Count - 1 ) {
				ClickedRectInd = Rects.Count - 1;
			}
		}
		if ( Cycle != FontList.Length - 1 ) {
			Cycle++;
			HarvestRects( Cycle, Undoing );
			return;
		}
		if ( SpriteEditor ) {
			CheckSprites();
		} else {
			CheckDupeChars();
		}
		CheckOverlapRects();
		GetFontInfoToGUI( Rects[ClickedRectInd].CIIndex, ( SpriteEditor ? Rects[ClickedRectInd].fontIndex : 0 ) );
		ClearLog();
	}


	//removes rect from the font settings and lists
	void RemoveRect () {
		ReFocusBlank = true;
		CharCount -= 1;
		SO = new SerializedObject( FontList[Rects[ClickedRectInd].fontIndex] );
		p = SO.FindProperty( "m_CharacterRects" );
		p.Next( true );
		p.DeleteArrayElementAtIndex( Rects[ClickedRectInd].CIIndex );
		SO.ApplyModifiedProperties(); //remove it from the actual font
		for ( int i = 0; i < Rects.Count; i++ ) { //Shift down all the CIindex for all rects in this font
			if ( Rects[i].fontIndex == Rects[ClickedRectInd].fontIndex && Rects[i].CIIndex > Rects[ClickedRectInd].CIIndex ) {
				Rects[i].CIIndex--;
			}
		}
		Rects.RemoveAt( ClickedRectInd ); //delete from the rect list
		if ( SpriteEditor ) { //check if removing has changed dupes
			CheckSprites();
		} else {
			CheckDupeChars();
		}
		ClickedRectInd--;
		ChrL--;
		if ( Rects.Count > 0 && ClickedRectInd == -1 ) {
			ClickedRectInd = 0;
		}
		if ( Rects.Count > 0 ) {
			GetFontInfoToGUI( Rects[ClickedRectInd].CIIndex, Rects[ClickedRectInd].fontIndex );
		} else {
			SpriteIndex = 0;
			SpriteIndexOld = 0;
		}
	}


	//transforms any gui draws to screen space, depending on window size, 'camera' pan and scale
	Rect ConvertPixelsToScreen ( Rect r ) {
		return new Rect( drawOffsetPos.x - ShrunkX - ( theTex.width / 2 * ( drawOffsetScale - 1 ) ) + ( r.x * drawOffsetScale ), drawOffsetPos.y - ShrunkY + ( theTex.height / 2 * ( drawOffsetScale + 1 ) ) - ( r.y + r.height ) * drawOffsetScale, r.width * drawOffsetScale, r.height * drawOffsetScale );
	}


	//Snaps 2 rects togther by their corners, for moving rects
	Rect SnapToPos ( Rect r, int ind ) {
		Vector2[] rCorner = new Vector2[] { new Vector2( r.x, r.y ), new Vector2( r.xMax, r.y ), new Vector2( r.x, r.yMax ), new Vector2( r.xMax, r.yMax ) };
		for ( int j = 0; j < Rects.Count; j++ ) {
			if ( j == ind ) { continue; }
			Vector2[] RCorner = new Vector2[] { new Vector2( Rects[j].rect.x, Rects[j].rect.y ), new Vector2( Rects[j].rect.xMax, Rects[j].rect.y ), new Vector2( Rects[j].rect.x, Rects[j].rect.yMax ), new Vector2( Rects[j].rect.xMax, Rects[j].rect.yMax ) };
			foreach ( Vector2 C in RCorner ) {
				for ( int i = 0; i < 4; i++ ) {
					if ( Vector2.Distance( rCorner[i], C ) < 10.0 / drawOffsetScale ) {
						Rect result = new Rect();
						switch ( i ) {
							case 0:
								result = new Rect( C.x, C.y, r.width, r.height );
								break;
							case 1:
								result = new Rect( C.x - r.width, C.y, r.width, r.height );
								break;
							case 2:
								result = new Rect( C.x, C.y - r.height, r.width, r.height );
								break;
							case 3:
								result = new Rect( C.x - r.width, C.y - r.height, r.width, r.height );
								break;
						}
						return result;
					}
				}
			}
		}
		return r;
	}


	//Snaps 2 rects togther by their corners, for resizing rects
	Rect SnapToSize ( Rect r, int ind ) {
		Vector2[] rCorner = new Vector2[] { new Vector2( r.x, r.y ), new Vector2( r.xMax, r.y ), new Vector2( r.xMax, r.yMax ) };
		for ( int j = 0; j < Rects.Count; j++ ) {
			if ( j == ind ) continue;
			Vector2[] RCorner = new Vector2[] { new Vector2( Rects[j].rect.x, Rects[j].rect.y ), new Vector2( Rects[j].rect.xMax, Rects[j].rect.y ), new Vector2( Rects[j].rect.x, Rects[j].rect.yMax ), new Vector2( Rects[j].rect.xMax, Rects[j].rect.yMax ) };
			foreach ( Vector2 C in RCorner ) {
				for ( int i = 0; i < 3; i++ ) {
					if ( Vector2.Distance( rCorner[i], C ) < 10.0 / drawOffsetScale ) {
						SnapFound = true;
						SnapRect = new Rect( Event.current.mousePosition.x - 7, Event.current.mousePosition.y - 7, 14, 14 );
						Rect result = new Rect();
						switch ( i ) {
							case 0:
								result = new Rect( C.x, C.y, r.width, Mathf.Abs( ( r.y + r.height ) - C.y ) );
								break;
							case 1:
								result = new Rect( r.x, C.y, Mathf.Abs( r.x - C.x ), Mathf.Abs( ( r.y + r.height ) - C.y ) );
								break;
							case 2:
								result = new Rect( r.x, r.y, Mathf.Abs( r.x - C.x ), r.height );
								break;
						}
						result.width = Mathf.Max( result.width, 2 );
						result.height = Mathf.Max( result.height, 2 );
						return result;
					}
				}
			}
		}
		return r;
	}


	//Auto set, looks for pixels which match the ignore condition, then floodfills to find the whole shape
	void AutoSet () {
		MakeTexAsInts();
		for ( int j = theTex.height - 1; j > -1; j-- ) {
			for ( int i = 0; i < theTex.width; i++ ) {
				if ( TexAsInts[i, j] != 0 && TexAsInts[i, j] != 2 ) {
					Shape.Clear();
					if ( SmartMode || SpriteEditor ) {
						ShapeSmartLine.Clear();
					}
					FloodFill( i, j );
					int xMin = theTex.width, xMax = 0, yMin = theTex.height, yMax = 0;
					foreach ( Vector2 a in Shape ) {
						if ( a.x > xMax ) xMax = (int) a.x;
						if ( a.x < xMin ) xMin = (int) a.x;
						if ( a.y > yMax ) yMax = (int) a.y;
						if ( a.y < yMin ) yMin = (int) a.y;
					}
					Rect theRect = new Rect( xMin - FloodfillPadding, yMin - FloodfillPadding, xMax - xMin + 1 + FloodfillPadding * 2, yMax - yMin + 1 + FloodfillPadding * 2 );
					SetRect newSetRect = new SetRect();
					newSetRect.rect = theRect;
					if ( SmartMode ) {
						if ( ShapeSmartLine.Count > 1 ) {
							if ( ShapeSmartLine[0].y == ShapeSmartLine[1].y ) {
								//Compare the first smartline pixel with the second, if they have the same y,
								//then the character is upright, otherwise rotated
								newSetRect.Orient = false;
								newSetRect.vOffset = theRect.yMax - ShapeSmartLine[0].y;
							} else {
								newSetRect.Orient = true;
								newSetRect.vOffset = theRect.xMax - ShapeSmartLine[0].x;
							}
						}
					}
					if ( SpriteEditor ) {
						if ( ShapeSmartLine.Count > 0 ) {
							newSetRect.spritePivot = new Vector2( ShapeSmartLine[0].x - theRect.x, ShapeSmartLine[0].y - theRect.y );
						}
					}
					UnsortAutoRectList.Add( newSetRect );
				}
			}
		}

		AutoRectList.Add( UnsortAutoRectList[0] );
		while ( UnsortAutoRectList.Count > 0 ) {
			AutoRectList.Add( UnsortAutoRectList[FindLeftMost()] );
			UnsortAutoRectList.Remove( AutoRectList[AutoRectList.Count - 1] );
		}
		AutoRectList.RemoveAt( 0 );

		if ( SpriteEditor ) {
			SpriteSetDialog = true;
		} else {
			AutoSetDialog = true;
		}
		SpriteIndex = 0;
		AutoSetIndex = 0;
		CameraOnRect( AutoRectList[0].rect );
	}


	//Gets the left-most rect, but only if it has a vertical position within the height of the previous rect
	int FindLeftMost () {
		int lowest = theTex.width, backuplowest = theTex.width, lowestInd = -1, backupInd = -1;
		for ( int i = 0; i < UnsortAutoRectList.Count; i++ ) {
			if ( UnsortAutoRectList[i].rect.x < lowest ) {
				if ( UnsortAutoRectList[i].rect.center.y < AutoRectList[AutoRectList.Count - 1].rect.center.y + AutoRectList[AutoRectList.Count - 1].rect.height / 2 &&
						UnsortAutoRectList[i].rect.center.y > AutoRectList[AutoRectList.Count - 1].rect.center.y - AutoRectList[AutoRectList.Count - 1].rect.height / 2 ) {
					lowest = (int) UnsortAutoRectList[i].rect.x;
					lowestInd = i;
				}
			}
			if ( UnsortAutoRectList[i].rect.x < backuplowest ) {
				backuplowest = (int) UnsortAutoRectList[i].rect.x;
				backupInd = i;
			}
		}
		if ( lowestInd != -1 ) {
			return lowestInd;
		} else {
			return backupInd;
		}
	}


	//floodfill to find the whole shape, using a scanline algo
	void FloodFill ( int x, int y ) {
		if ( TexAsInts[x, y] == 0 || TexAsInts[x, y] == 2 ) { return; }
		int y1 = y;
		while ( y1 < theTex.height && ( TexAsInts[x, y1] == 1 || TexAsInts[x, y1] == 3 ) ) {
			if ( SmartMode || SpriteEditor ) {
				if ( TexAsInts[x, y1] == 3 ) {
					ShapeSmartLine.Add( new Vector2( x, y1 ) );
				}
			}
			TexAsInts[x, y1] = 2;
			Shape.Add( new Vector2( x, y1 ) );
			y1++;
		}
		int maxY = y1 - 1;
		y1 = y - 1;
		while ( y1 > -1 && ( TexAsInts[x, y1] == 1 || TexAsInts[x, y1] == 3 ) ) {
			if ( SmartMode || SpriteEditor ) {
				if ( TexAsInts[x, y1] == 3 ) {
					ShapeSmartLine.Add( new Vector2( x, y1 ) );
				}
			}
			TexAsInts[x, y1] = 2;
			Shape.Add( new Vector2( x, y1 ) );
			y1--;
		}
		int minY = y1 + 1;
		for ( int i = minY; i < maxY + 1; i++ ) {
			if ( x > 0 && ( TexAsInts[x - 1, i] == 1 || TexAsInts[x - 1, i] == 3 ) ) {
				FloodFill( x - 1, i );
			}
			if ( x < theTex.width - 1 && ( TexAsInts[x + 1, i] == 1 || TexAsInts[x + 1, i] == 3 ) ) {
				FloodFill( x + 1, i );
			}
		}
	}


	//add rect to the font settings (during autoset mostly)
	void AddRect ( bool AddSprite, int fontInd ) {
		if ( Rects.Count == 0 && !RepeatSetting ) {
			CharRepeatList.Clear();
			SpriteIndexRepeatList.Clear();
		}
		SO = new SerializedObject( FontList[fontInd] );
		p = SO.FindProperty( "m_CharacterRects.Array.size" );
		p.intValue = FontList[fontInd].characterInfo.Length + 1;
		SO.ApplyModifiedProperties();
		if ( AddSprite ) {
			ChrL = SpriteIndex + 33;
		} else {
			ChrL = AutoSetCharStr[0];
		}
		if ( !RepeatSetting && !AddSprite ) CharRepeatList.Add( (char) ChrL );
		if ( !RepeatSetting && AddSprite ) SpriteIndexRepeatList.Add( new Vector3( SpriteIndex, RectFontIndex, (int) defaultPivotPoint ) );
		if ( defaultPivotPoint != 0 ) {
			AutoRectList[AutoSetIndex].spritePivot = new Vector2( Mathf.FloorToInt( AutoRectList[AutoSetIndex].rect.width * defaultPivots[(int) defaultPivotPoint].x ), Mathf.FloorToInt( AutoRectList[AutoSetIndex].rect.height * defaultPivots[(int) defaultPivotPoint].y ) );
		}
		AutoRectList[AutoSetIndex].fontIndex = RectFontIndex;
		AutoRectList[AutoSetIndex].CIIndex = FontList[RectFontIndex].characterInfo.Length - 1;
		Rects.Add( AutoRectList[AutoSetIndex] );
		UpdateFont( AutoSetIndex, AutoRectList[AutoSetIndex].CIIndex, fontInd, true );
		AutoSetCharStr = "";
		ReFocusBlank = true;
		CharCount++;
		if ( AutoSetIndex == AutoRectList.Count - 1 ) {
			EndAutoSet();
			return;
		}
		if ( AddSprite ) {
			SpriteIndex++;
		}
		AutoSetIndex++;
		CameraOnRect( AutoRectList[AutoSetIndex].rect );
	}


	//Skips a rect during autoset
	void SkipRect () {
		if ( Rects.Count == 0 ) {
			if ( SpriteEditor ) {
				SpriteIndexRepeatList.Clear();
			} else {
				CharRepeatList.Clear();
			}
		}
		if ( AutoSetIndex == AutoRectList.Count - 1 ) {
			EndAutoSet();
		} else {
			ReFocusBlank = true;
			AutoRectList.RemoveAt( AutoSetIndex );
			CameraOnRect( AutoRectList[AutoSetIndex].rect );
			if ( SpriteEditor ) {
				SpriteIndexRepeatList.Add( new Vector3( -1, 0, 0 ) );
			} else {
				CharRepeatList.Add( (char) 0 );
			}
		}
	}


	//moves the 'camera' to center on the currently inspect rect r
	void CameraOnRect ( Rect r ) {
		drawOffsetScaleDest = 3;
		drawOffsetPosDest = new Vector2( theTex.width / 2 - r.center.x, r.center.y - theTex.height / 2 ) * drawOffsetScaleDest;
	}


	//clear everything from lists and the font settings
	void ResetFont ( int fInd ) {
		ResetError();
		if ( CharRepeatList == null ) {
			CharRepeatList = new List<char>();
		}
		if ( SpriteIndexRepeatList == null ) {
			SpriteIndexRepeatList = new List<Vector3>();
		}

		ClickedOnRect = false;
		PerCharacterOffset = false;
		Rects.Clear();
		UnsortAutoRectList.Clear();
		AutoRectList.Clear();
		CharacterInfo[] newCI;
		if ( fInd != -1 ) {
			newCI = new CharacterInfo[( SpriteEditor ? 1 : 0 )];
			FontList[fInd].characterInfo = newCI;
		} else {
			for ( int i = 0; i < FontList.Length; i++ ) {
				newCI = new CharacterInfo[( SpriteEditor ? 1 : 0 )];
				FontList[i].characterInfo = newCI;
			}
		}
		if ( SpriteEditor ) {
			SpriteIndex = -1;
			SpriteIndexOld = -1;
			ChrL = -1;
		} else {
			ChrL = 47;
		}
		RectFontIndex = 0;
		CharCount = 0;
		ClickedRectInd = -1;
	}


	//Checks if image is NPOT, then checks if NPOT import is set to none
	bool CheckNPOT ( Texture2D T ) {
		bool result = false;
		if ( T != null ) {
			string assetPath = AssetDatabase.GetAssetPath( T );
			TextureImporter importer = AssetImporter.GetAtPath( assetPath ) as TextureImporter;

			if ( importer != null ) {
				object[] args = new object[2] { 0, 0 };
				MethodInfo M = typeof( TextureImporter ).GetMethod( "GetWidthAndHeight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance );
				M.Invoke( importer, args );
				int w = (int) args[0];
				int h = (int) args[1];
				result = ( ( w & ( w - 1 ) ) == 0 && ( h & ( h - 1 ) ) == 0 );
				if ( !result ) {
					if ( importer.npotScale == TextureImporterNPOTScale.None ) { result = true; }
				}
			}
		}
		return result;
	}


	//Compares image size to importer max size
	bool CheckImportSize ( Texture2D T ) {
		if ( T != null ) {
			string assetPath = AssetDatabase.GetAssetPath( T );
			TextureImporter importer = AssetImporter.GetAtPath( assetPath ) as TextureImporter;

			if ( importer != null ) {
				object[] args = new object[2] { 0, 0 };
				MethodInfo M = typeof( TextureImporter ).GetMethod( "GetWidthAndHeight", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance );
				M.Invoke( importer, args );
				int w = (int) args[0];
				int h = (int) args[1];

				if ( importer.maxTextureSize < w || importer.maxTextureSize < h ) {
					return true;
				} else {
					return false;
				}
			}
		}
		return true;
	}


	//Stops autoset, checks for duplicates and overlaps
	void EndAutoSet () {
		if ( SmartMode ) {
			SmartMode = false;
			PerCharacterOffset = true;
		}

		if ( SpriteEditor ) {
			CheckSprites();
		} else {
			CheckDupeChars();
		}
		CheckOverlapRects();
		AutoSetIndex = 0;
		AutoSetDialog = false;
		SpriteSetDialog = false;
		drawOffsetPosDest = Vector2.zero;
		drawOffsetScaleDest = 1;
		if ( Rects.Count == 0 ) {
			return;
		}
		ClickedRectInd = 0;
		if ( !SpriteEditor ) { ErrorMsg[(int) Errors.Spacebar] = true; }
		GetFontInfoToGUI( 0, 0 );
	}


	//Clears the status of all error messages, disable any prompts and dialogs
	void ResetError () {
		ResetPrompt = false;
		AutoPrompt = false;
		AutoSetDialog = false;
		PerCharacterOffset = false;
		for ( int i = 0; i < ErrorMsg.Length; i++ ) {
			ErrorMsg[i] = false;
		}
	}


	//Start the Rect Packer according to the selected gui buttons
	void BeginPack () {
		TimeA = System.DateTime.Now;

		if ( inTex ) {
			inTexWidth = inTex.width;
			inTexHeight = inTex.height;
		}

		StartPackRectList = new List<PackRect>();
		ResultPackRectList = new List<PackRect>();
		Anchors = new List<Vector2>();
		Partitions = new List<Rect>();

		HarvestStartPackRectList();

		int Total = 0;
		for ( int i = 0; i < StartPackRectList.Count; i++ ) {
			Total += (int) ( StartPackRectList[i].ResultRect.width * StartPackRectList[i].ResultRect.height );
		}
		Total = (int) Mathf.Sqrt( Total );
		Total = NearestPOT( Total );

		DestroyImmediate( outTex );
		outTex = new Texture2D( Total, Total );
		outTex.filterMode = FilterMode.Point;
		for ( int i = 0; i < outTex.width; i++ ) {
			for ( int j = 0; j < outTex.height; j++ ) {
				outTex.SetPixel( i, j, new Color( 0, 0, 0, 0 ) );
			}
		}
		outTex.Apply();

		for ( int i = 0; i < StartPackRectList.Count; i++ ) { //Sort rects according to toolbar choice
			switch ( SortMode ) {
				case 0:
					StartPackRectList[i].Height = StartPackRectList[i].ResultRect.height;
					break;
				case 1:
					StartPackRectList[i].Height = StartPackRectList[i].ResultRect.width;
					StartPackRectList[i].ResultRect = new Rect( StartPackRectList[i].ResultRect.x, StartPackRectList[i].ResultRect.y, StartPackRectList[i].ResultRect.height, StartPackRectList[i].ResultRect.width );
					StartPackRectList[i].SameOrient = !StartPackRectList[i].SameOrient;
					break;
				case 2:
					if ( StartPackRectList[i].ResultRect.height > StartPackRectList[i].ResultRect.width ) {
						StartPackRectList[i].Height = StartPackRectList[i].ResultRect.height;
					} else {
						StartPackRectList[i].Height = StartPackRectList[i].ResultRect.width;
						StartPackRectList[i].ResultRect = new Rect( StartPackRectList[i].ResultRect.x, StartPackRectList[i].ResultRect.y, StartPackRectList[i].ResultRect.height, StartPackRectList[i].ResultRect.width );
						StartPackRectList[i].SameOrient = !StartPackRectList[i].SameOrient;
					}
					break;
				case 3:
					if ( StartPackRectList[i].ResultRect.height > StartPackRectList[i].ResultRect.width ) {
						StartPackRectList[i].Height = StartPackRectList[i].ResultRect.width;
						StartPackRectList[i].ResultRect = new Rect( StartPackRectList[i].ResultRect.x, StartPackRectList[i].ResultRect.y, StartPackRectList[i].ResultRect.height, StartPackRectList[i].ResultRect.width );
						StartPackRectList[i].SameOrient = !StartPackRectList[i].SameOrient;
					} else {
						StartPackRectList[i].Height = StartPackRectList[i].ResultRect.height;
					}
					break;
			}
		}


		for ( int i = StartPackRectList.Count - 1; i > 0; i-- ) {
			CheckAbove( i );
		}



		switch ( PackMode ) { //Pack rects according to toolbar choice
			case 0:
				SimplePack();
				break;
			case 1:
				SwitchbackPack();
				break;
			case 2:
				PartitionPack();
				break;
			case 3:
				AnchorPack();
				break;
		}

		DrawResult( ResultPackRectList );

		TimeSpan TimeB = System.DateTime.Now - TimeA;
		Debug.Log( "Time taken: " + String.Format( "{0:ss ffffff}", TimeB ) );
	}


	//Checks given rect for overlap with all others, during pack, and if it fits into the pack area
	bool RectIsFree ( Rect r1 ) {
		bool NoOverlap = true;
		for ( int i = 0; i < ResultPackRectList.Count; i++ ) {
			Rect r2 = ResultPackRectList[i].ResultRect;
			if ( ( r1.xMin < r2.xMax ) && ( r1.xMax > r2.xMin ) && ( r1.yMin < r2.yMax ) && ( r1.yMax > r2.yMin ) ) {
				NoOverlap = false;
			}
		}

		if ( r1.xMin < 0 || r1.xMax > PackSizeX || r1.yMin < 0 || r1.yMax > PackSizeY ) {
			NoOverlap = false;
		}
		return NoOverlap;
	}


	//Checks given autorect for overlap with all others, after autoset, returns the index of overlap
	int AutoRectIsFree ( Rect r1 ) {
		for ( int i = 0; i < Rects.Count; i++ ) {
			Rect r2 = Rects[i].rect;
			if ( r1 == r2 ) { continue; }
			if ( ( r1.xMin < r2.xMax ) && ( r1.xMax > r2.xMin ) && ( r1.yMin < r2.yMax ) && ( r1.yMax > r2.yMin ) ) {
				return i;
			}
		}
		return -1;
	}


	//Slides pack rect down and left, if possible, during pack
	void Slide ( bool b ) {
		int y1 = (int) GetPackRect.ResultRect.y, safe = 0, step = 1;
		Rect testRect = new Rect( GetPackRect.ResultRect.x, y1, GetPackRect.ResultRect.width, GetPackRect.ResultRect.height );
		if ( RectIsFree( testRect ) ) {
			step = -1;
		}
		y1 += step;

		//safeLimit stops infinate loops occuring in the while loop. If you have a lot of characters, this might not be big enough.
		while ( true && safe < 1000 ) {
			if ( y1 < 0 ) {
				GetPackRect.ResultRect.y = y1 - step;
				return;
			}
			if ( y1 + GetPackRect.ResultRect.height > PackSizeY ) {
				if ( AllowNPOT ) {
					PackSizeY = y1 + (int) GetPackRect.ResultRect.height;
					Slide( b );
					return;
				} else {
					PackSizeY *= 2;
					Slide( b );
					return;
				}
			}
			testRect = new Rect( GetPackRect.ResultRect.x, y1, GetPackRect.ResultRect.width, GetPackRect.ResultRect.height );
			bool NoOverlap = RectIsFree( testRect );
			if ( step == 1 && NoOverlap ) {
				GetPackRect.ResultRect.y = y1;
				return;
			}
			if ( step == -1 && !NoOverlap ) {
				GetPackRect.ResultRect.y = y1 - step;
				int x1 = (int) testRect.x;
				step = -1;
				while ( b && true && safe < 300 ) {
					if ( x1 < 0 ) {
						GetPackRect.ResultRect.x = x1 - step;
						return;
					}
					testRect = new Rect( x1, GetPackRect.ResultRect.y, GetPackRect.ResultRect.width, GetPackRect.ResultRect.height );
					if ( !RectIsFree( testRect ) ) {
						GetPackRect.ResultRect.x = x1 - step;
						return;
					}
					x1 += step;
					safe++;
				}
				return;
			}
			y1 += step;
			safe++;
		}
		Debug.Log( "possible infinate loop, oops! Doublecheck that there is the correct number of chracters, if not, try a different packing method" );
		return;
	}


	//slides rect during an anchor pack, up or down depending on start condition
	Rect AnchorSlide ( Rect r ) {
		Rect testRect = new Rect( r.x, r.y, r.width, r.height );
		int leftMost = (int) r.x;
		//safeLimit stops infinate loops occuring in the while loop. If you have a lot of characters, this might not be big enough.
		int safe = 0;
		while ( RectIsFree( testRect ) && leftMost > 0 && safe < 1000 ) {
			leftMost = (int) testRect.x;
			testRect.x--;
			safe++;
		}
		testRect.x = r.x;
		int topMost = (int) r.y;
		while ( RectIsFree( testRect ) && topMost > 0 && safe < 1000 ) {
			topMost = (int) testRect.y;
			testRect.y--;
			safe++;
		}
		if ( ( r.x - leftMost ) > ( r.y - topMost ) ) {
			r.x = leftMost;
		} else {
			r.y = topMost;
		}
		if ( safe > 999 ) { Debug.Log( "possible infinate loop, oops! Doublecheck that there is the correct number of chracters, if not, try a different packing method" ); }
		return r;
	}


	//during partition pack, adds rect into the free partition
	void AddPartition ( Rect r, int i ) {
		GetPackRect.ResultRect = r;
		ResultPackRectList.Add( GetPackRect );
		if ( GetPackRect.ResultRect.xMax > MaxSizeX ) { MaxSizeX = (int) GetPackRect.ResultRect.xMax; }
		if ( GetPackRect.ResultRect.yMax > MaxSizeY ) { MaxSizeY = (int) GetPackRect.ResultRect.yMax; }
		Partitions.RemoveAt( i );
		if ( r.height < r.width ) {
			Partitions.Add( new Rect( r.xMax, r.yMin, 0, r.height ) );
			Partitions.Add( new Rect( r.xMin, r.yMax, 0, -1 ) );
		} else {
			Partitions.Add( new Rect( r.xMin, r.yMax, r.width, 0 ) );
			Partitions.Add( new Rect( r.xMax, r.yMin, -1, 0 ) );
		}
	}


	//pack by partition, roughly based on lightmap packing
	void PartitionPack () {
		//safeLimit stops infinate loops occuring in the while loop. If you have a lot of characters, this might not be big enough.
		int safeLimit = 1000;
		PackSizeX = 2;
		PackSizeY = 2;
		MaxSizeX = 2;
		MaxSizeY = 2;
		Partitions.Add( new Rect( 0, 0, PackSizeX, PackSizeY ) );
		for ( int i = StartPackRectList.Count; i > 0; i-- ) {
			GetFromHeap();
			int longest = (int) ( GetPackRect.ResultRect.width > GetPackRect.ResultRect.height ? GetPackRect.ResultRect.width : GetPackRect.ResultRect.height );
			//qwer Not needed?
			//int shortest = (int) (GetPackRect.ResultRect.width>GetPackRect.ResultRect.height?GetPackRect.ResultRect.height:GetPackRect.ResultRect.width);
			bool searching = true;
			int safe = 0;
			int j = -1;
			while ( searching && safe < safeLimit ) {
				safe++;
				j++;
				if ( j > Partitions.Count - 1 ) {
					if ( PackSizeX < PackSizeY ) {
						if ( AllowNPOT ) {
							PackSizeX += longest;
						} else {
							PackSizeX *= 2;
						}
					} else {
						if ( AllowNPOT ) {
							PackSizeY += longest;
						} else {
							PackSizeY *= 2;
						}
					}
					j = 0;
				}
				Rect testRect = new Rect( Partitions[j].x, Partitions[j].y, GetPackRect.ResultRect.width, GetPackRect.ResultRect.height );
				if ( RectIsFree( testRect ) ) {
					AddPartition( testRect, j );
					searching = false;
				} else if ( AllowRotationTest && RectIsFree( new Rect( testRect.x, testRect.y, testRect.height, testRect.width ) ) ) {
					GetPackRect.SameOrient = !GetPackRect.SameOrient;
					AddPartition( new Rect( testRect.x, testRect.y, testRect.height, testRect.width ), j );
					searching = false;
				}

			}
			if ( safe > safeLimit - 1 ) Debug.Log( "possible infinate loop, oops! If you have a lot of characters, try increasing the safe limit in PartitionPack ()" );
		}
	}


	//pack by anchors, added from top left and bottom right coreners of packed rects, sorted by sortmode
	void AnchorPack () {
		PackSizeX = 2;
		PackSizeY = 2;
		MaxSizeX = 2;
		MaxSizeY = 2;
		Anchors.Add( Vector2.zero );
		for ( int i = StartPackRectList.Count; i > 0; i-- ) {
			GetFromHeap();
			bool searching = true;
			int j = 0;
			while ( searching ) {
				if ( j > Anchors.Count - 1 ) {
					if ( PackSizeX < PackSizeY ) {
						if ( AllowNPOT ) {
							PackSizeX += (int) GetPackRect.ResultRect.width;
						} else {
							PackSizeX *= 2;
						}
					} else {
						if ( AllowNPOT ) {
							PackSizeY += (int) GetPackRect.ResultRect.height;
						} else {
							PackSizeY *= 2;
						}
					}
					j = 0;
				}
				Rect testRect = new Rect( Anchors[j].x, Anchors[j].y, GetPackRect.ResultRect.width, GetPackRect.ResultRect.height );
				if ( RectIsFree( testRect ) ) {
					testRect = AnchorSlide( testRect );
					GetPackRect.ResultRect = testRect;
					ResultPackRectList.Add( GetPackRect );
					if ( GetPackRect.ResultRect.xMax > MaxSizeX ) { MaxSizeX = (int) GetPackRect.ResultRect.xMax; }
					if ( GetPackRect.ResultRect.yMax > MaxSizeY ) { MaxSizeY = (int) GetPackRect.ResultRect.yMax; }
					Anchors.Add( new Vector2( testRect.xMax, testRect.yMin ) );
					Anchors.Add( new Vector2( testRect.xMin, testRect.yMax ) );
					Anchors.Sort( SortAnchors );
					searching = false;
				} else if ( AllowRotationTest && RectIsFree( new Rect( testRect.x, testRect.y, testRect.height, testRect.width ) ) ) {
					GetPackRect.SameOrient = !GetPackRect.SameOrient;
					testRect = new Rect( testRect.x, testRect.y, testRect.height, testRect.width );
					testRect = AnchorSlide( testRect );
					GetPackRect.ResultRect = testRect;
					ResultPackRectList.Add( GetPackRect );
					if ( GetPackRect.ResultRect.xMax > MaxSizeX ) { MaxSizeX = (int) GetPackRect.ResultRect.xMax; }
					if ( GetPackRect.ResultRect.yMax > MaxSizeY ) { MaxSizeY = (int) GetPackRect.ResultRect.yMax; }
					Anchors.Add( new Vector2( testRect.xMax, testRect.yMin ) );
					Anchors.Add( new Vector2( testRect.xMin, testRect.yMax ) );
					Anchors.Sort( SortAnchors );
					searching = false;
				}
				j++;
			}
		}
	}


	//pack left to right, then right to left
	void SwitchbackPack () {
		PackSizeX = outTex.width;
		PackSizeY = outTex.height;
		MaxSizeX = 2;
		MaxSizeY = 2;
		bool SwitchStep = true;
		for ( int i = StartPackRectList.Count; i > 0; i-- ) {
			GetFromHeap();
			if ( ResultPackRectList.Count == 0 ) {
				GetPackRect.ResultRect.x = 0;
				GetPackRect.ResultRect.y = 0;
				ResultPackRectList.Add( GetPackRect );
			} else {
				if ( SwitchStep ) {
					GetPackRect.ResultRect.x = ResultPackRectList[ResultPackRectList.Count - 1].ResultRect.x + ResultPackRectList[ResultPackRectList.Count - 1].ResultRect.width;
				} else {
					GetPackRect.ResultRect.x = ResultPackRectList[ResultPackRectList.Count - 1].ResultRect.x - GetPackRect.ResultRect.width;
				}
				GetPackRect.ResultRect.y = ResultPackRectList[ResultPackRectList.Count - 1].ResultRect.y;
				if ( GetPackRect.ResultRect.x > 0 && GetPackRect.ResultRect.xMax < outTex.width ) {
					Slide( SwitchStep );
					ResultPackRectList.Add( GetPackRect );
					if ( GetPackRect.ResultRect.xMax > MaxSizeX ) { MaxSizeX = (int) GetPackRect.ResultRect.xMax; }
					if ( GetPackRect.ResultRect.yMax > MaxSizeY ) { MaxSizeY = (int) GetPackRect.ResultRect.yMax; }
				} else {
					if ( SwitchStep ) {
						GetPackRect.ResultRect.x = outTex.width - GetPackRect.ResultRect.width;
					} else {
						GetPackRect.ResultRect.x = 0;
					}
					GetPackRect.ResultRect.y = ResultPackRectList[ResultPackRectList.Count - 1].ResultRect.y + ResultPackRectList[ResultPackRectList.Count - 1].ResultRect.height;
					SwitchStep = !SwitchStep;
					Slide( SwitchStep );
					ResultPackRectList.Add( GetPackRect );
					if ( GetPackRect.ResultRect.xMax > MaxSizeX ) { MaxSizeX = (int) GetPackRect.ResultRect.xMax; }
					if ( GetPackRect.ResultRect.yMax > MaxSizeY ) { MaxSizeY = (int) GetPackRect.ResultRect.yMax; }
				}
			}
		}
	}


	//basic pack from left to right, with a downward slide
	void SimplePack () {
		PackSizeX = outTex.width;
		PackSizeY = outTex.height;
		MaxSizeX = 2;
		MaxSizeY = 2;
		for ( int i = StartPackRectList.Count; i > 0; i-- ) {
			GetFromHeap();
			if ( ResultPackRectList.Count == 0 ) {
				NewLineIndex = 0;
				GetPackRect.ResultRect.x = 0;
				GetPackRect.ResultRect.y = 0;
			} else {
				if ( ResultPackRectList[ResultPackRectList.Count - 1].ResultRect.x + ResultPackRectList[ResultPackRectList.Count - 1].ResultRect.width + GetPackRect.ResultRect.width > outTex.width ) {
					GetPackRect.ResultRect.x = 0;
					GetPackRect.ResultRect.y = ResultPackRectList[NewLineIndex].ResultRect.y + ResultPackRectList[NewLineIndex].ResultRect.height;
					NewLineIndex = ResultPackRectList.Count;
					Slide( true );
				} else {
					GetPackRect.ResultRect.x = ResultPackRectList[ResultPackRectList.Count - 1].ResultRect.x + ResultPackRectList[ResultPackRectList.Count - 1].ResultRect.width;
					GetPackRect.ResultRect.y = ResultPackRectList[ResultPackRectList.Count - 1].ResultRect.y;
					Slide( true );
				}
			}
			ResultPackRectList.Add( GetPackRect );
			if ( GetPackRect.ResultRect.xMax > MaxSizeX ) { MaxSizeX = (int) GetPackRect.ResultRect.xMax; }
			if ( GetPackRect.ResultRect.yMax > MaxSizeY ) { MaxSizeY = (int) GetPackRect.ResultRect.yMax; }
		}
	}


	//get the top rect from the heap
	void GetFromHeap () {
		GetPackRect = StartPackRectList[0];
		StartPackRectList[0] = StartPackRectList[StartPackRectList.Count - 1];
		StartPackRectList.RemoveAt( StartPackRectList.Count - 1 );
		CheckBelow( 0 );
	}


	//bubble up in the heap
	void CheckAbove ( int i ) {
		if ( i == 0 ) { return; }
		int ParentIndex = (int) Mathf.Floor( ( i + 1 ) / 2 ) - 1;
		if ( StartPackRectList[i].Height > StartPackRectList[ParentIndex].Height ) {
			swapPackRect = StartPackRectList[ParentIndex];
			StartPackRectList[ParentIndex] = StartPackRectList[i];
			StartPackRectList[i] = swapPackRect;
			CheckBelow( i );
		}

	}


	//bubble down in the heap
	void CheckBelow ( int i ) {
		int child2Index = ( i + 1 ) * 2;
		int child1Index = child2Index - 1;
		int swapIndex = i;
		if ( child1Index < StartPackRectList.Count ) {
			if ( StartPackRectList[child1Index].Height > StartPackRectList[i].Height ) {
				swapIndex = child1Index;
			}
		}
		if ( child2Index < StartPackRectList.Count ) {
			if ( StartPackRectList[child2Index].Height > StartPackRectList[( swapIndex == i ) ? i : child1Index].Height ) {
				swapIndex = child2Index;
			}
		}
		if ( swapIndex != i ) {
			swapPackRect = StartPackRectList[swapIndex];
			StartPackRectList[swapIndex] = StartPackRectList[i];
			StartPackRectList[i] = swapPackRect;
			CheckBelow( swapIndex );
		}

	}


	//Sort anchors, either by dist from the origin, or dist to the closest pack boundary
	int SortAnchors ( Vector2 a, Vector2 b ) {
		if ( AnchorSort ) {
			int dx = PackSizeX - (int) a.x, dy = PackSizeY - (int) a.y, dx2 = PackSizeX - (int) b.x, dy2 = PackSizeY - (int) b.y;
			return ( dx2 > dy2 ? dx2 : dy2 ).CompareTo( dx > dy ? dx : dy );
		} else {
			return a.sqrMagnitude.CompareTo( b.sqrMagnitude );
		}
	}


	//draws the packed map, translating from startrect to resultrect
	void DrawResult ( List<PackRect> R ) {
		if ( PackBuffer != 0 ) {
			for ( int r = 0; r < R.Count; r++ ) {
				R[r].ResultRect.width = R[r].ResultRect.width - PackBuffer;
				R[r].ResultRect.height = R[r].ResultRect.height - PackBuffer;
			}
		}
		DestroyImmediate( outTex );
		outTex = new Texture2D( AllowNPOT ? ( FitToPOT ? NearestPOT( MaxSizeX ) : PackSizeX ) : PackSizeX, AllowNPOT ? ( FitToPOT ? NearestPOT( MaxSizeY ) : PackSizeY ) : PackSizeY );
		outTex.filterMode = FilterMode.Point;
		for ( int i = 0; i < outTex.width; i++ ) {
			for ( int j = 0; j < outTex.height; j++ ) {
				outTex.SetPixel( i, j, new Color( 0, 0, 0, 0 ) );
			}
		}
		outTex.Apply();
		for ( int r = 0; r < R.Count; r++ ) {
			for ( int j = 0; j < R[r].ResultRect.height; j++ ) {
				for ( int i = 0; i < R[r].ResultRect.width; i++ ) {
					Color Col;
					if ( R[r].SameOrient ) {
						Col = inTex.GetPixel( (int) R[r].StartRect.x + i, (int) R[r].StartRect.y + j );
					} else {
						if ( R[r].CI.flipped ) {
							Col = inTex.GetPixel( (int) ( R[r].StartRect.x + R[r].StartRect.width - ( R[r].StartRect.width - j ) ), (int) ( R[r].StartRect.y + ( R[r].StartRect.height - i ) - 1 ) );
						} else {
							Col = inTex.GetPixel( (int) ( R[r].StartRect.x + R[r].StartRect.width - j - 1 ), (int) R[r].StartRect.y + i );
						}
					}
					//Draw in the acutal texture pixel
					outTex.SetPixel( (int) R[r].ResultRect.x + i, (int) R[r].ResultRect.y + j, Col );
					//To draw a box around each character rect, uncomment the next 3 lines (helps dbug)
					//if(i == 0 || i == R[r].ResultRect.width-1 || j == 0 || j == R[r].ResultRect.height-1){
					//	outTex.SetPixel(R[r].ResultRect.x + i, R[r].ResultRect.y + j,Color.black);
					//}
				}
			}
		}
		outTex.Apply();
		byte[] bytes = outTex.EncodeToPNG();
		string path;
		if ( SpriteEditor ) {
			path = AssetDatabase.GetAssetPath( outFontContainer );
		} else {
			path = AssetDatabase.GetAssetPath( outFontList[0] );
		}
		path = path.Remove( path.LastIndexOf( '/' ) );
		path = path + "/" + ( SpriteEditor ? outFontContainer.name : outFontList[0].name ) + " PackedMap.png";
		File.WriteAllBytes( path, bytes );
		SetOutFont();
		if ( !EditorApplication.isPlaying && TextPreviewT && !SpriteEditor ) {
			TextPreviewT.offsetZ = 1;
			TextPreviewT.offsetZ = 0;
		}
		Debug.Log( "Pack Complete, Result texture size: " + outTex.width + "x" + outTex.height + " = " + outTex.width * outTex.height + " Pixels" );
		Debug.Log( "Pack fits inside " + MaxSizeX + "x" + MaxSizeY + " = " + MaxSizeX * MaxSizeY + " Pixels, and contains " + ResultPackRectList.Count + " characters" );
		drawOffsetPosDest = Vector2.zero;
		position = new Rect( position.x, position.y - 5, outTex.width + 170, Mathf.Max( outTex.height + 20, 532 ) );
	}


	//Draws the sprite map with pivot pixels
	void DrawSpritePivots () {
		DestroyImmediate( outTex );
		outTex = new Texture2D( theTex.width, theTex.height );
		outTex.filterMode = FilterMode.Point;
		for ( int i = 0; i < outTex.width; i++ ) {
			for ( int j = 0; j < outTex.height; j++ ) {
				outTex.SetPixel( i, j, theTex.GetPixel( i, j ) );
			}
		}

		for ( int i = 0; i < Rects.Count; i++ ) {
			outTex.SetPixel( (int) ( Rects[i].rect.x + Rects[i].spritePivot.x ), (int) ( Rects[i].rect.y + Rects[i].spritePivot.y ), SmartColor );
		}
		outTex.Apply();



		byte[] bytes = outTex.EncodeToPNG();
		string path;
		path = AssetDatabase.GetAssetPath( theTex );
		path = path.Remove( path.LastIndexOf( '/' ) );
		path = path + "/" + theTex.name + " Pivots.png";
		File.WriteAllBytes( path, bytes );
		AssetDatabase.Refresh();
	}


	//Get, rotate if needed, and set the characterinfo from infont to outfont
	void SetOutFont () {
		List<CharacterInfo[]> OutCharacterInfoCollection = new List<CharacterInfo[]>();
		for ( int i = 0; i < inFontList.Length; i++ ) {
			CharacterInfo[] OutCharacterInfo = new CharacterInfo[inFontList[i].characterInfo.Length];
			OutCharacterInfoCollection.Add( OutCharacterInfo );
		}
		for ( int i = 0; i < ResultPackRectList.Count; i++ ) {
			Rect newUvRect = new Rect( ResultPackRectList[i].ResultRect.x / outTex.width, ResultPackRectList[i].ResultRect.y / outTex.height, ResultPackRectList[i].ResultRect.width / outTex.width, ResultPackRectList[i].ResultRect.height / outTex.height );
			if ( ResultPackRectList[i].SameOrient ) {
				if ( ResultPackRectList[i].CI.flipped ) {
					newUvRect.y = newUvRect.y + newUvRect.height;
					newUvRect.height = newUvRect.height * -1;
				} else {
					//The rect hasnt been rotated, and wasnt originally flipped anyway.
				}
			} else {
				if ( ResultPackRectList[i].CI.flipped ) {
					ResultPackRectList[i].CI.flipped = false;
				} else {
					newUvRect.y = newUvRect.y + newUvRect.height;
					newUvRect.height = newUvRect.height * -1;
					ResultPackRectList[i].CI.flipped = true;
					ResultPackRectList[i].CI.vert.width = ResultPackRectList[i].ResultRect.height;
					ResultPackRectList[i].CI.vert.height = ResultPackRectList[i].ResultRect.width * -1;
				}
			}

			ResultPackRectList[i].CI.uv = newUvRect;
			OutCharacterInfoCollection[ResultPackRectList[i].fontIndex][ResultPackRectList[i].CIIndex] = ResultPackRectList[i].CI;
		}
		for ( int i = 0; i < outFontList.Length; i++ ) {
			if ( SpriteEditor ) {
				OutCharacterInfoCollection[i][0] = inFontList[i].characterInfo[0];
				outFontList[i].material = outFontContainer.spriteMaterial;
			}
			outFontList[i].characterInfo = OutCharacterInfoCollection[i];
			EditorUtility.SetDirty( outFontList[i] );
		}
		EditorApplication.SaveAssets();
		AssetDatabase.Refresh();
	}


	//harvest all charaterinfos from infont, into PackRect classes
	void HarvestStartPackRectList () {
		PackRect newPackRect = new PackRect();
		StartPackRectList.Clear();
		for ( int j = 0; j < inFontList.Length; j++ ) {
			for ( int i = 0; i < inFontList[j].characterInfo.Length; i++ ) {
				if ( SpriteEditor && i == 0 ) { continue; }
				Rect newRect = new Rect( 0, 0, 0, 0 );
				newRect.x = Mathf.RoundToInt( inFontList[j].characterInfo[i].uv.x * inTexWidth );
				newRect.y = Mathf.RoundToInt( inFontList[j].characterInfo[i].uv.y * inTexHeight );
				newRect.width = Mathf.RoundToInt( inFontList[j].characterInfo[i].uv.width * inTexWidth );
				newRect.height = Mathf.RoundToInt( inFontList[j].characterInfo[i].uv.height * inTexHeight );
				if ( newRect.height < 0 ) {
					newRect.height *= -1;
					newRect.y -= newRect.height;
				}
				newPackRect.StartRect = newRect;
				newRect = new Rect( 0, 0, 0, 0 );
				newRect.x = Mathf.RoundToInt( inFontList[j].characterInfo[i].uv.x * inTexWidth );
				newRect.y = Mathf.RoundToInt( inFontList[j].characterInfo[i].uv.y * inTexHeight );
				if ( inFontList[j].characterInfo[i].flipped ) {
					newRect.width = Mathf.RoundToInt( inFontList[j].characterInfo[i].uv.width * inTexWidth ) + PackBuffer;
					newRect.height = Mathf.RoundToInt( inFontList[j].characterInfo[i].uv.height * inTexHeight ) - PackBuffer;
				} else {
					newRect.width = Mathf.RoundToInt( inFontList[j].characterInfo[i].uv.width * inTexWidth ) + PackBuffer;
					newRect.height = Mathf.RoundToInt( inFontList[j].characterInfo[i].uv.height * inTexHeight ) + PackBuffer;
				}
				if ( newRect.height < 0 ) {
					newRect.height *= -1;
					newRect.y -= newRect.height;
				}
				newPackRect.ResultRect = newRect;
				newPackRect.CI = inFontList[j].characterInfo[i];
				newPackRect.CIIndex = i;
				newPackRect.fontIndex = j;
				StartPackRectList.Add( new PackRect( newPackRect ) );
			}
		}
	}


	//increases int to next POT
	int NearestPOT ( int n ) {
		int i = 1;
		while ( i < n ) {
			i = i << 1;
		}
		return i;
	}


	//Clears the console
	void ClearLog () {
		Assembly assembly = Assembly.GetAssembly( typeof( SceneView ) );
		System.Type type = assembly.GetType( "UnityEditorInternal.LogEntries" );
		MethodInfo method = type.GetMethod( "Clear" );
		UnityEngine.Object o = new UnityEngine.Object();
		method.Invoke( o, null );
	}


	//Find an existing 3Dtext, or create a new one.
	void LocatePreview ( Font f ) {
		if ( !TextPreview ) {
			TextPreview = GameObject.Find( "**Font Setter Preview Text" );
		}
		if ( TextPreview == null ) {
			TextPreview = new GameObject();
			TextPreview.name = "**Font Setter Preview Text";
			TextPreviewT = TextPreview.AddComponent<TextMesh>() as TextMesh;
			TextPreviewT.font = f;
			TextPreviewR = TextPreview.GetComponent<MeshRenderer>() as Renderer;
			if ( TextPreviewR == null ) TextPreviewR = TextPreview.AddComponent<MeshRenderer>() as Renderer;
			TextPreviewM = f.material;
			TextPreviewR.material = TextPreviewM;
			if ( Application.systemLanguage == SystemLanguage.English ) {
				TextPreviewT.text = "THE FIVE BOXING WIZARDS JUMP QUICKLY\nthe five boxing wizards jump quickly\n0123456789!\"£$%^&*()_+-={}[]:@~;'#<>?,./`¬";
			} else {
				try {
					TextPreviewT.text = "THE FIVE BOXING WIZARDS JUMP QUICKLY\nthe five boxing wizards jump quickly\n0123456789!()+-={}[]:@;'#<>?,./";
				}
				catch {
					//Some symbols apparently make errors in other languages
				}
			}
		} else {
			TextPreviewT = TextPreview.GetComponent<TextMesh>() as TextMesh;
			TextPreviewR = TextPreview.GetComponent<MeshRenderer>() as Renderer;
			TextPreviewM = f.material;
			TextPreviewT.font = f;
			TextPreviewR.material = TextPreviewM;
		}
	}


	//Find an existing Preview sprite, or create a new one.
	void LocatePreviewSprite ( GameObject f ) {
		if ( !SpritePreview ) {
			SpritePreview = GameObject.Find( "**Sprite Setter Preview Sprite" );
		}
		if ( SpritePreview == null ) {
			SpritePreview = Instantiate( f.gameObject ) as GameObject;
			SpritePreview.gameObject.name = "**Sprite Setter Preview Sprite";
			SpritePreview.gameObject.transform.parent = SpritePreview.gameObject.transform;
			SpritePreviewT = SpritePreview.GetComponent<TextMesh>() as TextMesh;
			SpritePreviewT.font = f.GetComponent<AnimatedSprite>().animations[0];
			SpritePreviewR = SpritePreview.GetComponent<MeshRenderer>() as Renderer;
			SpritePreviewR.material = f.GetComponent<AnimatedSprite>().spriteMaterial;
		} else {
			SpritePreviewT = SpritePreview.transform.GetComponent<TextMesh>() as TextMesh;
			SpritePreviewT.font = f.GetComponent<AnimatedSprite>().animations[0];
			SpritePreviewR = SpritePreview.transform.GetComponent<MeshRenderer>() as Renderer;
			SpritePreviewR.material = f.GetComponent<AnimatedSprite>().spriteMaterial;
		}
	}


	//compares the in and out font containers for identical fonts
	bool CheckInOutContainers () {
		if ( inFontContainer.animations.Length != outFontContainer.animations.Length ) {
			ErrorMsg[(int) Errors.UnevenLength] = true;
			return false;
		}
		for ( int i = 0; i < inFontContainer.animations.Length; i++ ) {
			for ( int j = 0; j < outFontContainer.animations.Length; j++ ) {
				if ( inFontContainer.animations[i] == outFontContainer.animations[j] ) {
					ErrorMsg[(int) Errors.ShareFont] = true;
					return false;
				}
			}
		}
		return true;
	}


	//Performs a series of checks on the font container fonts
	bool CheckFonts ( AnimatedSprite FC ) {
		if ( FC == null ) { return false; }
		if ( FC.animations == null || FC.animations.Length < 1 ) {
			ErrorMsg[(int) Errors.NoFonts] = true;
			return false;
		}
		if ( !FC.spriteMaterial ) {
			ErrorMsg[(int) Errors.NoSpriteMaterial] = true;
			return false;
		}

		if ( FC != outFontContainer ) {
			if ( !FC.spriteMaterial.mainTexture ) {
				ErrorMsg[(int) Errors.NoSpriteMaterial] = true;
				return false;
			}
		}
		for ( int i = 0; i < FC.animations.Length; i++ ) {
			if ( !FC.animations[i] ) {
				ErrorMsg[(int) Errors.MissedFont] = true;
				return false;
			}
			int count = 0;
			for ( int j = 0; j < FC.animations.Length; j++ ) {
				if ( FC.animations[i] == FC.animations[j] ) { count++; }
			}
			if ( count > 1 ) {
				ErrorMsg[(int) Errors.SameFont] = true;
				return false;
			}
		}
		if ( FC.gameObject.GetComponent<TextMesh>() == null || FC.gameObject.GetComponent<Renderer>() == null ) {
			ErrorMsg[(int) Errors.NoTextMesh] = true;
			return false;
		} else {
			FC.gameObject.GetComponent<TextMesh>().font = FC.animations[0];
			FC.gameObject.GetComponent<Renderer>().material = FC.spriteMaterial;
		}

		return true;
	}


	//Checks for duped characters
	void CheckDupeChars () {
		ErrorMsg[(int) Errors.DupeChar] = false;
		for ( int i = 0; i < Rects.Count; i++ ) {
			for ( int j = 0; j < Rects.Count; j++ ) {
				if ( i == j ) { continue; }
				if ( FontList[Rects[i].fontIndex].characterInfo[Rects[i].CIIndex].index == FontList[Rects[j].fontIndex].characterInfo[Rects[j].CIIndex].index ) {
					ErrorMsg[(int) Errors.DupeChar] = true;
					char C = (char) FontList[Rects[i].fontIndex].characterInfo[Rects[i].CIIndex].index;
					DupeErrorString = "There is more than one Rect for '" + C + "':";
					FirstDupeRect = i;
					SecondDupeRect = j;
					return;
				}
			}
		}
	}


	//Checks rects for overlap, sets error string
	void CheckOverlapRects () {
		ErrorMsg[(int) Errors.OverlapChar] = false;
		ErrorMsg[(int) Errors.OverlapSprite] = false;
		for ( int i = 0; i < Rects.Count; i++ ) {
			int n = AutoRectIsFree( Rects[i].rect );
			if ( n != -1 ) {
				if ( SpriteEditor ) {
					OverlapErrorString = Rects[n].fontIndex + "#" + FontList[Rects[n].fontIndex].name + " index " + ( FontList[Rects[n].fontIndex].characterInfo[Rects[n].CIIndex].index - 33 ) + "#" + Rects[i].fontIndex + "#" + FontList[Rects[i].fontIndex].name + " index " + ( FontList[Rects[i].fontIndex].characterInfo[Rects[i].CIIndex].index - 33 );
					ErrorMsg[(int) Errors.OverlapSprite] = true;
				} else {
					OverlapErrorString = "These Rects Overlap:#" + (char) FontList[Rects[i].fontIndex].characterInfo[Rects[i].CIIndex].index + "#" + (char) FontList[Rects[n].fontIndex].characterInfo[Rects[n].CIIndex].index;
					ErrorMsg[(int) Errors.OverlapChar] = true;
				}
				FirstOverlapRect = i;
				SecondOverlapRect = n;
			}
		}
	}


	//overload for checksprites for no provided index
	void CheckSprites () {
		CheckSprites( 0 );
	}


	//Checks for duped and skipped sprites, sets the relevant error string, returns the index of the duped or missed sprite 
	int CheckSprites ( int inIndex ) {
		ErrorMsg[(int) Errors.DupeSprite] = false;
		ErrorMsg[(int) Errors.SkipSprite] = false;
		List<List<int>> allInds = new List<List<int>>();
		for ( int i = 0; i < FontList.Length; i++ ) {
			List<int> Inds = new List<int>();
			allInds.Add( Inds );
		}
		for ( int i = 0; i < Rects.Count; i++ ) {
			allInds[Rects[i].fontIndex].Add( FontList[Rects[i].fontIndex].characterInfo[Rects[i].CIIndex].index - 33 );
			int count = 0;
			for ( int j = 0; j < Rects.Count; j++ ) {
				if ( j == i ) { continue; }
				if ( Rects[i].fontIndex == Rects[j].fontIndex && FontList[Rects[i].fontIndex].characterInfo[Rects[i].CIIndex].index ==
							FontList[Rects[j].fontIndex].characterInfo[Rects[j].CIIndex].index ) {
					count++;
					break;
				}
			}
			if ( count > 0 ) {
				ErrorMsg[(int) Errors.DupeSprite] = true;
				DupeErrorString = "More than one sprite in:#" + Rects[i].fontIndex + "#" + FontList[Rects[i].fontIndex].name + "#has index " + ( FontList[Rects[i].fontIndex].characterInfo[Rects[i].CIIndex].index - 33 );
				DupeMissOffset = 1;
				return i;
			}
		}
		for ( int i = 0; i < allInds.Count; i++ ) {
			allInds[i].Sort();
			for ( int j = 0; j < allInds[i].Count; j++ ) {
				if ( j != allInds[i][j] ) {
					ErrorMsg[(int) Errors.SkipSprite] = true;
					SkipErrorString = i + "#" + FontList[i].name + "#is missing index " + j;
					if ( inIndex == i ) {
						DupeMissOffset = j - allInds[i][j];
						return j;
					} else {
						break;
					}
				}
			}
		}
		return -1;
	}


	//Tests if the given Rect is inside the editor window
	bool RectOnScreen ( Rect r ) {
		Rect r2 = new Rect( 0, 0, position.width - 2 * MenuBar.width, position.height );
		return ( ( r.xMin < r2.xMax ) && ( r.xMax > r2.xMin ) && ( r.yMin < r2.yMax ) && ( r.yMax > r2.yMin ) );
	}


	//Checks if a Rect actually has pixels inside it, and if it is inside the bounds of the texture
	bool CheckShrinkForPixels ( int indx, Rect StartR ) {
		if ( Rects[indx].rect.x > theTex.width - 1 || Rects[indx].rect.y > theTex.height - 1 || Rects[indx].rect.xMax < 0 || Rects[indx].rect.yMax < 0 ) {
			//if a rect is entirely outside the texture, return false
			return false;
		}
		//the following 4 parts clip the rect inside the texture boundary
		if ( Rects[indx].rect.x < 0 ) {
			Rects[indx].rect.width += Rects[indx].rect.x;
			Rects[indx].rect.x = 0;
		}
		if ( Rects[indx].rect.xMax > theTex.width ) {
			Rects[indx].rect.xMax = theTex.width;
		}
		if ( Rects[indx].rect.y < 0 ) {
			Rects[indx].rect.height += Rects[indx].rect.y;
			Rects[indx].rect.y = 0;
		}
		if ( Rects[indx].rect.yMax > theTex.height ) {
			Rects[indx].rect.yMax = theTex.height;
		}
		//Check every pixel in the rect. if one is found, snap to pixel grid and return true
		for ( int i = 0; i < Rects[indx].rect.width; i++ ) {
			for ( int j = 0; j < Rects[indx].rect.height; j++ ) {
				if ( TexAsInts[(int) Rects[indx].rect.x + i, (int) Rects[indx].rect.y + j] != 0 ) {
					Rects[indx].rect = Rect.MinMaxRect( Mathf.FloorToInt( Rects[indx].rect.x ), Mathf.FloorToInt( Rects[indx].rect.y ), Mathf.CeilToInt( Rects[indx].rect.xMax ), Mathf.CeilToInt( Rects[indx].rect.yMax ) );
					return true;
				}
			}
		}
		//if the above check didnt find a pixel, put the rect back to non-clipped and snapped, and return false
		Rects[indx].rect = StartR;
		return false;
	}


	//During shrinkwrap, check all pixels along the specified side
	bool ShrinkSide ( int indx, int side, int startMode ) {
		for ( int i = 0; i < ( ( side == 0 || side == 2 ) ? Rects[indx].rect.width : Rects[indx].rect.height ); i++ ) {
			int x = 0, y = 0;
			switch ( side ) {
				case 0:
					x = (int) Rects[indx].rect.x + i;
					y = (int) Rects[indx].rect.y;
					break;
				case 1:
					x = (int) Rects[indx].rect.x;
					y = (int) Rects[indx].rect.y + i;
					break;
				case 2:
					x = (int) Rects[indx].rect.x + i;
					y = (int) ( Rects[indx].rect.y + Rects[indx].rect.height - 1 );
					break;
				case 3:
					x = (int) ( Rects[indx].rect.x + Rects[indx].rect.width - 1 );
					y = (int) Rects[indx].rect.y + i;
					break;
			}
			if ( TexAsInts[x, y] != 0 ) {
				if ( startMode == 1 ) {
					return true;
				} else if ( ( ( side == 1 || side == 3 ) && ( x == 0 || x == theTex.width - 1 ) ) ||
						   ( ( side == 0 || side == 2 ) && ( y == 0 || y == theTex.height - 1 ) ) ) {
					return startMode == 1 ? true : false;
				} else if ( TexAsInts[x + ( ( side == 0 || side == 2 ) ? 0 : ( side == 1 ? -1 : 1 ) ), y + ( ( side == 1 || side == 3 ) ? 0 : ( side == 0 ? -1 : 1 ) )] != 0 ) {
					return true;
				} else {
					continue;
				}
			}
		}
		return false;
	}


	//Shrink or expand all rects to accomodate all included pixels
	void ShrinkWrap () {
		string outString = "Shrinkwrapped ";
		MakeTexAsInts();
		for ( int j = 0; j < FontList.Length; j++ ) {
			if ( outString[outString.Length - 1] != " "[0] ) { outString += " "; }
			for ( int i = 0; i < Rects.Count; i++ ) {
				if ( Rects[i].fontIndex != j ) { continue; }
				Rect StartR = Rects[i].rect;
				if ( !CheckShrinkForPixels( i, Rects[i].rect ) ) { continue; }
				int step = 1;
				while ( step != 0 ) {
					step = 0;
					bool mode = ShrinkSide( i, 0, 3 );
					while ( mode == ShrinkSide( i, 0, mode ? 2 : 1 ) ) {
						step++;
						Rects[i].rect.y += mode ? -1 : 1;
						Rects[i].spritePivot.y -= mode ? -1 : 1;
						Rects[i].rect.height += mode ? 1 : -1;
					}

					mode = ShrinkSide( i, 1, 3 );
					while ( mode == ShrinkSide( i, 1, mode ? 2 : 1 ) ) {
						step++;
						Rects[i].rect.x += mode ? -1 : 1;
						Rects[i].spritePivot.x -= mode ? -1 : 1;
						Rects[i].rect.width += mode ? 1 : -1;
					}

					mode = ShrinkSide( i, 2, 3 );
					while ( mode == ShrinkSide( i, 2, mode ? 2 : 1 ) ) {
						step++;
						Rects[i].rect.height += mode ? 1 : -1;
					}

					mode = ShrinkSide( i, 3, 3 );
					while ( mode == ShrinkSide( i, 3, mode ? 2 : 1 ) ) {
						step++;
						Rects[i].rect.width += mode ? 1 : -1;
					}
				}
				if ( Rects[i].rect != StartR ) {
					if ( SpriteEditor ) {
						if ( outString[outString.Length - 1] == " "[0] ) {
							outString += FontList[Rects[i].fontIndex].name + ", frames:";
						}
						outString += ( FontList[Rects[i].fontIndex].characterInfo[Rects[i].CIIndex].index - 33 ) + ",";
					} else {
						char C = (char) FontList[Rects[i].fontIndex].characterInfo[Rects[i].CIIndex].index;
						outString += "'" + C + "' ";
					}
				}
				UpdateFont( i, Rects[i].CIIndex, Rects[i].fontIndex, false );
			}
		}
		CheckOverlapRects();
		if ( ClickedRectInd != -1 ) { GetFontInfoToGUI( Rects[ClickedRectInd].CIIndex, Rects[ClickedRectInd].fontIndex ); }
		if ( outString.Length > 20 ) {
			if ( outString[outString.Length - 1] == ',' ) outString = outString.TrimEnd( ',' );
			Debug.Log( outString );
		} else {
			Debug.Log( "No need for shrinkwrap" );
		}
	}


	//When reading the texture into int array, pixels that match this condition will be ignored in other voids
	bool IgnoreCondition ( Color c ) {
		return c.a == 0;
	}

	//When reading the texture into int array, pixels that match this condition will be treated as 'Smart'
	bool IsSmart ( Color c ) {
		return
			c.r > SmartColor.r - 0.02f && c.r < SmartColor.r + 0.02f &&
			c.g > SmartColor.g - 0.02f && c.g < SmartColor.g + 0.02f &&
			c.b > SmartColor.b - 0.02f && c.b < SmartColor.b + 0.02f &&
			c.a > SmartColor.a - 0.02f && c.a < SmartColor.a + 0.02f;
	}


	//To reduce repeated GetPixel calls, this creates a 2d array of ints, where 0 is ignored, 1 is a pixel, 2 is a checked pixel, 3 is a smart pixel
	void MakeTexAsInts () {
		TexAsInts = new int[theTex.width, theTex.height];
		for ( int i = 0; i < theTex.width; i++ ) {
			for ( int j = 0; j < theTex.height; j++ ) {
				Color c = theTex.GetPixel( i, j );
				TexAsInts[i, j] = IgnoreCondition( c ) ? 0 : ( ( SpriteEditor || SmartMode ) && ( IsSmart( c ) ) ) ? 3 : 1;
			}
		}
	}



	void LocatePivots () {
		string outString = "Couldnt find pivot for: ";
		MakeTexAsInts();
		for ( int j = 0; j < FontList.Length; j++ ) {
			if ( outString[outString.Length - 1] != " "[0] ) { outString += " "; }
			for ( int i = 0; i < Rects.Count; i++ ) {
				if ( Rects[i].fontIndex != j ) { continue; }
				if ( FindPivots( i ) ) {
					UpdateFont( i, Rects[i].CIIndex, Rects[i].fontIndex, false );
				} else {
					if ( outString[outString.Length - 1] == " "[0] ) {
						outString += FontList[Rects[i].fontIndex].name + ", frames:";
					}
					outString += ( FontList[Rects[i].fontIndex].characterInfo[Rects[i].CIIndex].index - 33 ) + ",";
				}
			}
		}
		if ( outString.Length > 27 ) {
			if ( outString[outString.Length - 1] == ',' ) outString = outString.TrimEnd( ',' );
			Debug.Log( outString );
		} else {
			Debug.Log( "All pivots in place" );
		}

	}


	//for spritesetter, move the pivot to the smart pixel in the texture
	bool FindPivots ( int RInd ) {
		for ( int i = 0; i < Rects[RInd].rect.width; i++ ) {
			for ( int j = 0; j < Rects[RInd].rect.height; j++ ) {
				if ( TexAsInts[(int) Rects[RInd].rect.x + i, (int) Rects[RInd].rect.y + j] == 3 ) {
					Rects[RInd].spritePivot = new Vector2( i, j );
					return true;
				}
			}
		}
		return false;
	}


	//finds which rect is anim: a frame: f
	int FindFrame () {
		for ( int i = 0; i < Rects.Count; i++ ) {
			if ( Rects[i].fontIndex == SpritePropertiesFontIndex && FontList[Rects[i].fontIndex].characterInfo[Rects[i].CIIndex].index == SpriteFrame + 33 ) {
				return i;
			}
		}
		Debug.Log( "Unable to find frames in " + FontList[SpritePropertiesFontIndex].name );
		return -1;
	}

	//returns a rect that is 'snapped' to unit measures
	Rect RoundedRect ( Rect r ) {
		return new Rect( Mathf.RoundToInt( r.x ), Mathf.RoundToInt( r.y ), Mathf.RoundToInt( r.width ), Mathf.RoundToInt( r.height ) );
	}


	//returns a Vector2 that is 'snapped' to unit measures
	Vector2 RoundedV2 ( Vector2 v ) {
		return new Vector2( Mathf.RoundToInt( v.x ), Mathf.RoundToInt( v.y ) );
	}


	////////////////////////////////////////////////////////////////////////////













}
