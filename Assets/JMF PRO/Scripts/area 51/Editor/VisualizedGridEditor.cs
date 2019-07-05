// Copyright (c) 2013 Sebastian Hein (nyaa.labs@gmail.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.


using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(VisualizedGrid))]
public class VisualizedGridEditor : Editor
{
	
	void OnSceneGUI()
	{
		VisualizedGrid grid = (VisualizedGrid) target;
		
		if(grid.gm == null){
			grid.gm = grid.GetComponent<GameManager>(); // assign gm ref if needed
		}
		if(grid.gm.showGrid || grid.gm.showCorners || grid.gm.showPaddedTile){
			// board size adjuster
			Handles.color = Color.green;
			Vector3 dotSize = grid.transform.position
				+ new Vector3((grid.gm.size * grid.ratio * (grid.gm.boardWidth/2f) ), 0, 0) 
					+ Vector3.right
					+ new Vector3(0, (grid.gm.size * grid.ratio * (grid.gm.boardHeight/2f)) 
					              + (grid.gm.size * grid.yOffset * 2), 0) 
					+ Vector3.up;
			grid.gm.size =
				Handles.ScaleValueHandle(grid.gm.size,
				                         dotSize, Quaternion.identity, 
				                         HandleUtility.GetHandleSize(grid.transform.position) * 1.5f,
				                         Handles.SphereCap, 1);
			
			// board width adjuster
			Handles.color = Color.yellow;
			Vector3 dotwidth = grid.transform.position
				+ new Vector3((grid.gm.size * grid.ratio * (grid.gm.boardWidth/2f) ), 0, 0) 
					+ Vector3.right;
			grid.gm.boardWidth = (int)
				Handles.ScaleValueHandle(grid.gm.boardWidth,
				                         dotwidth, Quaternion.identity, HandleUtility.GetHandleSize(grid.transform.position) * 1.5f,
				                         Handles.SphereCap, 1);
			
			// board height adjuster
			Handles.color = Color.red;
			Vector3 dotHeight = grid.transform.position
				+ new Vector3(0, (grid.gm.size * grid.ratio * (grid.gm.boardHeight/2f)) 
				              + (grid.gm.size * grid.yOffset * 2), 0) 
					+ Vector3.up;
			grid.gm.boardHeight = (int)
				Handles.ScaleValueHandle(grid.gm.boardHeight,
				                         dotHeight, Quaternion.identity,
				                         HandleUtility.GetHandleSize(grid.transform.position) * 1.5f,
				                         Handles.SphereCap,
				                         1);
			
			if (grid.gm.showToolTips)
			{
				GUIStyle style = new GUIStyle();
				style.normal.textColor = Color.red;
				
				style.alignment = TextAnchor.MiddleLeft;
				
				// drag tooltip
				Handles.Label(dotHeight + Vector3.up*8 + Vector3.left*6, 
				              "Drag the dots to resize the grid.", style);
				
				// size tooltip
				Handles.Label(dotSize + Vector3.right, 
				              " <-- Current size: " + grid.gm.size, style);
				
				// height tooltip
				Handles.Label(dotHeight + Vector3.up*3, 
				              "Current Height : " + grid.gm.boardHeight, style);
				
				// width tooltip
				Handles.Label(dotwidth + Vector3.right, 
				              " <-- Current Width: " + grid.gm.boardWidth, style);
				
				
				// board values display
				style.normal.textColor = Color.black;
				style.alignment = TextAnchor.MiddleCenter;
				
				Vector3 posAdjust = new Vector3((grid.gm.size * 0.15f), -grid.gm.size * 0.15f, 0);
				for (int x = 0; x < grid.gm.boardWidth; x++)
				{
					for (int y = 0; y < grid.gm.boardHeight; y++)
					{
						Handles.Label(grid[x, y] - posAdjust, "X: " + x + "\nY: " + y, style);
					}
				}
			}
		}
		if(GUI.changed){
			EditorUtility.SetDirty(target);
			EditorUtility.SetDirty(grid.gm);
		}
	}
}
