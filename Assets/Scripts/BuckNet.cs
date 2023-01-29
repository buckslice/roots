using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

// good vid by code monkey about netcode for game objects
//https://www.youtube.com/watch?v=3yuBOB3VrCk
// another couple vids for steam integration
//https://www.youtube.com/watch?v=9CYsQ2Rsr2c
//https://www.youtube.com/watch?v=j0n1mayb1cg

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
