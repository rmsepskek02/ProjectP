using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks
{
    #region ÇÊµå
    public TextMeshProUGUI nickName;

    protected InGameManager im;
    List<string> myDeck = new List<string>();
    List<string> myHand= new List<string>();
    GameObject uiInGame;
    GameObject myDeckButton;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        nickName.text = photonView.Owner.NickName;
        im = InGameManager.instance;
        uiInGame = GameObject.Find("UiInGame");
        myDeckButton = GameObject.Find("MyDeck");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
