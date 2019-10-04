using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MyNetworkManager : NetworkManager
{
    public bool online;

    public static event Action<NetworkConnection> onServerConnect;
    public static event Action<NetworkConnection> onClientConnect;
    public static event Action<NetworkConnection> onClientDisconnect;
    public static event Action<NetworkConnection> onServerDisconnect;

    private void Start() {
        online = false;
    }

    public static NetworkDiscovery Discovery 
    {
        get
        {
            return singleton.GetComponent<NetworkDiscovery>();
        }
    }

    public static NetworkMatch Match
    {
        get
        {
            return singleton.GetComponent<NetworkMatch>() ?? singleton.gameObject.AddComponent<NetworkMatch>();
        }
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

        if (conn.address == "localClient")
        {
            return;
        }

        Debug.Log("Client connected! Address: " + conn.address);

        onServerConnect?.Invoke(conn);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        if (conn.address == "localServer")
        {
            return;
        }

        Debug.Log("Connected to server! Address: " + conn.address);
        print(conn.connectionId);
        onClientConnect?.Invoke(conn);

    }
    //dont know if I need this
    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);
        onClientDisconnect?.Invoke(conn);
        if (!BoardManager.instance.endGame) {
            CanvasProcess.instance.endingButton.SetActive(true);
        }
    }
    
    public override void OnServerDisconnect(NetworkConnection conn) {
        base.OnServerDisconnect(conn);
        onServerDisconnect?.Invoke(conn);
        if (!BoardManager.instance.endGame) {
            CanvasProcess.instance.endingButton.SetActive(true);
        }
    }
    
    public override void OnClientError(NetworkConnection conn, int errorCode){
        base.OnClientError(conn, errorCode);
        print(conn.isConnected+ " Client");
        if (!conn.isConnected && !BoardManager.instance.endGame) {
            print("CalledClient");
        }
    }

    public override void OnServerError(NetworkConnection conn, int errorCode){
        base.OnServerError(conn, errorCode);
        print(conn.isConnected + " Server");
        if (!conn.isConnected && !BoardManager.instance.endGame) {
            print("CalledServer");
        }
    }

    //usado para setar qual será a interface usada (LocalGames / OnlineGames)
    //chamado apenas no menu multiplayer ao clicar uma das opções

}
