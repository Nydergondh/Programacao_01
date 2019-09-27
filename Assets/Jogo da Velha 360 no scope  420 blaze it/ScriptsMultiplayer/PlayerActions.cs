using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class PlayerActions : NetworkBehaviour {
    public LayerMask layer;
    private float rayDistance = 50;
    private PositionsMultiPlayer pos = null;
    public int playerID;

    void Start() {

        BoardManager.instance.AddPlayer(this);

    }

    void Update() {

        if (!isLocalPlayer) return;

        if (Input.GetMouseButtonDown(0) && BoardManager.currentPlayer == playerID) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, rayDistance ,layer)) {
                pos = hit.collider.gameObject.GetComponent<PositionsMultiPlayer>();
                if (pos != null) {
                    if (!CanvasProcess.instance.GetMultiplayerMenu() && !pos.isOccupied && BoardManager.instance.enabled) {
                        //passa o parametro que ocupa a posição e a atualização de qual posição é o last pos
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
        BoardManager.lastPos = pos.boardLocation;
        BoardManager.instance.onPieceSpawned();
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

}
