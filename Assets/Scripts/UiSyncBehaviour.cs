
using Unity.Netcode;
using UnityEngine.UI;

public class UiSyncBehaviour : NetworkBehaviour
{
    public Text text;

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
}
