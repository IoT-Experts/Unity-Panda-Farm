using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary> ##################################
/// 
/// NOTICE :
/// This script is the Board class.
/// It is the board position on the screen that will control whether pieces
/// or panels appear or not. It is the the container for GamePieces and BoardPanel.
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################
/// 

// enum for board direction
public enum BoardDirection { Top, Bottom, Left, Right, BottomRight, BottomLeft, TopLeft, TopRight }

public class Board
{ // the game board as individual boxes

    public Vector3 position; // board position on the GUI (world position)
    public Vector3 localPos; // board position (local position)
    GamePiece _piece; // the hidden control variable for GamePiece
    public GamePiece piece
    { // the game piece that is linked to this board
        get { return _piece; }
        set
        {
            _piece = value;
            if (_piece != null)
            {
                _piece.master = this; // auto set the master of this piece
            }
        }
    }
    public BoardPanel panel; // the panel that is linked to this board
    public HUDPopUp scoreHUD; // as the scoreHUD reference
    public bool isFilled
    { //= false; // determines if there is a piece on this board
        get
        {
            if (piece == null || piece.pd == null) { return false; } // no piece here, is not filled
            else { return true; } // piece exist, isFilled...
        } // set { } // restricted to read-only
    }
    public PieceDefinition pd
    { // gets the embeded piece definition quickly is it exists
        get
        {
            if (isFilled) { return piece.pd; } // returns the embeded pd
            else { return null; } // no valid pd here...
        }
    }
    public bool isFalling = false; // states whether the piece is falling into position
    public bool isActive = true; // states whether it was active since the last routine check
    public bool justCreated
    { // for match create power piece so that it doesnt get destroyed instantly
        get { if (isFilled)return piece.justCreated; return false; }
        set { if (isFilled) piece.justCreated = value; }
    }
    public bool isBeingDelayed = false; // state for the board performing the gravity delay
    public bool mustWait = true; // state that the board must wait for the gravity delay
    public int[] arrayRef; // to know its array number for reference
    public GameManager gm; // this script as a reference

    // board neighbours
    public Board top, bottom, left, right, topLeft, topRight, bottomLeft, bottomRight;
    public List<Board> allNeighbourBoards = new List<Board>();

    public Board(GameManager gameManager, int[] boardPosition, Vector3 pos)
    {
        gm = gameManager;
        arrayRef = boardPosition; // to help it remember it's position in the array
        localPos = pos;
        position = gm.transform.TransformPoint(pos);

         //scoreHUD display setup
        if (JMFUtils.vm.scoreHUD != null)
        {
            GameObject scoreHUDObj = (GameObject)Object.Instantiate(JMFUtils.vm.scoreHUD);
            scoreHUDObj.transform.parent = gm.gameObject.transform;
            scoreHUDObj.transform.position = new Vector3(position.x, position.y, -5);
            scoreHUD = scoreHUDObj.GetComponent<HUDPopUp>();
            if (scoreHUD == null)
            {
                Debug.Log("scoreHUD 'HUDPopUp' script missing from prefab~!!"); // tell developers the problem
            }
        }
        else
        {
            Debug.Log("No scoreHUD prefab detected..."); // tell developers the problem
        }
    }

    // function to init all the required stuff during OnStart()
    public void init()
    {
        piece.init(); // init piece (game objects are created now...)
        panel.initPanels(); // init panels (game objects are created now...)
    }

    public void onGameStart()
    {
        // call the GameStart() for custom pieces and panels
        if (piece != null && piece.pd != null) piece.pd.onGameStart(this);
        if (panel != null && panel.pnd != null) panel.pnd.onGameStart(this);
    }

    #region neighbouring boards codes
    /// $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
    /// Neighbouring Boards codes
    /// $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

    // function to help convert the enum to the appropriate board reference
    public Board boardEnumToReference(BoardDirection bd)
    {
        switch (bd)
        {
            case BoardDirection.Top:
                return top;
            case BoardDirection.Bottom:
                return bottom;
            case BoardDirection.Left:
                return left;
            case BoardDirection.Right:
                return right;
            case BoardDirection.TopLeft:
                return topLeft;
            case BoardDirection.TopRight:
                return topRight;
            case BoardDirection.BottomLeft:
                return bottomLeft;
            case BoardDirection.BottomRight:
                return bottomRight;
            default:
                return null;
        }
    }

