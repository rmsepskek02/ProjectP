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
    // 게임을 시작
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
            int randomIndex = Random.Range(0, playerADeck.Count); // 0부터 playerBDeck.Count-1까지의 랜덤 인덱스
            drawCard = playerADeck[randomIndex]; // 해당 인덱스의 카드 선택
            playerAHand.Add(drawCard);
            playerADeck.RemoveAt(randomIndex); // 리스트에서 카드 제거
            return drawCard;
        }
        else if (playerNumber == playerList[1])
        {
            if (playerBHand.Count >= 8) return drawCard;
            int randomIndex = Random.Range(0, playerBDeck.Count); // 0부터 playerBDeck.Count-1까지의 랜덤 인덱스
            drawCard = playerBDeck[randomIndex]; // 해당 인덱스의 카드 선택
            playerBHand.Add(drawCard);
            playerBDeck.RemoveAt(randomIndex); // 리스트에서 카드 제거
            return drawCard;
        }
        else
        {
            return drawCard;
        }
    }

    // IPunTurnManagerCallbacks 인터페이스 구현
    public void OnTurnBegins(int turn)
    {
        int playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        string removeCard;
        if (turn % 2 == 1)
        {
            // 마스터 턴 시작
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
            // 비마스터 턴 시작
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
        // 상대방 플레이어가 움직였을 때 호출됩니다.
        Debug.Log(player.NickName + " Move ");
    }

    public void OnPlayerFinished(Photon.Realtime.Player player, int turn, object move)
    {
        // 상대방 플레이어가 턴을 종료했을 때 호출됩니다.
        Debug.Log(player.NickName + " finished turn " + turn);
    }

    public void OnTurnTimeEnds(int turn)
    {
        // 턴 타이머가 종료되었을 때 호출됩니다.
    }

    // 유저가 Join 했을 때 호출
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New Player Entered Room: " + newPlayer.NickName);
        CheckPlayers();
    }

    // 현재 방에 있는 유저 숫자 확인
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

    // 방 나가기 버튼에 연결할 메서드
    public void LeaveRoom()
    {
        // 방 나가기 시도
        PhotonNetwork.LeaveRoom();
    }

    // 방을 성공적으로 나갔을 때 호출되는 콜백
    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("LobbyScene");  // 로비 씬으로 전환
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log("플레이어가 방을 나갔습니다: " + otherPlayer.NickName);
        CheckPlayers();
        InitRoom();
    }
    // 게임을 시작하는 버튼의 클릭 이벤트
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
    // 카드 선택 초기화
    public void InitClickedCard()
    {
        clickedMyCardIdx = 0;
        clickedMyCardNumber = "";
        clickedYourCardIdx = 0;
        clickedYourCardNumber = "";
    }
    // 방 초기화
    void InitRoom()
    {
        // TODO 핸드랑 필드랑 덱초기화, 게임 포톤초기화
        isStart = false;
        dc.enableDraw = false;
        endButton.interactable = false;
        playerADeck = new List<string> { "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "+", "+", "-", "-", "X", "X", "%", "%", "Joker", "Joker", };
        playerBDeck = new List<string> { "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "1", "2", "3", "4", "5", "+", "+", "-", "-", "X", "X", "%", "%", "Joker", "Joker", };
    }
}
