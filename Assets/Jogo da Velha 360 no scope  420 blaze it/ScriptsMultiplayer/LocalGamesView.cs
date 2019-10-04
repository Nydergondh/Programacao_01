
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LocalGamesView : MonoBehaviour {

    // Template de botão para cada partida que foi encontrada.
    // Será instanciado na UI um para cada partida.
    public GameObject MatchButtonTemplate;

    // Objetos raiz das hierarquias de UI para cada um dos estados do processo de conexão.
    public GameObject ConnectRoot;
    public GameObject WaitingForPlayersRoot;

    // Período entre atualizações da lista de partidas.
    public float DiscoveryUpdatePeriod = 0.5f;

    // Variáveis privadas de controle.
    private string _matchName;
    private float _timeToUpdate;
    private bool _isConnected;

    // Número máximo de partidas suportadas.
    private const int _maxMatches = 5;

    // Dados internos das partidas atualmente mostradas na UI.
    private List<GameObject> _currentMatches = new List<GameObject>();
    private NetworkBroadcastResult[] _currentMatchesData = new NetworkBroadcastResult[_maxMatches];

    /*
     * INICIALIZAÇÃO
     */
    public void Inicialize() {

        DeleteMatches();
        _isConnected = false;

        ConnectRoot.SetActive(true);
        WaitingForPlayersRoot.SetActive(false);

        // Inicializa o NetworkDiscovery.

        if (MyNetworkManager.Discovery.running) {
            MyNetworkManager.Discovery.StopBroadcast();
        }

        MyNetworkManager.Discovery.Initialize();

        // Pede para o NetworkDiscovery começar a ouvir pacotes de broadcast.
        MyNetworkManager.Discovery.StartAsClient();

        // Cria um botão para cada uma das partidas suportadas.
        MatchButtonTemplate.SetActive(false);
        for (int i = 0; i < _maxMatches; i++) {
            var buttonObject = Instantiate(MatchButtonTemplate, MatchButtonTemplate.transform.parent);
            _currentMatches.Add(buttonObject);

            var button = buttonObject.GetComponent<Button>();

            // Assina o evento de clique do botão, passando o índice correto para o handler.
            // Importante capturar o índice em uma variável separada para que o lambda faça
            // a captura do valor correto.
            int index = i;
            button.onClick.AddListener(() => OnMatchClicked(index));
        }
    }
    //Deleta todos os elementos existentes da lista de partidas e limpa ela caso o jogo reinicie
    private void DeleteMatches() {
        if (_currentMatches.Count > 0) {
            for (int i = 0; i < _maxMatches; i++) {
                Destroy(_currentMatches[i]);
            }
            _currentMatches.Clear();
        }
    }
    

    private void Start()
    {
        MyNetworkManager.onServerConnect += onServerConnect;
        MyNetworkManager.onClientConnect += onClientConnect;
        MyNetworkManager.onClientDisconnect += onClientDisconnect;
        MyNetworkManager.onServerDisconnect += onServerDisconnect;
        //TODO Adicionado por mim (Vinicius) perguntar se necessário
    }

    /*
     * CÓDIGO DO SERVIDOR
     */

    // Cria uma partida. Chamado pelo botão da UI respectivo.
    public void CreateMatch()
    {
        // Desliga o Network Discovery, que estava funcionando como cliente (ouvindo por pacotes).
        // Esse setup inicial foi feito no Start(), acima.
        MyNetworkManager.Discovery.StopBroadcast();

        // Cria um nome aleatório para a nossa partida.
        _matchName = Random.Range(100000, 1000000).ToString();

        // Seta o nome da partida como o dado a ser enviado para as outras máquinas.
        MyNetworkManager.Discovery.broadcastData = _matchName;

        // Inicia o broadcast de dados na rede para as outras máquinas pelo Network Discovery.
        MyNetworkManager.Discovery.StartAsServer();

        // Inicializa o NetworkManager como um Host Server.
        MyNetworkManager.singleton.StartHost();

        // Atualiza a UI para o estado "waiting for players".
        ConnectRoot.SetActive(false);
        WaitingForPlayersRoot.SetActive(true);

        // Seta o estado interno para "estamos conectados", afinal nós mesmos somos o servidor :)
        _isConnected = true;
    }

    // Chamada pelo NetworkManager quando um cliente conecta, através da assinatura feita no Start().
    public void onServerConnect(NetworkConnection conn)
    {
        // Atualiza o estado da UI para "in game".
        WaitingForPlayersRoot.SetActive(false);

        //TODO adicionar algo para começar o jogo realmente (Ligar o Board ETC...)
        // Neste momento seria enviado alguma mensagem de "iniciar partida",
        // carregamento de cena de gameplay etc.

        //adjusting the screen to start the game
        CanvasProcess.instance.StartGame();
    }
    
    public void onServerDisconnect(NetworkConnection conn) {
        _isConnected = false;
        print("Server Disconect");
    }
    

    /*
     * CÓDIGO DO CLIENTE
     */

    //TODO questionar se necessário
    public void onClientConnect(NetworkConnection conn) {
        print("GotHere");
        // Atualiza o estado da UI para "in game".
        WaitingForPlayersRoot.SetActive(false);

        //TODO adicionar algo para começar o jogo realmente (Ligar o Board ETC...)
        // Neste momento seria enviado alguma mensagem de "iniciar partida",
        // carregamento de cena de gameplay etc.

        //adjust the screen to the game
        CanvasProcess.instance.StartGame();
    }

    public void onClientDisconnect(NetworkConnection info) {
        _isConnected = false;
        print("Client Disconect");
    }


    // Chamada quando um botão relacionado a uma partida é clicado.
    private void OnMatchClicked(int index)
    {
        // Acessa os dados correspondentes à partida mapeada pelo botão.
        var matchData = _currentMatchesData[index];

        // Chama o NetworManager para fazer a conexão com o servidor. O endereço
        // do servidor está dentro da estrutura que é retornada pelo Network Discovery.
        MyNetworkManager.singleton.networkAddress = matchData.serverAddress;
        //MyNetworkManager.singleton.networkPort = ... // <-- Caso for usar uma porta diferente
        MyNetworkManager.singleton.StartClient();

        // Desliga o Network Discovery, não precisamos mais dele.
        MyNetworkManager.Discovery.StopBroadcast();

        // Marca nosso estado interno como "conectados".
        _isConnected = true;

        // Atualiza a UI para o estado "conectado".
        ConnectRoot.SetActive(false);
        //TODO adicionar algo para começar o jogo realmente (Ligar o Board ETC...)
    }

    private void Update()
    {
        if (!_isConnected)
        {
            // Atualiza o contador para fazer o Refresh das partidas atuais.
            _timeToUpdate -= Time.deltaTime;
            if (_timeToUpdate < 0)
            {
                // Chama o refresh de partidas.
                RefreshMatches();

                _timeToUpdate = DiscoveryUpdatePeriod;
            }
        }
    }

    // Lista temporária utilizada para ler as partidas encontradas pelo Network Discovery.
    private List<NetworkBroadcastResult> _matches = new List<NetworkBroadcastResult>();

    // Atualiza as partidas conhecidas através do Network Discovery.
    private void RefreshMatches()
    {
        // Atualiza a lista temporária de partidas a partir das partidas contidas nos dados do Network Discovery.
        _matches.Clear();
        foreach (var match in MyNetworkManager.Discovery.broadcastsReceived.Values)
        {
            // Remove duplicatas (acontece em loopback quando a máquina tem mais de uma placa de rede)
            if(_matches.Any(item => EqualsArray(item.broadcastData, match.broadcastData)))
            {
                continue;
            }

            // Adiciona a partida
            _matches.Add(match);
        }

        // Percorre as partidas conhecidas e atualiza a lista de botões de acordo.
        int i = 0;
        foreach(var match in _matches)
        {
            // Suportamos apenas '_maxMatches' como número máximo de partidas.
            // Lendo mais do que isso causaria um erro no array de botões pré-instanciados.
            if (i >= _maxMatches)
            {
                break;
            }

            // Lê o nome da partida dentro dos dados recebidos no broadcast do Network Discovery.
            // Esta conversão é necessária pois no servidor o que se seta é uma string, e do
            // outro lado esse dado se apresenta como um array de bytes ¯\_(ツ)_/¯
            string matchName = Encoding.Unicode.GetString(match.broadcastData);

            _currentMatchesData[i] = match;

            _currentMatches[i].SetActive(true);
            _currentMatches[i].GetComponentInChildren<Text>().text = "Join match: " + matchName;
            i++;
        }
        for(;i < _currentMatches.Count; i++)
        {
            _currentMatches[i].SetActive(false);
        }
    }

    // Método auxiliar para comparar 2 arrays de bytes.
    private bool EqualsArray(byte[] left, byte[] right)
    {
        if (left.Length != right.Length)
        {
            return false;
        }
        for (int i = 0; i < left.Length; i++)
        {
            if (left[i] != right[i])
            {
                return false;
            }
        }
        return true;
    }

}
