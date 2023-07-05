using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeacrivateScrippts : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }


    private void OnClientConnected(ulong clientId)
    {
        List<MonoBehaviour> ScriptToDeactivate = new List<MonoBehaviour>();

        MonoBehaviour temp = null;
        temp = GetComponent<Piece>(); if (temp != null) { ScriptToDeactivate.Add(temp); }
        temp = GetComponent<Cell>(); if (temp != null) { ScriptToDeactivate.Add(temp); }

        NetworkManager networkManager = NetworkManager.Singleton;

        if(!networkManager.IsServer)
        foreach (MonoBehaviour c in ScriptToDeactivate)
        {
            
            c.enabled = false;
        }
    }

}
