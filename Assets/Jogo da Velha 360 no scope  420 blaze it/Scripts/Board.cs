using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class manages the game state
public class Board : MonoBehaviour
{

    List<Position> positions;

    [SerializeField] GameObject xObject;
    [SerializeField] GameObject oObject;
    [SerializeField] Transform parentPiece;

    public bool isPlayerTurn;
    public bool pieceSpawned;

    public delegate void OnPieceSpawned();
    public OnPieceSpawned onPieceSpawned;

    void Awake() {

        positions = new List<Position>();

        pieceSpawned = false;
        //remove line bellow when finished testing
        isPlayerTurn = true;
        //TODO IMPORTANT: add these lines when functionality is working normaly to spawn pieces.
        /*
        if (Random.Range(0, 2) == 1) {
            isPlayerTurn = true;
        }
        else {
            isPlayerTurn = false;
        }
        */
        onPieceSpawned += PieceSpawned;

    }


    // Start is called before the first frame update
    void Start()
    {
        
        int i = 0;
        //fills in the array of positions
        foreach (Position pos in GetComponentsInChildren<Position>()) {
            print(GetComponentsInChildren<Position>().Length);
            positions.Add(pos);
            print("Got here!");
            pos.SetBoardLocation(i);
            i++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (pieceSpawned) {

            foreach (Position pos in positions) {

                if (Position.lastPos == pos.GetBoardLocation()) {              
                    PutPiece(positions[Position.lastPos].transform.position);
                }

            }

            pieceSpawned = false;
        }

    }

    void PutPiece(Vector3 piecePos) {
        
        if (isPlayerTurn) {
            Instantiate(xObject, piecePos ,Quaternion.identity);
        }
        else {
            //Run MinMax Algorithm and place the piece in the best place
        }

    }

    void PieceSpawned() { 
        pieceSpawned = !pieceSpawned;
    }

    

}
