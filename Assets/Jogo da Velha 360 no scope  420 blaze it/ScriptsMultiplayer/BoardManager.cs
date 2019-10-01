using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class BoardManager : MonoBehaviour {

    public static List<PlayerActions> players;
    public static BoardManager instance = null;
    public static int lastPos = -1;

    public List<PositionsMultiPlayer> positions;

    [SerializeField] GameObject xObject = null;
    [SerializeField] GameObject oObject = null;
    [SerializeField] Transform parentPiece = null;

    public Canvas canvas = null;
    public AudioSource audio = null;
    public AudioClip[] clips;

    public bool pieceSpawned;
    public bool endGame;

    public static int currentPlayer = 1;

    public delegate void OnPieceSpawned();
    public OnPieceSpawned onPieceSpawned;

    void Awake() {
        if (instance == null) {
            instance = this;
        }

        else if (instance != this) {
            Destroy(gameObject);
        }

        players = new List<PlayerActions>();
        positions = new List<PositionsMultiPlayer>();

        pieceSpawned = false;

        endGame = false;
        onPieceSpawned += PieceSpawned;
    }


    // Start is called before the first frame update
    void Start() {
        int i = 0;
        //fills in the array of positions
        foreach (PositionsMultiPlayer pos in GetComponentsInChildren<PositionsMultiPlayer>()) {
            positions.Add(pos);
            pos.SetBoardLocation(i);
            i++;
        }

    }

    public void Inicialize() {
        gameObject.SetActive(true);
    }

    public void PlayInPosition() {

        int[] tabuleiro = CheckEmptyPos();

        tabuleiro[lastPos] = currentPlayer;

        //Spawns new piece and update the variables to make the player play the game
        PutPiece(positions[lastPos].transform.position);

        if (CheckWinInt(positions[lastPos].PieceType, tabuleiro) && currentPlayer == 2) { //check if the player has won

            Debug.Log("Player 2 has Won");
            endGame = true;
            ChangeAudio(1);
            canvas.GetComponent<CanvasProcess>().thatsAllFolks(endGame);
        }

        else if (CheckWinInt(positions[lastPos].PieceType, tabuleiro) && currentPlayer == 1) {
            Debug.Log("Player 1 has Won");
            endGame = true;
            ChangeAudio(1);
            canvas.GetComponent<CanvasProcess>().thatsAllFolks(endGame);
        }


        else if (!CheckWinInt(positions[lastPos].PieceType, tabuleiro) && CheckBoardFullInt(tabuleiro)) {
            Debug.Log("Deu Velha");
            endGame = true;
            ChangeAudio(1);
            canvas.GetComponent<CanvasProcess>().thatsAllFolks(endGame);
        }

        //stop the GameLoop while it wait for the next player to click a valid to position
        pieceSpawned = !pieceSpawned;

        //if current player is 1 (X) then change it to 2 (O) and vice versa        
        if (currentPlayer == 1) {
            currentPlayer = 2;
        }
        else {
            currentPlayer = 1;
        }

    }

    private int[] EmptyPositions(int[] tabuleiro) {

        int i = 0;
        for (int j = 0; j < positions.Count; j++) {
            if (tabuleiro[j] == 0) {
                i++;
            }
        }
        int[] newTab = new int[i];
        int size = i;
        i = 0;

        for (int j = 0; i < size; j++) {
            if (tabuleiro[j] == 0) {
                newTab[i] = j;
                i++;
            }
        }

        return newTab;
    }

    void PutPiece(Vector3 piecePos) {
        
        //if currentPlayer = 1 put an "X" else if currentPlayer is 2 then put a "O".
        if (currentPlayer == 1) {
            Instantiate(xObject, piecePos, Quaternion.identity, parentPiece);
        }
        else if (currentPlayer == 2) {
            Instantiate(oObject, piecePos, Quaternion.identity, parentPiece);//Run MinMax Algorithm and place the piece in the best place
        }
        audio.Play();

    }

    void PieceSpawned() {
        //if not multiplayer then wait for IA play
        pieceSpawned = !pieceSpawned;
    }

    private int[] CheckEmptyPos() {
        int i = 0;
        int[] matrix = new int[positions.Count];
        foreach (PositionsMultiPlayer pos in positions) {
            if (pos.PieceType == 1) {
                matrix[i] = 1;
            }
            else if (pos.PieceType == 2) {
                matrix[i] = 2;
            }
            else {
                matrix[i] = 0;
            }
            i++;
        }
        return matrix;
    }

    bool CheckWinInt(int pieceType, int[] tabuleiro) {
        if (CheckWinHorInt(pieceType, tabuleiro)) {
            return true;
        }
        else if (CheckWinVertInt(pieceType, tabuleiro)) {
            return true;
        }
        else if (CheckWinDiagInt(pieceType, tabuleiro)) {
            return true;
        }
        else {
            return false;
        }
    }

    //resolvido
    bool CheckWinHorInt(int pieceType, int[] tabuleiro) {

        int sumHoriz;
        int positionsSize = (int)Mathf.Sqrt(tabuleiro.Length);

        for (int i = 0; i < positionsSize * positionsSize; i = i + positionsSize) {
            sumHoriz = -1;
            for (int j = 0; j < positionsSize; j++) {
                if (pieceType == tabuleiro[i + j]) {
                    sumHoriz++;
                    if (sumHoriz == 2) {
                        return true;
                    }

                }

            }
        }
        return false;
    }

    //maybe rigth

    bool CheckWinDiagInt(int pieceType, int[] tabuleiro) {

        int sumDiag = -1;
        int positionsSize = (int)Mathf.Sqrt(tabuleiro.Length);
        //for first diagonal
        for (int i = 0, j = 0; i < positionsSize; i++, j++) {

            if (pieceType == tabuleiro[(i * positionsSize) + j]) {
                sumDiag++;
                if (sumDiag == 2) {
                    return true;
                }

            }

        }

        sumDiag = -1;
        //for contrary case
        for (int i = positionsSize - 1, j = 0; i >= 0; i--, j++) {
            if (pieceType == tabuleiro[(i * positionsSize) + j]) {
                sumDiag++;
                if (sumDiag == 2) {
                    return true;
                }

            }

        }
        return false;
    }
    // rigth
    bool CheckWinVertInt(int pieceType, int[] tabuleiro) {

        int sumVert = -1;
        int positionsSize = (int)Mathf.Sqrt(tabuleiro.Length);

        for (int i = 0; i < positionsSize; i++) {
            sumVert = -1;
            for (int j = i; j < positionsSize * positionsSize; j = j + positionsSize) {

                if (pieceType == tabuleiro[j]) {
                    sumVert++;
                    if (sumVert == 2) {
                        return true;
                    }
                }

            }
        }
        return false;
    }

    private bool CheckBoardFullInt(int[] tabuleiro) {
        for (int i = 0; i < tabuleiro.Length; i++) {
            if (tabuleiro[i] == 0) {
                return false;
            }
        }
        return true;
    }

    /*
    void Calcula(int a, int b, out int x, out int y)    //como funciona os parametros out
    {
        x = a + b;
        y = a * b;
    }
    */
    //todo REMOVE LATER
    public struct Jogada {
        public int[] tabuleiro;
        public int score;
        public int index;
    }

    //destroy all the elments in the board and reset the positions attributes
    public void ResetStuff() {
        // multiplayer resets
        currentPlayer = 1;
        //SinglePlayer resets
        endGame = false;
        foreach (PositionsMultiPlayer pos in positions) {
            pos.PieceType = -1;
            pos.IsOccupied = false;
        }

        lastPos = -1;
        pieceSpawned = false;

        foreach (Transform ts in parentPiece) {
            GameObject obj = ts.gameObject;
            Destroy(obj);
        }

    }

    public void ChangeAudio(int i) {
        audio.clip = clips[i];
    }

    public void AddPlayer(PlayerActions player) {
        players.Add(player);
        player.playerID = players.Count;
    }
}
