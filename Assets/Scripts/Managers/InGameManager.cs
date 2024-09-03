using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine.UI;

public class InGameManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
{
    public static InGameManager instance;
    public PunTurnManager turnManager;
    public List<int> playerList = new List<int>();
    public List<string> playerADeck;
    public List<string> playerBDeck;
    public List<string> playerAHand;
    public List<string> playerBHand;
    public GameObject myHandCardList;
    public GameObject myFieldCardList;
    public GameObject yourHandCardList;
    public GameObject yourFieldCardList;
    public Button startButton;
    public Button endButton;
    public bool isStart;
    public GameObject uiInGame;
    public GameObject choice;
    public int clickedMyCardIdx;
    public string clickedMyCardNumber;
    public int clickedYourCardIdx;
    public string clickedYourCardNumber;
    DeckController dc;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        dc = uiInGame.transform.Find("MyDeck").GetComponent<DeckController>();
        turnManager = GetComponent<PunTurnManager>();
        turnManager.TurnManagerListener = this;
        choice.SetActive(false);
        InitRoom();
        CheckPlayers();
    }
    void Update()
    {

    }
    // ������ ����
    private void StartTurn()
    {
        startButton.gameObject.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            turnManager.BeginTurn();
        }
    }

    public string RemoveCardForDeck(int playerNumber)
    {
        string drawCard = "";
        if (playerNumber == playerList[0])
        {
            if (playerAHand.Count >= 8) return drawCard;
            int randomIndex = Random.Range(0, playerADeck.Count); // 0���� playerBDeck.Count-1������ ���� �ε���
            drawCard = playerADeck[randomIndex]; // �ش� �ε����� ī�� ����
            playerAHand.Add(drawCard);
            playerADeck.RemoveAt(randomIndex); // ����Ʈ���� ī�� ����
            return drawCard;
        }
        else if (playerNumber == playerList[1])
        {
            if (playerBHand.Count >= 8) return drawCard;
            int randomIndex = Random.Range(0, playerBDeck.Count); // 0���� playerBDeck.Count-1������ ���� �ε���
            drawCard = playerBDeck[randomIndex]; // �ش� �ε����� ī�� ����
            playerBHand.Add(drawCard);
            playerBDeck.RemoveAt(randomIndex); // ����Ʈ���� ī�� ����
            return drawCard;
        }
        else
        {
            return drawCard;
        }
    }

    // IPunTurnManagerCallbacks �������̽� ����
    public void OnTurnBegins(int turn)
    {
        int playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        string removeCard;
        if (turn % 2 == 1)
        {
            // ������ �� ����
            if (PhotonNetwork.IsMasterClient)
            {
                endButton.interactable = true;
                dc.enableDraw = true;
                removeCard = RemoveCardForDeck(playerNumber);
                dc.DrawCard(removeCard);
            }
            else
            {
                endButton.interactable = false;
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                endButton.interactable = false;
            }
            // �񸶽��� �� ����
            else
            {
                endButton.interactable = true;
                dc.enableDraw = true;
                removeCard = RemoveCardForDeck(playerNumber);
                dc.DrawCard(removeCard);
            }
        }
    }

    public void OnTurnCompleted(int turn)
    {
        Debug.Log("Turn " + turn + " completed!");
        //StartTurn();
    }

    public void OnPlayerMove(Photon.Realtime.Player player, int turn, object move)
    {
        // ���� �÷��̾ �������� �� ȣ��˴ϴ�.
        Debug.Log(player.NickName + " Move ");
    }

    public void OnPlayerFinished(Photon.Realtime.Player player, int turn, object move)
    {
        // ���� �÷��̾ ���� �������� �� ȣ��˴ϴ�.
        Debug.Log(player.NickName + " finished turn " + turn);
    }

    public void OnTurnTimeEnds(int turn)
    {
        // �� Ÿ�̸Ӱ� ����Ǿ��� �� ȣ��˴ϴ�.
    }

    // ������ Join ���� �� ȣ��
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New Player Entered Room: " + newPlayer.NickName);
        CheckPlayers();
    }

    // ���� �濡 �ִ� ���� ���� Ȯ��
    private void CheckPlayers()
    {
        playerList.Clear();
        foreach (var player in PhotonNetwork.PlayerList)
        {
            playerList.Add(player.ActorNumber);
            Debug.Log("PLAYERLIST = " + playerList);
            Debug.Log("PLAYERLIST A = " + player.ActorNumber);
        }
        if (playerList.Count == 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                startButton.gameObject.SetActive(true);
                startButton.interactable = true;
            }
            else
            {
                startButton.gameObject.SetActive(false);
            }
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                startButton.gameObject.SetActive(true);
                startButton.interactable = false;
            }
            else
            {
                startButton.gameObject.SetActive(false);
            }
        }
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log("Player " + player.NickName + " has ActorNumber: " + player.ActorNumber);
        }
    }

    // �� ������ ��ư�� ������ �޼���
    public void LeaveRoom()
    {
        // �� ������ �õ�
        PhotonNetwork.LeaveRoom();
    }

    // ���� ���������� ������ �� ȣ��Ǵ� �ݹ�
    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("LobbyScene");  // �κ� ������ ��ȯ
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log("�÷��̾ ���� �������ϴ�: " + otherPlayer.NickName);
        CheckPlayers();
        InitRoom();
    }
    // ������ �����ϴ� ��ư�� Ŭ�� �̺�Ʈ
    public void OnClickStart()
    {
        isStart = true;
        Debug.Log("isStart = " + isStart);
        StartTurn();
    }
    public void OnClickEnd()
    {
        turnManager.SendMove(null, true);
        Debug.Log("onClick End");
        turnManager.BeginTurn();
    }
    // ī�� ���� �ʱ�ȭ
    public void InitClickedCard()
    {
        clickedMyCardIdx = 0;
        clickedMyCardNumber = "";
        clickedYourCardIdx = 0;
        clickedYourCardNumber = "";
    }
    // �� �ʱ�ȭ
    void InitRoom()
    {
        // TODO �ڵ�� �ʵ�� ���ʱ�ȭ, ���� �����ʱ�ȭ
        isStart = false;
        dc.enableDraw = false;
        endButton.interactable = false;
        playerADeck = new List<string> { "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "+", "+", "-", "-", "X", "X", "%", "%", "Joker", "Joker", };
        playerBDeck = new List<string> { "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "+", "+", "-", "-", "X", "X", "%", "%", "Joker", "Joker", };
    }
}
