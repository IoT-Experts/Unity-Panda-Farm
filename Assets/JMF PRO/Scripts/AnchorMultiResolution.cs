    /// <summary>
/// Multi Resolution Anchor
/// *************
/// Currently only works with scenes with orthographic camera. OrthoSize is set to 1 as default.
/// Don't forget to Tag your camera as MainCamera (one of the default Unity tags)
///  
/// Free for any usage, modification, and distribution, which hereby granted by the decree of the Ministry of Magic.
/// Ravenclaw represent!
/// 
/// Usage: Attach this script to a GameObject. The anchors are called on Start(). Remove the Editor section (line 146 ~157) if you don't need them updating in the SceneView.
/// </summary>

using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode] // This allow us to store the position in scene view after pressing "Play". Editor class is located in the lower section for reference;


public class AnchorMultiResolution : MonoBehaviour
{
    float screenY = 0;
    float screenX = 0;

    public float orthoSize = 1;
    public Transform mainCamera;
    // NOTE: If you don't want to use the camera (especially at Start()): 
    // 1. Remove the mainCamera line above and also remove the Get Camera Section in setAnchor().
    // 2. Set the orthosize above to your main camera orthographic size.
    // 3. Don't forget to change camX and camY values to your main camera X and Y values in setAnchor().


    public bool viewOnUpdate = true;

    public float pixelInsetX = 0, pixelInsetY = 0;
    private float privInsetX = 0, privInsetY = 0;
    float pixelFactor = 1;

    public enum ScreenAnchor
    {
        TopLeft, TopCenter, TopRight,
        CenterLeft, Center, CenterRight,
        BottomLeft, BottomCenter, BottomRight
    }

    public ScreenAnchor anchor = ScreenAnchor.Center;

    public float xTransform = 0; //Use these two to tune the current transform in SceneView. 
    public float yTransform = 0; //They are called in setAnchor since Inspector transform is freezed due to the Update() in the editor section at the bottom of this script 

    //	private float initX = 0, initY = 0;

    void setAnchor()
    {
        screenY = Screen.height;
        screenX = Screen.width;

        // Get Camera section
        if (Camera.main == null)
        {
            Debug.Log("Main Camera not found. \n Camera must be TAGGED as \"MainCamera\" unless you want to set the ortho size manually or you prefer to use the default orthoSize as 1");
            Debug.Log("If you do not want to use the camera, check the script comments for more info.");
        }

        if (Camera.main != null)
        {
            if (Camera.main.orthographic)
            {
                mainCamera = Camera.main.transform;
                orthoSize = Camera.main.orthographicSize;
            }
            else { Debug.Log("Camera is not set to orthographic!"); }
        }
        //End Section

        // Pixel Factor : Just to get Unity dimension to pixel ratio.
        // screenY is divided by 2 since when we put camera at [0,0], 
        // the camera include lengths from BOTH positive and negative Y axis.

        pixelFactor = orthoSize / (screenY / 2);

        Transform currentTransform = this.gameObject.transform;

        float posX = currentTransform.position.x;
        float posY = currentTransform.position.y;
        //float posZ = currentTransform.position.z;

        float nudgeX = screenX / 2 * pixelFactor;
        float nudgeY = screenY / 2 * pixelFactor;

        float camX = mainCamera.position.x; //You can assign your own values/other camera transforms here
        float camY = mainCamera.position.y;

        privInsetX = pixelInsetX * pixelFactor;
        privInsetY = pixelInsetY * pixelFactor;

        switch (anchor)
        {

            case ScreenAnchor.TopLeft:
                posX = -nudgeX + privInsetX;
                posY = nudgeY - privInsetY;
                break;

            case ScreenAnchor.CenterLeft:
                posX = -nudgeX + privInsetX;
                posY = 0 + privInsetY;
                break;

            case ScreenAnchor.BottomLeft:
                posX = -nudgeX + privInsetX;
                posY = -nudgeY + privInsetY;
                break;

            case ScreenAnchor.TopCenter:
                posX = 0 + privInsetX;
                posY = nudgeY - privInsetY;
                break;

            case ScreenAnchor.Center:
                posX = 0 + privInsetX;
                posY = 0 + privInsetY;
                break;

            case ScreenAnchor.BottomCenter:
                posX = 0 + privInsetX;
                posY = -nudgeY + privInsetY;
                break;

            case ScreenAnchor.TopRight:
                posX = nudgeX - privInsetX;
                posY = nudgeY - privInsetY;
                break;

            case ScreenAnchor.CenterRight:
                posX = nudgeX - privInsetX;
                posY = 0 + privInsetY;
                break;

            case ScreenAnchor.BottomRight:
                posX = nudgeX - privInsetX;
                posY = -nudgeY + privInsetY;
                break;

        }
        currentTransform.position = new Vector3(posX + camX + xTransform, posY + camY + yTransform, 0);
    }

    // Set Anchor at Start

    private void Start()
    {
        setAnchor();
    }


    /// For the lazy///

#if UNITY_EDITOR
    // This only update the function in the editor. This portion won't be compiled into the target build.
    // Reference: http://docs.unity3d.com/Documentation/Manual/PlatformDependentCompilation.html
    // Feel free to remove this (or comment it out) if you do not need to see it updating in Scene View

    void Update()
    {
        if (viewOnUpdate)
        {
            this.setAnchor();
        }

    }

#endif

    /// End section ///

}