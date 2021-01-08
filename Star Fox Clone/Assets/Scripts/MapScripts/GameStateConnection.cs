using UnityEngine;

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

    public delegate void SwitchingPlayers();
    public SwitchingPlayers switchingPlayers;

    GameStateInfo gameStateInfo;

    Player[] players;


    Player frontlinePlayer;
    GameplayPlane gameplayPlane;
    public GameplayPlane Plane
    {
        get { return gameplayPlane; }
    }



    #region Loading maps
    //#######################################
    //Debugging
    [SerializeField]Player[] playerprefabs;
    public Player[] Playerprefabs
    {
        get { return playerprefabs; }
    }

    private void Start()
    {
        LoadGameMap();
    }
    //#######################################
    void LoadLoadoutMap()
    {
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

    void LoadGameMap()
    {
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

        Invoke("StartGameMap", 1f);
    }

    void StartGameMap()
    {
        for (int i = 0; i < gameStateInfo.PlayerNumber; i++)
        {
            players[i].StartGame(gameplayPlane);
        }
        gameplayPlane.GetComponent<FollowTrack>().StartFollow();
    }

    #endregion

    #region menus
    void returnToMenu()
    {
        for (int i = 0; i < gameStateInfo.PlayerNumber; i++)
        {
            Destroy(players[i]);
        }

        gameplayPlane = null;

        //Load Menu
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
}

class GameStateInfo
{
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

    public GameStateInfo(string filename)
    {
        LoadSaveFile(filename);
    }

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
