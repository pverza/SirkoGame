
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UiSyncBehaviour : NetworkBehaviour
{
    public Text text;
    public string mainMenuSceneName = "MainMenu";

    public void ChangeText(string newText)
    {

        text.text = newText;
        if (IsServer) {
            ChangeTextClientRpc(newText);
        }
    }

    [ClientRpc]
    void ChangeTextClientRpc(string newText) {
        text.text = newText;
    }

    public void DisconnectAndGoToMainMenu() {
        if (BetweenseceneParameters.gamePlayerType != GamePlayerType.SINGLEPLAYER) {
            NetworkManager.Singleton.Shutdown();
        }
        //UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
        Application.Quit();
    }
}
