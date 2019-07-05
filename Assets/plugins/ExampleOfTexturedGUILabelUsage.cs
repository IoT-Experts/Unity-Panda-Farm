using UnityEngine;
using System.Collections;

public class ExampleOfTexturedGUILabelUsage : MonoBehaviour {
	public Font theFont;
	void OnGUI () {
		//Use TexturedGUILabel to draw text in UnityGUI
		//NOTE: In order to call TexturedGUILabel like this (in C#), you need to be using the C# version of the TexturedGUILabel class
		new TexturedGUILabel(new Rect(20, 100, 400, 110), "This is an OnGUI Textured GUILabel", theFont);
	}
}
