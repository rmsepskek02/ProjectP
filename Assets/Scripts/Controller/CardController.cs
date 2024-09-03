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
                        // TODO 사칙연산, JOKER 처리
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
                    // TODO 다른 카드를 선택하도록 유도
                    // TODO 한번 더 누르면 취소
                    isAttack = !isAttack;
                    if (isAttack)
                    {
                        // TODO 공격 준비 이펙트
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
                        // TODO 카드 전투
                        Debug.Log("CARD FIGHT");
                        // Secret 확인
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
                        // TODO 사칙연산, JOKER 처리
                        return;
                    }
                    else
                    {
                        im.choice.SetActive(true);
                    }
                }
                else if (CheckCardPosition() == "MyField")
                {
                    // TODO 다른 카드를 선택하도록 유도
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
        // 카드 클릭하면
        // 오픈할지 덮어서 낼지 취소할지 - UI작업

        // 취소가아닌 경우
        // 필드로 옮기기 - transform 변경 
        // 서버에 알리기 - 상대화면에 내 카드가 한개줄고, 필드에 하나 생성
    }

    // 선택한 카드가 어디에 있는지 필드에 있는지 구분
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

    // 카드의 텍스트 확인
    bool CheckIsNumber(string text)
    {
        return int.TryParse(text, out _);
    }
    
    // 카드 선택 초기화
    void InitClickedCard()
    {
        im.clickedMyCardIdx = 0;
        im.clickedMyCardNumber= "";
        im.clickedYourCardIdx = 0;
        im.clickedYourCardNumber= "";
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
            Destroy(yourFieldCard);
            Debug.Log("WIN");
        }
        else if (_result < 0)
        {
            // 내 카드 파괴
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, yourCardParentName, myCardIdx);
            Destroy(myFieldCard);
            // 상대카드 사칙연산 결과 대입
            photonView.RPC("ChangeFieldCard", RpcTarget.Others, myCardParentName, yourCardIdx, Mathf.Abs(_result));
            yourFieldCard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Mathf.Abs(_result).ToString();
            Debug.Log("LOOSE");
        }
        else
        {
            // 내 카드 파괴
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, yourCardParentName, myCardIdx);
            Destroy(myFieldCard);
            // 상대카드 파괴
            photonView.RPC("DestroyFieldCard", RpcTarget.Others, myCardParentName, yourCardIdx);
            Destroy(yourFieldCard);
            Debug.Log("DRAW");
        }
        // 클릭한 카드 데이터 초기화
        InitClickedCard();
    }

    // 상대방에게 카드 파괴를 알림
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
            // 마스터 클라이언트에게 오브젝트 제거 요청
            photonView.RPC("RequestDestroyObject", RpcTarget.MasterClient, parent.gameObject.name, cardIdx);
        }
    }
    // 마스터 클라이언트에게 오브젝트 제거 요청
    void RequestDestroyObject(string parentName, int cardIdx)
    {
        Transform parent = GameObject.Find(parentName).transform;
        GameObject card = parent.transform.GetChild(cardIdx).gameObject;
        PhotonNetwork.Destroy(card);
    }
    // 상대방에게 카드 변화를 알림
    [PunRPC]
    void ChangeFieldCard(string parentName, int cardIdx, int changedNumber)
    {
        // TODO ENUM 으로 빼자
        string hexColor = "#FFFFFF";
        Color newColor;

        Transform parent = GameObject.Find(parentName).transform;
        GameObject card = parent.transform.GetChild(cardIdx).gameObject;

        // Card Number 변경
        card.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = changedNumber.ToString();

        // 해당 버튼의 색상 변경 Secret -> Open
        if (ColorUtility.TryParseHtmlString(hexColor, out newColor))
        {
            // 버튼의 색상 변경
            card.GetComponent<Image>().color = newColor;
        }
    }
}
