using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

/// <summary> ##################################
/// 
/// NOTICE :
/// This script is the Mother of all script~!
/// Everything that happens during the game will be controlled in this script.
/// (with public references from support scripts too ofcourse.)
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################



// ---
// global access board checking enums
// ---

public enum Check { UP, DOWN, LEFT, RIGHT, TopRight, TopLeft, BottomLeft, BottomRight }; // for scenario check of match directions
public enum Gravity { UP, DOWN, LEFT, RIGHT };
//public enum BoardType{ Square, Hexagon};
public enum BoardType { Square };
public enum SquareMode { CrossType, Box9x9Type };
//public enum NewPieceMethod { Appear, FallByGravity };
public enum NewPieceMethod { FallByGravity };
public enum GameState { GamePending, GameActive, GameFinalizing, GameOver };

// special pieces
public enum PowerType { NONE, POWH, POWV, POWT, POW5, POW6 };

[RequireComponent(typeof(CustomAnimations), typeof(BoardLayout), typeof(WinningConditions))]
[RequireComponent(typeof(VisualManager), typeof(VisualizedGrid))]
public class GameManager : MonoBehaviour
{

    // ===========================
    // GLOBAL VARIABLES
    // ===========================

#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The type of board you want to use for this game.")]
#endif
    public BoardType boardType = BoardType.Square;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("If you have the original 'PoolManager' asset, and want to use the integrated pooling feature.")]
#endif
    public bool usingPoolManager = false;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The current board's Width in boxes")]
#endif
    [Range(1, 20)]
    public int boardWidth = 4;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The current board's Height in boxes")]
#endif
    [Range(1, 20)]
    public int boardHeight = 4;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The current board's display size (visible in the Scene View if 'Show Grid' is enabled)")]
#endif
    public float size = 4; // the size we want the board to be
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The amount of spacing between each board box. (Does not affect board size)")]
#endif
    [Range(0.0f, 100.0f)]
    public float spacingPercentage = 0f; // the percentage of spacing user wants
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The amount of padding you want for the pieces inside the board box")]
#endif
    [Range(0.0f, 100.0f)]
    public float paddingPercentage = 20f; // the percentage of padding user wants
    [HideInInspector]
    public float boxPadding = 0; // the padding in each box **updated during "Awake()"
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("Visualize Grid : shows Corners of the board in the scene view.")]
#endif
    public bool showCorners = false;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("Visualize Grid : shows the Grids of the board in the scene view.")]
#endif
    public bool showGrid = false;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("Visualize Grid : shows the padded Tiles for the pieces of the board in the scene view.")]
#endif
    public bool showPaddedTile = false;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("Visualize Grid : shows the extra information relating to the board grid in the scene view")]
#endif
    public bool showToolTips = false;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The number of active colors in the game. (For non-special pieces)")]
#endif
    [Range(1, 9)]
    public int NumOfActiveType = 3; // remember not to exceed the normalPieces array~!
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("Helps eliminate pre-start game matches. (works best with minimum 3 active types)")]
#endif
    public bool eliminatePreStartMatch = false;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("if Enabled, Players can only swipe when the board has settled during the last move.")]
#endif
    public bool moveOnlyAfterSettle = false; // must the player wait for board to settle before next move?
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("If Enabled, each move the player takes resets the current Combo.")]
#endif
    public bool movingResetsCombo = true;// player moving will reset the combo?

#if UNITY_4_5 || UNITY_4_6
	[Tooltip("How the new pieces will be created after being destroyed.")]
#endif
    public NewPieceMethod newPieceMode = NewPieceMethod.FallByGravity;
    // appear type...
#if UNITY_4_5 || UNITY_4_6
		[Tooltip("The delay before the new pieces start appearing in 'Appear Mode'")]
#endif
    public float appearModeDelay = 0.6f;
#if UNITY_4_5 || UNITY_4_6
		[Tooltip("How fast the new piece will appear in 'Appear Mode' after the delay.")]
#endif
    public float appearModeSpeed = 0.8f;

    // gravity type...
#if UNITY_4_5 || UNITY_4_6
		[Tooltip("if Enabled, the pieces will be delayed (by the specified amount) before any gravity call")]
#endif
    public bool delayedGravity = true; // delay before a piece drops when there's an empty space
#if UNITY_4_5 || UNITY_4_6
		[Tooltip("The amount of delay before gravity takes affect for each individual piece.")]
#endif
    public float gravityDelayTime = 0.3f; // the delay in float seconds
#if UNITY_4_5 || UNITY_4_6
		[Tooltip("How fast the pieces will drop to the next board box.")]
#endif
    public float gravityDropSpeed = 0.25f;
#if UNITY_4_5 || UNITY_4_6
		[Tooltip("Makes the pieces drop faster the longer the distance to the bottom.")]
#endif
    public bool acceleratedVelocity = true; // drop pieces fall faster if it need to cover more distance
#if UNITY_4_5 || UNITY_4_6
		[Tooltip("Give an extra effect when pieces reach the bottom of the box (hardcoded effect)")]
#endif
    public bool pieceDropExtraEffect = true;
#if UNITY_4_5 || UNITY_4_6
		[Tooltip("The current gravity direction for the board.")]
#endif
    public Gravity currentGravity = Gravity.DOWN; // initial gravity of the game
    Vector3 gravityVector = new Vector3(); // gravity in vector3
    BoardDirection[] bd = null; // the direction array for landslide

#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The speed for the next Match-Check by the Game Engine")]
#endif
    public float matchCheckSpeed = 0.2f;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The update speed for the Game Engine (the routine checks of Gravity & possible moves)")]
#endif
    public float gameUpdateSpeed = 0.2f;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The amount of delay before the board initiates a board reset when no more moves are available.")]
#endif
    public float noMoreMoveResetTime = 2f;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The amount of delay before the board shows the player a legal suggested move.")]
#endif
    public float suggestionTimer = 5f;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The speed the pieces will switch with each other.")]
#endif
    public float gemSwitchSpeed = 0.2f;


    // pieces & panels prefabs
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The reference for the PieceManager Object.")]
#endif
    public GameObject pieceManager;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("The reference for the PanelManager Object.")]
