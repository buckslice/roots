using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BuckNet : MonoBehaviour {

    public static Dictionary<ulong, Color> clientColors = new Dictionary<ulong, Color>();

    void OnEnable() {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientConnect(ulong obj) {
        Debug.Log($"client {obj} connected");
        clientColors[obj] = obj == 0 ? Color.blue : Color.yellow;
    }
    private void OnClientDisconnect(ulong obj) {
        Debug.Log($"client {obj} disconnected, farewell!");
    }

}
