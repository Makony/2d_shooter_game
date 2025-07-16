using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

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

    private bool continueButtonPressed = false;


    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }

        dialogPanel.SetActive(false);
        StartButton.onClick.AddListener(HideIntro);
        //levelTitlePanel.SetActive(false);
        StartButton.gameObject.SetActive(false);
    }
    
    public void StartLevelIntro()
    {
        StartCoroutine(IntroSequence());
    }

    private IEnumerator IntroSequence()
    {

        levelTitlePanel.SetActive(true);
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(2f);
        Time.timeScale = 1f;

        string message = "Soldier! You have been tasked to find the kill codes to turn off the rogue robots!\n\nFor now we drop you in an old castle. You have to find Robot's teleporter and go to to their basement where you find your objective. \n\n I may contact again. \n\n Be careful, though! These nasty robots could be hiding in ancient Boxes! Maybe you could find some useful items for your upcoming fight!\n\n Holding Tab could help.";

        levelTitlePanelText.text = message;
        levelTitlePanelText.fontSize = 12;
        levelTitlePanelText.fontStyle = default;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(3f);
        StartButton.gameObject.SetActive(true);
    }



    public void StartLevelIntro2()
    {
        StartCoroutine(IntroSequence2());
    }

    private IEnumerator IntroSequence2()
    {
        string[] messages =
        {
            "Wait... what? This cannot be... \n The Portal sent us to our old HQ.",
            "We lost this HQ after THEY went rogue. \n The robots here are... let's just say they get a lot faster overtime.",
            "Your new objective is to find another teleporter, now!",
            "Don't be shy. Use your Flashlight. \n You don't know how to use it? Maybe press Tab again ;)",
            "And be careful, this place is crawling with our old security cameras.\n Honestly, I don't even remember what we programmed them to do. Good luck with that! :-)",
            "Okay, I've got the old HQ's map. Let me sync the path to your 'Super-Duper Goggles 3000'. \n Oh noooooooooooooooooooooooo my chips fell on the ground. \n Dang it, I have to move and grab them now. Give me a bit time"
        };

        levelTitlePanel.SetActive(true);
        Time.timeScale = 0f;


        foreach (string msg in messages)
        {
            levelTitlePanelText.text = msg;
            levelTitlePanelText.fontSize = 12;

            StartButton.gameObject.SetActive(true);

            continueButtonPressed = false;
            yield return new WaitUntil(() => continueButtonPressed);

            StartButton.gameObject.SetActive(false);
        }
        levelTitlePanel.SetActive(false);
        Time.timeScale = 1f;
        Dialogues.Instance.notdetected();
    }

    public void StartLevelIntro3()
    {
        StartCoroutine(IntroSequence3());
    }

    private IEnumerator IntroSequence3()
    {
        levelTitlePanel.SetActive(false);
        StartButton.gameObject.SetActive(false);

        string[] messages =
        {
            "This looks promising. Finally back where it all started.",
            "Good old times... I remember ruining my PC with a big mug of hot chocolate.",
            "Right after that, the robots went rogue and I... uh... ran away without taking the kill codes \n that were lying right there on the ground.",
            "Anyway, 100% not my fault. My life is just so hard",
            "Hmmm. The portal isn't reacting anymore. It probably needs 2-3 minutes to recharge.",
            "Let's just find the kill codes first. Afterwards, you can wait here a bit and I eat my tacos.", 
        };

        levelTitlePanel.SetActive(true);
        Time.timeScale = 0f;


        foreach (string msg in messages)
        {
            levelTitlePanelText.text = msg;
            levelTitlePanelText.fontSize = 12;

            StartButton.gameObject.SetActive(true);

            continueButtonPressed = false;
            yield return new WaitUntil(() => continueButtonPressed);

            StartButton.gameObject.SetActive(false);
        }
        levelTitlePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void EndLevel3()
    {
        StartCoroutine(EndingSequence());
    }

    public IEnumerator EndingSequence()
    {
        levelTitlePanel.SetActive(false);
        StartButton.gameObject.SetActive(false);

        string[] messages =
        {
            "(Imagine you every particle in your body is getting transported through air)",
            "LET'S GO. EVEN WITH MINUS BUDGET. YOU COULD IMAGINE IT :D",
            "Nice Work Soldier",
            "Thanks for playing this simulator!",
        };

        levelTitlePanel.SetActive(true);
        Time.timeScale = 0f;


        foreach (string msg in messages)
        {
            levelTitlePanelText.text = msg;
            levelTitlePanelText.fontSize = 12;

            StartButton.gameObject.SetActive(true);

            continueButtonPressed = false;
            yield return new WaitUntil(() => continueButtonPressed);

            StartButton.gameObject.SetActive(false);
        }
        levelTitlePanel.SetActive(false);
        LevelManager.Instance.Quit();
    }


    private void HideIntro()
    {
        if (SceneManager.GetActiveScene().name == "Level2" || SceneManager.GetActiveScene().name == "Level3")
        {
            OnContinuePressed();
            return;
        }
        Debug.Log("HideIntro method called!");
        Time.timeScale = 1f;
        levelTitlePanel.SetActive(false);
    }


    public void ShowDialog(string message, Boolean IsTimeStopped = false)
    {
        dialogPanel.SetActive(true);
        dialogText.text = message;
        if (IsTimeStopped == true)
            Time.timeScale = 0f;
    }

    public void ShowDialogWithTimer(string message, float Timer = 10f, Boolean IsTimeStopped = false)
    {
        dialogPanel.SetActive(true);
        dialogText.text = message;
        StartCoroutine(ShowDialogCoroutine(Timer, IsTimeStopped));
    }

    private IEnumerator ShowDialogCoroutine(float Timer, Boolean IsTimeStopped = false)
    {
        if (IsTimeStopped == true) Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(Timer);
        HideDialog();
    }

    private void OnContinuePressed()
    {
        continueButtonPressed = true;
    }

    public void HideDialog()
    {
        dialogPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}