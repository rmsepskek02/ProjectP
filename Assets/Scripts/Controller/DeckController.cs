using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckController : MonoBehaviourPunCallbacks
{
    public GameObject myCardItem;
    InGameManager im;
    PunTurnManager turnManager;

    public bool enableDraw = true;
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
        if (cardNumber == "") return;
        // ������ ��
        if (turnManager.Turn % 2 == 1)
        {
            // ������ Ŭ��
            if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[0])
            {
                string yourPrefabName = "Prefabs/Card/YourCard";
                Vector3 yourSpawnPosition = im.yourHandCardList.transform.position;
                string yourParentName = im.yourHandCardList.name;

                // ������ ī�� �߰�
                GameObject go = Instantiate(myCardItem, im.myHandCardList.transform);
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
                string yourPrefabName = "Prefabs/Card/YourCard";
                Vector3 yourSpawnPosition = im.yourHandCardList.transform.position;
                string yourParentName = im.yourHandCardList.name;

                // �񸶽��� ī�� �߰�
                GameObject go = Instantiate(myCardItem, im.myHandCardList.transform);
                go.GetComponentInChildren<TextMeshProUGUI>().text = cardNumber;

                // �����Ϳ��� �񸶽��Ͱ� ī�尡 �߰� �Ǿ��ٴ� ���� ������
                photonView.RPC("SpawnHandCard", RpcTarget.MasterClient, yourPrefabName, yourSpawnPosition, yourParentName);
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
        go.transform.SetParent(parent, false); // false�� �����Ͽ� ��ġ�� ȸ���� ����
    }
}
