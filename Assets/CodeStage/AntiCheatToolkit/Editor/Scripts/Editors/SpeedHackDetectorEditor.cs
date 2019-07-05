using CodeStage.AntiCheat.Detectors;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpeedHackDetector))]
public class SpeedHackDetectorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		GUIStyle textStyle = new GUIStyle();
		textStyle.normal.textColor = GUI.skin.label.normal.textColor;
		textStyle.alignment = TextAnchor.UpperLeft;
		textStyle.contentOffset = new Vector2(2, 0);
		textStyle.wordWrap = true;

		EditorGUILayout.LabelField(new GUIContent("Don't forget to start detection (check readme)!", "You should start detector from code using SpeedHackDetector.StartDetection(Action) method."), textStyle);
	}
}
