using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;
/// <summary> ##################################
/// 
/// NOTICE :
/// This script is conditions set to win/end the current game.
/// 
/// </summary> ##################################

public class WinningConditions : MonoBehaviour
{

#if UNITY_4_5 || UNITY_4_6
	[Tooltip("Starts the game running when scene loads.\n" +
	         "call 'startThisGame()' function yourself if you have other plans.")]
#endif
    public bool startGameImmediately = true;
    public float checkSpeed = 1;
    public bool specialTheLeftovers = true;
    public float secondsPerSpecial = 5;
    public int movesPerSpecial = 5;
    public bool popSpecialsBeforeEnd = true;

#if UNITY_4_5 || UNITY_4_6
	[Space(20)] // just some seperation
#endif
    // timer game
    public bool isTimerGame = false;
    public TextMesh timeLabel;
    public TextMesh timeText;
    public float TimeGiven = 120;

#if UNITY_4_5 || UNITY_4_6
	[Space(20)] // just some seperation
#endif
    // max move game
    public bool isMaxMovesGame = false;
    public TextMesh movesLabel;
    public TextMesh movesText;
    public int allowedMoves = 40;

#if UNITY_4_5 || UNITY_4_6
	[Space(20)] // just some seperation
#endif
    // score game
    public bool isScoreGame = true;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("If enabled, player must obtain a minimum score of 'scoreToReach'")]
#endif
    public bool scoreRequiredWin = true;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("If enabled, obtaining the minimum score will trigger end-game.")]
#endif
    public bool scoreEndsGame = false;
    public int scoreToReach = 10000;
    public int scoreMilestone2 = 20000;
    public int scoreMilestone3 = 30000;
    public int[] numToGet = new int[9];

#if UNITY_4_5 || UNITY_4_6
	[Space(20)] // just some seperation
#endif
    // clear shaded game
    public bool isClearShadedGame = false;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("If enabled, player must clear all the shaded panels to win.")]
#endif
    public bool shadesRequiredWin = true;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("If enabled, clearing all the shaded panels will trigger end-game.")]
#endif
    public bool shadesEndsGame = true;
    int shadesLeft = 1;

#if UNITY_4_5 || UNITY_4_6
	[Space(20)] // just some seperation
#endif
    // get type game
    public bool isGetTypesGame = false;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("If enabled, player must get all the specified types to win.")]
#endif
    public bool typesRequiredWin = true;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("If enabled, getting all the specified types will trigger end-game.")]
#endif
    public bool typeEndsGame = true;
    public GameObject placeholderPanel;
    public GameObject textHolder;
    bool collectedAll = false;

#if UNITY_4_5 || UNITY_4_6
	[Space(20)] // just some seperation
#endif
    // treasure game
    public bool isTreasureGame = false;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("If enabled, player must all the treasures to win.")]
#endif
    public bool treasureRequiredWin = true;
#if UNITY_4_5 || UNITY_4_6
	[Tooltip("If enabled, getting all the treasures will trigger end-game.")]
