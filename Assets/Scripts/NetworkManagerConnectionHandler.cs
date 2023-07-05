using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkManagerConnectionHandler : MonoBehaviour
{
    void Start()
    {
        switch (BetweenseceneParameters.gamePlayerType)
        {
            case GamePlayerType.SINGLEPLAYER:
                break;
            case GamePlayerType.MULTIPLAYER_CLIENT:
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(BetweenseceneParameters.ip, (ushort)BetweenseceneParameters.port);
                NetworkManager.Singleton.StartClient();
                break;
            case GamePlayerType.MULTIPLAYER_HOST:
                NetworkManager.Singleton.StartHost();
                break;
        }
    }

}
