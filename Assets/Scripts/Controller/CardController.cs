using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviourPunCallbacks
{
    #region �ʵ�
    public string cardPosition;
    public Color originColor;

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
        originColor = gameObject.GetComponent<Image>().color;
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
        string openName = "Prefabs/Card/OpenCard";
        string secretName = "Prefabs/Card/SecretCard";
        string parentName = gameObject.transform.parent.name;
        string cardNumber = transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        // TODO ī��� �ִ� 5������� �ʵ忡 �� �� �ֵ��� ����
        if (parentName == "HorizontalMyCard")
        {
            im.clickedMyCardIdx = gameObject.transform.GetSiblingIndex();
            im.clickedMyCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            if (CheckIsNumber(im.clickedMyCardNumber) == false)
            {
                if (im.clickedMyCardNumber == "Joker")
                {
                    im.joker.SetActive(true);
                }
                else if (im.clickedMyCardNumber == "+")
                {
                    foreach (Transform child in im.myFieldCardList.transform)
                    {
                        child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.YellowColor);
                    }
                    im.ResetYourFieldCardColor();
                    im.isPlus = true;
                    // ����� Card �ı�
                    PhotonNetwork.Destroy(im.myHandCardList.transform.GetChild(im.clickedMyCardIdx).gameObject);
                    // �ı��Ȱ� �˸�
                    photonView.RPC("RemoveHandCard", RpcTarget.Others, im.yourHandCardList.name);
                }
                else if (im.clickedMyCardNumber == "X")
                {
                    foreach (Transform child in im.myFieldCardList.transform)
                    {
                        child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.YellowColor);
                    }
                    im.ResetYourFieldCardColor();
                    im.isMultiple = true;
                    // ����� Card �ı�
                    PhotonNetwork.Destroy(im.myHandCardList.transform.GetChild(im.clickedMyCardIdx).gameObject);
                    // �ı��Ȱ� �˸�
                    photonView.RPC("RemoveHandCard", RpcTarget.Others, im.yourHandCardList.name);
                }
                else if (im.clickedMyCardNumber == "-")
                {
                    foreach (Transform child in im.yourFieldCardList.transform)
                    {
                        child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.YellowColor);
                    }
                    im.ResetMyFieldCardColor();
                    im.isMinus = true;
                    // ����� Card �ı�
                    PhotonNetwork.Destroy(im.myHandCardList.transform.GetChild(im.clickedMyCardIdx).gameObject);
                    // �ı��Ȱ� �˸�
                    photonView.RPC("RemoveHandCard", RpcTarget.Others, im.yourHandCardList.name);
                }
                else if (im.clickedMyCardNumber == "%")
                {
                    foreach (Transform child in im.yourFieldCardList.transform)
                    {
                        child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.YellowColor);
                    }
                    im.ResetMyFieldCardColor();
                    im.isDivision = true;
                    // ����� Card �ı�
                    PhotonNetwork.Destroy(im.myHandCardList.transform.GetChild(im.clickedMyCardIdx).gameObject);
                    // �ı��Ȱ� �˸�
                    photonView.RPC("RemoveHandCard", RpcTarget.Others, im.yourHandCardList.name);
                }
                isAttack = false;

                return;
            }
            else
            {
                im.choice.SetActive(true);
            }
        }
        else if (parentName == "HorizontalMyFieldCard")
        {
            // joker �̺�Ʈ
            if (im.isCopy == true)
            {
                if (originColor == Global.Colors.ChangeColor(Global.Colors.OpenColor))
                {
                    // ���濡�� �� �ʵ忡 ī�� �߰��� �˸���
                    photonView.RPC("SpawnHandCard", RpcTarget.Others, openName, im.yourFieldCardList.transform.position, im.yourFieldCardList.name, cardNumber, true);
                    // �� ȭ�鿡�� �� �ʵ忡 �߰��ϱ�
                    GameObject go = PhotonNetwork.Instantiate("Prefabs/Card/OpenCard", im.myFieldCardList.transform.position, Quaternion.identity);
                    Transform parent = GameObject.Find(im.myFieldCardList.name).transform;
                    go.transform.SetParent(parent, false);
                    go.GetComponentInChildren<TextMeshProUGUI>().text = cardNumber;
                }
                else if (originColor == Global.Colors.ChangeColor(Global.Colors.SecretColor))
                {
                    photonView.RPC("SpawnHandCard", RpcTarget.Others, secretName, im.yourFieldCardList.transform.position, im.yourFieldCardList.name, cardNumber, false);

                    GameObject go = PhotonNetwork.Instantiate("Prefabs/Card/SecretCard", im.myFieldCardList.transform.position, Quaternion.identity);
                    Transform parent = GameObject.Find(im.myFieldCardList.name).transform;
                    go.transform.SetParent(parent, false);
                    go.GetComponentInChildren<TextMeshProUGUI>().text = cardNumber;
                    go.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.SecretColor);
                }
                im.isCopy = false;
                isAttack = false;

                im.ResetMyFieldCardColor();
                im.ResetYourFieldCardColor();
                return;
            }

            // 1.
            // plus �̺�Ʈ
            if (im.isPlus == true)
            {
                if (im.firstCardNumber != "")
                {
                    im.secondCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                    int idx = im.firstCard.GetSiblingIndex();
                    int number = int.Parse(im.firstCardNumber) + int.Parse(im.secondCardNumber);
                    im.firstCard.GetComponentInChildren<TextMeshProUGUI>(true).text = number.ToString();
                    photonView.RPC("ChangeFieldCardAfterFundamental", RpcTarget.Others, "HorizontalYourFieldCard", idx, number);

                    im.isPlus = false;
                    im.ResetMyFieldCardColor();
                    im.ResetYourFieldCardColor();
                    im.clickedMyCardNumber = "";
                    im.firstCard = null;
                    im.firstCardNumber = "";
                    im.secondCardNumber = "";

                    return;
                }

                im.firstCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                im.firstCard = this.gameObject.transform;
                im.ResetMyFieldCardColor();
                foreach (Transform child in im.yourFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                }
                foreach (Transform child in im.myFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                }
                return;
            }
            // multiple �̺�Ʈ
            if (im.isMultiple == true)
            {
                if (im.firstCardNumber != "")
                {
                    im.secondCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                    int idx = im.firstCard.GetSiblingIndex();
                    int number = int.Parse(im.firstCardNumber) * int.Parse(im.secondCardNumber);
                    im.firstCard.GetComponentInChildren<TextMeshProUGUI>(true).text = number.ToString();
                    photonView.RPC("ChangeFieldCardAfterFundamental", RpcTarget.Others, "HorizontalYourFieldCard", idx, number);

                    im.isMultiple = false;
                    im.ResetMyFieldCardColor();
                    im.ResetYourFieldCardColor();
                    im.clickedMyCardNumber = "";
                    im.firstCard = null;
                    im.firstCardNumber = "";
                    im.secondCardNumber = "";

                    return;
                }

                im.firstCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                im.firstCard = this.gameObject.transform;
                im.ResetMyFieldCardColor();
                foreach (Transform child in im.yourFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                }
                foreach (Transform child in im.myFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                }
                return;
            }

            // 2.
            // minus �̺�Ʈ
            if (im.isMinus == true)
            {
                if (im.firstCardNumber != "")
                {
                    im.secondCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                    int idx = im.firstCard.GetSiblingIndex();
                    int number = int.Parse(im.firstCardNumber) - int.Parse(im.secondCardNumber);
                    im.firstCard.GetComponentInChildren<TextMeshProUGUI>(true).text = number.ToString();
                    if (number <= 0)
                    {
                        PhotonNetwork.Destroy(im.yourFieldCardList.transform.GetChild(im.firstCard.GetSiblingIndex()).gameObject);
                    }
                    photonView.RPC("ChangeFieldCardAfterFundamental", RpcTarget.Others, "HorizontalMyFieldCard", idx, number);

                    im.isMinus = false;
                    im.ResetMyFieldCardColor();
                    im.ResetYourFieldCardColor();
                    im.clickedMyCardNumber = "";
                    im.firstCard = null;
                    im.firstCardNumber = "";
                    im.secondCardNumber = "";

                    return;
                }

                im.firstCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                im.firstCard = this.gameObject.transform;
                im.ResetMyFieldCardColor();
                foreach (Transform child in im.yourFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                }
                foreach (Transform child in im.myFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                }
                return;
            }
            // division �̺�Ʈ
            if (im.isDivision == true)
            {
                if (im.firstCardNumber != "")
                {
                    im.secondCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                    int idx = im.firstCard.GetSiblingIndex();
                    int quotient = int.Parse(im.firstCardNumber) / int.Parse(im.secondCardNumber);
                    int remainder = int.Parse(im.firstCardNumber) % int.Parse(im.secondCardNumber);
                    im.firstCard.GetComponentInChildren<TextMeshProUGUI>(true).text = quotient.ToString();
                    if (quotient <= 0)
                    {
                        PhotonNetwork.Destroy(im.yourFieldCardList.transform.GetChild(im.firstCard.GetSiblingIndex()).gameObject);
                    }
                    photonView.RPC("ChangeFieldCardAfterFundamental", RpcTarget.Others, "HorizontalMyFieldCard", idx, quotient);

                    im.isDivision = false;
                    im.ResetMyFieldCardColor();
                    im.ResetYourFieldCardColor();
                    im.clickedMyCardNumber = "";
                    im.firstCard = null;
                    im.firstCardNumber = "";
                    im.secondCardNumber = "";

                    return;
                }

                im.firstCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                im.firstCard = this.gameObject.transform;
                im.ResetMyFieldCardColor();
                foreach (Transform child in im.yourFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                }
                foreach (Transform child in im.myFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                }
                return;
            }

            isAttack = !isAttack;
            if (isAttack)
            {
                foreach (Transform child in im.yourFieldCardList.transform)
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                foreach (Transform child in im.myFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = child.GetComponent<CardController>().originColor;
                    child.GetComponent<CardController>().isAttack = false;
                }
                gameObject.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.GreenColor);
                //isAttack = true;

                im.clickedMyCardIdx = gameObject.transform.GetSiblingIndex();
                im.clickedMyCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            }
            else
            {
                gameObject.GetComponent<Image>().color = originColor;
                im.ResetYourFieldCardColor();
                im.clickedMyCardNumber = "";
            }
        }
        else if (parentName == "HorizontalYourFieldCard")
        {
            im.clickedYourCardIdx = gameObject.transform.GetSiblingIndex();
            im.clickedYourCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

            // joker �̺�Ʈ
            if (im.isDelete == true)
            {
                im.ResetMyFieldCardColor();
                im.ResetYourFieldCardColor();

                photonView.RPC("DestroyFieldCard", RpcTarget.Others, im.myFieldCardList.name, im.clickedYourCardIdx);
                PhotonNetwork.Destroy(im.yourFieldCardList.transform.GetChild(im.clickedYourCardIdx).gameObject);
                im.isDelete = false;
                return;
            }

            //1.
            // minus �̺�Ʈ
            if (im.isMinus == true)
            {
                if (im.firstCardNumber != "")
                {
                    im.secondCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                    int idx = im.firstCard.GetSiblingIndex();
                    int number = int.Parse(im.firstCardNumber) - int.Parse(im.secondCardNumber);
                    im.firstCard.GetComponentInChildren<TextMeshProUGUI>(true).text = number.ToString();
                    if (number <= 0)
                    {
                        PhotonNetwork.Destroy(im.yourFieldCardList.transform.GetChild(im.firstCard.GetSiblingIndex()).gameObject);
                    }
                    photonView.RPC("ChangeFieldCardAfterFundamental", RpcTarget.Others, "HorizontalMyFieldCard", idx, number);

                    im.isMinus = false;
                    im.ResetMyFieldCardColor();
                    im.ResetYourFieldCardColor();
                    im.clickedMyCardNumber = "";
                    im.firstCard = null;
                    im.firstCardNumber = "";
                    im.secondCardNumber = "";

                    return;
                }

                im.firstCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                im.firstCard = this.gameObject.transform;
                im.ResetMyFieldCardColor();
                foreach (Transform child in im.yourFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                }
                foreach (Transform child in im.myFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                }
                return;
            }
            // division �̺�Ʈ
            if (im.isDivision == true)
            {
                if (im.firstCardNumber != "")
                {
                    im.secondCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                    int idx = im.firstCard.GetSiblingIndex();
                    int quotient = int.Parse(im.firstCardNumber) / int.Parse(im.secondCardNumber);
                    int remainder = int.Parse(im.firstCardNumber) % int.Parse(im.secondCardNumber);
                    im.firstCard.GetComponentInChildren<TextMeshProUGUI>(true).text = quotient.ToString();
                    if (quotient <= 0)
                    {
                        PhotonNetwork.Destroy(im.yourFieldCardList.transform.GetChild(im.firstCard.GetSiblingIndex()).gameObject);
                    }
                    photonView.RPC("ChangeFieldCardAfterFundamental", RpcTarget.Others, "HorizontalMyFieldCard", idx, quotient);

                    im.isDivision = false;
                    im.ResetMyFieldCardColor();
                    im.ResetYourFieldCardColor();
                    im.clickedMyCardNumber = "";
                    im.firstCard = null;
                    im.firstCardNumber = "";
                    im.secondCardNumber = "";

                    return;
                }

                im.firstCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                im.firstCard = this.gameObject.transform;
                im.ResetMyFieldCardColor();
                foreach (Transform child in im.yourFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                }
                foreach (Transform child in im.myFieldCardList.transform)
                {
                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
                }
                return;
            }

            // 2.
            // plus �̺�Ʈ
            if (im.isPlus == true)
            {
                im.secondCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                int idx = im.firstCard.GetSiblingIndex();
                int number = int.Parse(im.firstCardNumber) + int.Parse(im.secondCardNumber);
                im.firstCard.GetComponentInChildren<TextMeshProUGUI>().text = number.ToString();
                photonView.RPC("ChangeFieldCardAfterFundamental", RpcTarget.Others, "HorizontalYourFieldCard", idx, number);

                im.isPlus = false;
                im.ResetMyFieldCardColor();
                im.ResetYourFieldCardColor();
                im.clickedMyCardNumber = "";
                im.firstCard = null;
                im.firstCardNumber = "";
                im.secondCardNumber = "";
                return;
            }
            // multiple �̺�Ʈ
            if (im.isMultiple == true)
            {
                im.secondCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
                int idx = im.firstCard.GetSiblingIndex();
                int number = int.Parse(im.firstCardNumber) * int.Parse(im.secondCardNumber);
                im.firstCard.GetComponentInChildren<TextMeshProUGUI>().text = number.ToString();
                photonView.RPC("ChangeFieldCardAfterFundamental", RpcTarget.Others, "HorizontalYourFieldCard", idx, number);

                im.isMultiple = false;
                im.ResetMyFieldCardColor();
                im.ResetYourFieldCardColor();
                im.clickedMyCardNumber = "";
                im.firstCard = null;
                im.firstCardNumber = "";
                im.secondCardNumber = "";
                return;
            }

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
            im.ResetYourFieldCardColor();
        }
        // Ŭ���� ī�� ������ �ʱ�ȭ
        im.InitClickedCard();
        isAttack = false;
        im.ResetMyFieldCardColor();
        im.ResetYourFieldCardColor();

        myFieldCard.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.OpenColor);
        myFieldCard.GetComponent<CardController>().originColor = Global.Colors.ChangeColor(Global.Colors.OpenColor);
    }
    // ���濡�� �ڵ�ī�� ���Ÿ� �˸�
    [PunRPC]
    void RemoveHandCard(string parentName)
    {
        Transform parent = GameObject.Find(parentName).transform;
        PhotonNetwork.Destroy(parent.GetChild(0).gameObject);
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
        Transform text = card.transform.GetChild(0);
        // Card Number ����
        text.GetComponent<TextMeshProUGUI>().text = changedNumber.ToString();
        text.gameObject.SetActive(true);
        // ���󺯰�
        card.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.WhiteColor);
    }

    // ��Ģ���� ��� �ݿ��� �˸�
    [PunRPC]
    void ChangeFieldCardAfterFundamental(string parentName, int cardIdx, int changedNumber)
    {
        Transform parent = GameObject.Find(parentName).transform;
        GameObject card = parent.transform.GetChild(cardIdx).gameObject;
        Transform text = card.transform.GetChild(0);
        text.GetComponent<TextMeshProUGUI>().text = changedNumber.ToString();
        if (changedNumber <= 0)
        {
            PhotonNetwork.Destroy(card);
        }
    }

    // ���濡�� ī�� ���縦 �˸�
    [PunRPC]
    void SpawnHandCard(string prefabName, Vector3 position, string parentName, string number, bool isOpen)
    {
        GameObject go = PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);
        //if (prefabName  == "Prefabs/Card/SecretCard")
        //{
        //    go.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.SecretColor);
        //}
        Transform parent = GameObject.Find(parentName).transform;
        go.transform.SetParent(parent, false);
        int childCount = parent.childCount;
        GameObject card = parent.transform.GetChild(childCount - 1).gameObject;
        Transform text = card.transform.GetChild(0);
        text.GetComponent<TextMeshProUGUI>().text = number.ToString();
        text.gameObject.SetActive(isOpen);
    }
}

// TODO :: �ڱ��ڽ��� ��Ģ������ �ι�°ī��� ��� ���ϵ��� 
// ���������� �ٲپ� ������ ���� ���ϵ��� �����ٰ