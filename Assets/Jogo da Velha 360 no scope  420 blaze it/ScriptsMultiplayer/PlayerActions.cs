using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public LayerMask layer;
    private float rayDistance = 50;
    private PositionsMultiPlayer pos = null;
    public int playerID;

    void Update() {

        if (Input.GetMouseButtonDown(0) && BoardManager.currentPlayer == playerID) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, rayDistance ,layer)) {
                pos = hit.collider.gameObject.GetComponent<PositionsMultiPlayer>();
                if (!CanvasProcess.instance.GetMultiplayerMenu() && !pos.isOccupied && BoardManager.instance.enabled) {
                    pos.isOccupied = true;
                    PositionsMultiPlayer.lastPos = pos.boardLocation;
                    BoardManager.instance.onPieceSpawned();
                    print(pos.boardLocation);
                }
            }
        }
        
    }

}
