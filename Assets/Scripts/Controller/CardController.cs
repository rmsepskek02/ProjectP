using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviourPunCallbacks
{
    Transform myFieldCardListContent;
    Transform yourFieldCardListContent;
    InGameManager im;
    PunTurnManager turnManager;
    public string cardPosition;
    bool isAttack = false;
    // Start is called before the first frame update
    void Start()
    {
        im = InGameManager.instance;
        turnManager = im.GetComponent<PunTurnManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickCard()
    {
        if (turnManager.Turn % 2 == 1)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[0])
            {
                if (CheckCardPosition() == "MyHand")
                {
                    im.clickedMyCardIdx = gameObject.transform.GetSiblingIndex();
                    im.clickedMyCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                    if (CheckIsNumber(im.clickedMyCardNumber) == false)
                    {
                        // TODO ��Ģ����, JOKER ó��
                        Debug.Log("MyHand NOT NUMBER");
                        return;
                    }
                    else
                    {
                        im.choice.SetActive(true);
                    }
                }
                else if (CheckCardPosition() == "MyField")
                {
                    // TODO �ٸ� ī�带 �����ϵ��� ����
                    // TODO �ѹ� �� ������ ���
                    isAttack = !isAttack;
                    if (isAttack)
                    {
                        // TODO ���� �غ� ����Ʈ
                        im.clickedMyCardIdx = gameObject.transform.GetSiblingIndex();
                        im.clickedMyCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                    }
                    else
                    {
                        im.clickedMyCardNumber = "";
                    }
                    Debug.Log("MyField");
                }
                else if (CheckCardPosition() == "YourField")
                {
                    Debug.Log("YourField");
                    im.clickedYourCardIdx = gameObject.transform.GetSiblingIndex();
                    im.clickedYourCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

                    if (im.clickedMyCardNumber != "")
                    {
                        // TODO ī�� ����
                        Debug.Log("CARD FIGHT");
                        // Secret Ȯ��
                        if (gameObject.transform.GetChild(0).gameObject.activeSelf == false)
                        {
                            gameObject.transform.GetChild(0).gameObject.SetActive(true);
                            Debug.Log("SECRET OPEN");
                        }
                        int myNumber = int.Parse(im.clickedMyCardNumber);
                        int yourNumber = int.Parse(im.clickedYourCardNumber);
                        int result = (myNumber - yourNumber);
                        FightCard(result, im.clickedMyCardIdx, im.clickedYourCardIdx);
                    }
                }
                else
                {
                    Debug.Log("ELSE");
                    cardPosition = "";
                }
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[1])
                return;
            else { }
        }
        else
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[0])
                return;
            else if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[1])
            {
                im.clickedMyCardIdx = gameObject.transform.GetSiblingIndex();
                im.clickedMyCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
                if (CheckCardPosition() == "MyHand")
                {
                    if (CheckIsNumber(im.clickedMyCardNumber) == false)
                    {
                        // TODO ��Ģ����, JOKER ó��
                        return;
                    }
                    else
                    {
                        im.choice.SetActive(true);
                    }
                }
                else if (CheckCardPosition() == "MyField")
                {
                    // TODO �ٸ� ī�带 �����ϵ��� ����
                }
                else if (CheckCardPosition() == "YourField")
                {

                }
                else
                {
                    cardPosition = "";
                }
            }
            else { }
        }
        // ī�� Ŭ���ϸ�
        // �������� ��� ���� ������� - UI�۾�

        // ��Ұ��ƴ� ���
        // �ʵ�� �ű�� - transform ���� 
        // ������ �˸��� - ���ȭ�鿡 �� ī�尡 �Ѱ��ٰ�, �ʵ忡 �ϳ� ����
    }

    // ������ ī�尡 ��� �ִ��� �ʵ忡 �ִ��� ����
    public string CheckCardPosition()
    {
        if (gameObject.transform.parent.name == "HorizontalMyCard")
            cardPosition = "MyHand";
        else if (gameObject.transform.parent.name == "HorizontalMyFieldCard")
            cardPosition = "MyField";
        else if (gameObject.transform.parent.name == "HorizontalYourFieldCard")
            cardPosition = "YourField";
        return cardPosition;
    }

    // ī���� �ؽ�Ʈ Ȯ��
    bool CheckIsNumber(string text)
    {
        return int.TryParse(text, out _);
    }
    
    // ī�� ���� �ʱ�ȭ
    void InitClickedCard()
    {
        im.clickedMyCardIdx = 0;
        im.clickedMyCardNumber= "";
        im.clickedYourCardIdx = 0;
        im.clickedYourCardNumber= "";
    }
    // ī�� ����
    void FightCard(int _result, int myCardIdx, int yourCardIdx)
    {
        string myCardParentName = im.myFieldCardList.name;
        string yourCardParentName = im.yourFieldCardList.name;
        GameObject myFieldCard = im.myFieldCardList.transform.GetChild(myCardIdx).gameObject;
        GameObject yourFieldCard = im.yourFieldCardList.transform.GetChild(yourCardIdx).gameObject;

        if (_result > 0)
        {
            // ��ī�� ��Ģ���� ��� ����
            photonView.RPC("ChangeFieldCard", RpcTarget.Others, yourCardParentName, myCardIdx, _result);
            myFieldCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _result.ToString();
            // ���ī�� �ı�
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, myCardParentName, yourCardIdx);
            Destroy(yourFieldCard);
            Debug.Log("WIN");
        }
        else if (_result < 0)
        {
            // �� ī�� �ı�
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, yourCardParentName, myCardIdx);
            Destroy(myFieldCard);
            // ���ī�� ��Ģ���� ��� ����
            photonView.RPC("ChangeFieldCard", RpcTarget.Others, myCardParentName, yourCardIdx, Mathf.Abs(_result));
            yourFieldCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mathf.Abs(_result).ToString();
            Debug.Log("LOOSE");
        }
        else
        {
            // �� ī�� �ı�
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, yourCardParentName, myCardIdx);
            Destroy(myFieldCard);
            // ���ī�� �ı�
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, myCardParentName, yourCardIdx);
            Destroy(yourFieldCard);
            Debug.Log("DRAW");
        }
        // Ŭ���� ī�� ������ �ʱ�ȭ
        InitClickedCard();
    }

    // ���濡�� ī�� �ı��� �˸�
    [PunRPC]
    void DestroyFieldCard(string parentName, int cardIdx)
    {
        Transform parent = GameObject.Find(parentName).transform;
        GameObject card = parent.transform.GetChild(cardIdx).gameObject;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(card);
        }
        else
        {
            //photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            // ������ Ŭ���̾�Ʈ���� ������Ʈ ���� ��û
            photonView.RPC("RequestDestroyObject", RpcTarget.MasterClient, parent.gameObject.name, cardIdx);
        }
    }
    // ������ Ŭ���̾�Ʈ���� ������Ʈ ���� ��û
    void RequestDestroyObject(string parentName, int cardIdx)
    {
        Transform parent = GameObject.Find(parentName).transform;
        GameObject card = parent.transform.GetChild(cardIdx).gameObject;
        PhotonNetwork.Destroy(card);
    }
    // ���濡�� ī�� ��ȭ�� �˸�
    [PunRPC]
    void ChangeFieldCard(string parentName, int cardIdx, int changedNumber)
    {
        // TODO ENUM ���� ����
        string hexColor = "#FFFFFF";
        Color newColor;

        Transform parent = GameObject.Find(parentName).transform;
        GameObject card = parent.transform.GetChild(cardIdx).gameObject;

        // Card Number ����
        card.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = changedNumber.ToString();

        // �ش� ��ư�� ���� ���� Secret -> Open
        if (ColorUtility.TryParseHtmlString(hexColor, out newColor))
        {
            // ��ư�� ���� ����
            card.GetComponent<Image>().color = newColor;
        }
    }
}
