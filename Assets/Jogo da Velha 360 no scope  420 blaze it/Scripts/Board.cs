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
    public bool endGame;

    public delegate void OnPieceSpawned();
    public OnPieceSpawned onPieceSpawned;

    void Awake() {

        positions = new List<Position>();

        pieceSpawned = false;
        //remove line bellow when finished testing
        isPlayerTurn = true;

        endGame = false;
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
            positions.Add(pos);
            pos.SetBoardLocation(i);
            i++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (pieceSpawned && !endGame) {

            foreach (Position pos in positions) {

                if (Position.lastPos == pos.GetBoardLocation()) {              
                    PutPiece(positions[Position.lastPos].transform.position);
                    if (isPlayerTurn) {
                        positions[Position.lastPos].PieceType = 1;
                    }
                    else {
                        positions[Position.lastPos].PieceType = 2;
                    }
                    
                    if (CheckWin(positions[Position.lastPos].PieceType)) { //check if one of the player has won

                        if (isPlayerTurn) {
                            Debug.Log("Player Won!");
                        }
                        else if (!isPlayerTurn) {
                            Debug.Log("AI Won!");
                        }
                        
                    }
                    else if(!CheckWin(positions[Position.lastPos].PieceType) && CheckBoardFull()) {
                        Debug.Log("Deu Velha");
                    }

                    break;//stops the loop for going further in case alredy found where the piece has spawned
                    
                }

            }
            
            isPlayerTurn = !isPlayerTurn;
            pieceSpawned = false; // sets the update veryfier
            /*
            // make AI play
            if(!isPlayerTurn)
            {
                // faz coisas pra desobrir onde jogar
                
                int positionToPlayAI = 5; // TODO: minimax               

                // mark as occupied
                foreach (Position pos in positions)
                {

                    if (positionToPlayAI == pos.GetBoardLocation())
                    {
                        pos.IsOccupied = true;
                        break;
                    }
                }

                Position.lastPos = positionToPlayAI;
                PieceSpawned();
               
            }
            */
        }

    }

    void PutPiece(Vector3 piecePos) {

        if (isPlayerTurn) {
            Instantiate(xObject, piecePos, Quaternion.identity);
        }
        else {
            Instantiate(oObject, piecePos, Quaternion.identity);//Run MinMax Algorithm and place the piece in the best place
        }

    }

    void PieceSpawned() { 
        pieceSpawned = !pieceSpawned;
    }


    //check who won
    bool CheckWin(int pieceType) {

        if (CheckWinHorizontal(pieceType)) {
            endGame = true;
            return true;
        }
        else if (CheckWinVertical(pieceType)) {
            endGame = true;
            return true;
        }
        else if (CheckWinDiagonal(pieceType)) {
            endGame = true;
            return true;
        }
        else {
            if (CheckBoardFull()) {
                endGame = true;
                return false;
            }
            else {
                return false;
            }
        }
    }

    //resolvido
    bool CheckWinHorizontal(int pieceType) {

        int sumHoriz = -1;
        int positionsSize = (int)Mathf.Sqrt(positions.Count);

        for (int i = 0; i < positionsSize; i = i + positionsSize) {
            sumHoriz = -1;
            for (int j = 0; j < positionsSize; j++) {

                if (pieceType == positions[i + j].PieceType) {
                    sumHoriz++;
                    print("Got Here");
                    if (sumHoriz == 2) {
                        return true;
                    }

                }

            }
        }
        return false;
    }

    //maybe rigth

    bool CheckWinDiagonal(int pieceType) {

        int sumDiag = -1;
        int positionsSize = (int)Mathf.Sqrt(positions.Count);
        //for first diagonal
        for (int i = 0, j = 0; i < positionsSize; i++, j++) {

            if (pieceType == positions[(i * positionsSize) + j].PieceType) {
                sumDiag++;
                if (sumDiag == 2) {
                    return true;
                }

            }

        }

        sumDiag = -1;
        //for contrary case
        for (int i = positionsSize-1, j = 0; i >= 0; i--, j++) {
            if (pieceType == positions[(i * positionsSize) + j].PieceType) {
                sumDiag++;
                if (sumDiag == 2) {
                    return true;
                }

            }

        }
        return false;
    }
    // rigth
    bool CheckWinVertical(int pieceType) {

        int sumVert = -1;
        int positionsSize = (int)Mathf.Sqrt(positions.Count);

        for (int i = 0; i < positionsSize; i++) {
            sumVert = -1;
            for (int j = i; j < positionsSize * positionsSize; j = j + positionsSize) {

                if (pieceType == positions[j].PieceType) {
                    sumVert++;
                    if (sumVert == 2) {
                        return true;
                    }
                }

            }
        }
        return false;
    }

    private bool CheckBoardFull() {
        for (int i = 0; i < positions.Count; i++) {
            if (positions[i].IsOccupied == false) {
                return false;
            }
        }
        return true;
    }

    private int[] CheckEmptyPos()
    {
        int i = 0;
        int[] matrix = new int[(int)Mathf.Sqrt(positions.Count)];
        foreach (Position pos in positions)
        {
            if (pos.IsOccupied == false && isPlayerTurn)
            {
                matrix[i] = 1;
            }
            else if (pos.IsOccupied == false && !isPlayerTurn)
            {
                matrix[i] = 2;
            }
            else
            {
                matrix[i] = 0;
            }
            i++;
        }
        return matrix;
    }

    /*

   função Minimax( tabuleiro )
       se FimDeJogo( tabuleiro )
           retorne CalcularScore( tabuleiro )

       se OponenteEstáJogando( tabuleiro ) {
           valor = 9999999
           para todas ramificações do tabuleiro
           valor = mínimo(valor,Minimax(ramificação))
       }
       senão {
           valor = -9999999
           para todas ramificações do tabuleiro
           valor = máximo(valor,Minimax(ramificação))
       }
       retornar valor
  }

        função CalcularScore(tabuleiro){

            se computadorGanhou então retorna +1;

            se oponenteGanhou então retorna -1;

            retorna 0; //empate
        } Bruno Ferreira e Sílvia M.W. Moraes Inteligênc


    */
    //Checar essa merda!!!!
    public int MinMax(int[] tabuleiro)
    {
        int score;

        if (CheckWin(positions[Position.lastPos].PieceType)) //TODO MUDAR A LASTPOS!!!!
        {
            return CalculadorScore();
        }

        if(isPlayerTurn)
        {
            score = 9999999;
            for(int i = 0; i < tabuleiro.Length; i++)
            {
                if (tabuleiro[i] == 0)
                {
                    tabuleiro[i] = 1;
                    score = MinMax(tabuleiro);
                }
            }
        }

        else
        {
            score = -9999999;
            for (int i = 0;i < tabuleiro.Length ; i++)
            {
                if (tabuleiro[i] == 0)
                {
                    tabuleiro[i] = 2;
                    score = MinMax(tabuleiro);
                }

            }
        }

        return score;

    }


    int CalculadorScore()
    {
        if (CheckWin(positions[Position.lastPos].PieceType) && positions[Position.lastPos].PieceType == 1)
        {
            return +1; 
        }

        else if (CheckWin(positions[Position.lastPos].PieceType) && positions[Position.lastPos].PieceType == 2)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

}
