using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using CodeStage.AntiCheat.ObscuredTypes;

/// <summary> ##################################
/// 
/// NOTICE :
/// This script is a setup function to customize the gameBoard looks during gameplay.
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################

public class BoardLayout : MonoBehaviour
{

    public GameManager gm;
    //string data = "0,5,2,8,0,8,0,8,0,8,0,8,3,8,4,8,0,8,0,8,0,8,0,8,0,8,0,4,0,8,0,6,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0,8,0";
    public bool randomOnStart = false;
    public bool useSelector = false;
    public bool showHexGrid = true;
    public bool hidePanel1 = false;
    public bool hidePanel2 = false;
    public bool hidePanel3 = false;

    public PanelDefinition[] panelScripts; // panel reference scripts to be used
    public PieceDefinition[] pieceScripts; // piece reference scripts to be used

    // these are max values for a panel type during randomize
    public PanelLimit[] randomPanelLimit;
    [System.Serializable]
    public class PanelLimit
    {
        [HideInInspector]
        public string name;
        public int randomLimit;
    }

    // these are the counter for the max panels above
    public int[] randomPanelCount;

    // these are the texture array for representing the panels
    public TextureArray[] panelEditVisuals;
    // these are the texture array for representing the pieces
    public TextureArray[] pieceEditVisuals;
    [System.Serializable]
    public class TextureArray
    {
        [HideInInspector]
        public string name;
        public Texture texture;
    }

    public int[] panelArray; // the PanelType[] converted to be and int reference so that it is serialisable
    public int[] pStrength; // the strength of the panel assigned

    public int[] pieceArray; // the piece type to be assigned during gameplay.
    public int[] colorArray; // the manual skin to assign ( semi randomized )

    // these textures are for inspector visuals only - does not effect gameplay
    // paired and referenced by "BoardSetup" GUI script
    public Vector2 scrollPos; // for the scrollbars
    public Vector2 scrollPos2; // for the scrollbar
    public Vector2 scrollPos3; // for the scrollbar

    // for post manual color pre-start match
    bool[,] isManual;

    // weighted colors
    public List<WeightedLayout> colorWeight;
    public WeightedLayout displayedWeight;
    [System.Serializable]

    public class WeightedLayout
    {
        [HideInInspector]
        public string name;
        public bool useWeights = false;
        [Range(0, 100)]
        public List<int> weights = new List<int>(9);
    }

    int totalWeight = 0; // variable to hold the total weights
    int selected = 0; // a variable to store the selected random range for weights
    int addedWeight = 0; // a variable to store the cumulative added weight for calculations

    // called by GameManager for panel setup during pre-game init
    public void setupGamePanels()
    {
        GP_ConfigData configData = FindObjectOfType<GP_ConfigData>();
        if (randomOnStart)
        {
            randomize();
        }
        int level = ObscuredPrefs.GetInt("level");
        TextAsset file = (TextAsset)Resources.Load("Level/Level" + level);
        if (file == null)
        {
            Debug.Log("null file");
            return;
        }
        configData.LoadJsonData(file.text, gm, panelScripts);
    }

