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

public class VisualizedGrid : MonoBehaviour
{
    [Range(1, 50)]
    public int refreshRate = 7;
    int internalTimer = 0;
    [HideInInspector]
    public GameManager gm;
    [HideInInspector]
    public VectorFrame Corners = new VectorFrame(Vector2.zero);

    [HideInInspector]
    public float ratio
    {
        get
        {
            if (gm == null) { return 1f; }
            return 1f;
        }
    }
    [HideInInspector]
    public float yOffset
    {
        get
        {
            if (gm == null) { return 0f; }
            return 0f;
        }
    }

    [HideInInspector]
    public Vector3 TileSize2D
    {
        get
        {
            return new Vector3(gm.size * ratio, gm.size * ratio, 0.01f);
        }
    }
    [HideInInspector]
    public Vector3 TileSize2DPadded
    {
        get
        {
            return new Vector3(gm.size * ratio * (1f - (gm.paddingPercentage / 100)),
                               gm.size * ratio * (1f - (gm.paddingPercentage / 100)), 0.01f);
        }
    }

    public Vector3 this[int x, int y]
    {
        get
        {
            return Corners.CalculatePosFromBottomLeft(x, y, gm.size);
        }
    }

    public struct VectorFrame
    {
        public Vector2 TopL;
        public Vector2 TopR;

        public Vector2 BottomL;
        public Vector2 BottomR;

        public float z;

        public VectorFrame(Vector2 kickstart)
        {
            TopL = TopR = BottomL = BottomR = kickstart;
            z = 0;
        }

        public VectorFrame(Vector2 topL, Vector2 topR, Vector2 bottomL, Vector2 bottomR)
            : this(topL, topR, bottomL, bottomR, 0f)
        {
        }

        public VectorFrame(Vector2 topL, Vector2 topR, Vector2 bottomL, Vector2 bottomR, float frameZ)
        {
            TopL = topL;
            TopR = topR;
            BottomL = bottomL;
            BottomR = bottomR;
            z = frameZ;
        }

        public VectorFrame Set(Vector2 topL, Vector2 topR, Vector2 bottomL, Vector2 bottomR, float frameZ = 0f)
        {
            TopL = topL;
            TopR = topR;
            BottomL = bottomL;
            BottomR = bottomR;
            z = frameZ;

            return this;
        }

        public Vector3 CalculatePosFromTopLeft(int x, int y, float step)
        {
            return new Vector3(TopL.x + ((x * step) + (step * 0.5f)), TopL.y - ((y * step) + (step * 0.5f)), z);
        }

        public Vector3 CalculatePosFromBottomLeft(int x, int y, float step)
        {
            return new Vector3(BottomL.x + ((x * step) + (step * 0.5f)), BottomL.y + ((y * step) + (step * 0.5f)), z);
        }
        public Vector3 CalculatePosFromBottomLeftHex(int x, int y, float step)
        {
            Vector3 pos = CalculatePosFromBottomLeft(x, y, step);
            if (x % 2 == 0)
            { // displacement for hexagon type
                pos.Set(pos.x * 0.865f, pos.y + (0.25f * step), pos.z);
            }
            else
            {
                pos.Set(pos.x * 0.865f, pos.y - (0.25f * step), pos.z);
            }
            return pos;
        }

        public Vector3 CalculatePosFromTopRight(int x, int y, float step)
        {
            return new Vector3(TopR.x - ((x * step) + (step * 0.5f)), TopR.y - ((y * step) + (step * 0.5f)), z);
        }

        public Vector3 CalculatePosFromBottomRight(int x, int y, float step)
        {
            return new Vector3(BottomR.x - ((x * step) + (step * 0.5f)), BottomR.y + ((y * step) + (step * 0.5f)), z);
        }
    }

    // ####################################
    // start of functions
    // ####################################

