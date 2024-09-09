using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviourPunCallbacks
{
    #region 필드
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

    // 카드 클릭 이벤트 - 턴과 현재 클라이언트 구분
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

    // 카드 클릭으로 발생하는 이벤트 함수 - TODO 구현할작업도 많고 정리도해야함
    void CardEvent()
    {
        string parentName = gameObject.transform.parent.name;
        if (parentName == "HorizontalMyCard")
        {
            im.clickedMyCardIdx = gameObject.transform.GetSiblingIndex();
            im.clickedMyCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            if (CheckIsNumber(im.clickedMyCardNumber) == false)
            {
                if (im.clickedMyCardNumber == "Joker")
                {
                    im.joker.SetActive(true);
                    Debug.Log("JOKER CLICK");
                }
                else if (im.clickedMyCardNumber == "+")
                {
                    Debug.Log("+");
                }
                // TODO 사칙연산, JOKER 처리
                return;
            }
            else
            {
                im.choice.SetActive(true);
            }
        }
        else if (parentName == "HorizontalMyFieldCard")
        {
            isAttack = !isAttack;
            if (isAttack)
            {
                foreach(Transform child in im.yourFieldCardList.transform)
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                foreach (Transform child in im.myFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.WhiteColor);
                    child.GetComponent<CardController>().isAttack = false;
                }
                gameObject.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.GreenColor);
                isAttack = true;

                im.clickedMyCardIdx = gameObject.transform.GetSiblingIndex();
                im.clickedMyCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            }
            else
            {
                gameObject.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.WhiteColor);
                im.ResetYourFieldCardColor();
                im.clickedMyCardNumber = "";
            }
        }
        else if (parentName == "HorizontalYourFieldCard")
        {
            im.clickedYourCardIdx = gameObject.transform.GetSiblingIndex();
            im.clickedYourCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

            if (im.clickedMyCardNumber != "")
            {
                // Secret 확인
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

    // 카드의 텍스트 확인
    bool CheckIsNumber(string text)
    {
        return int.TryParse(text, out _);
    }

    // 카드 전투
    void FightCard(int _result, int myCardIdx, int yourCardIdx)
    {
        string myCardParentName = im.myFieldCardList.name;
        string yourCardParentName = im.yourFieldCardList.name;
        GameObject myFieldCard = im.myFieldCardList.transform.GetChild(myCardIdx).gameObject;
        GameObject yourFieldCard = im.yourFieldCardList.transform.GetChild(yourCardIdx).gameObject;

        if (_result > 0)
        {
            // 내카드 사칙연산 결과 대입
            photonView.RPC("ChangeFieldCard", RpcTarget.Others, yourCardParentName, myCardIdx, _result);
            myFieldCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _result.ToString();
            // 상대카드 파괴
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, myCardParentName, yourCardIdx);
            PhotonNetwork.Destroy(yourFieldCard);
        }
        else if (_result < 0)
        {
            // 내 카드 파괴
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, yourCardParentName, myCardIdx);
            PhotonNetwork.Destroy(myFieldCard);
            // 상대카드 사칙연산 결과 대입
            photonView.RPC("ChangeFieldCard", RpcTarget.Others, myCardParentName, yourCardIdx, Mathf.Abs(_result));
            yourFieldCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mathf.Abs(_result).ToString();
        }
        else
        {
            // 내 카드 파괴
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, yourCardParentName, myCardIdx);
            PhotonNetwork.Destroy(myFieldCard);
            // 상대카드 파괴
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, myCardParentName, yourCardIdx);
            PhotonNetwork.Destroy(yourFieldCard);
            im.ResetYourFieldCardColor();
        }
        // 클릭한 카드 데이터 초기화
        im.InitClickedCard();
        isAttack = false;
        im.ResetMyFieldCardColor();
        im.ResetYourFieldCardColor();
    }

    // 상대방에게 카드 파괴를 알림
    [PunRPC]
    void DestroyFieldCard(string parentName, int cardIdx)
    {
        Transform parent = GameObject.Find(parentName).transform;
        GameObject card = parent.transform.GetChild(cardIdx).gameObject;
        PhotonNetwork.Destroy(card);
    }

    // 상대방에게 카드 변화를 알림
    [PunRPC]
    void ChangeFieldCard(string parentName, int cardIdx, int changedNumber)
    {
        Transform parent = GameObject.Find(parentName).transform;
        GameObject card = parent.transform.GetChild(cardIdx).gameObject;

        // Card Number 변경
        card.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = changedNumber.ToString();
        // 색상변경
        card.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.WhiteColor);
    }
}
