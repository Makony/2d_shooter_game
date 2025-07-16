using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

// Class for managing the main menu and starting the game
public class MainMenuManager : MonoBehaviour
{
    // Method that starts the game after clicking "play"
    public void OnStartClick()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void BigButtonClick()
    {
        SoundManager.Instance.ButtonClickSound();
    }

    public void HoverSound()
    {
        SoundManager.Instance.ButtonHoverSound();
    }

    private void AddHoverSound(GameObject buttonObject)
    {
        EventTrigger trigger = buttonObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = buttonObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((eventData) =>
        {
            SoundManager.Instance.ButtonHoverSound();
        });

        trigger.triggers.Add(entry);
    }
}

