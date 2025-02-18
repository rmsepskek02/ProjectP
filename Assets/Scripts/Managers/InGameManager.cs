using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine.UI;

public class InGameManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
{
    #region �ʵ�
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
    public GameObject joker;
    public int clickedMyCardIdx;
    public string clickedMyCardNumber;
    public int clickedYourCardIdx;
    public string clickedYourCardNumber;
    public string firstCardNumber = "";
    public Transform firstCard;
    public string secondCardNumber = "";
    public bool isCopy;
    public bool isDelete;
    public bool isPlus;
    public bool isMinus;
    public bool isMultiple;
    public bool isDivision;

    DeckController dc;
    #endregion
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
        joker.SetActive(false);
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
        ResetYourFieldCardColor();
        ResetMyFieldCardColor();
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
    // �� �ʵ�ī�� ���� �ʱ�ȭ
    public void ResetMyFieldCardColor()
    {
        foreach (Transform child in myFieldCardList.transform)
        {
            Color childColor = child.GetComponent<Image>().color;
            Color openColor = Global.Colors.ChangeColor(Global.Colors.OpenColor);
            Color secretColor = Global.Colors.ChangeColor(Global.Colors.SecretColor);
            if (childColor != openColor && childColor != secretColor)
            {
                child.GetComponent<Image>().color = child.GetComponent<CardController>().originColor;
            }
        }
    }
    // ��� �ʵ�ī�� ���� �ʱ�ȭ
    public void ResetYourFieldCardColor()
    {
        foreach (Transform child in yourFieldCardList.transform)
        {
            if (child.GetChild(0).gameObject.activeSelf)
                child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.WhiteColor);
            else
                child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.SecretColor);
        }
    }
    // �� �ʱ�ȭ
    void InitRoom()
    {
        // TODO ���� �����ʱ�ȭ
        isStart = false;
        dc.enableDraw = false;
        endButton.interactable = false;
        playerADeck = new List<string> { 
            "1", "2", "3", "4", "5", 
            "1", "2", "3", "4", "5", 
            //"1", "2", "3", "4", "5", 
            //"1", "2", "3", "4", "5", 
            "+", "+", "-", "-", "X", "X", "%", "%", 
            //"Joker", "Joker", 
        };
        playerBDeck = new List<string> { 
            "1", "2", "3", "4", "5", "" +
            "1", "2", "3", "4", "5", 
            //"1", "2", "3", "4", "5", 
            //"1", "2", "3", "4", "5",
            "+", "+", "-", "-", "X", "X", "%", "%",
            //"Joker", "Joker", 
        };
        DestroyChild(myHandCardList);
        DestroyChild(myFieldCardList);
        DestroyChild(yourHandCardList);
        DestroyChild(yourFieldCardList);
    }
    void DestroyChild(GameObject go)
    {
        foreach (Transform child in go.transform)
        {
            Destroy(child.gameObject);
        }
    }
}