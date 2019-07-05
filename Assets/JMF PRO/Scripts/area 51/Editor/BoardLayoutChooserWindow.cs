using UnityEditor;
using UnityEngine;



/// <summary> ##################################
/// 
/// NOTICE :
/// This script is just an editor extension to call and open the board layout custom
/// inspector onto a gui window. This makes it easier to see the custom inspector as
/// it is naturally going to be quite big.
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################


public class BoardLayoutChooserWindow : EditorWindow
{
	static bool showPieces = false;
	static BoardLayout bl;
	static int pos = 0;

	public static void setDisplayType (int position,bool _showPieces) 
    {
		pos = position;
		showPieces = _showPieces;
		EditorWindow bsc = EditorWindow.GetWindow (typeof (BoardLayoutChooserWindow),false, "Choose Type");
		bsc.position = new Rect( Event.current.mousePosition.x + 150,Event.current.mousePosition.y + 250,
		                        bsc.position.width,bsc.position.height);
    }

    void OnGUI () {
		if( Selection.activeGameObject != null){
			bl = Selection.activeGameObject.GetComponent<BoardLayout>();

			if (bl != null)
	        {
				showChoices();
	        } else { 
				showErrorMsg(); // tells user to select the GameManger object
			}
		} else {
			showErrorMsg(); // tells user to select the GameManger object
		}
    }
	
	void showErrorMsg(){
		EditorGUILayout.LabelField("\n* Please select the object that contains the " +
					"\"GameManager\" script.\nThen check back here again.", GUILayout.Height(45));
	}

	void showChoices(){
		if(showPieces){
			setupPieces();
		} else {
			setupPanels();
		}
	}


	void setupPieces(){
		GUILayoutOption[] layoutParams = {GUILayout.Width(40),GUILayout.Height(40)};
		EditorGUILayout.BeginHorizontal();
		for(int x = 0; x < bl.pieceScripts.Length; x++){

			if(x % 5 == 0) {
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
			}

			// pieces selection
			if(bl.pieceEditVisuals[x] != null && bl.pieceEditVisuals[x].texture != null){ // assigned texture version
				if(GUILayout.Button( bl.pieceEditVisuals[x].texture, GUI.skin.label, layoutParams ) ){
					bl.setPiece(pos,x);
					EditorWindow.GetWindow (typeof (BoardLayoutWindow),false, "Board Setup");
					this.Close();
				}
			} else if(bl.pieceScripts[x] != null){ // assigned texture version
				if(GUILayout.Button( bl.pieceScripts[x].GetType().ToString().Replace("Piece",""),
				                    new GUILayoutOption[] {GUILayout.ExpandWidth(false)} ) ){
					bl.setPiece(pos,x);
					EditorWindow.GetWindow (typeof (BoardLayoutWindow),false, "Board Setup");
					this.Close();
				}
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	void setupPanels(){
		GUILayoutOption[] layoutParams = {GUILayout.Width(40),GUILayout.Height(40)};
		EditorGUILayout.BeginHorizontal();
		for(int x = 0; x < bl.panelScripts.Length; x++){
			if(x % 5 == 0) {
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.BeginHorizontal();
			}
			// panels selection
			if(bl.panelEditVisuals[x] != null && bl.panelEditVisuals[x].texture != null){ // assigned texture version
				if(GUILayout.Button( bl.panelEditVisuals[x].texture,GUI.skin.box, layoutParams ) ){
					bl.setPanel(pos,x);
					EditorWindow.GetWindow (typeof (BoardLayoutWindow),false, "Board Setup");
					this.Close();
				}
			} else { // script name version
				if(GUILayout.Button( bl.panelScripts[x].GetType().Name.Substring(0,
				                    Mathf.Min (5,bl.panelScripts[x].name.Length)).ToString().Replace("Panel",""),
				                    new GUILayoutOption[] {GUILayout.ExpandWidth(false)} ) ){
					bl.setPanel(pos,x);
					EditorWindow.GetWindow (typeof (BoardLayoutWindow),false, "Board Setup");
					this.Close();
				}
			}
		}
		EditorGUILayout.EndHorizontal();
	}
}