using UnityEngine;
using UnityEngine.SceneManagement; // Needed to change scenes

public class MainMenuController : MonoBehaviour
{
    public void GoToArcade()
    {
        // Loads the Single Player Game directly
        SceneManager.LoadScene("ArcadeScene");
    }

    public void GoToLobby()
    {
        // Loads the Host/Join screen for Multiplayer
        SceneManager.LoadScene("LobbyScene");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting Game...");
    }
}