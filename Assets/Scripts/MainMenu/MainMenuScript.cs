using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public string sceneName = "Level";
    public TMP_Text feedbackText;
    public TMP_InputField ipAndPortInput;

    public GameObject mainMenu;
    public GameObject clientConnectOptionsMenu;

    public void LoadScene(int gamePlayerType) {

        if (gamePlayerType == 0)
        {
            LoadScene(GamePlayerType.SINGLEPLAYER);
        }
        else if (gamePlayerType == 1) {
            LoadScene(GamePlayerType.MULTIPLAYER_CLIENT);
        } 
        else if (gamePlayerType == 2) {
            LoadScene(GamePlayerType.MULTIPLAYER_HOST);
        }
    
    }

    public void LoadScene(GamePlayerType gamePlayerType)
    {
        switch (gamePlayerType)
        {
            case GamePlayerType.SINGLEPLAYER:
                BetweenseceneParameters.gamePlayerType = GamePlayerType.SINGLEPLAYER;
                break;
            case GamePlayerType.MULTIPLAYER_CLIENT:
                BetweenseceneParameters.gamePlayerType = GamePlayerType.MULTIPLAYER_CLIENT;
                if (!CheckIpAndPort(ipAndPortInput.text)) {
                    return;
                }
                break;
            case GamePlayerType.MULTIPLAYER_HOST:
                BetweenseceneParameters.gamePlayerType = GamePlayerType.MULTIPLAYER_HOST;
                break;
        }
        // Load the scene and pass the scene parameter object
        SceneManager.LoadScene(sceneName);
    }

    private bool CheckIpAndPort(string input)
    {
        // Regex pattern to match IP address and optional port
        string pattern = @"^([0-9]{1,3}\.){3}[0-9]{1,3}(:[0-9]+)?$";

        // Check if the input matches the expected pattern
        if (Regex.IsMatch(input, pattern))
        {
            // Extract IP address and port using regex groups
            Match match = Regex.Match(input, @"^([0-9]{1,3}\.){3}[0-9]{1,3}(:([0-9]+))?$");

            BetweenseceneParameters.ip = match.Groups[0].Value.Split(':')[0]; // Remove trailing dot
            BetweenseceneParameters.port = match.Groups[3].Success ? int.Parse(match.Groups[3].Value) : 7777; // Use default port if not provided
        }
        else
        {
            // Invalid input format, handle the error or display a message
            feedbackText.text = "Invalid input format: " + input + "\n it should be something like 127.0.0.1 or 127.0.0.1:7777";
            return false;
        }

        // Use the extracted IP and port as needed
        feedbackText.text = "Connecting to: IP: " + BetweenseceneParameters.ip + ", Port: " + BetweenseceneParameters.port;
        return true;
    }

    public void OpenJoinGameMenu() {
        mainMenu.SetActive(false);
        clientConnectOptionsMenu.SetActive(true);
    }

    public void GoToMainMenu()
    {
        feedbackText.text = "";
        mainMenu.SetActive(true);
        clientConnectOptionsMenu.SetActive(false);
    }

    public void CloseGame() {
        Application.Quit();
    }

}

static class BetweenseceneParameters {
    public static GamePlayerType gamePlayerType = GamePlayerType.SINGLEPLAYER;
    public static string ip = "";
    public static int port = -1;
}

public enum GamePlayerType { 
    SINGLEPLAYER,
    MULTIPLAYER_CLIENT,
    MULTIPLAYER_HOST
}
