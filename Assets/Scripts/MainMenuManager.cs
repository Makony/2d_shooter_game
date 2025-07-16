using UnityEngine;
using UnityEngine.SceneManagement;

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
}
