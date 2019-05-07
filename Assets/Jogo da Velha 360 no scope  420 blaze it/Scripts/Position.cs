using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{

    private int boardLocation;
    private bool isOccupied;
    public int PieceType { get; set; } // is different because I tried testing this new configuration of get/set (that I didnt knew)

    public static int lastPos = -1; //remember to adjust before restarting scene or something!!!

    void Awake() {
        isOccupied = false;
        PieceType = -1;
    }

    public void SetBoardLocation (int value){
        boardLocation = value;
    }

    public int GetBoardLocation (){
        return boardLocation;
    }

    public bool IsOccupied {
        get { return isOccupied; }
        set { isOccupied = value; }
    }    

    void OnMouseDown() {

        if (!isOccupied) {
            isOccupied = true;
            lastPos = boardLocation;
            GetComponentInParent<Board>().onPieceSpawned();
        }       

    }

}