    // neighbouring function to get a board in this specific direction and distance
    public Board getBoardFromDirection(BoardDirection bd, int distance)
    {
        Board target = boardEnumToReference(bd); // initial 
        for (int x = 1; x < distance; x++)
        { // recursively find the board in the specified direction
            if (target == null)
            {
                break; // no board here... do not continue...
            }
            target = target.boardEnumToReference(bd);
        }
        return target;
    }

    // neighbouring function to get all boards in this specific direction
    public List<Board> getAllBoardInDirection(BoardDirection bd)
    {
        List<Board> list = new List<Board>();
        Board chain = boardEnumToReference(bd);
        while (chain != null)
        { // recursively add the boards in the specified direction
            list.Add(chain);
            chain = chain.boardEnumToReference(bd);
        }
        return list; // returns the list of boards in the direction
    }

    // function to set the neighbour of this board
    public void initNeighbourReferences()
    {
        // board position in x & y reference
        int x = arrayRef[0];
        int y = arrayRef[1];

        // null the references first
        top = bottom = left = right = topLeft = topRight = bottomLeft = bottomRight = null;

        if (y + 1 < gm.boardHeight)
        {
            top = gm.board[x, y + 1]; // top reference
            allNeighbourBoards.Add(top); // add to the neighbourlist
        }
        if (y - 1 >= 0)
        {
            bottom = gm.board[x, y - 1]; // bottom reference
            allNeighbourBoards.Add(bottom); // add to the neighbourlist
        }

        if (gm.boardType == BoardType.Square)
        { // exclusive to square types only
            if (x + 1 < gm.boardWidth)
            {
                right = gm.board[x + 1, y]; // right reference
                allNeighbourBoards.Add(right); // add to the neighbourlist
            }
            if (x - 1 >= 0)
            {
                left = gm.board[x - 1, y]; // left reference
                allNeighbourBoards.Add(left); // add to the neighbourlist
            }
        }

        // TODO diagonal square neighbours disabled in JMFP
        //		if(gm.boardType == BoardType.Square ){ // square mode references
        //			if(y-1 >= 0 && x+1 < gm.boardWidth) {
        //				bottomRight = gm.board[x+1,y-1]; // bottomRight reference
        //				allNeighbourBoards.Add(bottomRight); // add to the neighbourlist
        //			}
        //			if(y-1 >= 0 && x-1 >= 0) {
        //				bottomLeft = gm.board[x-1,y-1]; // bottomRight reference
        //				allNeighbourBoards.Add(bottomLeft); // add to the neighbourlist
        //			}
        //			if(y+1 < gm.boardHeight && x+1 < gm.boardWidth) {
        //				topRight = gm.board[x+1,y+1]; // bottomRight reference
        //				allNeighbourBoards.Add(topRight); // add to the neighbourlist
        //			}
        //			if(y+1 < gm.boardHeight && x-1 >= 0) {
        //				topLeft = gm.board[x-1,y+1]; // bottomRight reference
        //				allNeighbourBoards.Add(topLeft); // add to the neighbourlist
        //			}
        //		}

        //if(gm.boardType == BoardType.Hexagon){ // exclusive to Hexagon mode only
        //    int refY = y; // initial value of y
        //    if(x%2 == 1){ // hex correction only
        //        refY = y-1; // compensate for hex squiggly line 
        //    }
        //    if(refY >= 0 && refY < gm.boardHeight && x+1 < gm.boardWidth) {
        //        bottomRight = gm.board[x+1,refY]; // bottomRight reference
        //        allNeighbourBoards.Add(bottomRight); // add to the neighbourlist
        //    }
        //    if(refY >= 0 && refY < gm.boardHeight && x-1 >= 0) {
        //        bottomLeft = gm.board[x-1,refY]; // bottomRight reference
        //        allNeighbourBoards.Add(bottomLeft); // add to the neighbourlist
        //    }
        //    if(refY+1 >= 0 && (refY+1) < gm.boardHeight && x+1 < gm.boardWidth) {
        //        topRight = gm.board[x+1,refY+1]; // bottomRight reference
        //        allNeighbourBoards.Add(topRight); // add to the neighbourlist
        //    }
        //    if(refY+1 >= 0 && (refY+1) < gm.boardHeight && x-1 >= 0) {
        //        topLeft = gm.board[x-1,refY+1]; // bottomRight reference
        //        allNeighbourBoards.Add(topLeft); // add to the neighbourlist
        //    }
        //}
    }
    #endregion neighbouring boards codes

