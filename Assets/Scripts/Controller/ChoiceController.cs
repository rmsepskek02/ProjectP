using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceController : MonoBehaviourPunCallbacks
{
    #region �ʵ�
    public Button openButton;
    public Button secretButton;
    public Button cancelButton;

    InGameManager im;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        im = InGameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnClickOpen()
    {
        // �̰� �ı��ϰ� ���漭���� ����ؼ� �ϳ� �ʵ忡 ����
        Transform childCard = im.myHandCardList.transform.GetChild(im.clickedMyCardIdx);
        childCard.SetParent(im.myFieldCardList.transform);

        string yourPrefabName = "Prefabs/Card/OpenCard";
        Vector3 yourSpawnPosition = im.yourFieldCardList.transform.position;
        string yourFieldParentName = im.yourFieldCardList.name;
        string yourHandParentName = im.yourHandCardList.name;
        string yourFieldCardname = im.clickedMyCardNumber;

        photonView.RPC("SpawnFieldOpenCard", RpcTarget.Others, yourPrefabName, yourSpawnPosition, yourFieldParentName, yourFieldCardname);
        photonView.RPC("RemoveHandCard", RpcTarget.Others, yourHandParentName);

        int playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        if (playerNumber == im.playerList[0])
            im.playerAHand.RemoveAt(im.clickedMyCardIdx);
        else if (playerNumber == im.playerList[1])
            im.playerBHand.RemoveAt(im.clickedMyCardIdx);

        gameObject.SetActive(false);
    }
    public void OnClickSecret()
    {
        Transform childCard = im.myHandCardList.transform.GetChild(im.clickedMyCardIdx);
        childCard.GetComponent<Image>().color = Global.Colors.ChangeColor(Global.Colors.SecretColor);

        childCard.SetParent(im.myFieldCardList.transform);

        string yourPrefabName = "Prefabs/Card/SecretCard";
        Vector3 yourSpawnPosition = im.yourFieldCardList.transform.position;
        string yourFieldParentName = im.yourFieldCardList.name;
        string yourHandParentName = im.yourHandCardList.name;

        photonView.RPC("SpawnFieldSecretCard", RpcTarget.Others, yourPrefabName, yourSpawnPosition, yourFieldParentName, im.clickedMyCardNumber);
        photonView.RPC("RemoveHandCard", RpcTarget.Others, yourHandParentName);

        int playerNumber = PhotonNetwork.LocalPlayer.ActorNumber;
        if (playerNumber == im.playerList[0])
            im.playerAHand.RemoveAt(im.clickedMyCardIdx);
        else if (playerNumber == im.playerList[1])
            im.playerBHand.RemoveAt(im.clickedMyCardIdx);

        gameObject.SetActive(false);
    }
    public void OnClickCancel()
    {
        gameObject.SetActive(false);
    }

    // ���濡�� ��ũ���ʵ�ī�� ������ �˸�
    [PunRPC]
    void SpawnFieldSecretCard(string prefabName, Vector3 position, string parentName, string cardNumber)
    {
        GameObject go = PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);
        go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cardNumber;
        go.transform.GetChild(0).gameObject.SetActive(false);
        Transform parent = GameObject.Find(parentName).transform;
        go.transform.SetParent(parent, false); 
    }
    // ���濡�� �����ʵ�ī�� ������ �˸�
    [PunRPC]
    void SpawnFieldOpenCard(string prefabName, Vector3 position, string parentName, string cardNumber)
    {
        GameObject go = PhotonNetwork.Instantiate(prefabName, position, Quaternion.identity);
        go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = cardNumber;
        Transform parent = GameObject.Find(parentName).transform;
        go.transform.SetParent(parent, false); 
    }
    // ���濡�� �ڵ�ī�� ���Ÿ� �˸�
    [PunRPC]
    void RemoveHandCard(string parentName)
    {
        Transform parent = GameObject.Find(parentName).transform;
        Destroy(parent.GetChild(0).gameObject);
    }
}
