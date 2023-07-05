using System.Net;
using System.Net.Sockets;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class IPInfo : MonoBehaviour
{
    public Text ipText;
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        ipText.text = "";
    }

    private void OnClientConnected(ulong clientId)
    {
        if (NetworkManager.Singleton.IsServer) {
            getIpInformations();
        }
    }

    private void getIpInformations() {
        string localIP = GetLocalIPAddress();
        string externalIP = GetExternalIPAddress();

        ipText.text = ("LocalIP (LAN):\n" +
            localIP + "\n" +
            "External IP (Internet):\n" +
            externalIP + "\n");
    }

    private string GetLocalIPAddress()
    {
        string localIP = string.Empty;

        // Get the host name
        string hostName = Dns.GetHostName();

        // Get the IP addresses associated with the host name
        IPAddress[] addresses = Dns.GetHostAddresses(hostName);

        // Find the first IPv4 address
        foreach (IPAddress address in addresses)
        {
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = address.ToString();
                break;
            }
        }

        return localIP;
    }

    private string GetExternalIPAddress()
    {
        using (var client = new WebClient())
        {
            try
            {
                // Request the external IP from a service
                string externalIP = client.DownloadString("https://api.ipify.org");

                return externalIP;
            }
            catch (WebException e)
            {
                Debug.LogError("Failed to get external IP: " + e.Message);
                return string.Empty;
            }
        }
    }
}