    public void setupGamePieces()
    {
        // code below sets up the pieces as per shown in the inspectors
        // color randomization
        int randomColor = Random.Range(0, 9);
        isManual = new bool[gm.boardWidth, gm.boardHeight]; // size of the board
        int count = 0;
        for (int y = gm.boardHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < gm.boardWidth; x++)
            {
                // init default value
                isManual[x, y] = false;
                // set the piece type first
                if (pieceArray[count] != 0)
                {
                    gm.board[x, y].setSpecialPiece(pieceScripts[pieceArray[count]]);
                    isManual[x, y] = true; // manual override is true
                }

                // then set the color (if defined...)
                if (colorArray[count] != 0 && gm.board[x, y].isFilled && !gm.board[x, y].piece.pd.isSpecial)
                {
                    //if (x == 0 && y == 0 || x == 1 && y == 0 || x == 3 && y == 0 || x == 2 && y == 1)
                    //{
                    //    randomColor = 1;
                    //}
                    gm.board[x, y].piece.slotNum = (colorArray[count] + randomColor) % gm.NumOfActiveType;
                    isManual[x, y] = true; // manual override is true
                }

                else if (gm.board[x, y].isFilled && !gm.board[x, y].piece.pd.isSpecial && colorWeight[count].useWeights)
                { // weights distribution functionality
                    // run once weighted calculation...
                    totalWeight = 0; // reset the value first...
                    for (int z = 0; z < gm.NumOfActiveType; z++)
                    { // adds all available skin based on active type
                        if (z < colorWeight[count].weights.Count)
                        { // ensure we have allocated weights and add to the list
                            totalWeight += colorWeight[count].weights[z];
                        }
                    }
                    selected = Random.Range(1, totalWeight + 1); // the selected weight by random
                    addedWeight = 0; // resets the value first...
                    for (int z = 0; z < colorWeight[count].weights.Count; z++)
                    {
                        addedWeight += colorWeight[count].weights[z];
                        if (colorWeight[count].weights[z] > 0 && addedWeight > selected)
                        {
                            gm.board[x, y].piece.slotNum = z; // found the skin we want to use based on the selected weight
                            break;
                        }
                    }
                    isManual[x, y] = true; // manual override is true
                }
                count++;
            }
        }
        // pre-game eliminate pre-start match
        postPrestartMatch();
    }

    void postPrestartMatch()
    {
        // redesign the board until there's no starting match
        if (gm.eliminatePreStartMatch && gm.NumOfActiveType >= 2)
        {
            int count = 0;
            for (int x = 0; x < gm.boardWidth; x++)
            { // iterate through each board block
                for (int y = 0; y < gm.boardHeight; y++)
                {
                    count++;
                    if (findPrematches(x, y))
                    { // find any match and change its type
                        x = 0; y = -1; // restart the loop
                    }
                    if (count > 9999)
                    { // if cannot solve by this num of tries, break!
                        break;
                    }
                }
                if (count > 9999)
                { // if cannot solve by this num of tries, break!
                    Debug.LogError("failed to eliminate pre-start match...");
                    break;
                }
            }
        }
    }

    // function to eliminate pre-start matches
    bool findPrematches(int xPos, int yPos)
    {
        // variables to keep track of the match potentials
        int matchingRows = 0;
        int matchingCols = 0;
        //int matchingTR = 0;
        //int matchingTL = 0;

        if (!(gm.board[xPos, yPos].isFilled && gm.board[xPos, yPos].canBeMatched() || isManual[xPos, yPos]))
        {
            return false; // no match can be made from here... quit~
        }
        int mType = gm.board[xPos, yPos].piece.slotNum; // identifier of the current block type

        //  check columns
        for (int y = (yPos + 1); y < gm.boardHeight; y++)
        { //check the top side of the cube 
            if (gm.board[xPos, y].canBeMatched() && gm.board[xPos, y].piece.slotNum == mType)
            {
                matchingCols++; // increase linked counter
            }
            else
            {
                break; // exit loop as no more match this side...
            }
        }
        if (matchingCols > 1)
        { // if a column is matching
            if (!(yPos + 2 >= gm.boardHeight || !gm.board[xPos, yPos + 2].panel.pnd.hasStartingPiece || isManual[xPos, yPos + 2]))
            {
                gm.board[xPos, yPos + 2].createObject(gm.pieceTypes[0], gm.ranType()); // assign a new type
            }
            else if (!(yPos + 1 >= gm.boardHeight || !gm.board[xPos, yPos + 1].panel.pnd.hasStartingPiece || isManual[xPos, yPos + 1]))
            {
                gm.board[xPos, yPos + 1].createObject(gm.pieceTypes[0], gm.ranType()); // assign a new type
            }
            else
            {
                gm.board[xPos, yPos].createObject(gm.pieceTypes[0], gm.ranType()); // assign a new type
            }
            return true;
        }

        switch (gm.boardType)
        {
            case BoardType.Square: // square type pre-match criteria...
                // check rows
                for (int x = (xPos + 1); x < gm.boardWidth; x++)
                { //check the right side of the cube 
                    if (gm.board[x, yPos].canBeMatched() && gm.board[x, yPos].piece.slotNum == mType)
                    {
                        matchingRows++; // increase linked counter
                    }
                    else
                    {
                        break; // exit loop as no more match this side...
                    }
                }
                if (matchingRows > 1)
                { // if a row is matching
                    if (!(xPos + 2 >= gm.boardWidth || !gm.board[xPos + 2, yPos].panel.pnd.hasStartingPiece || isManual[xPos + 2, yPos]))
                    {
                        gm.board[xPos + 2, yPos].createObject(gm.pieceTypes[0], gm.ranType()); // assign a new type
                    }
                    else if (!(xPos + 1 >= gm.boardWidth || !gm.board[xPos + 1, yPos].panel.pnd.hasStartingPiece || isManual[xPos + 1, yPos]))
                    {
                        gm.board[xPos + 1, yPos].createObject(gm.pieceTypes[0], gm.ranType()); // assign a new type
                    }
                    else
                    {
                        gm.board[xPos, yPos].createObject(gm.pieceTypes[0], gm.ranType()); // assign a new type
                    }
                    return true;
                }
                break;
        }
        return false; // piece is ok... move to the next
    }

    // cycles through each panel type based on the "Panel Definition" scripts. any changes there will reflect here.
    public void togglePanel(int position, int val)
    {
        panelArray[position] = (panelArray[position] + val) % panelScripts.Length;
        if (panelArray[position] < 0)
        {
            panelArray[position] = panelScripts.Length - 1; // loop backwards
        }
        setDefaultStrength(position);
    }

    // set panel directly
    public void setPanel(int position, int val)
    {
        panelArray[position] = val;
        setDefaultStrength(position);
    }

    // cycles through each piece type based on the "Piece Definition" scripts. any changes there will reflect here.
    public void togglePiece(int position, int val)
    {
        pieceArray[position] = (pieceArray[position] + val) % pieceScripts.Length;
        if (pieceArray[position] < 0)
        {
            pieceArray[position] = pieceScripts.Length - 1; // loop backwards
        }
    }
    // set piece directly
    public void setPiece(int position, int val)
    {
        pieceArray[position] = val;
    }

    // cycles through each piece type based on the "Piece Definition" scripts. any changes there will reflect here.
    public void toggleColor(int position, int val)
    {
        colorArray[position] = (colorArray[position] + val) % (gm.NumOfActiveType + 1);
        if (colorArray[position] < 0)
        {
            colorArray[position] = gm.NumOfActiveType; // loop backwards
        }
    }

    void setDefaultStrength(int position)
    {
        for (int x = 0; x < panelScripts.Length; x++)
        { // search the array
            if (panelScripts[x] == panelScripts[panelArray[position]])
            { // if found the correct array
                pStrength[position] = panelScripts[x].defaultStrength; // return the associated default strength
            }
        }
    }
    // just a simple function to reset everything!
    public void resetMe()
    {
        int count = 0;
        for (int x = 0; x < gm.boardWidth; x++)
        {
            for (int y = 0; y < gm.boardHeight; y++)
            {
                panelArray[count] = 0;
                pieceArray[count] = 0;
                colorArray[count] = 0;
                colorWeight = new List<WeightedLayout>();
                setDefaultStrength(count);
            }
            count++;
        }
    }
    // just a simple function to reset all pieces to BASIC type
    public void resetPieceOnly()
    {
        int count = 0;
        for (int x = 0; x < gm.boardWidth; x++)
        {
            for (int y = 0; y < gm.boardHeight; y++)
            {
                pieceArray[count] = 0;
                count++;
            }
        }
    }
    // just a simple function to reset all piece color type to random
    public void resetColorOnly()
    {
        int count = 0;
        for (int x = 0; x < gm.boardWidth; x++)
        {
            for (int y = 0; y < gm.boardHeight; y++)
            {
                colorArray[count] = 0;
                colorWeight = new List<WeightedLayout>();
                count++;
            }
        }
    }
    // just a simple function to reset all panels to basic
    public void resetPanelOnly()
    {
        int count = 0;
        for (int x = 0; x < gm.boardWidth; x++)
        {
            for (int y = 0; y < gm.boardHeight; y++)
            {
                panelArray[count] = 0;
                setDefaultStrength(count);
                count++;
            }
        }
    }
    // just a simple function to click all panels
    public void clickAll(int val)
    {
        int count = 0;
        for (int x = 0; x < gm.boardWidth; x++)
        {
            for (int y = 0; y < gm.boardHeight; y++)
            {
                togglePanel(count, val);
                count++;
            }
        }
    }
    // just a simple function to randomize all panels
    public void randomize()
    {
        int count = 0;
        for (int x = 0; x < randomPanelCount.Length; x++)
        {
            randomPanelCount[x] = 0;
        }
        for (int x = 0; x < gm.boardWidth; x++)
        {
            for (int y = 0; y < gm.boardHeight; y++)
            {
                panelArray[count] = generateNumber(); // generate and assigns a random number
                setDefaultStrength(count);
                count++;
            }
        }
    }

    // an internal function to generate a number but also keep within the max limits
    // of the panels defined.
    int generateNumber()
    {
        int generated = Random.Range(0, panelScripts.Length);
        //Debug.Log("abcd");
        if (generated > 0)
        {
            if (Random.Range(0, 2) == 0)
            { // 1/2 chance to make a special panel
                if (randomPanelCount[generated] < randomPanelLimit[generated].randomLimit)
                {
                    randomPanelCount[generated]++;
                    return generated;
                }
            }
        }
        return 0; // if nothing happens above, return default panel
    }
}
