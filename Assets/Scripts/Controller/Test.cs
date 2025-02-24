//using Photon.Pun;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;

//public class Test : MonoBehaviour
//{
//    // 카드 클릭으로 발생하는 이벤트 함수 - TODO 구현할작업도 많고 정리도해야함
//    void CardEvent()
//    {
//        string openName = "Prefabs/Card/OpenCard";
//        string secretName = "Prefabs/Card/SecretCard";
//        string parentName = gameObject.transform.parent.name;
//        string cardNumber = transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
//        // TODO 카드는 최대 5장까지만 필드에 낼 수 있도록 수정

//        // 로컬 헬퍼: 특수 카드(Joker, +, X, -, %) 이벤트 처리
//        void ProcessSpecialCard(Action highlightAction, Action resetOtherField, Action setFlag)
//        {
//            highlightAction();
//            resetOtherField();
//            setFlag();
//            // 사용한 Card 파괴
//            PhotonNetwork.Destroy(im.myHandCardList.transform.GetChild(im.clickedMyCardIdx).gameObject);
//            // 파괴된거 알림
//            photonView.RPC("RemoveHandCard", RpcTarget.Others, im.yourHandCardList.name);
//        }

//        // 로컬 헬퍼: 산술(plus, multiple, minus, division) 이벤트 처리
//        // 연산 결과에 따라 RPC를 호출하고 상태를 초기화한 후 함수에서 빠져나갑니다.
//        void ProcessArithmeticEvent(string operation, Func<int, int, int> op, string rpcTargetName, bool checkDestroy = false)
//        {
//            if (!string.IsNullOrEmpty(im.firstCardNumber))
//            {
//                if (im.firstCard == transform)
//                    return;
//                im.secondCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
//                int idx = im.firstCard.GetSiblingIndex();
//                int num1 = int.Parse(im.firstCardNumber);
//                int num2 = int.Parse(im.secondCardNumber);
//                int result = op(num1, num2);
//                im.firstCard.GetComponentInChildren<TextMeshProUGUI>(true).text = result.ToString();
//                if (checkDestroy && result <= 0)
//                {
//                    PhotonNetwork.Destroy(im.yourFieldCardList.transform.GetChild(im.firstCard.GetSiblingIndex()).gameObject);
//                }
//                photonView.RPC("ChangeFieldCardAfterFundamental", RpcTarget.Others, rpcTargetName, idx, result);
//                // 연산에 따른 플래그 초기화
//                switch (operation)
//                {
//                    case "plus": im.isPlus = false; break;
//                    case "multiple": im.isMultiple = false; break;
//                    case "minus": im.isMinus = false; break;
//                    case "division": im.isDivision = false; break;
//                }
//                im.ResetMyFieldCardColor();
//                im.ResetYourFieldCardColor();
//                im.clickedMyCardNumber = "";
//                im.firstCard = null;
//                im.firstCardNumber = "";
//                im.secondCardNumber = "";
//                return;
//            }
//            else
//            {
//                im.firstCardNumber = GetComponentInChildren<TextMeshProUGUI>(true).text;
//                im.firstCard = this.gameObject.transform;
//                im.ResetMyFieldCardColor();
//                foreach (Transform child in im.yourFieldCardList.transform)
//                {
//                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
//                }
//                foreach (Transform child in im.myFieldCardList.transform)
//                {
//                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
//                }
//                gameObject.GetComponent<Image>().color = originColor;
//                return;
//            }
//        }

//        if (parentName == "HorizontalMyCard")
//        {
//            im.clickedMyCardIdx = gameObject.transform.GetSiblingIndex();
//            im.clickedMyCardNumber = transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
//            if (CheckIsNumber(im.clickedMyCardNumber) == false)
//            {
//                if (im.clickedMyCardNumber == "Joker")
//                {
//                    im.joker.SetActive(true);
//                }
//                else if (im.clickedMyCardNumber == "+")
//                {
//                    ProcessSpecialCard(
//                        () =>
//                        {
//                            foreach (Transform child in im.myFieldCardList.transform)
//                            {
//                                child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.YellowColor);
//                            }
//                        },
//                        () => { im.ResetYourFieldCardColor(); },
//                        () => { im.isPlus = true; }
//                    );
//                }
//                else if (im.clickedMyCardNumber == "X")
//                {
//                    ProcessSpecialCard(
//                        () =>
//                        {
//                            foreach (Transform child in im.myFieldCardList.transform)
//                            {
//                                child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.YellowColor);
//                            }
//                        },
//                        () => { im.ResetYourFieldCardColor(); },
//                        () => { im.isMultiple = true; }
//                    );
//                }
//                else if (im.clickedMyCardNumber == "-")
//                {
//                    ProcessSpecialCard(
//                        () =>
//                        {
//                            foreach (Transform child in im.yourFieldCardList.transform)
//                            {
//                                child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.YellowColor);
//                            }
//                        },
//                        () => { im.ResetMyFieldCardColor(); },
//                        () => { im.isMinus = true; }
//                    );
//                }
//                else if (im.clickedMyCardNumber == "%")
//                {
//                    ProcessSpecialCard(
//                        () =>
//                        {
//                            foreach (Transform child in im.yourFieldCardList.transform)
//                            {
//                                child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.YellowColor);
//                            }
//                        },
//                        () => { im.ResetMyFieldCardColor(); },
//                        () => { im.isDivision = true; }
//                    );
//                }
//                isAttack = false;
//                return;
//            }
//            else
//            {
//                im.choice.SetActive(true);
//            }
//        }
//        else if (parentName == "HorizontalMyFieldCard")
//        {
//            // joker 이벤트
//            if (im.isCopy == true)
//            {
//                if (originColor == Global.Colors.ChangeColor(Global.Colors.OpenColor))
//                {
//                    // 상대방에게 내 필드에 카드 추가를 알리기
//                    photonView.RPC("SpawnHandCard", RpcTarget.Others, openName, im.yourFieldCardList.transform.position, im.yourFieldCardList.name, cardNumber, true);
//                    // 내 화면에서 내 필드에 추가하기
//                    GameObject go = PhotonNetwork.Instantiate("Prefabs/Card/OpenCard", im.myFieldCardList.transform.position, Quaternion.identity);
//                    Transform parent = GameObject.Find(im.myFieldCardList.name).transform;
//                    go.transform.SetParent(parent, false);
//                    go.GetComponentInChildren<TextMeshProUGUI>().text = cardNumber;
//                }
//                else if (originColor == Global.Colors.ChangeColor(Global.Colors.SecretColor))
//                {
//                    photonView.RPC("SpawnHandCard", RpcTarget.Others, secretName, im.yourFieldCardList.transform.position, im.yourFieldCardList.name, cardNumber, false);
//                    GameObject go = PhotonNetwork.Instantiate("Prefabs/Card/SecretCard", im.myFieldCardList.transform.position, Quaternion.identity);
//                    Transform parent = GameObject.Find(im.myFieldCardList.name).transform;
//                    go.transform.SetParent(parent, false);
//                    go.GetComponentInChildren<TextMeshProUGUI>().text = cardNumber;
//                    go.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.SecretColor);
//                }
//                im.isCopy = false;
//                isAttack = false;
//                im.ResetMyFieldCardColor();
//                im.ResetYourFieldCardColor();
//                return;
//            }

