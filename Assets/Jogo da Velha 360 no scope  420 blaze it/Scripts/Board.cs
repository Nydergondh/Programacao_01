using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class manages the game state
public class Board : MonoBehaviour
{

    public static Board instance = null;

    List<Position> positions;

    [SerializeField] GameObject xObject = null;
    [SerializeField] GameObject oObject = null;
    [SerializeField] Transform parentPiece = null;

    public Canvas canvas = null;
    public AudioSource audio = null;
    public AudioClip[] clips;

    public bool isMultiplayer;
    public bool isPlayerTurn;
    public bool pieceSpawned;
    public bool endGame;

    private bool waitAIPlay;

    int bestPlay; //saves the IA best play at minimax
    int difficulty;
    int currentPlayer = 1;

    public delegate void OnPieceSpawned();
    public OnPieceSpawned onPieceSpawned;

    void Awake() {
        if (instance == null) {
            instance = this;
        }

        else if (instance != this) {
            Destroy(gameObject);
        }

        difficulty = 3;
        positions = new List<Position>();

        isMultiplayer = false;
        pieceSpawned = false;
        waitAIPlay = false;
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
        if (!isMultiplayer) {


            if (pieceSpawned && !endGame && isPlayerTurn) {

                int pieceType = 1;
                bestPlay = Position.lastPos;
                int[] tabuleiro = CheckEmptyPos();
                PlayInPosition(tabuleiro, pieceType);

            }
            //TODO CHECK THIS COUROTINE

            else if (!isPlayerTurn && !endGame) {

                bool smartPlay = CheckDifficulty();  // check if the AI gonna play smart or Dumb
                if (!waitAIPlay) {
                    waitAIPlay = true;
                    StartCoroutine(WaitForAIPlay(smartPlay));
                }
            }
        }
        else {

            if (pieceSpawned && !endGame && currentPlayer == 1) {

                int pieceType = 1;
                int[] tabuleiro = CheckEmptyPos();
                PlayInPosition(tabuleiro, currentPlayer);

            }
            else if(pieceSpawned && !endGame && currentPlayer == 2) {
                int pieceType = 1;
                int[] tabuleiro = CheckEmptyPos();
                PlayInPosition(tabuleiro, currentPlayer);
            }
            
        }

    }

    private IEnumerator WaitForAIPlay(bool smartP) {
        yield return new WaitForSeconds(0.5f);
        waitAIPlay = false;
        MakePlayAI(smartP);
    }

    private void MakePlayAI(bool smartPlay) {

        int pieceType = 2; // set the piece type to the AI piece type

        //JOGADA INTELIGENTE
        if (smartPlay) {
            //faz um tabuleiro virtual para rodar os testes
            int[] tabuleiro = CheckEmptyPos();
            // faz coisas pra desobrir onde jogar
            MinMax(tabuleiro, isPlayerTurn); 
            PlayInPosition(tabuleiro, pieceType);
        }

        //JOGADA BURRA
        else {
            int[] tabuleiro = CheckEmptyPos();
            //faz um tabuleiro virtual para rodar os testes     

            int[] freePositions = EmptyPositions(tabuleiro);
            bestPlay = freePositions[Random.Range(0, freePositions.Length)];

            PlayInPosition(tabuleiro, pieceType);
        }

    }

    private void PlayInPosition(int[] tabuleiro, int pieceType) {

        foreach (Position pos in positions) {

            if (bestPlay == pos.GetBoardLocation()) {
                pos.IsOccupied = true;
                pos.PieceType = pieceType;
                tabuleiro[bestPlay] = pieceType;
                break;
            }
        }

        Position.lastPos = bestPlay;

        //Spawns new piece and update the variables to make the player play the game
        PutPiece(positions[Position.lastPos].transform.position);

        if (CheckWinInt(positions[Position.lastPos].PieceType, tabuleiro) && pieceType == 2) { //check if the player has won

            Debug.Log("AI Won!");
            endGame = true;
            ChangeAudio(1);
            canvas.GetComponent<CanvasProcess>().thatsAllFolks(endGame);
        }

        else if (CheckWinInt(positions[Position.lastPos].PieceType, tabuleiro) && pieceType == 1) {
            Debug.Log("Player Won");
            endGame = true;
            ChangeAudio(1);
            canvas.GetComponent<CanvasProcess>().thatsAllFolks(endGame);
        }


        else if (!CheckWinInt(positions[Position.lastPos].PieceType, tabuleiro) && CheckBoardFullInt(tabuleiro)) {
            Debug.Log("Deu Velha");
            endGame = true;
            ChangeAudio(1);
            canvas.GetComponent<CanvasProcess>().thatsAllFolks(endGame);
        }

        //check if is player turn to change spawned piece (function that controls when to wait for player interaction)
        if (pieceType == 1) {
            pieceSpawned = !pieceSpawned;
        }
        isPlayerTurn = !isPlayerTurn;

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

        if (isPlayerTurn) {
            Instantiate(xObject, piecePos, Quaternion.identity, parentPiece);
        }
        else {
            Instantiate(oObject, piecePos, Quaternion.identity, parentPiece);//Run MinMax Algorithm and place the piece in the best place
        }
        audio.Play();

    }

    void PieceSpawned() {
        if (isPlayerTurn) {
            pieceSpawned = !pieceSpawned;
        }        
    }

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
    void Calcula(int a, int b, out int x, out int y)    //como funciona os parametros out
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
    //destroy all the elments in the board and reset the positions attributes
    public void ResetStuff() {
        isPlayerTurn = true;
        endGame = false;
        foreach (Position pos in positions) {
            pos.PieceType = -1;
            pos.IsOccupied = false;
        }

        Position.lastPos = -1;

        foreach (Transform ts in parentPiece) {
            GameObject obj = ts.gameObject;
            Destroy(obj);                
        }
        
    }

    public void SetDiffculty(int d) {
        difficulty = d;
    }

    private bool CheckDifficulty() {
        //80% de chance de ser burro
        if (difficulty == 0) {
            if (Random.Range(0, 10) < 8) {
                return false;
            }
            else {
                return true;
            }
        }
        //50% de chance de ser burro
        else if (difficulty == 1) {
            if (Random.Range(0, 10) < 5) {
                return false;
            }
            else {
                return true;
            }
        }
        //20% de chance de ser burro
        else if (difficulty == 2) {
            if (Random.Range(0, 10) < 2) {
                return false;
            }
            else {
                return true;
            }
        }
        //0% de chance de ser burro
        else if (difficulty == 3) {
            return true;            
        }

        else {
            return false;
        }

    }

    public void ChangeAudio(int i) {

        audio.clip = clips[i];

    }
}
