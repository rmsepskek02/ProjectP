using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviourPunCallbacks
{
    #region �ʵ�
    public string cardPosition;

    Transform myFieldCardListContent;
    Transform yourFieldCardListContent;
    InGameManager im;
    PunTurnManager turnManager;
    bool isAttack = false;
    #endregion
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

    // ī�� Ŭ�� �̺�Ʈ - �ϰ� ���� Ŭ���̾�Ʈ ����
    public void OnClickCard()
    {
        if (turnManager.Turn % 2 == 1)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == im.playerList[0])
            {
                CardEvent();
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
                CardEvent();
            }
            else { }
        }
    }

    // ī�� Ŭ������ �߻��ϴ� �̺�Ʈ �Լ� - TODO �������۾��� ���� �������ؾ���
    void CardEvent()
    {
        string parentName = gameObject.transform.parent.name;
        if (parentName == "HorizontalMyCard")
        {
            im.clickedMyCardIdx = gameObject.transform.GetSiblingIndex();
            im.clickedMyCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
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
        else if (parentName == "HorizontalMyFieldCard")
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
        }
        else if (parentName == "HorizontalYourFieldCard")
        {
            im.clickedYourCardIdx = gameObject.transform.GetSiblingIndex();
            im.clickedYourCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

            if (im.clickedMyCardNumber != "")
            {
                // Secret Ȯ��
                if (gameObject.transform.GetChild(0).gameObject.activeSelf == false)
                {
                    gameObject.transform.GetChild(0).gameObject.SetActive(true);
                }
                int myNumber = int.Parse(im.clickedMyCardNumber);
                int yourNumber = int.Parse(im.clickedYourCardNumber);
                int result = (myNumber - yourNumber);
                FightCard(result, im.clickedMyCardIdx, im.clickedYourCardIdx);
            }
        }
        else
        {
            cardPosition = "";
        }
    }

    // ī���� �ؽ�Ʈ Ȯ��
    bool CheckIsNumber(string text)
    {
        return int.TryParse(text, out _);
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
            PhotonNetwork.Destroy(yourFieldCard);
        }
        else if (_result < 0)
        {
            // �� ī�� �ı�
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, yourCardParentName, myCardIdx);
            PhotonNetwork.Destroy(myFieldCard);
            // ���ī�� ��Ģ���� ��� ����
            photonView.RPC("ChangeFieldCard", RpcTarget.Others, myCardParentName, yourCardIdx, Mathf.Abs(_result));
            yourFieldCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mathf.Abs(_result).ToString();
        }
        else
        {
            // �� ī�� �ı�
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, yourCardParentName, myCardIdx);
            PhotonNetwork.Destroy(myFieldCard);
            // ���ī�� �ı�
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, myCardParentName, yourCardIdx);
            PhotonNetwork.Destroy(yourFieldCard);
        }
        // Ŭ���� ī�� ������ �ʱ�ȭ
        im.InitClickedCard();
        isAttack = false;
    }

    // ���濡�� ī�� �ı��� �˸�
    [PunRPC]
    void DestroyFieldCard(string parentName, int cardIdx)
    {
        Transform parent = GameObject.Find(parentName).transform;
        GameObject card = parent.transform.GetChild(cardIdx).gameObject;
        PhotonNetwork.Destroy(card);
    }

    // ���濡�� ī�� ��ȭ�� �˸�
    [PunRPC]
    void ChangeFieldCard(string parentName, int cardIdx, int changedNumber)
    {
        Transform parent = GameObject.Find(parentName).transform;
        GameObject card = parent.transform.GetChild(cardIdx).gameObject;

        // Card Number ����
        card.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = changedNumber.ToString();
        // ���󺯰�
        card.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.WhiteColor);
    }
}