    // values that is refreshed each call
    private void SetValues()
    {
        // for refresh rate... reduce performance of Unity editor
        internalTimer++;
        if (internalTimer < refreshRate)
        {
            return;
        }
        internalTimer = 0;

        float halfWidth = gm.size * gm.boardWidth * 0.5f;
        float halfHeight = gm.size * gm.boardHeight * 0.5f;

        Vector3 TempPos = gm.transform.position;

        Corners.Set(
            new Vector2(TempPos.x - halfWidth, TempPos.y + halfHeight),
            new Vector2(TempPos.x + halfWidth, TempPos.y + halfHeight),
            new Vector2(TempPos.x - halfWidth, TempPos.y - halfHeight),
            new Vector2(TempPos.x + halfWidth, TempPos.y - halfHeight),
            TempPos.z
            );
    }

    public void OnDrawGizmos()
    {
        if (gm != null)
        {
            SetValues(); // refresh all values that is needed

            // show corners
            if (gm.showCorners)
            {
                #region corners
                Gizmos.color = Color.red;
                Gizmos.DrawLine(
                    new Vector3((Corners.TopL.x) * ratio, Corners.TopL.y + yOffset, Corners.z - 0.5f),
                    new Vector3((Corners.TopL.x + gm.size) * ratio, Corners.TopL.y + yOffset, Corners.z - 0.5f)
                    );
                Gizmos.DrawLine(
                    new Vector3((Corners.TopL.x) * ratio, Corners.TopL.y + yOffset, Corners.z - 0.5f),
                    new Vector3((Corners.TopL.x) * ratio, Corners.TopL.y + yOffset - gm.size, Corners.z - 0.5f)
                    );
                Gizmos.DrawLine(
                    new Vector3((Corners.TopR.x) * ratio, Corners.TopR.y + yOffset, Corners.z - 0.5f),
                    new Vector3((Corners.TopR.x - gm.size) * ratio, Corners.TopR.y + yOffset, Corners.z - 0.5f)
                    );
                Gizmos.DrawLine(
                    new Vector3((Corners.TopR.x) * ratio, Corners.TopR.y + yOffset, Corners.z - 0.5f),
                    new Vector3((Corners.TopR.x) * ratio, Corners.TopR.y + yOffset - gm.size, Corners.z - 0.5f)
                    );
                Gizmos.DrawLine(
                    new Vector3((Corners.BottomL.x) * ratio, Corners.BottomL.y - yOffset, Corners.z - 0.5f),
                    new Vector3((Corners.BottomL.x + gm.size) * ratio, Corners.BottomL.y - yOffset, Corners.z - 0.5f)
                    );
                Gizmos.DrawLine(
                    new Vector3((Corners.BottomL.x) * ratio, Corners.BottomL.y - yOffset, Corners.z - 0.5f),
                    new Vector3((Corners.BottomL.x) * ratio, Corners.BottomL.y - yOffset + gm.size, Corners.z - 0.5f)
                    );
                Gizmos.DrawLine(
                    new Vector3((Corners.BottomR.x) * ratio, Corners.BottomR.y - yOffset, Corners.z - 0.5f),
                    new Vector3((Corners.BottomR.x - gm.size) * ratio, Corners.BottomR.y - yOffset, Corners.z - 0.5f)
                    );
                Gizmos.DrawLine(
                    new Vector3((Corners.BottomR.x) * ratio, Corners.BottomR.y - yOffset, Corners.z - 0.5f),
                    new Vector3((Corners.BottomR.x) * ratio, Corners.BottomR.y - yOffset + gm.size, Corners.z - 0.5f)
                    );
                #endregion corners
            }

            // show grid box / padded boxes using one loop
            if (gm.showGrid || gm.showPaddedTile)
            {
                for (int x = 0; x < gm.boardWidth; x++)
                {
                    for (int y = 0; y < gm.boardHeight; y++)
                    {
                        if (gm.showGrid)
                        { // show grid box
                            Gizmos.color = Color.green;
                            Gizmos.DrawWireCube(this[x, y], TileSize2D);
                        }
                        if (gm.showPaddedTile)
                        { // show padded boxes
                            Gizmos.color = Color.blue;
                            Gizmos.DrawWireCube(this[x, y], TileSize2DPadded);
                        }

                    }
                }
            }
        }
    }
}