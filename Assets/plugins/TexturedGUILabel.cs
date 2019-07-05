#pragma warning disable 0618

using UnityEngine;

class TexturedGUILabel {
	public TexturedGUILabel (Rect r1, string s, Font f) {
		if (f.material.mainTexture) { //make sure the font has a texture

			//Uncomment this to draw a Box the same size as the input rect, can help with visualising where the text is drawn
			//GUI.Box(r1,"");

			float w = 0; //spacing between draws
			float v = 0; //spacing between lines, but only if there is some way to determine the Fonts line spacing =(
			for (int i = 0; i < s.Length; i++) {
				char C = s[i]; //the char in the input string
				CharacterInfo c;
				if (f.GetCharacterInfo(C, out c)) { //Test if the chracter is in the font, and set c to its CharacterInfo
					if (c.index == 32) {
						w += c.vert.width;
						w += ( c.width - c.vert.width );
						continue;
					}
					if (c.flipped) { //If the character is flipped (rotated) need to rotate the gui matrix to draw it upright
						Matrix4x4 MB = GUI.matrix;
						GUIUtility.RotateAroundPivot(90f, new Vector2(r1.x + w + c.vert.x, r1.y - c.vert.y));
						GUI.DrawTextureWithTexCoords(new Rect(r1.x + w + c.vert.x, v + r1.y - c.vert.y, -c.vert.height, -c.vert.width), f.material.mainTexture, new Rect(c.uv.x + c.uv.width, c.uv.y + c.uv.height, -c.uv.width, -c.uv.height));
						GUI.matrix = MB;
					} else { //Draw the font bitmap, using the UVs taken from the CharacterInfo
						GUI.DrawTextureWithTexCoords(new Rect(r1.x + w + c.vert.x, v + r1.y - c.vert.y, c.vert.width, -c.vert.height), f.material.mainTexture, c.uv);
					}
					w += c.vert.width;
					w += ( c.width - c.vert.width );
				} else {
					if (C == 10) {
						//These 2 lines would continue the label drawing on a new line, but theres no way to determine line spacing
						//w = 0;
						//v += p.floatValue;
					}
				}
			}
		}
	}
}