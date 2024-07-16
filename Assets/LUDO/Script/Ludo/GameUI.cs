using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    #region DEBUG
    //public Text tweenError;
    //public static GameUI instance;

    public void RollDiceWith(int wantedFaceCount)
    {
        GameLogicRef.currentWantedRolledDiceFaceCount = wantedFaceCount;
        GameLogicRef.isTestingWithWantedDice = true;
    }

    //public void TweenError(string errorMessage)
    //{
    //	tweenError.text = errorMessage;
    //}
    #endregion

    //Give genric UI dialig script to all panels , which contains show and hide method
    public GameBoardSetup GameBoardSetupRef;
    public GameLogic GameLogicRef;

    [Header("Main Selection Screen")]
    public GameObject mainSelectionScreenPanel;
    public RectTransform playButtonRect;
    public Image playButtonBackground;
    public Image[] colorSelectionImages;

    [Header("Gameover Screen")]
    public GameObject gameOverScreenPanel;
    public GameObject[] winnerListArray;
    public RectTransform gameOverTitielImageRect;
    //public Text winnerListText;

    [Header("Gameplay Screen")]
    public GameObject gameplayBoard;
    public GameObject gameplayScreen;
    public Image confirmationUIBK;
    public RectTransform confirmationUI;
    public RectTransform confirmationUI_RefStart;
    public RectTransform confirmationUI_RefStop;

    [Header("Logo")]
    public Text logoText;

    [Header("Colors")]
    public Color[] brightColors;
    public Color[] darkColor;

    [Header("Color Sprites")]
    public Sprite[] brightColorSprite;
    public Sprite[] darkColorSprite;

    [Header("Win element")]
    public GameObject[] winElements;
    public Sprite[] winRankSprites;

    PlayerColor selectedUserColor;
    GameType selectedGameType;
    PlayerCount selectedPlayerCount;
    GameTheme selectedGameTheme;

    Color[] currentColors;

    void Awake()
    {
        HideScreen(gameplayBoard);
        HideScreen(gameplayScreen);
        HideScreen(gameOverScreenPanel);

        InitializeUserSelection();

        //TWEEN
        PlayMainScreenAnimations();
    }

    private void InitializeUserSelection()
    {
        //Initialize default selection modes
        selectedGameType = GameType.Local;
        selectedUserColor = PlayerColor.Red;
        selectedPlayerCount = PlayerCount.Two;
        selectedGameTheme = GameTheme.Bright;
        currentColors = brightColors;
    }

    internal void DisableAllWinElements()
    {
        for (int i = 0; i < winElements.Length; i++)
        {
            winElements[i].SetActive(false);
        }
    }

    public void EnableWinElementOf(int winIndex, int winRank)
    {
        if (selectedPlayerCount == PlayerCount.Two && winIndex == 1)
        {
            winElements[2].SetActive(true);

            int calculatedIndex = GameBoardSetupRef.pieceColorSpritesIndex[2];

            winElements[2].GetComponent<GenericWinElement>().SetColorAndSprite(currentColors[calculatedIndex], winRankSprites[winRank]);
        }
        else
        {
            winElements[winIndex].SetActive(true);

            int calculatedIndex = GameBoardSetupRef.pieceColorSpritesIndex[winIndex];

            winElements[winIndex].GetComponent<GenericWinElement>().SetColorAndSprite(currentColors[calculatedIndex], winRankSprites[winRank]);
        }
    }

    //private void Update()
    //{
    //    #region DEBUG
    //    //if (Input.GetKeyDown(KeyCode.Escape))
    //    //{
    //    //	UnityEditor.EditorApplication.isPaused = true;
    //    //}
    //    #endregion
    //}

    #region ASSIGNABLE_EVENTS

    public void OnValueChangedGameModeToggle(Toggle currentToggle)
    {
        if (currentToggle.isOn)
        {
            //Debug.Log(currentToggle.GetComponentInChildren<Text>().text + " => " + currentToggle.isOn);
            selectedGameType = currentToggle.GetComponent<GenericGameModeToggle>().gameType;
        }
    }

    public void OnValueChangedPlayerColorToggle(Toggle currentToggle)
    {
        if (currentToggle.isOn)
        {
            //Debug.Log(currentToggle.GetComponentInChildren<Text>().text + " => " + currentToggle.isOn);
            selectedUserColor = currentToggle.GetComponent<GenericPlayerColorToggle>().playerColor;
        }
    }

    public void OnValueChangedPlayerCountToggle(Toggle currentToggle)
    {
        if (currentToggle.isOn)
        {
            //Debug.Log(currentToggle.GetComponentInChildren<Text>().text + " => " + currentToggle.isOn);
            selectedPlayerCount = currentToggle.GetComponent<GenericPlayerCountToggle>().playerCount;
        }
    }

    public void OnValueChangedGameThemeToggle(Toggle currentToggle)
    {
        if (currentToggle.isOn)
        {
            //Debug.Log(currentToggle.GetComponentInChildren<Text>().text + " => " + currentToggle.isOn);
            selectedGameTheme = currentToggle.GetComponent<GenericGameThemeToggle>().gameTheme;

            if (selectedGameTheme == GameTheme.Bright)
            {
                logoText.text = "<color=#FF5A5AFF>L</color><color=#00C0FFFF>U</color><color=#55E756FF>D</color><color=#FFD85AFF>O</color>";

                currentColors = brightColors;

                for (int i = 0; i < colorSelectionImages.Length; i++)
                {
                    colorSelectionImages[i].sprite = brightColorSprite[i];
                }
            }
            else if (selectedGameTheme == GameTheme.Dark)
            {
                logoText.text = "<color=#EA4335FF>L</color><color=#4285F4FF>U</color><color=#34A853FF>D</color><color=#FBBC05FF>O</color>";

                currentColors = darkColor;

                for (int i = 0; i < colorSelectionImages.Length; i++)
                {
                    colorSelectionImages[i].sprite = darkColorSprite[i];
                }
            }
        }
    }

    public void OnClickPlay()
    {
        ShowScreen(gameplayBoard);
        ShowScreen(gameplayScreen);
        HideScreen(MainMenuHandler.instance.waitingPanel);
        HideScreen(gameOverScreenPanel);
        StopMainScreenAnimations();
        DisableAllWinElements();

        #region Multiplayer
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("data set as MasterClient");
            selectedGameType = GameType.Local;
            selectedUserColor = PlayerColor.Red;
            selectedPlayerCount = PlayerCount.Two;
            selectedGameTheme = GameTheme.Dark;
        }
        else
        {
            Debug.Log("data set as NON MasterClient");
            selectedGameType = GameType.Local;
            selectedUserColor = PlayerColor.Green;
            selectedPlayerCount = PlayerCount.Two;
            selectedGameTheme = GameTheme.Dark;
        }
        #endregion

        GameBoardSetupRef.SetupGameBoard(selectedUserColor, selectedGameType, selectedPlayerCount, selectedGameTheme);

        if (PhotonNetwork.IsMasterClient)
        {
            object[] data =
            {
                PhotonNetwork.LocalPlayer,
            };
            PhotonController.instance.RaiseEvt(StaticData.GAME_START, data, ReceiverGroup.Others);
        }
    }

    public void OnClickHomeFromGameplay()
    {
        confirmationUIBK.gameObject.SetActive(true);
        LeanTween.move(confirmationUI, confirmationUI_RefStop.anchoredPosition, 0.5F).setEaseOutCirc();
    }

    public void OnClickHomeFromGameover()
    {
        ResetGameBoard();
        NavigateToMainScreenFromHome();
    }

    public void ConfirmationYes()
    {
        LeanTween.move(confirmationUI, confirmationUI_RefStart.anchoredPosition, 0.5F).setEaseOutCirc().setOnComplete(() =>
        {
            confirmationUIBK.gameObject.SetActive(false);
            ResetGameBoard();
            NavigateToMainScreenFromHome();
        });
    }

    public void ConfirmationNo()
    {
        LeanTween.move(confirmationUI, confirmationUI_RefStart.anchoredPosition, 0.5F).setEaseOutCirc();
        confirmationUIBK.gameObject.SetActive(false);
    }

    public void NavigateToMainScreenFromHome()
    {
        LeanTween.cancelAll(false);
        HideScreen(gameplayBoard);
        HideScreen(gameplayScreen);
        HideScreen(gameOverScreenPanel);

        ShowScreen(MainMenuHandler.instance.createJoinRoomPanel);
        PlayMainScreenAnimations();
    }

    public void ResetGameBoard()
    {
        GameBoardSetupRef.ClearGameBoard();
        GameLogicRef.ClearGameData();
        StopGameOverTitileImageTween();
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    public void OnRate()
    {

    }

    public void OnPrivacyPolicy()
    {
    }

    #endregion

    public void ShowScreen(GameObject genericScreen)
    {
        genericScreen.SetActive(true);
    }

    public void HideScreen(GameObject genericScreen)
    {
        genericScreen.SetActive(false);
    }

    public void ShowGameOverScreen(List<int> winnerList)
    {
        HideScreen(gameplayBoard);
        HideScreen(gameplayScreen);

        string winnerString = "";
        //Disabling all elements of list
        for (int i = 0; i < winnerListArray.Length; i++)
        {
            winnerListArray[i].SetActive(false);
        }

        for (int i = 0; i < winnerList.Count; i++)
        {
            winnerString += winnerList[i].ToString();

            winnerListArray[i].SetActive(true);

            winnerListArray[i].GetComponent<Image>().color = currentColors[GameBoardSetupRef.pieceColorSpritesIndex[winnerList[i]]];

            winnerListArray[i].transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Image>().color = currentColors[GameBoardSetupRef.pieceColorSpritesIndex[winnerList[i]]];
        }
        //winnerListText.text = winnerString;

        ShowScreen(gameOverScreenPanel);
        PlayGameOverTitleImageTween();
    }

    #region MAIN_SCREEN_UI_TWEEN

    void StartPlayButtonTween()
    {
        LeanTween.rotateZ(playButtonRect.gameObject, -720F, 2F).setEaseOutQuart().setLoopCount(-1);
        LeanTween.scale(playButtonRect, Vector2.one * 0.5F, 2F).setEasePunch().setLoopCount(-1);
    }

    void StartPlayButtonBkTween()
    {
        LeanTween.color(playButtonBackground.GetComponent<RectTransform>(), currentColors[1], 1F).setDelay(1F).setOnComplete(() =>
        {
            LeanTween.color(playButtonBackground.GetComponent<RectTransform>(), currentColors[2], 1F).setDelay(1F).setOnComplete(() =>
            {
                LeanTween.color(playButtonBackground.GetComponent<RectTransform>(), currentColors[3], 1F).setDelay(1F).setOnComplete(() =>
                {
                    LeanTween.color(playButtonBackground.GetComponent<RectTransform>(), currentColors[0], 1F).setDelay(1F).setOnComplete(() =>
                    {
                        StartPlayButtonBkTween();
                    });
                });
            });
        });
    }

    internal void PlayMainScreenAnimations()
    {
        StartPlayButtonTween();
        StartPlayButtonBkTween();
    }

    internal void StopMainScreenAnimations()
    {
        LeanTween.cancelAll();

        playButtonBackground.color = currentColors[0];
        playButtonRect.localScale = Vector2.one;
        playButtonRect.localRotation = Quaternion.identity;
    }

    #endregion

    #region GAME_OVER_SCREEN_UI_TWEEN
    void PlayGameOverTitleImageTween()
    {
        LeanTween.rotateZ(gameOverTitielImageRect.gameObject, -720F, 3F).setEaseLinear().setLoopCount(-1);
    }

    void StopGameOverTitileImageTween()
    {
        if (LeanTween.isTweening(gameOverTitielImageRect.gameObject))
        {
            LeanTween.cancel(gameOverTitielImageRect.gameObject);
            LeanTween.rotateZ(gameOverTitielImageRect.gameObject, 0F, 0.5F).setEaseOutCubic();
        }
    }
    #endregion
}
