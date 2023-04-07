using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public int count = 0;
    public GameObject TPlayer;
    public GameObject QPlayer;
    public GameObject Player;
    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreateMMOCharacterMessage>(OnCreateCharacter);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        // you can send the message here, or wherever else you want
        CreateMMOCharacterMessage characterMessage = new CreateMMOCharacterMessage
        {
            race = Race.Elvish,
            name = "Joe Gaba Gaba",
            hairColor = Color.red,
            eyeColor = Color.green
        };

        NetworkClient.Send(characterMessage);
    }

    void OnCreateCharacter(NetworkConnectionToClient conn, CreateMMOCharacterMessage message)
    {
        if (count == 0)
        {
            count++;
            Player = Instantiate(TPlayer);
            Debug.Log("TPlayer is Spawn");

        }
        else
        {
            Player = Instantiate(QPlayer);
            Debug.Log("QPlayer is Spawn");
        }

        // playerPrefab is the one assigned in the inspector in Network
        // Manager but you can use different prefabs per race for example

        // Apply data from the message however appropriate for your game
        // Typically Player would be a component you write with syncvars or properties


        // call this to use this gameobject as the primary controller
        NetworkServer.AddPlayerForConnection(conn, Player);
    }
}
