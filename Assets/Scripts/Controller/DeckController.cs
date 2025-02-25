using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckController : MonoBehaviourPunCallbacks
{
    #region 필드
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

    // 덱을 클릭
    // TODO 카드를 몇장 뽑을지, 체력을 몇 소모할지
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

        // 마스터 턴
        if (turnManager.Turn % 2 == 1)
        {
            // 마스터 클릭
            if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[0])
            {
                GameObject go = PhotonNetwork.Instantiate(myPrefabName, mySpawnPosition, Quaternion.identity);
                Transform parent = GameObject.Find(myParentName).transform;
                go.transform.SetParent(parent, false);
                go.GetComponentInChildren<TextMeshProUGUI>().text = cardNumber;

                // 비마스터에게 마스터가 카드가 추가 되었다는 것을 보여줌
                photonView.RPC("SpawnHandCard", RpcTarget.Others, yourPrefabName, yourSpawnPosition, yourParentName);
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[1])
                return;
            else { }

        }
        // 비마스터 턴
        else
        {
            // 비마스터 클릭
            if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[0])
                return;
            else if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[1])
            {
                GameObject go = PhotonNetwork.Instantiate(myPrefabName, mySpawnPosition, Quaternion.identity);
                Transform parent = GameObject.Find(myParentName).transform;
                go.transform.SetParent(parent, false);
                go.GetComponentInChildren<TextMeshProUGUI>().text = cardNumber;

                // 마스터에게 비마스터가 카드가 추가 되었다는 것을 보여줌
                photonView.RPC("SpawnHandCard", RpcTarget.Others, yourPrefabName, yourSpawnPosition, yourParentName);
            }
            else { }
        }
    }

    // 상대방에게 카드 드로우를 알림
    [PunRPC]
    void SpawnHandCard(string prefabName, Vector3 position, string parentName)
    {
        GameObject go = PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);
        Transform parent = GameObject.Find(parentName).transform;
        go.transform.SetParent(parent, false);
    }
}
