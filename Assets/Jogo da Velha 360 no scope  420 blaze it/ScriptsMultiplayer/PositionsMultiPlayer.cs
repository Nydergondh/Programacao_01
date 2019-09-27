using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionsMultiPlayer : MonoBehaviour
{


    public int boardLocation;
    public bool isOccupied;
    public int PieceType; // { get; set; } // is different because I tried testing this new configuration of get/set (that I didnt knew)

    void Awake() {
        isOccupied = false;
        PieceType = -1;
    }

    public void SetBoardLocation(int value) {
        boardLocation = value;
    }

    public bool IsOccupied {
        get { return isOccupied; }
        set { isOccupied = value; }
    }


}