    /// $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
    /// Panel stuff
    /// $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$


    public void panelHit()
    {
        if (panel.gotHit())
        {
            isActive = true; // panel activity registered, set board active for checks
        }
    }

    // piece and panel splash damage call
    public void SplashDamage()
    {
        // 
        // panel splash call
        //
        if (panel.splashDamage())
        {
            isActive = true; // panel activity registered, set board active for checks
        }

        //
        // piece splash call
        //
        if (isFilled)
        {
            piece.pd.splashDamage(this); // function call ( if any )
        }
    }

    /// $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$
    /// Others
    /// $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$

    // destroy the piece in this board
    public void destroyBox()
    {
        if (!justCreated && panel.isDestructible() && isFilled && !piece.markedForDestroy)
        {
            if (piece.pd.isDestructible)
            { // valid for destroy
                if (piece.pd.performPower(arrayRef))
                { // if true, mark for delayed destroy
                    piece.markedForDestroy = true;
                }
                if (!piece.markedForDestroy)
                { // if not marked for delayed destroy, destroy immediately
                    piece.pd.splashDamage(this); // virtual function call ( if any )
                    piece.destroy(0);
                    isFalling = false;
                    panelHit(); // reduce panel durability ( if possible )
                }
            }
            else
            {
                panelHit(); // reduce panel durability ( if possible )
            }
        }
    }

    // destroy the piece in this board
    public void forceDestroyBox()
    {
        if (isFilled && !piece.markedForDestroy)
        { // valid for destroy
            if (piece.pd.performPower(arrayRef))
            { // if true, mark for delayed destroy
                piece.markedForDestroy = true;
            }
            if (!piece.markedForDestroy)
            { // if not marked for delayed destroy, destroy immediately
                piece.destroy(0);
                //				isFilled = false;
                isFalling = false;
                panelHit(); // reduce panel durability ( if possible )
            }
        }
    }

    // for external scripts to call, signify that it is time to destroy it after being delayed...
    public void destroyMarked()
    {

        if (piece != null)
        {
            piece.markedForDestroy = false;
            piece.destroy(1);
        }
        isFalling = false;
        panelHit(); // reduce panel durability ( if possible )
    }

    // converts a piece that is here to be a special piece
    public void convertToSpecial(PieceDefinition pd)
    {
        if (isFilled)
        {
            piece.pd.performPower(arrayRef); // trigger specials if any
        }
        piece.destroy(0);
        piece.specialMe(pd);
    }
    public void convertToSpecial(PieceDefinition pd, int newSlotNum)
    {
        if (isFilled)
        {
            piece.pd.performPower(arrayRef); // trigger specials if any
        }
        piece.destroy(1);
        piece.slotNum = newSlotNum;
        piece.specialMe(pd);
    }

    // converts a piece that is here to be a special piece
    public void convertToSpecialNoDestroy(PieceDefinition pd, int newSlotNum)
    {
        piece.removePiece(1);
        piece.slotNum = newSlotNum;
        piece.specialMe(pd);
    }
    // sets the piece that is here to be a special piece
    public void setSpecialPiece(PieceDefinition pd)
    {
        if (panel.pnd.hasStartingPiece)
        {
            piece.removePiece(1);
            if (pd.isSpecial)
            { // if it's a special type, define the appropriate skin
                piece.slotNum = pd.skinToUseDuringSpawn(arrayRef[0], arrayRef[1]);
            }
            piece.pd = pd; // sets the pd type
        }
    }

    // to create a new piece object when the board is new
    public void createObject(PieceDefinition pd, int skinNum)
    {
        piece = new GamePiece(pd, this, skinNum, position);
        isFalling = false;
        isActive = true;
    }

