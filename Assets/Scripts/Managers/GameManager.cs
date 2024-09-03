using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region �ʵ�
    public static GameManager instance;

    private const int MINWIDTH = 800;
    private const int MINHEIGHT = 300;
    private const int MINHEIGHT_INGAME = 600;
    string currentSceneName;
    #endregion
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;
        CheckScreenSize();
    }

    void CheckScreenSize()
    {
        int screenWidth = Screen.width >= MINWIDTH ? Screen.width : MINWIDTH;
        int screenHeight;
        if(currentSceneName == "GameScene")
            screenHeight = Screen.height >= MINHEIGHT_INGAME ? Screen.height : MINHEIGHT_INGAME;
        else 
            screenHeight = Screen.height >= MINHEIGHT ? Screen.height : MINHEIGHT;

        // TODO
        // â��� �ػ� ���� ���� �۾��ʿ� 
        Screen.SetResolution(screenWidth , screenHeight, FullScreenMode.Windowed);
    }
}