#endif
    public GameObject panelManager;
    [HideInInspector]
    public PieceDefinition[] pieceTypes;
    [HideInInspector]
    public PanelDefinition[] panelTypes;

    public Board[,] board; // the board array

    // scoring stuff
    [HideInInspector]
    public long score = 0;
    [HideInInspector]
    public int currentCombo = 0;
    [HideInInspector]
    public int maxCombo = 0;
    [HideInInspector]
    public ComboPopUp comboScript;
    [HideInInspector]
    public int moves = 0;
    [HideInInspector]
    public int[] matchCount = new int[9];

    // suggestion variables
    [HideInInspector]
    public bool checkedPossibleMove = false;
    [HideInInspector]
    public bool isCheckingPossibleMoves = false;
    List<GameObject> suggestedPieces = new List<GameObject>(); // to hold all the possible moves
    List<Board> suggestedBoards = new List<Board>(); // to hold all the possible moves
    Vector3 pieceOriginalSize;
    [HideInInspector]
    public bool canMove = true; // switch to determine if player can make the next move

    [HideInInspector]
    public CustomAnimations animScript;

    // environment control variable
    [HideInInspector]
    public GameState gameState = GameState.GameActive;

    //bool useItem2 = false;


    // ================================================
    // ENGINE FUNCTIONS
    // ================================================


    #region Easy Access Functions
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    // Easy Access FUNCTIONS
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    // an easy access function to call the board from an int-array
    public Board iBoard(int[] arrayRef)
    {
        return board[arrayRef[0], arrayRef[1]];
    }

    public Vector3 getBoardPosition(int[] boardPosition)
    { // OVERLOAD METHOD for int array
        return board[boardPosition[0], boardPosition[1]].position;
    }

    public Vector3 getBoardPosition(int x, int y)
    { // OVERLOAD METHOD for int x & y
        return board[x, y].position;
    }
    #endregion Easy Access Functions

    #region pre-game functions
    void preGameSetup()
    {
        // call the board panels preGameSetup...
        GetComponent<BoardLayout>().setupGamePanels();

        // call the board piece preGameSetup...
        GetComponent<BoardLayout>().setupGamePieces();
    }

    #endregion pre-game functions

    #region Misc Functions
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    // Misc. functions
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    // start game preparation
    void initializeGame()
    {

        boxPadding = 1f - (paddingPercentage / 100); // set the padding value

        pieceTypes = pieceManager.GetComponents<PieceDefinition>();
        panelTypes = panelManager.GetComponents<PanelDefinition>();


        // support sub-scripts initialization
        //audioScript = GetComponent<AudioPlayer>();
        animScript = GetComponent<CustomAnimations>();

        // creates a 2D board
        board = new Board[boardWidth, boardHeight];

        //
        // loop to create the board with blocks
        //

        for (int x = 0; x < boardWidth; x++)
        {
            // for the board height size
            for (int y = 0; y < boardHeight; y++)
            {
                // create board centralized to the game object in unity
                Vector3 pos = new Vector3(x - (boardWidth / 2.0f) + 0.5f, y - (boardHeight / 2.0f) + 0.5f, 0);
                board[x, y] = new Board(this, new int[2] { x, y }, pos * size);
                //place a cube here to start with...
                board[x, y].createObject(pieceTypes[0], ranType());
            }
        }

        foreach (Board _board in board)
        {
            _board.initNeighbourReferences();
        }
    }

    // the gravity check as a function call - to keep the updater() neat
    void gravityCheck()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                dropPieces(x, y);
            }
        }
    }

    // primarily for the suggestion functions... but you can do other stuff when the board change as you like...
    public void notifyBoardHasChanged()
    {
        checkedPossibleMove = false; // board has changed, will check possible moves again

        // clears the suggestion animation if any
        StopCoroutine("suggestPiece");
    }

    // increase the combo counter & display to GUI(dont worry, combo is reset elsewhere)
    public void increaseCombo()
    {
        // increase combo count!
        currentCombo += 1;

        JMFRelay.onCombo();

        // relay to the combo script
        if (currentCombo > 1 && comboScript != null)
        { // only show if 2 or more combo
            comboScript.StopCoroutine("displayCombo");
            comboScript.StartCoroutine("displayCombo", currentCombo);
        }

        if (maxCombo < currentCombo)
        {
            maxCombo = currentCombo; // just to keep track of the max combo
        }
    }

    // OVERLOAD FUNCTION for increaseScore
    public void increaseScore(int num, int[] arrayRef)
    {
        increaseScore(num, arrayRef[0], arrayRef[1]);
    }

    // increase the score counter (for external scripts to update)
    public void increaseScore(int num, int x, int y)
    {
        num = JMFRelay.onScoreIssue(num, x, y); // relay call for modified score
        if (currentCombo > 0)
        {
            num = (int)(num * (1.5 + (currentCombo / 10.0))); // increase with multiplier from combo
        }

        if (JMFUtils.vm.displayScoreHUD && board[x, y].scoreHUD != null)
        { // display the HUD?
            board[x, y].scoreHUD.display(num);
        }
        score += num; // add to the game score
    }

    // OVERLOAD METHOD for destroyInTime
    public void destroyInTime(int[] arrayRef, float delay, int mScore)
    {
        destroyInTime(iBoard(arrayRef), delay, mScore);
    }

    // OVERLOAD METHOD for destroyInTime
    public void destroyInTime(int x, int y, float delay, int mScore)
    {
        destroyInTime(board[x, y], delay, mScore);
    }

    // OVERLOAD METHOD for destroyInTime
    public void destroyInTime(Board _board, float delay, int mScore)
    {
        StartCoroutine(destroyInTimeRoutine(_board, delay, mScore));
    }

    // OVERLOAD METHOD for destroyInTime
    public void destroyInTime(GamePiece gp, float delay, int mScore)
    {
        StartCoroutine(destroyInTimeRoutine(gp.master, delay, mScore));
    }

    // destroys the box after a given time so that it looks cooler
    IEnumerator destroyInTimeRoutine(Board _board, float delay, int mScore)
    {
        if (_board.isFilled && _board.piece.markedForDestroy)
        { // ignore those marked for destroy
            yield break; // do not continue... it is already marked
        }

        yield return new WaitForSeconds(delay); // wait for it...

        if (_board.isFilled && _board.piece.pd.isDestructible)
        {
            increaseScore(mScore, _board.arrayRef[0], _board.arrayRef[1]); // add to the score
        }
        _board.destroyBox();
        if (!_board.panel.isDestructible())
        { // if the panel is NOT a solid type with no piece to destroy...
            _board.panelHit(); // got hit by power attack~!
        }
    }

    // OVERLOAD METHOD for destroyInTimeMarked
    public void destroyInTimeMarked(int[] arrayRef, float delay, int mScore)
    {
        destroyInTimeMarked(arrayRef[0], arrayRef[1], delay, mScore);
    }
    // OVERLOAD METHOD for destroyInTimeMarked
    public void destroyInTimeMarked(GamePiece gp, float delay, int mScore)
    {
        destroyInTimeMarked(gp.master.arrayRef[0], gp.master.arrayRef[1], delay, mScore);
    }
    // OVERLOAD METHOD for destroyInTimeMarked
    public void destroyInTimeMarked(Board _board, float delay, int mScore)
    {
        destroyInTimeMarked(_board.arrayRef[0], _board.arrayRef[1], delay, mScore);
    }
    // OVERLOAD METHOD for destroyInTimeMarked
    public void destroyInTimeMarked(int x, int y, float delay, int mScore)
    {
        StartCoroutine(destroyInTimeMarkedRoutine(x, y, delay, mScore));
    }

    // destroys the box after a given time so that it looks cooler - object being marked for delayed destruction
    IEnumerator destroyInTimeMarkedRoutine(int x, int y, float delay, int mScore)
    {
        if (!board[x, y].isFilled)
        {
            board[x, y].isFalling = false;
            yield break;
        }

        // save the piece reference
        GamePiece refPiece = board[x, y].piece;

        if (refPiece.markedForDestroy)
        {
            yield break; // do not continue as it is already marked...
        }

        // mark the piece as to be destroyed later
        refPiece.markedForDestroy = true;
        refPiece.thisPiece.GetComponent<PieceTracker>().enabled = false; // no longer movable

        yield return new WaitForSeconds(delay); // wait for it...

        if (refPiece.master.isFilled)
        {
            increaseScore(mScore, refPiece.master.arrayRef[0], refPiece.master.arrayRef[1]); // add to the score
        }

        refPiece.master.destroyMarked();

        if (!refPiece.master.panel.isDestructible())
        { // if the panel is a solid type with no piece to destroy...
            refPiece.master.panelHit(); // got hit by power attack~!
        }
    }

    // function call for the ieNumerator version
    public void lockJustCreated(int x, int y, float time)
    {
        StartCoroutine(lockJustCreatedRoutine(x, y, time));
    }

    // function to lock a piece from being destroyed with a cooldown timer
    IEnumerator lockJustCreatedRoutine(int x, int y, float time)
    {
        // lock the piece so that it isnt destroyed so fast
        GamePiece refPiece = null;
        if (board[x, y].isFilled)
        {
            refPiece = board[x, y].piece;
            refPiece.justCreated = true;
            refPiece.master.isActive = false;
            yield return new WaitForSeconds(time); // wait for it...
            // un-lock the piece again
            refPiece.justCreated = false;
            refPiece.master.isActive = true;
        }
    }

    // OVERLOADED Method for mergePieces (non ieNumerator
    public void mergePieces(int[] arrayRef1, int[] arrayRef2, bool both)
    {
        StartCoroutine(mergePiecesRoutine(arrayRef1[0], arrayRef1[1], arrayRef2[0], arrayRef2[1], both));
    }
    // OVERLOADED Method for mergePieces (non ieNumerator
    public void mergePieces(int posX1, int posY1, int posX2, int posY2, bool both)
    {
        StartCoroutine(mergePiecesRoutine(posX1, posY1, posX2, posY2, both));
    }

    // tween the merging piece ( mostly for gui effect only to show something is happening...)
    IEnumerator mergePiecesRoutine(int posX1, int posY1, int posX2, int posY2, bool both)
    {
        // freeze the boxes involved
        board[posX1, posY1].isFalling = true;
        board[posX2, posY2].isFalling = true;

        // switch the two pieces around in memory (not visual in GUI yet)
        GamePiece holder = board[posX1, posY1].piece;
        board[posX1, posY1].piece = board[posX2, posY2].piece;
        board[posX2, posY2].piece = holder;

        // since the pieceTracker info
        board[posX1, posY1].piece.thisPiece.GetComponent<PieceTracker>().arrayRef = board[posX1, posY1].arrayRef;
        board[posX2, posY2].piece.thisPiece.GetComponent<PieceTracker>().arrayRef = board[posX2, posY2].arrayRef;

        // tween it ( now only visual in GUI)
        if (both)
        {
            board[posX1, posY1].applyTweening(gemSwitchSpeed); // two sided if u want, else disabled
        }
        board[posX2, posY2].applyTweening(gemSwitchSpeed); // one sided tweening

        yield return new WaitForSeconds(gemSwitchSpeed); // the timer

        // un-freeze the boxes involved
        board[posX1, posY1].isFalling = false;
        board[posX2, posY2].isFalling = false;
    }

    // OVERLOADED function for splashFromHere
    public void splashFromHere(int[] arrayRef)
    {
        splashFromHere(arrayRef[0], arrayRef[1]);
    }

    // helper function - called by matchType class to splash damage to its neighbouring boards
    public void splashFromHere(int x, int y)
    {
        foreach (Board _board in board[x, y].allNeighbourBoards)
        {
            _board.SplashDamage(); // splash all neighbour boards
        }
    }

    // resets the board due to no more moves
    IEnumerator resetBoard()
    {
        animScript.doAnim(animType.NOMOREMOVES, 0, 0);
        JMFRelay.onNoMoreMoves();
        yield return new WaitForSeconds(noMoreMoveResetTime);
        notifyBoardHasChanged(); // reset the board status
        // for the board width size
        for (int x = 0; x < boardWidth; x++)
        {
            // for the board height size
            for (int y = 0; y < boardHeight; y++)
            {
                //reset the pieces with a random type..
                board[x, y].reset(pieceTypes[0], ranType());
            }
        }
        JMFRelay.onComboEnd();
        JMFRelay.onBoardReset();
        isCheckingPossibleMoves = false;
    }

    // used to determine the number of unfilled board boxes beyond the current panel
    // limited by panels that pieces cannot pass through
    public int countUnfilled(int x, int y, bool ignoreTotalCount)
    { // extra function currently un-used by GameManager...
        int count = 0;
        switch (currentGravity)
        {
            case Gravity.UP:
                for (int cols = y + 1; cols < boardHeight; cols++)
                {
                    if (board[x, cols].replacementNeeded())
                    {
                        count++;
                        if (ignoreTotalCount) return count; // performance saver, reduce redundant check
                    }
                    if (!board[x, cols].panel.isStealable())
                    {
                        break; // do not check further as it cannot pass through here
                    }
                }
                break;
            case Gravity.DOWN:
                for (int cols = y - 1; cols >= 0; cols--)
                {
                    if (board[x, cols].replacementNeeded())
                    {
                        count++;
                        if (ignoreTotalCount) return count; // performance saver, reduce redundant check
                    }
                    if (!board[x, cols].panel.isStealable())
                    {
                        break; // do not check further as it cannot pass through here
                    }
                }
                break;
            case Gravity.RIGHT:
                for (int rows = x + 1; rows < boardWidth; rows++)
                {
                    if (board[rows, y].replacementNeeded())
                    {
                        count++;
                        if (ignoreTotalCount) return count; // performance saver, reduce redundant check
                    }
                    if (!board[rows, y].panel.isStealable())
                    {
                        break; // do not check further as it cannot pass through here
                    }
                }
                break;
            case Gravity.LEFT:
                for (int rows = x - 1; rows >= 0; rows--)
                {
                    if (board[rows, y].replacementNeeded())
                    {
                        count++;
                        if (ignoreTotalCount) return count; // performance saver, reduce redundant check
                    }
                    if (!board[rows, y].panel.isStealable())
                    {
                        break; // do not check further as it cannot pass through here
                    }
                }
                break;
        }
        return count;
    }

    /// used to determine the number of unfilled board boxes beyond the current panel
    // limited by panels that block gravity
    public int countBlockedUnfilled(int x, int y, bool ignoreTotalCount)
    {
        int count = 0;
        if (!board[x, y].panel.isStealable())
        {
            return count; // cannot proceed.. distance = 0
        }
        switch (currentGravity)
        {
            case Gravity.UP:
                for (int cols = y + 1; cols < boardHeight; cols++)
                {
                    if (board[x, cols].replacementNeeded())
                    {
                        count++;
                        if (ignoreTotalCount && count > 0) return count; // performance saver, reduce redundant check
                    }
                    if (!board[x, cols].panel.allowsGravity() || !board[x, cols].panel.isStealable())
                    {
                        break; // do not check further as it cannot pass through here
                    }
                    if (!board[x, cols].panel.pnd.hasStartingPiece) count--;
                }
                break;
            case Gravity.DOWN:
                for (int cols = y - 1; cols >= 0; cols--)
                {
                    if (board[x, cols].replacementNeeded())
                    {
                        count++;
                        if (ignoreTotalCount && count > 0) return count; // performance saver, reduce redundant check
                    }
                    if (!board[x, cols].panel.allowsGravity() || !board[x, cols].panel.isStealable())
                    {
                        break; // do not check further as it cannot pass through here
                    }
                    if (!board[x, cols].panel.pnd.hasStartingPiece) count--;
                }
                break;
            case Gravity.RIGHT:
                for (int rows = x + 1; rows < boardWidth; rows++)
                {
                    if (board[rows, y].replacementNeeded())
                    {
                        count++;
                        if (ignoreTotalCount && count > 0) return count; // performance saver, reduce redundant check
                    }
                    if (!board[rows, y].panel.allowsGravity() || !board[rows, y].panel.isStealable())
                    {
                        break; // do not check further as it cannot pass through here
                    }
                    if (!board[rows, y].panel.pnd.hasStartingPiece) count--;
                }
                break;
            case Gravity.LEFT:
                for (int rows = x - 1; rows >= 0; rows--)
                {
                    if (board[rows, y].replacementNeeded())
                    {
                        count++;
                        if (ignoreTotalCount && count > 0) return count; // performance saver, reduce redundant check
                    }
                    if (!board[rows, y].panel.allowsGravity() || !board[rows, y].panel.isStealable())
                    {
                        break; // do not check further as it cannot pass through here
                    }
                    if (!board[rows, y].panel.pnd.hasStartingPiece) count--;
                }
                break;
        }
        return count;
    }

    // used to determine the number of unfilled board boxes in a line to fall through.
    public int emptyBoxesBeyond(int x, int y)
    { // extra function currently un-used by GameManager...
        int count = 0;
        switch (currentGravity)
        {
            case Gravity.UP:
                for (int cols = y + 1; cols < boardHeight; cols++)
                {
                    if (board[x, cols].replacementNeeded())
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            case Gravity.DOWN:
                for (int cols = y - 1; cols >= 0; cols--)
                {
                    if (board[x, cols].replacementNeeded())
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            case Gravity.RIGHT:
                for (int rows = x + 1; rows < boardWidth; rows++)
                {
                    if (board[rows, y].replacementNeeded())
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                break;
            case Gravity.LEFT:
                for (int rows = x - 1; rows >= 0; rows--)
                {
                    if (board[rows, y].replacementNeeded())
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                break;
        }
        return count;
    }

    public void playerMadeAMove()
    {
        if (movingResetsCombo) JMFRelay.onComboEnd(); // end the combo if no special override...
        moves++; // merging, so number of moves increase

        JMFRelay.onPlayerMove();
        notifyBoardHasChanged(); // notify the change~!
    }

    #endregion Misc Functions

    #region loop routine & related

    // looper for the boardCheck based on the set interval
    IEnumerator boardCheckLooper()
    {
        while (gameState != GameState.GameOver)
        {  // loop again (infinite) until game over
            yield return new WaitForSeconds(matchCheckSpeed); // wait for the given intervals
            // then check the board
            boardChecker();
        }
    }

    // status update on given intervals
    IEnumerator updater()
    {
        while (gameState != GameState.GameOver)
        {  // loop again (infinite) until game over
            gravityCheck(); // for dropping pieces into empty board box
            detectPossibleMoves(); // to make sure the game doesn't get stuck with no more possible moves
            yield return new WaitForSeconds(gameUpdateSpeed); // wait for the given intervals
        }
    }

    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    // Matcher functions
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    // Matcher - phase 1 : board block checker for potential matches
    void boardChecker()
    {
        for (int i = pieceTypes.Length - 1; i >= 0; i--)
        { // loop for each special piece + normal match 3
            for (int x = 0; x < boardWidth; x++)
            { // iterate through each board block
                for (int y = 0; y < boardHeight; y++)
                {
                    if (board[x, y].isFilled && !board[x, y].isFalling && board[x, y].isActive)
                    { // means the board block has a ready cube
                        pieceTypes[i].checkPattern(x, y, i); // check pattern based on piece definition
                    }
                    if (i == 0)
                    { // finished cycling through each matching criteria
                        board[x, y].isActive = false; // turns this block to passive
                    }
                }
            }
        }
    }

    // Matcher - phase 2 : perform clean up matches based on external script's decision
    public void validateMatch(int checkNum, int xPos, int yPos, List<Board> linkedCubesX,
                List<Board> linkedCubesY, List<Board> linkedCubesTRBL, List<Board> linkedCubesTLBR)
    {

        if (pieceTypes[checkNum].matchConditions(xPos, yPos, linkedCubesX, linkedCubesY, linkedCubesTRBL, linkedCubesTLBR))
        {

            int scorePerCube = pieceTypes[checkNum].scorePerCube;

            MusicControll.musicControll.MakeSound(MusicControll.musicControll.dropSoundFx);

            // manage the combo
            increaseCombo();

            increaseScore(scorePerCube, xPos, yPos); // give out score for the main reference piece
            // to cause a splash damage for panels that are damaged only by splash
            splashFromHere(xPos, yPos); // splash from the origin board

            foreach (Board mBoardX in linkedCubesX)
            {
                mBoardX.destroyBox(); // destroy the linked boxes too
                // to cause a splash damage for panels that are damaged only by splash
                splashFromHere(mBoardX.arrayRef);
                increaseScore(scorePerCube, mBoardX.arrayRef); // give out score
            }
            foreach (Board mBoardY in linkedCubesY)
            {
                mBoardY.destroyBox(); // destroy the linked boxes too
                // to cause a splash damage for panels that are damaged only by splash
                splashFromHere(mBoardY.arrayRef);
                increaseScore(scorePerCube, mBoardY.arrayRef); // give out score
            }
            foreach (Board mBoardTRBL in linkedCubesTRBL)
            {
                mBoardTRBL.destroyBox(); // destroy the linked boxes too
                // to cause a splash damage for panels that are damaged only by splash
                splashFromHere(mBoardTRBL.arrayRef);
                increaseScore(scorePerCube, mBoardTRBL.arrayRef); // give out score
            }
            foreach (Board mBoardTLBR in linkedCubesTLBR)
            {
                mBoardTLBR.destroyBox(); // destroy the linked boxes too
                // to cause a splash damage for panels that are damaged only by splash
                splashFromHere(mBoardTLBR.arrayRef);
                increaseScore(scorePerCube, mBoardTLBR.arrayRef); // give out score
            }


            // free the memory just in case? or perhaps not neccesary for auto GC...
            linkedCubesX.Clear();
            linkedCubesY.Clear();
            linkedCubesTRBL.Clear();
            linkedCubesTLBR.Clear();
        }
    }

    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    // possible moves detector + suggestor  ( DO NOT TOUCH UNLESS NECCESSARY~! )
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    // moves detector phase 1
    void detectPossibleMoves()
    {

        //checks through each board boxes
        if (!checkedPossibleMove && !isCheckingPossibleMoves)
        {
            isCheckingPossibleMoves = true;
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    if (board[x, y].isBeingDelayed || board[x, y].isFalling || board[x, y].isActive)
                    {
                        isCheckingPossibleMoves = false;
                        return; // do not continue, wait for board to clear and stabilize
                    }
                }
            }
            checkedPossibleMove = true; // once we checked, no need to check again until needed

            JMFRelay.onBoardStabilize();

            suggestedBoards.Clear(); // remove any outstanding suggested boards...
            foreach (Board _board in board)
            {
                if (_board.isFilled && _board.panel.isSwitchable() &&
                   checkNeighbourMatch(_board, _board.piece.slotNum))
                {
                    // recognize possible moves and save the piece location
                    suggestedBoards.Add(_board);
                }
            }

            if (suggestedBoards.Count == 0)
            { // no more possible moves
                StartCoroutine(resetBoard()); // reset board in co-routine mode for delayed event
            }
            else
            {
                // suggest the found possible move to player
                suggestedPieces.Clear(); // clear the current list

                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                // to show all the suggested piece..
                //				foreach(Board _board in suggestedBoards){
                //					suggestedPieces.Add(_board.piece.thisPiece); // add the new chain to the list
                //				}
                // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

                // ====================================
                // to show a random suggested piece...
                suggestedPieces.Add(suggestedBoards[Random.Range(0, suggestedBoards.Count)].piece.thisPiece);
                // ====================================

                suggestedBoards.Clear(); // remove stored memory
                pieceOriginalSize = suggestedPieces[0].transform.localScale; // remember the current size
                StartCoroutine("suggestPiece"); // its a string coroutine so that we can use StopCoroutine!
                isCheckingPossibleMoves = false;
            }

        }
    }

    // moves detector sub-routine phase 2-a - check its surroundings
    bool checkNeighbourMatch(Board temp, int type)
    {

        // this piece is a power piece that can be merged?
        if (!(temp.piece.pd is NormalPiece))
        {
            if (specialToPosition(temp.top, temp) || specialToPosition(temp.bottom, temp) ||
                   specialToPosition(temp.left, temp) || specialToPosition(temp.right, temp))
            {
                return true; // can special merge 
            }
        }

        if (temp.piece.pd.isSpecial)
        {
            return false; // piece cannot match normally.. return
        }

        if (checkThisPosition(temp.top, type, Check.UP) || checkThisPosition(temp.bottom, type, Check.DOWN) ||
               checkThisPosition(temp.left, type, Check.LEFT) || checkThisPosition(temp.right, type, Check.RIGHT))
        {
            return true; // can special merge 
        }
        return false; // if it reaches here, means no match if this piece moved...
    }

    // moves detector sub-routine phase 2-b - can this piece move here to special merge?
    public bool specialToPosition(Board _board, Board origin)
    {

        if (_board == null || !_board.isFilled || _board.isFalling ||
           !_board.panel.isSwitchable())
        { // no board / the piece cannot move here, quit ~!
            return false;
        }
        int x1 = origin.arrayRef[0]; int y1 = origin.arrayRef[1];
        int x2 = _board.arrayRef[0]; int y2 = _board.arrayRef[1];

        if (origin.pd.powerMatched(x1, y1, x2, y2, false, origin.pd, _board.pd) ||
           _board.pd.powerMatched(x1, y1, x2, y2, false, _board.pd, origin.pd) ||
           origin.pd.powerMatched(origin.arrayRef, _board.arrayRef, false, origin.piece, _board.piece) ||
           _board.pd.powerMatched(origin.arrayRef, _board.arrayRef, false, _board.piece, origin.piece))
        {
            return true; // is a power piece combo
        }
        return false; // not a power piece combo
    }

    // OVERLOADED Method for checkThisPosition
    bool checkThisPosition(Board _board, int type, Check dir)
    {
        if (_board != null && _board.isFilled)
        {
            return checkThisPosition(_board.arrayRef[0], _board.arrayRef[1], type, dir);
        }
        return false;
    }

    // moves detector sub-routine phase 2-c - scenario when this piece is moved in this direction
    bool checkThisPosition(int xPos, int yPos, int mType, Check dir)
    {

        if (xPos < 0 || xPos >= boardWidth || yPos < 0 || yPos >= boardHeight)
        {
            return false; // assumption is out of bounds ... stop this check
        }
        if (!board[xPos, yPos].isFilled || !board[xPos, yPos].panel.isSwitchable())
        { // the piece cannot move here, quit too~!
            return false;
        }

        // count of possible matching blocks
        int count = 0;

        // up & down check...
        if (dir != Check.UP)
        {
            for (int y = (yPos - 1); y >= 0; y--)
            { //check the bottom side of the cube
                if (board[xPos, y].canBeMatched() && board[xPos, y].piece.slotNum == mType)
                {
                    count++; // increase linked counter
                }
                else
                {
                    break; // exit loop as no more match this side...
                }
            }
        }
        if (dir != Check.DOWN)
        {
            for (int y = (yPos + 1); y < boardHeight; y++)
            { //check the top side of the cube 
                if (board[xPos, y].canBeMatched() && board[xPos, y].piece.slotNum == mType)
                {
                    count++; // increase linked counter
                }
                else
                {
                    break; // exit loop as no more match this side...
                }
            }
        }
        if (count > 1)
        { // there is a matching row...
            return true; // no need to go further as there is already a possible match
        }
        else
        {
            count = 0; // reset count for column matching...
        }

        if (dir != Check.RIGHT)
        {
            for (int x = (xPos - 1); x >= 0; x--)
            { //check the left side of the cube 
                if (board[x, yPos].canBeMatched() && board[x, yPos].piece.slotNum == mType)
                {
                    count++; // increase linked counter
                }
                else
                {
                    break; // exit loop as no more match this side...
                }
            }
        }

        if (dir != Check.LEFT)
        {
            for (int x = (xPos + 1); x < boardWidth; x++)
            { //check the right side of the cube 
                if (board[x, yPos].canBeMatched() && board[x, yPos].piece.slotNum == mType)
                {
                    count++; // increase linked counter
                }
                else
                {
                    break; // exit loop as no more match this side...
                }
            }
        }
        if (count > 1)
        { // there is a matching row...
            return true; // no need to go further as there is already a possible match
        }
        else
        {
            return false; // reset count for column matching...
        }
    }

    // suggest a piece after a given time...
    IEnumerator suggestPiece()
    {
        yield return new WaitForSeconds(suggestionTimer); // wait till it's time
        if (gameState != GameState.GameActive)
        {
            yield break; // game no longer active... do not display suggestion...
        }
        foreach (GameObject go in suggestedPieces)
        {
            if (go == null || !go.activeSelf)
            {
                notifyBoardHasChanged(); // something changed... perform checks again!
                yield break;
            }
            float currentSize = pieceOriginalSize.x;
            // main scaler loop
            LeanTween.value(go, suggestPieceScaler, currentSize * 0.75f, currentSize * 1.25f, 1f)
                .setLoopPingPong().setOnUpdateParam(go);
            // sub rotate loop
            go.transform.localEulerAngles = new Vector3(0, 0, 340f);
            LeanTween.rotateZ(go, 20f, 0.8f).setLoopPingPong();
        }
    }

    // the function for leanTween to scale the suggested pieces
    void suggestPieceScaler(float val, object go)
    {
        if (checkedPossibleMove)
        {
            ((GameObject)go).transform.localScale = new Vector3(val, val, 1); // scale to value
        }
        else
        {
            LeanTween.cancel((GameObject)go); // cancel all tweens
            ((GameObject)go).transform.localScale = pieceOriginalSize; // resets scale to normal
            ((GameObject)go).transform.localEulerAngles = Vector3.zero; // resets rotate to normal
            JMFUtils.autoScalePadded((GameObject)go); // as a precaution to reset size
        }
    }

    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    // Board Piece position Fall by gravity function ( DO NOT TOUCH UNLESS NECCESSARY~! )
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    // (main gravity function)
    public void dropPieces(int x, int y)
    {
        if (!(x >= 0 && x < boardWidth && y >= 0 && y < boardHeight))
        {
            return; // index out of bounds... do not continue~!
        }
        if (board[x, y].replacementNeeded())
        {
            board[x, y].isBeingDelayed = true; // status to verify that board already active in drop sequence
            StartCoroutine(movePieces(x, y)); // coroutine that can be delayed
        }
    }

    IEnumerator appearModePieces(int x, int y)
    {
        board[x, y].isFalling = true;
        yield return new WaitForSeconds(appearModeDelay); // wait for the delay..
        // for custom pieces spawn rate
        PieceDefinition spawned;
        for (int w = 0; w < pieceTypes.Length; w++)
        {
            spawned = pieceTypes[w].chanceToSpawnThis(x, y);
            Debug.Log(spawned.name);
            if (spawned != null)
            {
                board[x, y].spawnNewAppear(spawned, appearModeSpeed, spawned.skinToUseDuringSpawn(x, y));
                break;
            }
            if (w == pieceTypes.Length - 1)
            {
                // reached the end, no custom spawn... spawn the default
                board[x, y].spawnNewAppear(pieceTypes[0], appearModeSpeed, ranType());
            }
        }
        notifyBoardHasChanged(); // board structure changed, so notify the change~!
        yield return new WaitForSeconds(appearModeSpeed); // wait for appear mode speed
        board[x, y].isFalling = false;
        board[x, y].isBeingDelayed = false; // reset status once delay is over
    }

    // secondary gravity function as a coroutine for delay ability
    IEnumerator movePieces(int x, int y)
    {
        if (delayedGravity && board[x, y].mustWait)
        { // if delay is required by GameManager or by board
            yield return new WaitForSeconds(gravityDelayTime); // delay time between each dropped pieces
        }
        board[x, y].mustWait = false; // change status of board to drop other pieces without delay
        board[x, y].isBeingDelayed = false; // reset status once delay is over

        Board tBoard = null;

        switch (currentGravity)
        {
            case Gravity.DOWN:
                gravityVector = new Vector3(0, -size, 0); // gravity in vector3
                tBoard = board[x, y].top;
                bd = new BoardDirection[]{BoardDirection.Left,
					BoardDirection.Right,BoardDirection.Bottom};
                break;
            case Gravity.UP:
                gravityVector = new Vector3(0, size, 0); // gravity in vector3
                tBoard = board[x, y].bottom;
                bd = new BoardDirection[]{BoardDirection.Left,
					BoardDirection.Right,BoardDirection.Top};
                break;
            case Gravity.LEFT:
                gravityVector = new Vector3(-size, 0, 0); // gravity in vector3
                tBoard = board[x, y].right;
                bd = new BoardDirection[]{BoardDirection.Top,
					BoardDirection.Bottom,BoardDirection.Left};
                break;
            case Gravity.RIGHT:
                gravityVector = new Vector3(size, 0, 0); // gravity in vector3
                tBoard = board[x, y].left;
                bd = new BoardDirection[]{BoardDirection.Top,
					BoardDirection.Bottom,BoardDirection.Right};
                break;
        }

        if (tBoard == null)
        { // if board to steal from...
            StartCoroutine(spawnNew(x, y, gravityVector)); // spawn a new piece
            yield break; // finished gravity on this pass... move to the next
        }

        if (tBoard.causesLandslideEffect())
        { // check for landslide effect
            Board boardL = tBoard.boardEnumToReference(bd[0]); // the board's hypothetical left
            Board boardR = tBoard.boardEnumToReference(bd[1]); // the board's hypothetical right

            // landslide code
            if (boardL != null &&
               !boardL.causesLandslideEffect() && boardL.allowGravity())
            { // find pieces on the left
                tBoard = boardL;
            }
            else if (boardR != null &&
                    !boardR.causesLandslideEffect() && boardR.allowGravity())
            { // find pieces on the right
                tBoard = boardR;
            }

            if (tBoard != boardL && tBoard != boardR)
            { // needs to look deeper down...
                // cause landslide below as the top is blocked...
                List<Board> list = tBoard.getAllBoardInDirection(bd[2]); // list of boards hypothetical bottom

                foreach (Board boardB in list)
                {
                    if (boardB.boardEnumToReference(bd[2]) == null)
                    {
                        break; // we reached the bottom of the board... do not continue...
                    }
                    x = boardB.boardEnumToReference(bd[2]).arrayRef[0];
                    y = boardB.boardEnumToReference(bd[2]).arrayRef[1];

                    if (!board[x, y].replacementNeeded())
                    {
                        break; // if the board here already has a piece, do not continue...
                    }

                    boardL = boardB.boardEnumToReference(bd[0]); // the board's hypothetical left
                    boardR = boardB.boardEnumToReference(bd[1]); // the board's hypothetical right
                    if (boardL != null && !boardL.causesLandslideEffect()
                       && boardL.allowGravity())
                    { // find pieces on the left
                        tBoard = boardL;
                        break;
                    }
                    else if (boardR != null && !boardR.causesLandslideEffect()
                            && boardR.allowGravity())
                    { // find pieces on the right
                        tBoard = boardR;
                        break;
                    }
                }
            }
        }


        if (tBoard != null && tBoard.allowGravity())
        { // a valid target to steal a piece from...
            if (board[x, y].piece != null)
            {
                board[x, y].piece.removePiece(0); // just in case the reference is lost without removal
            }
            board[x, y].piece = tBoard.piece; // steal the piece
            tBoard.piece = null;
            StartCoroutine(animateMove(x, y)); // animate the change

            // do the same check on the board we stole from as itself needs replacement
            dropPieces(tBoard.arrayRef[0], tBoard.arrayRef[1]);
        }
    }

    // sub-function to update the board box and tween the piece due to gravity movement
    IEnumerator animateMove(int x, int y)
    {
        // update the local data...
        board[x, y].isFalling = true; // board is falling...

        int distance = countBlockedUnfilled(x, y, false);
        float delay = gravityDropSpeed;
        if (acceleratedVelocity)
        {
            delay = gravityDropSpeed / Mathf.Max(distance, 1);
        }

        board[x, y].applyTweening(delay);
        notifyBoardHasChanged(); // board structure changed, so notify the change~!

        // the timer according to the drop speed or updatespeed (whichever longer)
        yield return new WaitForSeconds(delay);

        // update the board box once animation has finished..
        board[x, y].isFalling = false; // no longer falling into position
        board[x, y].isActive = true; // piece is active for checks

        if (distance < 1)
        { // check if it has reached bottom
            board[x, y].mustWait = true; // reached bottom, re-activate gravity delay
            if (pieceDropExtraEffect)
            { // if extra effect is enabled
                board[x, y].applyTweeningAfterEffects(gravityDropSpeed, getVectorEffect(x, y));
            }
            MusicControll.musicControll.MakeSound(MusicControll.musicControll.dropSoundFx);
        }
        else
        {
            // check if this new piece needs to fall or not...
            if (board[x, y].boardEnumToReference(bd[2]) != null)
            {
                dropPieces(board[x, y].boardEnumToReference(bd[2]).arrayRef[0],
                           board[x, y].boardEnumToReference(bd[2]).arrayRef[1]);
            }
        }
    }

    // gravity effect after falling down - simulates easeInBack
    Vector3[] getVectorEffect(int x, int y)
    {

        float offset = 0.35f * size; // the amount of offset you wish for effect
        Vector3 position = board[x, y].position;
        if (board[x, y].isFilled)
        {
            position.z = board[x, y].piece.thisPiece.transform.position.z; // ensure the Z order stays when tweening
        }

        Vector3 pos;

        switch (currentGravity)
        {
            case Gravity.DOWN:
            default:
                pos = new Vector3(0f, offset, 0f);
                return new Vector3[] { position, (position - pos), position, position };
            case Gravity.UP:
                pos = new Vector3(0f, offset / 2.5f, 0f);
                return new Vector3[] { (position + pos), position, position, position };
            case Gravity.LEFT:
                pos = new Vector3(offset / 3, 0f, 0f);
                return new Vector3[] { (position - pos), position, position, position };
            case Gravity.RIGHT:
                pos = new Vector3(offset / 3, 0f, 0f);
                return new Vector3[] { (position + pos), position, position, position };
        }
    }

    // sub-function to compensate delay of a new spawned piece tweening process
    public IEnumerator spawnNew(int x, int y, Vector3 spawnPoint)
    {
        board[x, y].isFalling = true; // board is falling...

        int distance = countBlockedUnfilled(x, y, false);
        float delay = gravityDropSpeed;
        if (acceleratedVelocity)
        {
            delay = gravityDropSpeed / Mathf.Max(distance, 1);
        }

        // for custom pieces spawn rate
        PieceDefinition spawned;
        for (int w = 0; w < pieceTypes.Length; w++)
        {
            spawned = pieceTypes[w].chanceToSpawnThis(x, y);
            if (spawned != null)
            {
                board[x, y].spawnNew(spawned, spawnPoint, delay, spawned.skinToUseDuringSpawn(x, y));
                break;
            }
            if (w == pieceTypes.Length - 1)
            {
                // reached the end, no custom spawn... spawn the default
                board[x, y].spawnNew(pieceTypes[0], spawnPoint, delay, ranType());
            }
        }

        notifyBoardHasChanged(); // board structure changed, so notify the change~!

        // the timer according to the drop speed or updatespeed (whichever longer)
        yield return new WaitForSeconds(delay);
        // update the board box once animation has finished..
        board[x, y].isFalling = false;
        board[x, y].isActive = true;
        board[x, y].mustWait = true; // reached bottom, re-activate gravity delay
        if (distance < 1)
        { // check if it has reached bottom			
            if (pieceDropExtraEffect)
            { // if extra effect is enabled
                board[x, y].applyTweeningAfterEffects(gravityDropSpeed, getVectorEffect(x, y));
            }
                        MusicControll.musicControll.MakeSound(MusicControll.musicControll.dropSoundFx);
        }
        else
        {
            // check if this new piece needs to fall or not...
            if (board[x, y].boardEnumToReference(bd[2]) != null)
            {
                dropPieces(board[x, y].boardEnumToReference(bd[2]).arrayRef[0],
                           board[x, y].boardEnumToReference(bd[2]).arrayRef[1]);
            }
        }
    }

    #endregion loop routine & related

    #region PieceTracker related
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    // PieceTracker movement notifier ( DO NOT TOUCH UNLESS NECCESSARY~! )
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    // for external source call method (called from PieceTracker.cs script), 
    // this is to drag gems on the board
    public void draggedFromHere(int[] pos, int[] partner)
    {
        if (moveOnlyAfterSettle)
        {
            if (!checkedPossibleMove)
            {
                return; // player needs to wait for board to settle before making the next move...
            }
        }
        if (!canMove)
        {
            return; // if cannot move, exit~! ( player perhaps made a bad move previously )
        }

        int posX1, posY1; // the calling board position
        int posX2, posY2; // the partner board position
        posX1 = pos[0]; posY1 = pos[1]; // get the calling position
        posX2 = partner[0]; posY2 = partner[1]; // get the partner
        // extra conditioning check if pieces can be moved
        if (!(posX2 >= 0 && posX2 < boardWidth) || !(posY2 >= 0 && posY2 < boardHeight) ||
           !board[posX1, posY1].panel.isSwitchable() || !board[posX2, posY2].panel.isSwitchable() ||
            board[posX1, posY1].isFalling || !board[posX1, posY1].isFilled ||
            board[posX2, posY2].isFalling || !board[posX2, posY2].isFilled ||
            !board[posX1, posY1].piece.thisPiece.GetComponent<PieceTracker>().enabled ||
            !board[posX2, posY2].piece.thisPiece.GetComponent<PieceTracker>().enabled)
        {
            // condition above states the box are not legit selections, do not proceed!!!
            return;
        }

        PieceDefinition pdMain = board[posX1, posY1].piece.pd; // the calling piece definition
        PieceDefinition pdSub = board[posX2, posY2].piece.pd; // the partner piece definition

        //check if we are merging two power gems
        if (pdMain.powerMatched(posX1, posY1, posX2, posY2, true, pdMain, pdSub) ||
           pdSub.powerMatched(posX1, posY1, posX2, posY2, true, pdSub, pdMain) ||
           pdMain.powerMatched(board[posX1, posY1].arrayRef, board[posX2, posY2].arrayRef, true, board[posX1, posY1].piece, board[posX2, posY2].piece) ||
           pdSub.powerMatched(board[posX1, posY1].arrayRef, board[posX2, posY2].arrayRef, true, board[posX2, posY2].piece, board[posX1, posY1].piece))
        {
            playerMadeAMove(); // call the function when player makes a valid move
            return; // we are merging, so it is handled elsewhere, job done here...so, return!
        }

        // initiate the switch if the two board pieces are switchable
        StartCoroutine(switchPositions(posX1, posY1, posX2, posY2));
    }

    // tween the pieces and perform actions after the given time (to accomodate the tweening)
    public IEnumerator switchPositions(int posX1, int posY1, int posX2, int posY2)
    {
        GP_TrayItem trayItem = FindObjectOfType<GP_TrayItem>();
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.switchSoundFx);

        // freeze the boxes involved
        board[posX1, posY1].isFalling = true;
        board[posX2, posY2].isFalling = true;

        // switch the two pieces around in memory (not visual in GUI yet)
        GamePiece holder = board[posX1, posY1].piece;
        board[posX1, posY1].piece = board[posX2, posY2].piece;
        board[posX2, posY2].piece = holder;

        // tween it ( now only visual in GUI)
        board[posX1, posY1].applyTweening(gemSwitchSpeed);
        board[posX2, posY2].applyTweening(gemSwitchSpeed);

        yield return new WaitForSeconds(gemSwitchSpeed); // the timer

        // assign the type in a shorter reference just for easier usage
        int t1 = board[posX1, posY1].piece.slotNum;
        int t2 = board[posX2, posY2].piece.slotNum;

        //check use item 2;
        // extensive check to verify that the move is legit
        if (checkThisPosition(posX1, posY1, t1, Check.UP) || checkThisPosition(posX1, posY1, t1, Check.DOWN)
           || checkThisPosition(posX1, posY1, t1, Check.LEFT) || checkThisPosition(posX1, posY1, t1, Check.RIGHT)
           || checkThisPosition(posX1, posY1, t1, Check.TopLeft) || checkThisPosition(posX1, posY1, t1, Check.TopRight)
           || checkThisPosition(posX1, posY1, t1, Check.BottomLeft) || checkThisPosition(posX1, posY1, t1, Check.BottomRight)
           || checkThisPosition(posX2, posY2, t2, Check.UP) || checkThisPosition(posX2, posY2, t2, Check.DOWN)
           || checkThisPosition(posX2, posY2, t2, Check.LEFT) || checkThisPosition(posX2, posY2, t2, Check.RIGHT)
           || checkThisPosition(posX2, posY2, t2, Check.TopLeft) || checkThisPosition(posX2, posY2, t2, Check.TopRight)
           || checkThisPosition(posX2, posY2, t2, Check.BottomLeft) || checkThisPosition(posX2, posY2, t2, Check.BottomRight) || trayItem.bantayClicked)
        {
            //Debug.Log("a");
            //if legit, un- freeze the boxes involved

            board[posX1, posY1].isFalling = false;
            board[posX2, posY2].isFalling = false;

            board[posX1, posY1].isActive = true; // make the piece active for checks
            board[posX2, posY2].isActive = true; // make the piece active for checks

            playerMadeAMove();
            if (trayItem.bantayClicked)
            {
                trayItem.AllButtonNormal();
            }

            // call the function when player makes a valid move
        }
        else
        {
            // if move is not legit, revert it back
            StartCoroutine(revertMove(posX1, posY1, posX2, posY2)); // to revert the last move
        }
    }

    // to revert the actions if the last move was an invalid move
    IEnumerator revertMove(int posX1, int posY1, int posX2, int posY2)
    {
        // NOTE : remember that the boxes is still frozen... (in switchPositions() )

        MusicControll.musicControll.MakeSound(MusicControll.musicControll.badMoveSoundFx);

           
        canMove = false; // player cannot move until it is reverted
        // switch it back around...
        GamePiece holder = board[posX1, posY1].piece;
        board[posX1, posY1].piece = board[posX2, posY2].piece;
        board[posX2, posY2].piece = holder;

        // tween it ( make it visual in GUI)
        board[posX1, posY1].applyTweening(gemSwitchSpeed);
        board[posX2, posY2].applyTweening(gemSwitchSpeed);

        yield return new WaitForSeconds(gemSwitchSpeed); // the timer
        // un- freeze the boxes involved for checks
        board[posX1, posY1].isFalling = false;
        board[posX2, posY2].isFalling = false;
        board[posX1, posY1].isActive = true; // make the piece active for checks
        board[posX2, posY2].isActive = true; // make the piece active for checks

        canMove = true; // give power back to the player
    }

    #endregion PieceTracker related

    #region Helper Functions
    // ===========================
    // Helper FUNCTIONS
    // ===========================

    // random cubeType generator , just coz the code is too long
    public int ranType()
    {
        return Random.Range(0, Mathf.Min(NumOfActiveType, pieceTypes[0].skin.Length));
        // limited by normalpieces types available if numOfActiveType is declared out of bounds
    }

    // OVERLOADED FUNCTION of getBoardsDistance *range of distance Type*
    public List<Board> getBoardsFromDistance(int[] point, int distMin, int distMax)
    {
        return getBoardsFromDistance(iBoard(point), distMin, distMax);
    }
    // get all the board from a specific distance range~!
    public List<Board> getBoardsFromDistance(Board point, int distMin, int distMax)
    {
        List<Board> temp = new List<Board>();
        for (int x = distMin; x <= distMax; x++)
        {
            temp.AddRange(getBoardsFromDistance(point, x)); // add the boards within the range specified
        }
        return temp;
    }

    // OVERLOADED FUNCTION of getBoardsDistance
    public List<Board> getBoardsFromDistance(int[] point, int dist)
    {
        return getBoardsFromDistance(iBoard(point), dist);
    }

    // get all the board from a specific distance
    public List<Board> getBoardsFromDistance(Board point, int dist)
    {
        List<Board> temp = new List<Board>();

        foreach (Board itr in board)
        {
            if (boardRadiusDistance(point, itr) == dist)
            { // is on this specific distance
                temp.Add(itr); // add the board to the list
            }
        }
        return temp;
    }

    // OVERLOADED FUNCTION of boardRadiusDistance
    public int boardRadiusDistance(Board boardA, Board boardB)
    {
        return boardRadiusDistance(boardA.arrayRef, boardB.arrayRef);
    }
    // function to calculate the relative distance between two board locations
    public int boardRadiusDistance(int[] bPosA, int[] bPosB)
    {
        int totalX = Mathf.Abs(bPosA[0] - bPosB[0]);
        int totalY = Mathf.Abs(bPosA[1] - bPosB[1]);

        // TODO squareMode not available in JMFP...
        //			if(squareMode == squareMode.CrossType){ // specific for cross-type square
        //				return totalX + totalY; // each box = 1 distance... no diagonals
        //			} else {
        return Mathf.Max(totalX, Mathf.Max(totalY, Mathf.Abs(totalX - totalY)));
        //			}
    }

    #endregion Helper Functions

    #region HEXAGON related functions
    // ===========================
    // HEXAGON FUNCTIONS
    // ===========================

    // returns the unsquiggled Hexagon grid
    public int[] hexUnsquiggleArray(int[] array)
    {
        return new int[] { array[0], array[1] - array[0] + (array[0] / 2) };
    }

    // returns a vector3 array for distance calculation
    public Vector3 hexGetCalcVector(int[] array)
    {
        array = hexUnsquiggleArray(array);
        return new Vector3(array[0], array[1], (array[0] + array[1]) * -1);
    }

    #endregion HEXAGON related functions

    #region game-start sequence
    public void StartGame()
    { // when the game is actually running...
        if (gameState == GameState.GamePending)
        {
            gameState = GameState.GameActive; // change the state to active...
            // Initialize Timers and settings
            StartCoroutine(updater()); // initiate the update loop
            StartCoroutine(boardCheckLooper()); // initiate the check loop
            canMove = true; // allows player to move the pieces
            // call the gameStart for the board objects
            foreach (Board _board in board)
            {
                _board.onGameStart();
            }
            JMFRelay.onGameStart();
        }
        else
        {
            Debug.Log("Game already started... cannot start the game again!");
        }

    }
    #endregion game-start sequence

    #region Unity Functions
    // ===========================
    // UNITY FUNCTIONS
    // ===========================

    void Awake()
    { // board needs to be initialized before other scripts can access it
        JMFUtils.gm = this; // make a easy reference to the GameManager ( this script ! ) 
        JMFUtils.wc = GetComponent<WinningConditions>(); // make a easy reference to the WinningConditions script~!
        JMFUtils.vm = GetComponent<VisualManager>(); // make a easy reference to the VisualManager script~!
        transform.GetComponent<SetUpListQua>().Settup_ListQua();
        JMFRelay.onPreGameStart();
        initializeGame();
        preGameSetup();
        //Application.
       // Application.targetFrameRate = 60;
        canMove = false; // initially cannot be moved...
        gameState = GameState.GamePending; // game is waiting to be started...
    }

    void Start()
    {
        // init the board objects
        foreach (Board _board in board)
        {
            _board.init(); // inits the GUIs for the object
        }
    }

    // Update is called once per frame
    void Update()
    {
        // woohoo~ nothing here??
    }
    #endregion Unity
}