//            // plus 이벤트
//            if (im.isPlus == true)
//            {
//                ProcessArithmeticEvent("plus", (a, b) => a + b, "HorizontalYourFieldCard");
//                return;
//            }
//            // multiple 이벤트
//            if (im.isMultiple == true)
//            {
//                ProcessArithmeticEvent("multiple", (a, b) => a * b, "HorizontalYourFieldCard");
//                return;
//            }
//            // minus 이벤트
//            if (im.isMinus == true)
//            {
//                ProcessArithmeticEvent("minus", (a, b) => a - b, "HorizontalMyFieldCard", true);
//                return;
//            }
//            // division 이벤트
//            if (im.isDivision == true)
//            {
//                ProcessArithmeticEvent("division", (a, b) => a / b, "HorizontalMyFieldCard", true);
//                return;
//            }

//            isAttack = !isAttack;
//            if (isAttack)
//            {
//                foreach (Transform child in im.yourFieldCardList.transform)
//                    child.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.BlueColor);
//                foreach (Transform child in im.myFieldCardList.transform)
//                {
//                    child.GetComponent<Image>().color = child.GetComponent<CardController>().originColor;
//                    child.GetComponent<CardController>().isAttack = false;
//                }
//                gameObject.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.GreenColor);
//                //isAttack = true;
//                im.clickedMyCardIdx = gameObject.transform.GetSiblingIndex();
//                im.clickedMyCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
//            }
//            else
//            {
//                gameObject.GetComponent<Image>().color = originColor;
//                im.ResetYourFieldCardColor();
//                im.clickedMyCardNumber = "";
//            }
//        }
//        else if (parentName == "HorizontalYourFieldCard")
//        {
//            im.clickedYourCardIdx = gameObject.transform.GetSiblingIndex();
//            im.clickedYourCardNumber = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;

//            // joker 이벤트
//            if (im.isDelete == true)
//            {
//                im.ResetMyFieldCardColor();
//                im.ResetYourFieldCardColor();

//                photonView.RPC("DestroyFieldCard", RpcTarget.Others, im.myFieldCardList.name, im.clickedYourCardIdx);
//                PhotonNetwork.Destroy(im.yourFieldCardList.transform.GetChild(im.clickedYourCardIdx).gameObject);
//                im.isDelete = false;
//                return;
//            }

//            // minus 이벤트
//            if (im.isMinus == true)
//            {
//                ProcessArithmeticEvent("minus", (a, b) => a - b, "HorizontalMyFieldCard", true);
//                return;
//            }
//            // division 이벤트
//            if (im.isDivision == true)
//            {
//                ProcessArithmeticEvent("division", (a, b) => a / b, "HorizontalMyFieldCard", true);
//                return;
//            }
//            // plus 이벤트
//            if (im.isPlus == true)
//            {
//                ProcessArithmeticEvent("plus", (a, b) => a + b, "HorizontalYourFieldCard");
//                return;
//            }
//            // multiple 이벤트
//            if (im.isMultiple == true)
//            {
//                ProcessArithmeticEvent("multiple", (a, b) => a * b, "HorizontalYourFieldCard");
//                return;
//            }

//            if (im.clickedMyCardNumber != "")
//            {
//                // Secret 확인
//                if (gameObject.transform.GetChild(0).gameObject.activeSelf == false)
//                {
//                    gameObject.transform.GetChild(0).gameObject.SetActive(true);
//                }
//                int myNumber = int.Parse(im.clickedMyCardNumber);
//                int yourNumber = int.Parse(im.clickedYourCardNumber);
//                int result = (myNumber - yourNumber);
//                FightCard(result, im.clickedMyCardIdx, im.clickedYourCardIdx);
//            }
//        }
//        else
//        {
//            cardPosition = "";
//        }
//    }

//}
