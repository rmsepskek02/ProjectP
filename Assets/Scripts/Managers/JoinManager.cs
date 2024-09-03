using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class JoinManager : MonoBehaviourPunCallbacks
{
    public Button joinButton;
    public TMP_InputField inputId;
    public TMP_InputField inputPassword;
    // Start is called before the first frame update
    void Start()
    {
        // ���� ����
        // AppId, ����, ������ ��û
        // ��û�ϰ� �޾ƾ� ��
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ������ ���� ���� ������ ȣ�� (Lobby�� ������ �� ���� ����)
    public override void OnConnected()
    {
        base.OnConnected();
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);
    }

    // ������ ���� ���� ������ ȣ�� (Lobby�� ������ �� �ִ� ���� �� �� ȣ�Ⱑ��)
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        //Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

        
    }

    // �κ� ���� ������ ȣ��
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

        PhotonNetwork.NickName = inputId.text;
        //Debug.Log("TEXT = " + inputId.text);
        // �κ�� �̵�
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public void OnClickButton()
    {
        // �κ� ���� ��û
        PhotonNetwork.JoinLobby();
    }
}
