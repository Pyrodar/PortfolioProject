using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    GameStateConnection myGame;

    public override void Awake()
    {
        base.Awake();
        myGame = GetComponent<GameStateConnection>();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //base.OnServerAddPlayer(conn);

        if (myGame.LoadPlayerOnServer)
        {
            //TODO: adjust prefab based on savefile
            var newPlayer = Instantiate(playerPrefab);

            //Change values
            var id = newPlayer.GetComponent<NetworkIdentity>();

            NetworkServer.AddPlayerForConnection(conn, newPlayer.gameObject);
            id.AssignClientAuthority(conn);

            Player p = newPlayer.GetComponent<Player>();
            myGame.AddPlayerToLoadoutScene(p);
        }
    }
}
