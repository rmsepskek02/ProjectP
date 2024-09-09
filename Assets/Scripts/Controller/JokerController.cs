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

        // ����� Joker Card �ı�
        PhotonNetwork.Destroy(im.myHandCardList.transform.GetChild(im.clickedMyCardIdx).gameObject);
        // �ı��Ȱ� �˸�
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

    // ���濡�� �ڵ�ī�� ���Ÿ� �˸�
    [PunRPC]
    void RemoveHandCard(string parentName)
    {
        Transform parent = GameObject.Find(parentName).transform;
        PhotonNetwork.Destroy(parent.GetChild(0).gameObject);
    }
}