    // reset the board when no more moves
    public void reset(PieceDefinition pd, int skinNum)
    {
        if (panel.isFillable())
        { // if the panel can hold a game piece
            if (isFilled)
            {
                piece.resetMe(pd, skinNum); // reset it
            }
            else
            { // game piece was stolen by another board and the reference is wrong. create a new piece
                piece = new GamePiece(pd, this, skinNum, position);
                piece.init();
            }
            isFalling = false;
            isActive = true;
        }
    }

    public bool causesLandslideEffect()
    {
        if ((isFilled && pd.landslideEffect && panel.isFillable()
            && (isFalling || !panel.isStealable())) || panel.isSolid())
        {
            return true;
        }
        return false;
    }

    // function to determine if pieces are allowed to be stolen by other boards
    public bool allowGravity()
    {
        if (panel.isStealable() && isFilled && !isFalling && piece.pd.allowGravity)
        {
            return true;
        }
        return false;
    }
    // function to determine if this board requires a piece replacement when empty
    public bool replacementNeeded()
    {
        if (panel.allowsGravity() && !isBeingDelayed && !isFilled && !isFalling)
        {
            return true;
        }
        return false;
    }

    // call function to determine if the board is ready to be matched by 'MatchCheck()' in GameManager
    public bool canBeMatched()
    {
        if (panel.isMatchable() && !justCreated && !isFalling && isFilled
           && !piece.markedForDestroy && !piece.pd.isSpecial)
        {
            return true;
        }
        return false;
    }

    // call function to determine if the board has a match by 'eliminate prestart-match' in GameManager
    public bool preStartMatched()
    {
        if (!isFilled)
        {
            return false; // does not have a piece here... ( non-piece holding panel )
        }
        if (panel.isMatchable())
        {
            return true;
        }
        return false;
    }

    // to spawn a new object dropping out of the box by gravity
    public void spawnNew(PieceDefinition pd, Vector3 pos, float dropSpeed, int skinNum)
    {
        isActive = true;
        piece = new GamePiece(pd, this, skinNum, position - pos);
        piece.init();
        applyTweening(dropSpeed);
    }

    // spawn a new piece on the boar d itself (appear mode) which scales from small to big
    public void spawnNewAppear(PieceDefinition pd, float appearSpeed, int skinNum)
    {
        isActive = true;
        piece = new GamePiece(pd, this, skinNum, position);
        piece.init();
        LeanTween.cancel(piece.thisPiece); // cancel any active tweens on this object
        float scaleSize = 0;
        scaleSize = piece.thisPiece.transform.localScale.x;
        piece.thisPiece.transform.localScale = Vector3.zero; // appear from scale 0
        LeanTween.value(piece.thisPiece, appearTweeningSubFunction, 0f, scaleSize, appearSpeed).setOnUpdateParam(piece.thisPiece);
    }

    // the function for leanTween to scale the piece for appear mode
    void appearTweeningSubFunction(float val, object go)
    {
        ((GameObject)go).transform.localScale = new Vector3(val, val, 1);
    }

    // moves the pieces on the GUI for visual feedback
    public void applyTweening(float dropSpeed)
    {
        if (!isFilled)
        {
            isFalling = false;
            return; // likely destroyed by other powers already before it managed to tween, reset the board
        }
        piece.position = this.position; // sync the position data
        if (piece.thisPiece != null)
        {
            piece.thisPiece.GetComponent<PieceTracker>().arrayRef = arrayRef;
            LeanTween.cancel(piece.thisPiece, piece.extraEffectID);
            Vector3 movePos = position;
            movePos.z = piece.thisPiece.transform.position.z; // ensure the Z order stays when tweening
            LeanTween.move(piece.thisPiece, movePos, 0.15f);
        }
        else
        { // likely destroyed by other powers already before it managed to tween, reset the board
            isFalling = false;
        }
    }

    // special effects tweening... 
    public void applyTweeningAfterEffects(float effectSpeed, Vector3[] path)
    {
        if (isFilled && piece.thisPiece != null)
        {
            // play the visual effect
            piece.extraEffectID = LeanTween.move(piece.thisPiece, path, effectSpeed).id;
        }
        else
        { // likely destroyed by other powers already before it managed to tween, reset tshe board
            isFalling = false;
        }
    }
} // end of Board class
