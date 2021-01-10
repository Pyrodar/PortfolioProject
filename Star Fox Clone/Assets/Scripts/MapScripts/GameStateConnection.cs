using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateConnection : MonoBehaviour
{
    #region singleton and Awake
    static GameStateConnection instance;

    public static GameStateConnection Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (GameStateConnection.instance != null)
        {
            Debug.LogWarning("more than one instance of GameStateConnection");
            return;
        }
        instance = this;

        DontDestroyOnLoad(this);
        gameStateInfo = new GameStateInfo("path");
        Debug.Log("Loaded Gamestate");
    }
    #endregion

    #region Variables

    #region Delegates
    public delegate void SwitchingPlayers();
    public SwitchingPlayers switchingPlayers;
    #endregion

    #region SaveFile
    GameStateInfo gameStateInfo;
    #endregion

    #region Players
    Player[] players;


    Player frontlinePlayer;
    GameplayPlane gameplayPlane;
    public GameplayPlane Plane
    {
        get { return gameplayPlane; }
    }
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
    //Debugging
    [SerializeField]Player[] playerprefabs;
    public Player[] Playerprefabs
    {
        get { return playerprefabs; }
    }
    //#######################################

    private void OnLevelWasLoaded(int level)
    {
        if (level == 0) return;     //skipping loading screen

        Debug.Log("Done Loading");
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


    #region LoadoutMap
    public void LevelSelectedAndStartPressed(LevelData ld)
    {
        levelData = ld;
        LoadLoadoutMap();
    }
    
    void LoadLoadoutMap()
    {
        levelToLoad = 2;
        SceneManager.LoadScene(0);
    }

    void StartLoadoutMap()
    {
        Debug.Log("Starting up Loadout map");
        MapLayoutInfo mapLayoutInfo = MapLayoutInfo.Instance;
        if (mapLayoutInfo == null)
        {
            Debug.LogError("ERROR: No 'mapLayoutInfo' detected");
            return;
        }

        players = new Player[2];

        for (int i = 0; i < gameStateInfo.PlayerNumber; i++)
        {
            players[i] = Instantiate(gameStateInfo.PlayerObjects[i]);
            players[i].transform.position = mapLayoutInfo.LoadoutMapPlayerPositions[i].position;
            DontDestroyOnLoad(players[i]);
        }

    }
    #endregion

    #region GameMap
    public void LoadGameMap()
    {
        levelToLoad = levelData.LevelToLoad.handle;
        SceneManager.LoadScene(0);
    }

    void StartGameMap()
    {
        Debug.Log($"Starting up {levelData.LevelName}");
        
        MapLayoutInfo mapLayoutInfo = MapLayoutInfo.Instance;
        if (mapLayoutInfo == null)
        {
            Debug.LogError("ERROR: No 'mapLayoutInfo' detected");
            return;
        }

        gameplayPlane = mapLayoutInfo.Plane;

        if(players == null) players = new Player[2];

        for (int i = 0; i < gameStateInfo.PlayerNumber; i++)
        {
            if (players[i] == null)
            {
                Debug.LogWarning("Player " + i + " had to be loaded but should already exist");
                players[i] = Instantiate(gameStateInfo.PlayerObjects[i]);
            }

            //Set player connections
            players[i].transform.SetParent(gameplayPlane.Playerpositions[i]);
            players[i].transform.position = gameplayPlane.Playerpositions[i].position;
            players[i].Plane = gameplayPlane;
            players[i].Cam = mapLayoutInfo.Cameras[i];
            players[i].HUD = mapLayoutInfo.HUD[i];
            //set camera connections
            mapLayoutInfo.Cameras[i].GetComponent<CameraScript>().setPlayer(players[i]);
        }

        //resetting frontlinePlayer
        frontlinePlayer = players[0];

        Invoke("StartGame", 0.2f);
    }

    void StartGame()
    {
        for (int i = 0; i < gameStateInfo.PlayerNumber; i++)
        {
            players[i].StartGame(gameplayPlane);
        }
        gameplayPlane.GetComponent<FollowTrack>().StartFollow();
    }
    #endregion

    #endregion

    #region menus
    void returnToMenu()
    {
        for (int i = 0; i < gameStateInfo.PlayerNumber; i++)
        {
            Destroy(players[i]);
        }

        gameplayPlane = null;

        levelToLoad = 1;
        SceneManager.LoadScene(0);
    }
    #endregion

    #region Playerpositions
    public Player getFrontlinePlayer()
    {
        if (frontlinePlayer == null)
        {
            SwitchPlayerPositions();
        }
        return frontlinePlayer;
    }

    public void SwitchPlayerPositions()
    {
        if (gameStateInfo.PlayerNumber < 2) return;

        if (frontlinePlayer == null)
        {
            frontlinePlayer = players[0]; 
        }

        else if (frontlinePlayer = players[0])
        {
            frontlinePlayer = players[1];
            gameplayPlane.switchPlayerPositions(1);
        }

        else if (frontlinePlayer = players[1])
        {
            frontlinePlayer = players[0];
            gameplayPlane.switchPlayerPositions(0);
        }

        switchingPlayers();
    }
    #endregion

    #region GameOver
    public void loosePlayer(Player P)
    {
        Debug.Log("Lost Player");

        if (players.Length > 1 || frontlinePlayer == P)
        {
            switchingPlayers();
        }


        bool stillInGame = false;
        foreach (Player p in players)
        {
            if (p == null) continue;

            if (p.IsInGame)
            {
                stillInGame = true;
            }
        }

        if (!stillInGame) GameOver();
    }

    public void GameOver()
    {
        Debug.LogWarning("GAME OVER BOYS");
    }
    #endregion
}

class GameStateInfo
{
    #region Player
    int playerNumber;
    public int PlayerNumber
    {
        get { return playerNumber; }
        set { playerNumber = value; }
    }

    Player[] playerObjects;
    public Player[] PlayerObjects
    {
        get { return playerObjects; }
        set { playerObjects = value; }
    }
    #endregion

    #region saveFile
    public GameStateInfo(string filename)
    {
        LoadSaveFile(filename);
    }
    #endregion

    void LoadSaveFile(string file)
    {
        Debug.Log("loading: " + file);

        //#################
        //TODO: Load a save file
        //#################



        //Debugging:#######
        playerNumber = 1;

        playerObjects = new Player[2];
        playerObjects[0] = GameStateConnection.Instance.Playerprefabs[0];
        playerObjects[1] = GameStateConnection.Instance.Playerprefabs[0];
        //#################
    }
}
