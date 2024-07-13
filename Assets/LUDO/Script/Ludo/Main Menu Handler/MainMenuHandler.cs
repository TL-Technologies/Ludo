using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [Space(10)]
    [Header("Panels")]
    [SerializeField] internal GameObject namePanel;
    [SerializeField] internal GameObject mainMenu;

    [Space(10)]
    [Header("Input and Next Button")]
    [SerializeField] internal TMP_InputField _name;
    [SerializeField] internal Button _nextButton;


    void Start()
    {
        _nextButton.onClick.AddListener(OnNameSubmit);
    }


    public void OnNameSubmit()
    {
        if (string.IsNullOrEmpty(_name.text) || string.IsNullOrWhiteSpace(_name.text))
        {
            return;

        }
        StaticData.SetPlayerName(_name.text);
        OpenMainMenu();
    }



    private void OpenMainMenu()
    {
        namePanel.SetActive(false);
        mainMenu.SetActive(true);
    }
}
