using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckController : MonoBehaviourPunCallbacks
{
    #region �ʵ�
    public GameObject myCardItem;
    public bool enableDraw = true;

    InGameManager im;
    PunTurnManager turnManager;
    #endregion
    void Start()
    {
        im = InGameManager.instance;
        turnManager = im.GetComponent<PunTurnManager>();
    }

    // ���� Ŭ��
    // TODO ī�带 ���� ������, ü���� �� �Ҹ�����
    public void OnClickDeck()
    {
        if (enableDraw == false) return;

        enableDraw = false;
        int playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        DrawCard(im.RemoveCardForDeck(playerNumber));
        DrawCard(im.RemoveCardForDeck(playerNumber));
        DrawCard(im.RemoveCardForDeck(playerNumber));
    }

    public void DrawCard(string cardNumber)
    {
        if (im.myHandCardList.transform.childCount >= 9) return;
        if (cardNumber == "") return;

        string myPrefabName = "Prefabs/Card/OpenCard";
        string yourPrefabName = "Prefabs/Card/SecretCard";
        string myParentName = im.myHandCardList.name;
        Vector3 mySpawnPosition = im.myHandCardList.transform.position;
        Vector3 yourSpawnPosition = im.yourHandCardList.transform.position;
        string yourParentName = im.yourHandCardList.name;

        // ������ ��
        if (turnManager.Turn % 2 == 1)
        {
            // ������ Ŭ��
            if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[0])
            {
                GameObject go = PhotonNetwork.Instantiate(myPrefabName, mySpawnPosition, Quaternion.identity);
                Transform parent = GameObject.Find(myParentName).transform;
                go.transform.SetParent(parent, false);
                go.GetComponentInChildren<TextMeshProUGUI>().text = cardNumber;

                // �񸶽��Ϳ��� �����Ͱ� ī�尡 �߰� �Ǿ��ٴ� ���� ������
                photonView.RPC("SpawnHandCard", RpcTarget.Others, yourPrefabName, yourSpawnPosition, yourParentName);
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[1])
                return;
            else { }

        }
        // �񸶽��� ��
        else
        {
            // �񸶽��� Ŭ��
            if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[0])
                return;
            else if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[1])
            {
                GameObject go = PhotonNetwork.Instantiate(myPrefabName, mySpawnPosition, Quaternion.identity);
                Transform parent = GameObject.Find(myParentName).transform;
                go.transform.SetParent(parent, false);
                go.GetComponentInChildren<TextMeshProUGUI>().text = cardNumber;

                // �����Ϳ��� �񸶽��Ͱ� ī�尡 �߰� �Ǿ��ٴ� ���� ������
                photonView.RPC("SpawnHandCard", RpcTarget.Others, yourPrefabName, yourSpawnPosition, yourParentName);
            }
            else { }
        }
    }

    // ���濡�� ī�� ��ο츦 �˸�
    [PunRPC]
    void SpawnHandCard(string prefabName, Vector3 position, string parentName)
    {
        GameObject go = PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);
        Transform parent = GameObject.Find(parentName).transform;
        go.transform.SetParent(parent, false);
    }
}