#endif
    public bool treasureEndsGame = true;
    public TextMesh treasureLabel;
    public TextMesh treasureText;
    public int numOfTreasures1 = 3;
    public int numOfTreasures2 = 3;
    [HideInInspector]
    public int countSpwanTreasure1 = 3;
    [HideInInspector]
    public int countSpwanTreasure2 = 3;
    public int maxOnScreen = 2;
    [Range(0, 30)]
    public int chanceToSpawn = 10;
    public List<Vector2> treasureGoal = new List<Vector2>();
    public List<GamePiece> treasureList = new List<GamePiece>();
    [HideInInspector]
    public int treasuresCollected = 0;
    [HideInInspector]
    public int treasuresSpawned = 0;

    public GameObject GameOverMessage;
    public Animator LayerWinAnim;
    public Animator LayerGameOverAnim;

    // [HideInInspector]
    public List<GameObject> ListQuaMission;
    [HideInInspector]
    public List<Text> ListTextMesh;
    [HideInInspector]
    public List<string> lstSoluongquaComplete;
    GameManager gm;

    float timeKeeper = 0; // just an in-game timer to find out how long the round has been playing..
    bool isGameOver = false;
    //bool hetgio;
    public GameObject[] Starts;

    public ParticleSystem particleStar;

    //isBom
    public bool isFruitBom;
    public int MaxBom;
    public bool checkGameoverbom;

    // check giỏ
    public bool checkgio;
    [HideInInspector]
    public int countBom;
    GP_ConfigData configdata;
    public TextMesh txtWinScore;
    public TextMesh txtWinLevel;
    public TextMesh txtFailScore;
    public TextMesh txtFailLevel;

    public GameObject SauBom;
    public GameObject halo;
    public GameObject LayerShare;

    //public Text txtTest;

    /// <summary>
    /// 
    /// Below are properties of interest...
    /// 
    /// gm.score   <--- the current score accumulated by the player
    /// gm.moves   <--- the total moves the player has made
    /// gm.currentCombo    <--- the current combo count of any given move ( will reset to 0 each move )
    /// gm.maxCombo   <--- the max combo the player has achieved in the gaming round
    /// gm.gameState    <--- the status of the GameManager
    /// gm.checkedPossibleMove   <--- a boolean that signifies the board has stabilized from the last move
    ///                               ( use this when you want the board to stop only after finish combo-ing )
    /// gm.canMove     <--- a boolean to allow players to move the pieces. true = can move; false = cannot move.
    /// gm.board[x,y]      <--- use this to reference the board if you needed more board properties
    /// gm.notifyBoardHasChanged()    <--- to tell the board to continue checks after it has settled
    /// gm.matchCount[x]   <--- the count of the type that has been destroyed.
    /// 
    /// </summary>


    #region routines & related
    int count;
    IEnumerator updateStatus()
    {
        while (gm.gameState != GameState.GameOver)
        {// loop infinitely until game over
            // updates the status...
            if (isTimerGame)
            {
                if (timeText != null)
                {
                    if ((TimeGiven - timeKeeper) >= 0)
                    {
                        timeText.text = (TimeGiven - timeKeeper).ToString(); // outputs the time to the text label
                    }
                    else
                    {
                        timeText.text = "0"; // outputs the time to the text label
                    }
                    //movesText.transform.localScale = new Vector3(0, 0);
                }
            }
            if (isMaxMovesGame)
            {
                if (movesText != null)
                {
                    if ((allowedMoves - gm.moves) >= 0)
                    {
                        movesText.text = (allowedMoves - gm.moves).ToString(); // outputs the time to the text label
                        int a = allowedMoves - gm.moves;
                        if (a <= 5 && gm.gameState == GameState.GameActive)
                        {
                            halo.SetActive(true);
                        }
                    }
                    else
                    {
                        movesText.text = "0"; // outputs the time to the text label
                    }
                    //timeText.transform.localScale = new Vector3(0, 0);
                }

            }
            if (isClearShadedGame)
            { // updates the 'shadesLeft' variable...
                shadesLeft = 0;
                for (int x = 0; x < gm.boardWidth; x++)
                {
                    for (int y = 0; y < gm.boardHeight; y++)
                    {
                        if (gm.board[x, y].panel.pnd is ShadedPanel)
                        {
                            shadesLeft += gm.board[x, y].panel.durability + 1; // increase count as this is a shaded panel
                        }
                    }
                }
            }
            if (isGetTypesGame)
            {
                collectedAll = true;
                foreach (var item in ListTextMesh)
                {
                    if (item.text != "0")
                    {
                        collectedAll = false;
                    }
                }
            }
            // function to collect treasure as well as update the status...
            if (isTreasureGame)
            {
                foreach (Vector2 pos in treasureGoal)
                {
                    foreach (GamePiece gp in treasureList)
                    { // loop each treasure piece
                        Vector2 temp = new Vector2(gp.master.arrayRef[0], gp.master.arrayRef[1]);
                        if (temp == pos && !gp.master.isFalling)
                        {
                            if (gp.thisPiece.tag == "sau1")
                            {
                                int a = int.Parse(ListTextMesh[0].text);
                                ListTextMesh[0].text = (a - 1).ToString();
                            }
                            if (gp.thisPiece.tag == "sau2")
                            {
                                int a = int.Parse(ListTextMesh[1].text);
                                ListTextMesh[1].text = (a - 1).ToString();
                            }
                            treasuresCollected++; // increase collected count
                            gp.pd.onPieceDestroyed(gp); // the destroy call for treasure object
                            gp.removePiece(1); // destroy the treasure
                            treasureList.Remove(gp); // remove from the list
                            break;
                        }
                    }
                }
                if (treasureText != null)
                {
                    treasureText.text = ((numOfTreasures1 + numOfTreasures2) - treasuresCollected).ToString();
                }
            }
            yield return new WaitForSeconds(checkSpeed); // wait for the refresh speed
        }
    }

    IEnumerator routineCheck()
    {
        while (!isGameOver)
        {// loop infinitely until game over
            if (isTimerGame)
            {
                checkTime();
            }
            if (isMaxMovesGame)
            {
                checkMoves();
            }
            if (isScoreGame && scoreEndsGame && !isGetTypesGame && !isTreasureGame)
            {
                checkScore();
            }
            //if (isClearShadedGame && shadesEndsGame)
            //{
            //    checkShaded();
            //}
            if (isGetTypesGame && typeEndsGame)
            {
                checkNumsOfType();
            }
            if (isTreasureGame && treasureEndsGame)
            {
                checkTreasures();
            }

            if (isFruitBom)
            {
                CheckBom();
            }
            if (checkgio)
            {
                CheckGio();
            }
            // CheckBasket();
            yield return new WaitForSeconds(checkSpeed); // wait for the refresh speed
        }
    }

    IEnumerator timer()
    {
        while (!isGameOver)
        {// loop infinitely until game over
            timeKeeper++; // timer increase in time
            yield return new WaitForSeconds(1f); // ticks every second...
        }
    }


    void CheckGio()
    {
        if (checkgio)
        {
            StartCoroutine(gameOver());
        }
    }
    // function to check the time
    void checkTime()
    {
        if (TimeGiven <= timeKeeper)
        {
            StartCoroutine(gameOver());
        }
    }

    // function to compare score
    void checkScore()
    {
        if (gm.score > scoreToReach && !isGetTypesGame && !isTreasureGame)
        {
            StartCoroutine(gameOver());
        }
    }

    // function to compare moves left
    void checkMoves()
    {
        if (gm.moves >= allowedMoves)
        {
            StartCoroutine(gameOver());
        }
    }

    // function to check whether there are any shaded panels left...
    //void checkShaded()
    //{
    //    if (shadesLeft == 0)
    //    { // when no shaded panels are found, game over
    //        StartCoroutine(gameOver());
    //    }
    //}

    // function to check whether the number of types to get is reached...
    void checkNumsOfType()
    {
        if (collectedAll)
        {
            StartCoroutine(gameOver()); // collected all, initiate game over
        }
    }

    // function to check that the player has collected all treasures
    void checkTreasures()
    {
        if (treasuresCollected == (numOfTreasures1 + numOfTreasures2))
        {
            StartCoroutine(gameOver()); // collected all, initiate game over
        }
    }


    void CheckBom()
    {
        if (checkGameoverbom)
        {
            StartCoroutine(gameOver());
        }
    }
    //void CheckBasket()
    //{
    //    if (GP_TrayBasket.totalBaket <= 0 && GP_TrayBasket.SoluongBasket <= 0)
    //    {
    //        Debug.Log("bc");
    //        hetgio = true;
    //        //StartCoroutine(gameOver());
    //    }
    //}

    #endregion routines & related

    #region endgame sequence
    IEnumerator gameOver()
    {
        //GP_TrayItem trayItem = FindObjectOfType<GP_TrayItem>();
        //  Debug.Log("check game over");

        gm.canMove = false; // player not allowed to move anymore
        gm.gameState = GameState.GameFinalizing; // game in finalizing mode...
        isGameOver = true; // game over, all routine loops will be disabled
        //trayItem.LockAllItem();
        yield return new WaitForSeconds(0.3f); // wait for board to finish its routine actions
        if (specialTheLeftovers)
        {
            while (gm.checkedPossibleMove == false)
            {
                // pause here till board has finished stabilizing...
                yield return new WaitForSeconds(0.5f); // just to calm down from being so fast...
            }
            if (isTimerGame)
            {
                while (convertTime())
                { // converts time every second until no more time.
                    yield return new WaitForSeconds(0.5f);
                }
            }
            if (isMaxMovesGame)
            {
                while (convertMoves())
                { // converts moves every second until no more moves.
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
        if (popSpecialsBeforeEnd)
        { // the feature is enabled
            while (true)
            {
                while (gm.checkedPossibleMove == false)
                {
                    // pause here till board has finished stabilizing...
                    yield return new WaitForSeconds(0.3f); // just to calm down from being so fast...
                }
                if (hasRemainingSpecials())
                {

                    popASpecialPiece();
                    yield return new WaitForSeconds(gm.gameUpdateSpeed); // wait for gravity
                }
                else
                {
                    break;
                }
            }
        }
        else
        { // the feature is disabled
            while (gm.checkedPossibleMove == false)
            {
                // pause here till board has finished stabilizing...
                yield return new WaitForSeconds(1f); // just to calm down from being so fast...
            }
        }

        gm.gameState = GameState.GameOver; // stops gameManager aswell...

        StartCoroutine(validateWinLose());
    }

    IEnumerator validateWinLose()
    {
        int starStatus = 0; // just a little extra star status ( 3 star system game )
        //string starMsg = "~You Won~\n" +
        //"But didn't earn any star..."; // variable message changes according to star rating...
        bool playerWon = true; // initial state

        // check the star status...
        if (gm.score > scoreMilestone3)
        {
            starStatus = 3;
            //starMsg = "~You Won~\n" +
            //"Congrats on 3 stars~!!";
        }
        else if (gm.score > scoreMilestone2)
        {
            starStatus = 2;
            //starMsg = "~You Won~\n" +
            //	"Obtained 2 stars~!";
        }
        else if (gm.score > scoreToReach)
        {
            starStatus = 1;
            //starMsg = "~You Won~\n" +
            //	"You earned 1 star!";
        }
        for (int i = 0; i < starStatus; i++)
        {
            yield return new WaitForSeconds(0.7f);
            Starts[i].SetActive(true);
            Animator anim = Starts[i].GetComponent<Animator>();
            anim.SetTrigger("star");
            ParticleSystem a = (ParticleSystem)Instantiate(particleStar, Starts[i].transform.position, Quaternion.identity);
            a.transform.parent = Starts[i].transform;
        }

        //if (hetgio)
        //{
        //    playerWon = false;

        //    ShowLayerGameover(playerWon, starStatus);
        //}
        //if (isScoreGame && scoreRequiredWin && starStatus == 0)
        //{
        //    playerWon = false; // fail to meet minimum score...
        //}
        //if (isClearShadedGame && shadesRequiredWin && shadesLeft > 0)
        //{
        //    playerWon = false; // fail to clear all shades
        //}
        if (isGetTypesGame && typesRequiredWin && !collectedAll || isGetTypesGame && typesRequiredWin && gm.score < scoreToReach)
        {
            playerWon = false; // fail to collect all required colors/gems
        }
        if (isTreasureGame && treasureRequiredWin && ((numOfTreasures1 + numOfTreasures2) > treasuresCollected) || isGetTypesGame && typesRequiredWin && gm.score < scoreToReach)
        {
            playerWon = false; // fail to collect all treasures
        }
        if (isFruitBom && checkGameoverbom)
        {
            playerWon = false;
        }

        if (checkgio)
        {
            playerWon = false;
        }
        // game over message in the prefab
        ShowLayerGameover(playerWon, starStatus);
    }


    public void ShowLayerGameover(bool playerWon, int star)
    {
        if (GameOverMessage != null)
        {
            int level = ObscuredPrefs.GetInt("level");
            if (playerWon)
            { // player won...
                LayerWinAnim.SetTrigger("LayerWinShow");

                txtWinLevel.text = level.ToString();
                txtWinScore.text = gm.score.ToString();
                SaveLevel(star);
                MusicControll.musicControll.MakeSound(MusicControll.musicControll.winner);

            }
            else
            { // player lost...              
                MusicControll.musicControll.MakeSound(MusicControll.musicControll.gameover);
                txtFailLevel.text = level.ToString();
                txtFailScore.text = gm.score.ToString();
                LayerGameOverAnim.SetTrigger("LayerGameOverShow");
            }
            if (admodads.bannerView!=null)
            {
                admodads.showbanneradmob();
            }
            else
            {
                admodads.RequestBanner();
            }           
        }
        return;
    }
    #endregion endgame sequence

    #region other functions
    // function to convert remaining time to special pieces
    bool convertTime()
    {
        if ((TimeGiven - timeKeeper) >= 1)
        {
            randomSpecialABoard();
            timeKeeper += secondsPerSpecial; // convert every x seconds
            return true;

        }
        return false; // no more time to convert...
    }
    // function to convert remaining moves to special pieces
    bool convertMoves()
    {
        if ((allowedMoves - gm.moves) >= 1)
        {
            randomSpecialABoard();
            allowedMoves -= movesPerSpecial; // convert every x moves
            return true;
        }
        return false; // no more moves to convert...
    }

    // randomly assign a special to this board
    void randomSpecialABoard()
    {
        Board selected = getRandomBoard();
        // play audio visuals
        //gm.audioScript.convertingSpecialFx.play();
        MusicControll.musicControll.MakeSound(MusicControll.musicControll.convertingSpecialFx);
        gm.animScript.doAnim(animType.CONVERTSPEC, selected.arrayRef[0], selected.arrayRef[1]);

        // get the gameobject reference
        GameObject pm = gm.pieceManager;

        switch (Random.Range(0, 3))
        {
            case 0:
                selected.convertToSpecialNoDestroy(pm.GetComponent<HorizontalPiece>(), selected.piece.slotNum); // convert to H-type
                break;
            case 1:
                selected.convertToSpecialNoDestroy(pm.GetComponent<VerticalPiece>(), selected.piece.slotNum); // convert to V-type
                break;
            case 2:
                selected.convertToSpecialNoDestroy(pm.GetComponent<BombPiece>(), selected.piece.slotNum); // convert to T-type
                break;
        }
    }

    Board getRandomBoard()
    { // as the title sez, get a random board that is filled...
        Board selected;
        List<Board> randomBoard = new List<Board>();
        foreach (Board _board in gm.board)
        {
            randomBoard.Add(_board); // a list of all the boards in the game
        }

        while (randomBoard.Count > 0)
        { // repeat while list is not empty
            selected = randomBoard[Random.Range(0, randomBoard.Count)];
            if (selected.isFilled && selected.piece.pd is NormalPiece)
            {
                return selected;
            }
            randomBoard.Remove(selected); // remove the board from the list once checked.
        }

        while (true)
        { // contingency plan... choose a non-special powered gem
            selected = gm.board[Random.Range(0, gm.boardWidth), Random.Range(0, gm.boardHeight)];
            if (selected.isFilled && !selected.piece.pd.isSpecial)
            {
                return selected;
            }
        }
    }

    // method to check if the board still has special pieces
    bool hasRemainingSpecials()
    {
        for (int x = 0; x < gm.boardWidth; x++)
        {
            for (int y = 0; y < gm.boardHeight; y++)
            {
                if (gm.board[x, y].piece != null && gm.board[x, y].piece.pd != null &&
                   !(gm.board[x, y].piece.pd is NormalPiece)
                   && gm.board[x, y].piece.pd.isDestructible)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // method to cause a special piece to trigger it's ability
    void popASpecialPiece()
    {
        for (int x = 0; x < gm.boardWidth; x++)
        {
            for (int y = 0; y < gm.boardHeight; y++)
            {
                if (gm.board[x, y].piece != null && gm.board[x, y].piece.pd != null &&
                   !(gm.board[x, y].piece.pd is NormalPiece)
                   && gm.board[x, y].piece.pd.isDestructible)
                {
                    gm.board[x, y].forceDestroyBox(); // force pop the special piece
                    gm.notifyBoardHasChanged();
                    return;
                }
            }
        }
    }

    // function to set up the types remaining to get for this game
    void setUpTypes()
    {
        //if (placeholderPanel != null)
        //{
        //    int count = 0;
        //    for (int x = 0; x < gm.pieceTypes.Length; x++)
        //    { // creates the visual cue on the panel
        //        if (numToGet[x] > 0 && x < gm.NumOfActiveType)
        //        {
        //            // the visual image for player reference (e.g., red gem)
        //            GameObject img = (GameObject)Instantiate(gm.pieceTypes[0].skin[x]);
        //            img.transform.localScale = new Vector3(0, 0);
        //            ArrayFruitType[x].GetComponent<SpriteRenderer>().sprite = img.GetComponent<SpriteRenderer>().sprite;
        //            if (textHolder) desc[x] = ((GameObject)Instantiate(textHolder)).GetComponent<TextMesh>();

        //            desc[x].transform.parent = placeholderPanel.transform;
        //            desc[x].transform.localPosition = new Vector3(5, -(count * 3 + 3), 0); // position going downwards
        //            count++;
        //        }
        //    }
        //}
        //else
        //{ // warning developers of missing panel reference... 
        //    Debug.LogError("Placeholder panel missing for types... unable to create." +
        //        "Check winning conditions script again!");
        //}
    }

    public bool canSpawnTreasure()
    {
        if (isTreasureGame && (treasuresCollected + treasureList.Count) < (numOfTreasures1 + numOfTreasures2) &&
           treasureList.Count < maxOnScreen)
        {
            int probability = (int)(1.0 / (chanceToSpawn / 100.0));
            int result = Random.Range(0, probability); // random chance to spawn
            if (result == 0)
            {
                return true; // spawn a treasure
            }
        }
        return false; // cannot spawn...
    }

    #endregion other functions

    #region important phases

    // set up the variables
    void Start()
    {
        gm = GetComponent<GameManager>();

        // disable those not used...
        if (!isTimerGame)
        {
            if (timeLabel) timeLabel.gameObject.SetActive(false);
            if (timeText) timeText.gameObject.SetActive(false);
        }
        if (!isMaxMovesGame)
        {
            if (movesLabel != null) movesLabel.gameObject.SetActive(false);
            if (movesText != null) movesText.gameObject.SetActive(false);
        }
        if (!isTreasureGame)
        {
            if (treasureLabel != null) treasureLabel.gameObject.SetActive(false);
            if (treasureText != null) treasureText.gameObject.SetActive(false);
        }
        LayerShare.SetActive(false);
        StartCoroutine(updateStatus());
        StartCoroutine(routineCheck());
        if (startGameImmediately) startThisGame();
    }

    // function to start the timer running as well as to call GameManger's start sequence...
    public void startThisGame()
    {
        StartCoroutine(timer());
        gm.StartGame();
    }

    #endregion important phases

    public void RemoveFruit(string name)
    {
        if (isGetTypesGame)
        {
            int index = ListQuaMission.FindIndex(0, ListQuaMission.Count, item => item.name == name);
            if (index != -1)
            {

                int a = int.Parse(ListTextMesh[index].text.Split('/')[0]);
                a--;
                if (a <= 0)
                {
                    ListTextMesh[index].text = "0";
                    ListTextMesh[index].color = new Color(0, 0, 0, 0);
                    ListQuaMission[index].transform.GetChild(2).gameObject.SetActive(true);

                }
                else
                {
                    ListTextMesh[index].text = a.ToString() + "/" + lstSoluongquaComplete[index];
                }
            }
        }
    }
    public void SaveLevel(int star)
    {
        var obj = JsonConvert.DeserializeObject<Levemapmanager>(ObscuredPrefs.GetString("DataMap"));

        // string levelString = ObscuredPrefs.GetString("level");
        //int level = int.Parse(levelString.Substring(5));
        int levelMax = ObscuredPrefs.GetInt("levelmax");
        int level = ObscuredPrefs.GetInt("level");
        if (level > levelMax)
        {
            ObscuredPrefs.SetInt("levelmax", level);
            Debug.Log("levelmax=" + level);

        }
        obj.LevelMaps[level - 1].KeyLock = -1;
        obj.LevelMaps[level].KeyLock = 1;
        obj.LevelMaps[level - 1].Star = star;
        var a = JsonConvert.SerializeObject(obj);
        ObscuredPrefs.SetString("DataMap", a);
        if (level % 10 == 0)
        {
            LayerShare.SetActive(true);
        }
        if (!string.IsNullOrEmpty(GetDataFacebook.IDFacebook))
        {
            ObscuredPrefs.SetString("score", gm.score.ToString());

            AzureUILeaderboard azure = FindObjectOfType<AzureUILeaderboard>();
            if (azure != null)
            {
                azure.QueryIdFacebook(GetDataFacebook.IDFacebook, "Level" + level);
            }
        }
    }
}
