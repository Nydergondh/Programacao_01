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

    int bestPlay; //saves the IA best play at minimax

    public delegate void OnPieceSpawned();
    public OnPieceSpawned onPieceSpawned;

    void Awake() {

        positions = new List<Position>();

        pieceSpawned = false;
        //remove line bellow when finished testing
        isPlayerTurn = true;

        endGame = false;
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
        if (pieceSpawned && !endGame && isPlayerTurn) {

            foreach (Position pos in positions) {
                int[] tabuleiro = CheckEmptyPos();
                if (Position.lastPos == pos.GetBoardLocation()) {

                    PutPiece(positions[Position.lastPos].transform.position);
                    //set both the virtual board and the true board to spawn a player piece in the position clicked
                    positions[Position.lastPos].PieceType = 1;
                    tabuleiro[Position.lastPos] = 1;

                    if (CheckWinInt(positions[Position.lastPos].PieceType, tabuleiro)) { //check if the player has won

                        Debug.Log("Player Won!");
                        endGame = true;
                    }
                    else if (!CheckWinInt(positions[Position.lastPos].PieceType, tabuleiro) && CheckBoardFullInt(tabuleiro)) {
                        Debug.Log("Deu Velha");
                        endGame = true;
                    }

                    break;//stops the loop for going further in case alredy found where the piece has spawned

                }

            }
            isPlayerTurn = !isPlayerTurn;
        }


        // make AI play
        else if (!isPlayerTurn && !endGame)
        {
            //faz um tabuleiro virtual para rodar os testes
            int[] tabuleiro = CheckEmptyPos();
            // faz coisas pra desobrir onde jogar
            MinMax(tabuleiro, isPlayerTurn); //TODO não sei porque funciona PERGUNTAR
            // mark as occupied
            foreach (Position pos in positions)
            {

                if (bestPlay == pos.GetBoardLocation())
                {
                    pos.IsOccupied = true;
                    pos.PieceType = 2;
                    tabuleiro[bestPlay] = 2;
                    break;
                }
            }
            //set last pos to best position indentified by minmax
            Position.lastPos = bestPlay;

            //Spawns new piece and update the variables to make the player play the game
            PutPiece(positions[Position.lastPos].transform.position);

            if (CheckWinInt(positions[Position.lastPos].PieceType, tabuleiro))
            { //check if the player has won

                Debug.Log("AI Won!");
                endGame = true;
            }
            else if (!CheckWinInt(positions[Position.lastPos].PieceType, tabuleiro) && CheckBoardFullInt(tabuleiro))
            {
                Debug.Log("Deu Velha");
                endGame = true;
            }

            PieceSpawned();
            isPlayerTurn = !isPlayerTurn;
        }

    }



    void PutPiece(Vector3 piecePos) {

        if (isPlayerTurn) {
            Instantiate(xObject, piecePos, Quaternion.identity, parentPiece);
        }
        else {
            Instantiate(oObject, piecePos, Quaternion.identity, parentPiece);//Run MinMax Algorithm and place the piece in the best place
        }

    }

    void PieceSpawned() {
        pieceSpawned = !pieceSpawned;
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

    private int[] CheckEmptyPos()
    {
        int i = 0;
        int[] matrix = new int[positions.Count];
        foreach (Position pos in positions)
        {
            if (pos.PieceType == 1)
            {
                matrix[i] = 1;
            }
            else if (pos.PieceType == 2)
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

    public int MinMax(int[] tabuleiro, bool playerTurn)
    {

        int score;
        int pieceT;

        
        pieceT = playerTurn == true ? 2 : 1; //Pog U  :O    TODO perguntar porque.

        if (CheckWinInt(pieceT, tabuleiro)  || (!CheckWinInt(pieceT, tabuleiro) && CheckBoardFullInt(tabuleiro)))//Berryyyyyy Coll Berryyyyy Nice
        {
            return CalculadorScore(pieceT, tabuleiro);
        }

        List<Jogada> jogadas = new List<Jogada>();

        if (playerTurn)
        {
            score = 9999999;
            for (int i = 0; i < tabuleiro.Length; i++)
            {
                Jogada jogada;
                if (tabuleiro[i] == 0)
                {

                    jogada.tabuleiro = new int[tabuleiro.Length];
                    tabuleiro.CopyTo(jogada.tabuleiro, 0);

                    jogada.index = i;
                    jogada.tabuleiro[i] = 1;

                    jogada.score = MinMax(jogada.tabuleiro, !playerTurn);

                    jogadas.Add(jogada);
                }
            }
            score = Minimo(score, jogadas);

        }

        else
        {
            score = -9999999;
            for (int i = 0; i < tabuleiro.Length; i++)
            {
                Jogada jogada;
                if (tabuleiro[i] == 0) // if a empty position has been found, mark it with the player of the acctual layer 
                {
                    
                    jogada.tabuleiro = new int[tabuleiro.Length];
                    tabuleiro.CopyTo(jogada.tabuleiro, 0);

                    jogada.index = i;
                    jogada.tabuleiro[i] = 2;

                    jogada.score = MinMax(jogada.tabuleiro, !playerTurn); //parabens Vinicius :)

                    jogadas.Add(jogada);
                }

            }
            score = Maximo(score, jogadas);

        }

        return score;

    }


    int CalculadorScore(int pieceType, int[] tabuleiro)
    {

        if (CheckWinInt(pieceType, tabuleiro) && pieceType == 2)
        {
            return +1;
        }

        else if (CheckWinInt(pieceType, tabuleiro) && pieceType == 1)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    bool CheckWinInt(int pieceType, int[] tabuleiro)
    {
        if (CheckWinHorInt(pieceType, tabuleiro))
        {
            return true;
        }
        else if (CheckWinVertInt(pieceType, tabuleiro))
        {
            return true;
        }
        else if (CheckWinDiagInt(pieceType, tabuleiro))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //resolvido
    bool CheckWinHorInt(int pieceType,  int[] tabuleiro)
    {

        int sumHoriz;
        int positionsSize = (int)Mathf.Sqrt(tabuleiro.Length);

        for (int i = 0; i < positionsSize * positionsSize; i = i + positionsSize)
        {
            sumHoriz = -1;
            for (int j = 0; j < positionsSize; j++)
            {
                if (pieceType == tabuleiro[i + j])
                {
                    sumHoriz++;
                    if (sumHoriz == 2)
                    {
                        return true;
                    }

                }

            }
        }
        return false;
    }

    //maybe rigth

    bool CheckWinDiagInt(int pieceType, int [] tabuleiro)
    {

        int sumDiag = -1;
        int positionsSize = (int)Mathf.Sqrt(tabuleiro.Length);
        //for first diagonal
        for (int i = 0, j = 0; i < positionsSize; i++, j++)
        {

            if (pieceType == tabuleiro[(i * positionsSize) + j])
            {
                sumDiag++;
                if (sumDiag == 2)
                {
                    return true;
                }

            }

        }

        sumDiag = -1;
        //for contrary case
        for (int i = positionsSize - 1, j = 0; i >= 0; i--, j++)
        {
            if (pieceType == tabuleiro[(i * positionsSize) + j])
            {
                sumDiag++;
                if (sumDiag == 2)
                {
                    return true;
                }

            }

        }
        return false;
    }
    // rigth
    bool CheckWinVertInt(int pieceType, int[] tabuleiro)
    {

        int sumVert = -1;
        int positionsSize = (int)Mathf.Sqrt(tabuleiro.Length);

        for (int i = 0; i < positionsSize; i++)
        {
            sumVert = -1;
            for (int j = i; j < positionsSize * positionsSize; j = j + positionsSize)
            {

                if (pieceType == tabuleiro[j])
                {
                    sumVert++;
                    if (sumVert == 2)
                    {
                        return true;
                    }
                }

            }
        }
        return false;
    }

    private bool CheckBoardFullInt(int[] tabuleiro)
    {
        for (int i = 0; i < tabuleiro.Length; i++)
        {
            if (tabuleiro[i] == 0)
            {
                return false;
            }
        }
        return true;
    }

    int Maximo(int bestScore, List<Jogada> jogadas)
    {

        foreach (Jogada jogada in jogadas)
        {

            if (jogada.score > bestScore)
            {
                bestScore = jogada.score;
                bestPlay = jogada.index;
            }
            
        }

        return bestScore;
        
    }

    int Minimo(int bestScore, List<Jogada> jogadas)
    {
        
        foreach (Jogada jogada in jogadas)
        {

            if (jogada.score < bestScore)
            {

                bestScore = jogada.score;
                bestPlay = jogada.index;
            }

        }

        return bestScore;

    }
    /*
    void Calcula(int a, int b, out int x, out int y)
    {
        x = a + b;
        y = a * b;
    }
    */
    public struct Jogada
    {
        public int[] tabuleiro;
        public int score;
        public int index;
    }

}
