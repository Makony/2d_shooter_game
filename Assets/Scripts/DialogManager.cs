using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [Header("Dialog Components")]
    [SerializeField] private GameObject dialogPanel;
    [SerializeField] private TextMeshProUGUI dialogText;


    [Header("Level Intro Components")]
    [SerializeField] private GameObject levelTitlePanel;
    [SerializeField] private TextMeshProUGUI levelTitlePanelText;
    [SerializeField] private Button StartButton;


    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }

        dialogPanel.SetActive(false);
        StartButton.onClick.AddListener(HideIntro);
        levelTitlePanel.SetActive(false);
        StartButton.gameObject.SetActive(false);
    }
    
    public void StartLevelIntro()
    {
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {

        levelTitlePanel.SetActive(true);

        yield return new WaitForSeconds(2f);


        string message = "Soldier! You have been tasked to find the kill codes to turn off the rogue robots!\n\nFor now we drop you in an old castle. You have to find Robot's teleporter and go to to their basement where you find your objective. \n\n I may contact again. \n\n Be careful, though! These nasty robots could be hiding in ancient Boxes! Maybe you could find some useful items for your upcoming fight!\n\n Holding Tab could help.";

        levelTitlePanelText.text = message;
        levelTitlePanelText.fontSize = 12;
        levelTitlePanelText.fontStyle = default;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(3f);
        StartButton.gameObject.SetActive(true);
    }

    private void HideIntro()
    {
        Debug.Log("HideIntro method called!");
        Time.timeScale = 1f;
        levelTitlePanel.SetActive(false);
    }


    public void ShowDialog(string message)
    {
        dialogPanel.SetActive(true);
        dialogText.text = message;

    }

    public void HideDialog()
    {
        dialogPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}