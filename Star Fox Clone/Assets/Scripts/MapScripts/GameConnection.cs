using UnityEngine;
using UnityEngine.SceneManagement;

public class GameConnection : MonoBehaviour
{
    #region singleton and Awake
    
    static GameConnection instance;
    public static GameConnection Instance
    {
        get { return instance; }
    }

    /// <summary>
    /// Creates a Singleton,
    /// connects to the NetworkManager,
    /// prepares the levelloaded function
    /// and loads the Gamestateinfo
    /// </summary>
    private void Awake()
    {
        #region Singleton
        if (GameConnection.instance != null)
        {
            Debug.LogWarning("more than one instance of GameStateConnection");
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
        #endregion

        #region Load GameInfo
        gameInfo = new GameInfo("File_01");
        //Debug.Log("Gamestate created");
        #endregion

        SceneManager.sceneLoaded += OnLevelLoaded;
    }
    #endregion

    #region Variables

    #region Delegates
    public delegate void SwitchingPlayers();
    public SwitchingPlayers switchingPlayers;
    #endregion

    #region SaveFile
    SaveManagement saveManagement = new SaveManagement();
    GameInfo gameInfo;
    public GameInfo GameInfo { get { return gameInfo; } }
    public int LevelsUnlocked { get { return gameInfo.currentSaveFile.LevelsUnlocked; } }
    public int AMS_Unlocked { get { return gameInfo.currentSaveFile.AMSTurretsUnlocked; } }
    public int ATG_Unlocked { get { return gameInfo.currentSaveFile.ATGTurretsUnlocked; } }
    public int MSL_Unlocked { get { return gameInfo.currentSaveFile.MSLTurretsUnlocked; } }



    #endregion

    #region Players
    Player[] players;
    public Player[] Players { get { return players; } }
    public int NumberOfPlayers { get { return gameInfo.NumberOfLocalPlayers; } }

    Player frontlinePlayer;
    GameplayPlane gameplayPlane;
    public GameplayPlane Plane
    {
        get { return gameplayPlane; }
    }

    /// <summary>
    /// Tells the server wether or not to load a network player or if a local player will be loaded
    /// </summary>
    public bool LoadPlayerOnServer { get { return gameInfo.Connection == ConnectionType.Host || gameInfo.Connection == ConnectionType.Client; } }
    #endregion

    #region LoadingLevel
    LevelData levelData;
    public LevelData LevelData { get { return levelData; } }

    int levelToLoad;
    public int LevelToLoad { get { return levelToLoad; } }
    #endregion

    #endregion

    #region Loading maps
    //#######################################
    //Debugging. This will later be loaded via Gamestate
    [SerializeField]Player[] playerprefabs;
    public Player[] Playerprefabs
    {
        get { return playerprefabs; }
    }
    //#######################################

    public void SetConnectionType(ConnectionType type)
    {
        gameInfo.Connection = type;
    }

    #region Loadingscreen

    /// <summary>
    /// Loads the loading screen scene, where the "LoadingScreen"-script automatically loads the scene saved in "levelData" in the background while showing a progress bar
    /// </summary>
    void StartLoadingScreen()
    {
        SceneManager.LoadScene(0);
        string newSceneName = SceneManager.GetSceneByBuildIndex(levelToLoad).name;  //Returns null?!

        //TODO: GetSceneByBuildindex returns a scene but that scene does not contain a name for some reason?
        //DEBUGGING#########################################
        switch (levelToLoad)
        {
            case 2:
                newSceneName = "ChangeLoadout";
                break;
            case 3:
                newSceneName = "TutorialScene";
                break;
            default:
                newSceneName = "MainMenu";
                break;
        }
        //#################################################

    }

    private void OnLevelLoaded(Scene level, LoadSceneMode loadSceneMode)
    {
        if (level.buildIndex == 0) return;     //skipping loading screen

        DoneLoading();
    }

    public void DoneLoading()
    {
        switch (levelToLoad)
        {
            case 0:                                     //Loading Screen
                break;
            case 1:                                     //Loaded Main Menu
                break;
            case 2:                                     //Loaded Loadout Map
                    Invoke("StartLoadoutMap", 0.2f);
                break;
            default:                                    //Loaded actual level
                    Invoke("StartGameMap", 0.2f);
                break;
        }
    }
    #endregion

    #region LoadoutMap
    public void LevelSelectedAndStartPressed(LevelData ld)
    {
        levelData = ld;
        LoadLoadoutMap();
    }
    
    void LoadLoadoutMap()
    {
        levelToLoad = 2;
        StartLoadingScreen();
    }

    void StartLoadoutMap()
    {
        Debug.Log("Starting up Loadout map");

        players = new Player[gameInfo.NumberOfTotalPlayers];
        frontlinePlayer = players[0];

        spawnPlayerInLoadout();
    }
    #endregion

    #region GameMap
    public void LoadGameMap()
    {
        levelToLoad = levelData.LevelToLoad.handle;
        StartLoadingScreen();
    }

    //public for debugging
    public void StartGameMap()
    {
        //Debug.Log($"Starting up {levelData.LevelName}");

        for (int i = 0; i < gameInfo.NumberOfLocalPlayers; i++)
        {
            if (players == null)
            {
                players = new Player[gameInfo.NumberOfTotalPlayers];
                frontlinePlayer = players[0];

                Debug.LogWarning("Player " + i + " had to be loaded but should already exist");
                spawnPlayerInGame(i);
                players[i].RemoveTurretModules();
            }

            GetComponent<InputManagment>().ConnectInputs();     //Setting up PlayerInputs

            if (gameInfo.Connection == ConnectionType.Client)
            {
                if (i == 0) setupPuppet(players[i], i);
                else setupPlayer(players[i], i);
            }
            else if (gameInfo.Connection == ConnectionType.Host)
            {
                if (i == 1) setupPuppet(players[i], i);
                else setupPlayer(players[i], i);
            }
            else
            {
                setupPlayer(players[i], i);
            }
        }

        //resetting frontlinePlayer
        frontlinePlayer = players[0];

        Invoke("StartGame", 0.2f);
    }

    void setupPlayer(Player playerToSetup, int i)
    {
        MapLayoutInfo mapLayoutInfo = MapLayoutInfo.Instance;
        if (mapLayoutInfo == null)
        {
            Debug.LogError("ERROR: No 'mapLayoutInfo' detected");
            return;
        }
        gameplayPlane = mapLayoutInfo.Plane;

        playerToSetup.MakePuppet = false;

        //Set player connections
        playerToSetup.transform.SetParent(gameplayPlane.Playerpositions[i]);
        playerToSetup.transform.localPosition = Vector3.zero;//gameplayPlane.Playerpositions[i].position;
        playerToSetup.Plane = gameplayPlane;

        //Debug.Log("Setting HUD " + i);
        playerToSetup.Cam = mapLayoutInfo.Cameras[i];
        mapLayoutInfo.Cameras[i].gameObject.SetActive(true);
        playerToSetup.HUD = (InGameHUD)mapLayoutInfo.HUD[i];

        //set camera connections and viewport
        mapLayoutInfo.Cameras[i].GetComponent<CameraScript>().setPlayer(players[i]);
        setupCameraViewport(mapLayoutInfo.Cameras[i], i);

        //Set up HUDs
        mapLayoutInfo.HUD[i].gameObject.SetActive(true);
        mapLayoutInfo.HUD[i].GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        mapLayoutInfo.HUD[i].GetComponent<Canvas>().worldCamera = mapLayoutInfo.Cameras[i];

        //remove Layout funktions
        players[i].RemoveTurretModules();
    }

    void setupPuppet(Player playerToPuppet, int i)
    {
        MapLayoutInfo mapLayoutInfo = MapLayoutInfo.Instance;
        if (mapLayoutInfo == null)
        {
            Debug.LogError("ERROR: No 'mapLayoutInfo' detected");
            return;
        }
        gameplayPlane = mapLayoutInfo.Plane;

        playerToPuppet.MakePuppet = true;

        //Set player connections
        playerToPuppet.transform.SetParent(gameplayPlane.Playerpositions[i]);
        playerToPuppet.transform.localPosition = Vector3.zero;//gameplayPlane.Playerpositions[i].position;
        playerToPuppet.Plane = gameplayPlane;

        //Debug.Log("Removing HUD " + i);
        mapLayoutInfo.Cameras[i].gameObject.SetActive(false);
        mapLayoutInfo.HUD[i].gameObject.SetActive(false);

        //remove Layout funktions
        players[i].RemoveTurretModules();
    }

    void StartGame()
    {
        for (int i = 0; i < gameInfo.NumberOfLocalPlayers; i++)
        {
            players[i].StartGame();
        }
        gameplayPlane.GetComponent<FollowTrack>().StartFollow();
    }

    void setupCameraViewport(Camera cam, int player)
    {
        cam.gameObject.SetActive(true);
        cam.cullingMask = ~(1 << 14 - player);  //Ignore other player crosshair


        //vertical
        cam.rect = new Rect(.5f * player, 0f, 1f / NumberOfPlayers, 1f);

        //horizontal
        //cam.rect = new Rect(0f, 0.5f * player, 1f, 1f / NumberOfPlayers);
    }

    #region RestartMap

    public void RestartMap()
    {
        //TODO: reset players
        Debug.Log("Reset Players pls");
        StartLoadingScreen();
    }

    #endregion

    #endregion

    #region spawning players
    /// <summary>
    /// Spawns the ships either as player or as puppet
    /// </summary>
    public void spawnPlayerInLoadout()
    {
        switch (gameInfo.Connection)
        {
            case ConnectionType.Client:
                for (int i = 0; i < gameInfo.NumberOfLocalPlayers; i++)
                {
                    Player player = Instantiate(playerprefabs[0]);  //TODO: replace PlayerPrefabs

                    if(i == 1)AddPlayerToLoadoutScene(player, i);
                    else AddPuppetToLoadoutScene(player, i);
                }
                break;
            case ConnectionType.Host:
                for (int i = 0; i < gameInfo.NumberOfLocalPlayers; i++)
                {
                    Player player = Instantiate(playerprefabs[0]);    //TODO: replace PlayerPrefabs

                    if (i == 0) AddPlayerToLoadoutScene(player, i);
                    else AddPuppetToLoadoutScene(player, i);
                }
                break;
            default:
                for (int i = 0; i < gameInfo.NumberOfLocalPlayers; i++)
                {
                    Player player = Instantiate(playerprefabs[0]);
                    AddPlayerToLoadoutScene(player, i);
                }
                break;
        }
    }

    public void AddPlayerToLoadoutScene(Player player, int playerNumber)
    {
        players[playerNumber] = player;
        players[playerNumber].SetPlayerNumber(playerNumber);

        #region preparing Map

        MapLayoutInfo mapLayoutInfo = MapLayoutInfo.Instance;
        if (mapLayoutInfo == null)
        {
            Debug.LogError("ERROR: No 'mapLayoutInfo' detected");
            return;
        }

        players[playerNumber].transform.position = mapLayoutInfo.LoadoutMapPlayerPositions[playerNumber].position;
        DontDestroyOnLoad(players[playerNumber]);

        //Load Modules
        players[playerNumber].AddTurretModules((LoadoutHUD)mapLayoutInfo.HUD[playerNumber]);

        //Set up cameras
        mapLayoutInfo.HUD[playerNumber].gameObject.SetActive(true);
        mapLayoutInfo.HUD[playerNumber].PlayerNumber = playerNumber;
        mapLayoutInfo.HUD[playerNumber].GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        mapLayoutInfo.HUD[playerNumber].GetComponent<Canvas>().worldCamera = mapLayoutInfo.Cameras[playerNumber];
        mapLayoutInfo.HUD[playerNumber].Invoke("Initialize", 1f);   //Waiting for turrets to be loaded
        setupCameraViewport(mapLayoutInfo.Cameras[playerNumber], playerNumber);

        #endregion
    }


    public void AddPuppetToLoadoutScene(Player player, int playerNumber)
    {
        players[playerNumber] = player;
        players[playerNumber].SetPlayerNumber(playerNumber);

        #region preparing Map

        MapLayoutInfo mapLayoutInfo = MapLayoutInfo.Instance;
        if (mapLayoutInfo == null)
        {
            Debug.LogError("ERROR: No 'mapLayoutInfo' detected");
            return;
        }

        players[playerNumber].transform.position = mapLayoutInfo.LoadoutMapPlayerPositions[playerNumber].position;
        DontDestroyOnLoad(players[playerNumber]);

        //Remove HUD
        mapLayoutInfo.HUD[playerNumber].gameObject.SetActive(false);

        #endregion
    }


    /// <summary>
    /// Only used if the player wasn't spawned via the Loadout scene
    /// </summary>
    /// <param name="playerNumber"></param>
    public void spawnPlayerInGame(int playerNumber)
    {
        Player player = Instantiate(playerprefabs[0]);
        AddPlayerToGameScene(player);
    }

    public void AddPlayerToGameScene(Player player)
    {
        Debug.LogError("ADDING PLAYER TO GAME SCENE IS NOT YET IMPLEMENTED");
    }
    #endregion

    #endregion

    #region menus
    public void ReturnToMenu()
    {
        foreach (Player player in players)
        {
            Destroy(player.gameObject);
        }

        gameplayPlane = null;
        players = null;
       
        levelToLoad = 1;
        StartLoadingScreen();
    }
    #endregion

    #region Playerpositions
    public Player getFrontlinePlayer()
    {
        return frontlinePlayer;
    }

    public void SwitchPlayerPositions(int playerNumber)
    {
        if (gameInfo.NumberOfLocalPlayers < 2) return;

        if (frontlinePlayer == null)
        {
            frontlinePlayer = players[0];
            return;
        }
        if (frontlinePlayer != players[playerNumber]) return;   //only the player in front can decide to fall back


        else if (frontlinePlayer == players[0])
        {
            if (players[1] == null || players[1].IsInGame == false) return;
            if (gameplayPlane.requestPlayerSwitch(1))
            {
                frontlinePlayer = players[1];
                switchingPlayers?.Invoke();
            }
        }

        else if (frontlinePlayer == players[1])
        {
            if (players[0] == null || players[0].IsInGame == false) return;
            if (gameplayPlane.requestPlayerSwitch(0))
            {
                frontlinePlayer = players[0];
                switchingPlayers?.Invoke();
            }
        }
    }
    #endregion

    #region GameOver
    public void loosePlayer(Player P)
    {
        Debug.Log("Lost Player");

        if (players.Length > 1 && frontlinePlayer == P)
        {
            forcePlayerSwitch(P);
        }

        bool stillInGame = false;
        foreach (Player p in players)
        {
            //if (p == null) continue;

            if (p.IsInGame)
            {
                stillInGame = true;
            }
        }

        if (!stillInGame) GameOver();
    }

    void forcePlayerSwitch(Player oldPlayer)
    {
            Debug.Log("ForcingPlayerSwitch");
            gameplayPlane.forcePlayerSwitch();

            if (oldPlayer == players[0]) frontlinePlayer = players[1];
            if (oldPlayer == players[1]) frontlinePlayer = players[0];
            
            switchingPlayers();
    }

    public void GameOver()
    {
        Plane.stopTrack();
        foreach (var hud in MapLayoutInfo.Instance.HUD)
        {
            InGameHUD gameHUD = (InGameHUD)hud;
            gameHUD.GameOver();
        }
        //Debug.LogWarning("GAME OVER BOYS");
    }
    #endregion

    #region SaveFiles
    public void Save()
    {
        saveManagement.SaveGame("File_01", gameInfo);
    }

    public void Load()
    {
        gameInfo.LoadSaveFile("File_01");
    }

    #region Debugging
    public void LevelUnlock(int value)
    {
        gameInfo.currentSaveFile.LevelsUnlocked += value;
        Debug.Log($"Levels currently unlocked: {gameInfo.currentSaveFile.LevelsUnlocked}");
        Save();
    }

    public void AMSUnlock(int value)
    {
        gameInfo.currentSaveFile.AMSTurretsUnlocked += value;
        Debug.Log($"AMS Systems currently unlocked: {gameInfo.currentSaveFile.AMSTurretsUnlocked}");
        Save();
    }
    public void ATGUnlock(int value)
    {
        gameInfo.currentSaveFile.ATGTurretsUnlocked += value;
        Debug.Log($"ATG Systems currently unlocked: {gameInfo.currentSaveFile.ATGTurretsUnlocked}");
        Save();
    }
    public void MSLUnlock(int value)
    {
        gameInfo.currentSaveFile.MSLTurretsUnlocked += value;
        Debug.Log($"MSL Systems currently unlocked: {gameInfo.currentSaveFile.MSLTurretsUnlocked}");
        Save();
    }
    #endregion

    #endregion
}


