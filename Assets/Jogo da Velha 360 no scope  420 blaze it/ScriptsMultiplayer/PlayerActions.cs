using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerActions : NetworkBehaviour {
    public LayerMask layer;
    private float rayDistance = 50;
    private bool played; //variavel que controla se voce ja jogou sua vez
                        //para impedir que no tempo que o server atualize voce possa jogar em outras posições
    private PositionsMultiPlayer pos = null;
    public int playerID;


    void Start() {

        MyNetworkManager.onClientDisconnect += OnPlayerClientDisconnect;

        BoardManager.instance.AddPlayer(this);

        played = false;
    }

    void Update() {

        if (!isLocalPlayer) return;

        if (Input.GetMouseButtonDown(0) && BoardManager.currentPlayer == playerID && !BoardManager.instance.endGame && !played) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, rayDistance ,layer)) {
                pos = hit.collider.gameObject.GetComponent<PositionsMultiPlayer>();
                if (pos != null) {
                    if (!CanvasProcess.instance.GetMultiplayerMenu() && !pos.isOccupied && BoardManager.instance.enabled) {
                        //passa o parametro que ocupa a posição e a atualização de qual posição é o last pos
                        played = true;
                        CmdDoMove(pos.boardLocation);
                    }

                }
            }
        }
        
    }

    [Command]
    private void CmdDoMove(int lastP) {
        RpcDoMove(lastP);
    }

    [ClientRpc]
    private void RpcDoMove(int lastP) {

        pos = GetPositionPlayed(lastP);
        pos.isOccupied = true;
        pos.PieceType = BoardManager.currentPlayer;
        BoardManager.lastPos = pos.boardLocation;
        BoardManager.instance.onPieceSpawned();

        BoardManager.instance.PlayInPosition();
        played = false;
    }
    
    //Pede para o server replicar o prenchimento da posição e a atualização do lastPos
    //isso atualiza o ultimo clique valido em uma posição pelo player atual

    private PositionsMultiPlayer GetPositionPlayed(int lastP) {
        foreach (PositionsMultiPlayer position in BoardManager.instance.positions) {
            if (position.boardLocation == lastP) {
                return position;
            }
        }
        return null;
    }

    //chama no server quando um cliente disconectar para destruir o objeto que não representa o player local
    public void OnPlayerClientDisconnect(NetworkConnection info) {
        if (this != null) {
            if (!isLocalPlayer) {
                Destroy(this);
            }
        }
    }
    //função que retorna 0 caso não seja o player local e o Id do player caso seja o Player Local
    //Usada no BoardManager quando o jogo terminar para mostrar se voce perdeu ou venceu a partida
    public int ReturnIdLocalPlayer() {
        if (isLocalPlayer) {
            return playerID;
        }
        else {
            return 0;
        }
    }
    
    
}
