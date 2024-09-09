using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JokerController : MonoBehaviourPunCallbacks
{
    InGameManager im;
    DeckController dc;
    // Start is called before the first frame update
    void Start()
    {
        im = InGameManager.instance;
        dc = im.uiInGame.transform.Find("MyDeck").GetComponent<DeckController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickDraw()
    {
        int playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        string removeCard1;
        string removeCard2;
        string removeCard3;
        removeCard1 = im.RemoveCardForDeck(playerNumber);
        removeCard2 = im.RemoveCardForDeck(playerNumber);
        removeCard3 = im.RemoveCardForDeck(playerNumber);
        dc.DrawCard(removeCard1);
        dc.DrawCard(removeCard2);
        dc.DrawCard(removeCard3);

        string yourHandParentName = im.yourHandCardList.name;

        // 사용한 Joker Card 파괴
        PhotonNetwork.Destroy(im.myHandCardList.transform.GetChild(im.clickedMyCardIdx).gameObject);
        // 파괴된거 알림
        photonView.RPC("RemoveHandCard", RpcTarget.Others, yourHandParentName);

        gameObject.SetActive(false);
    }
    public void OnClickCopy()
    {
        Debug.Log("SELECT OnClickCopy");

        gameObject.SetActive(false);
    }
    public void OnClickDelete()
    {
        Debug.Log("SELECT OnClickDelete");

        gameObject.SetActive(false);
    }
    public void OnClickCancel()
    {
        gameObject.SetActive(false);
    }

    // 상대방에게 핸드카드 제거를 알림
    [PunRPC]
    void RemoveHandCard(string parentName)
    {
        Transform parent = GameObject.Find(parentName).transform;
        PhotonNetwork.Destroy(parent.GetChild(0).gameObject);
    }
}