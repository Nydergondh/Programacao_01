using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    // Start is called before the first frame update
    private int boardLocation;
    private bool isOccupied;

    public static int lastPos = -1;

    void Awake() {
        isOccupied = false;
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
