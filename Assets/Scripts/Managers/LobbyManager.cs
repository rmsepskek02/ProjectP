using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    #region �ʵ�
    public TextMeshProUGUI width;
    public TextMeshProUGUI height;
    public TMP_InputField roomNameInputField;
    public TMP_InputField roomPasswordInputField;
    public Transform roomListContent;
    public GameObject roomItemFactory;

    Dictionary<string, RoomInfo> roomCache = new Dictionary<string, RoomInfo>();
    string roomNameText;
    string roomPasswordText;
    #endregion
    void Start()
    {
        OnConnectedToMaster();
    }

    void Update()
    {
        roomNameText = roomNameInputField.text;
        roomPasswordText = roomPasswordInputField.text;

        // ȭ�� ũ�� ǥ��
        width.text = $"{Screen.width}";
        height.text = $"{Screen.height}";
    }

    public void CreateRoom(string roomName, string roomPassword)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "roomName", roomName },
                { "roomPassword", roomPassword }
            },
            CustomRoomPropertiesForLobby = new[] { "roomName", "roomPassword" }
        };

        PhotonNetwork.CreateRoom(roomName + roomPassword, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
    }

    public void JoinRoom(string roomName, string password)
    {
        PhotonNetwork.JoinRoom(roomName + password);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.LoadLevel("GameScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        // �� ��� ����
        UpdateRoomCache(roomList);
        UpdateRoomListUI();
    }

    void UpdateRoomCache(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
                roomCache.Remove(room.Name);
            else
                roomCache[room.Name] = room;
        }
    }

    void UpdateRoomListUI()
    {
        // ���� �� ��� UI �ʱ�ȭ
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }

        // �� ��� UI ������Ʈ
        foreach (RoomInfo roomInfo in roomCache.Values)
        {
            GameObject roomItem = Instantiate(roomItemFactory, roomListContent);
            RoomItem itemComponent = roomItem.GetComponent<RoomItem>();
            itemComponent.SetInfo(roomInfo);

            // ���� ������ �� ó���� ����
            itemComponent.OnClickAction = (string roomName) =>
            {
                roomNameInputField.text = roomName;
            };
        }
    }
    public override void OnConnectedToMaster()
    {
        // ������ ����� �� �κ�� ������ �� �ֽ��ϴ�.
        PhotonNetwork.JoinLobby();
    }
    public void OnClickCreate()
    {
        CreateRoom(roomNameText, roomPasswordText);
    }

    public void OnClickJoin()
    {
        JoinRoom(roomNameText, roomPasswordText);
    }

    public void OnClickRefresh()
    {
        // �κ񿡼� �� ��� ���� ��������
        PhotonNetwork.JoinLobby();  // �κ� �ٽ� �����Ͽ� �� ����� ������Ʈ�մϴ�.
    }
}
