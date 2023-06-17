
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{

    //[SerializeField]
    //private Button serverButton;
    //[SerializeField]
    //private Button hostButton;
    //[SerializeField]
    //private Button cientButton;

    //private void Awake()
    //{
    //    serverButton.onClick.AddListener(() =>
    //    {
    //        NetworkManager.Singleton.StartServer();
    //    });
    //
    //    serverButton.onClick.AddListener(() =>
    //    {
    //        NetworkManager.Singleton.StartHost();
    //    });
    //
    //    serverButton.onClick.AddListener(() =>
    //    {
    //        NetworkManager.Singleton.StartClient();
    //    });
    //}

    public void StartHost() {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient() {
        NetworkManager.Singleton.StartClient();
    }
}
