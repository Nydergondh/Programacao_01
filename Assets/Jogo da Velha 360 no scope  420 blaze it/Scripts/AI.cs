using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    int[] virtualPositions;
    bool endSimulation;
    bool isPlayerTurn;
    // Start is called before the first frame update
    void Start()
    {
        endSimulation = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Position> VirtualPosition {
        get { return virtualPositions; }
        set { virtualPositions = value; }
    }

    private int[] CheckEmptyPos(List<Position> positions) {
        int i = 0;
        int[] matrix = new int[(int)Mathf.Sqrt(positions.Count)];
        foreach (Position pos in positions) {
            if(pos.IsOccupied == false && isPlayerTurn) {
                matrix[i] = 1;
            }
            else if(pos.IsOccupied == false && !isPlayerTurn) {
                matrix[i] = 2;
            }
            else {
                matrix[i] = 0;
            }
            i++;
        }
        return matrix;
    }

    bool CheckWinDiagonal(int pieceType) {

        int sumDiag = -1;
        int positionsSize = (int)Mathf.Sqrt(virtualPositions.Count);
        //for first diagonal
        for (int i = 0, j = 0; i < positionsSize; i++, j++) {

            if (pieceType == virtualPositions[(i * positionsSize) + j].PieceType) {
                sumDiag++;
                if (sumDiag == 2) {
                    return true;
                }

            }

        }

        sumDiag = -1;
        //for contrary case
        for (int i = positionsSize - 1, j = 0; i >= 0; i--, j++) {
            print(i + j);
            if (pieceType == virtualPositions[(i * positionsSize) + j].PieceType) {
                sumDiag++;
                if (sumDiag == 2) {
                    return true;
                }

            }

        }
        return false;
    }

    bool CheckWinVertical(int pieceType) {

        int sumVert = -1;
        int positionsSize = (int)Mathf.Sqrt(virtualPositions.Count);

        for (int i = 0; i < positionsSize; i++) {
            sumVert = -1;
            for (int j = i; j < positionsSize * positionsSize; j = j + positionsSize) {

                if (pieceType == virtualPositions[j].PieceType) {
                    sumVert++;
                    if (sumVert == 2) {
                        return true;
                    }
                }

            }
        }
        return false;
    }

    bool CheckWinHorizontal(int pieceType) {

        int sumHoriz = -1;
        int positionsSize = (int)Mathf.Sqrt(virtualPositions.Count);

        for (int i = 0; i < positionsSize; i = i + positionsSize) {
            sumHoriz = -1;
            for (int j = 0; j < positionsSize; j++) {

                if (pieceType == virtualPositions
[i + j].PieceType) {
                    sumHoriz++;
                    if (sumHoriz == 2) {
                        return true;
                    }
                }

            }
        }
        return false;
    }

    private bool CheckBoardFull() {
        for (int i = 0; i < virtualPositions.Count; i++) {
            if (virtualPositions[i].IsOccupied == false) {
                return false;
            }
        }
        return true;
    }

    bool CheckWin(int pieceType) {

        if (CheckWinHorizontal(pieceType)) {
            endSimulation = true;
            return true;
        }
        else if (CheckWinVertical(pieceType)) {
            endSimulation = true;
            return true;
        }
        else if (CheckWinDiagonal(pieceType)) {
            endSimulation = true;
            return true;
        }
        else {
            if (CheckBoardFull()) {
                endSimulation = true;
                return false;
            }
            else {
                return false;
            }
        }
    }

    public int MinMax(int pos, bool isMax) {

        

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
}
