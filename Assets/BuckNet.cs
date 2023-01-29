using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BuckNet : MonoBehaviour {

    void OnEnable() {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted() {
        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnLoadComplete;
    }

    private void OnLoadComplete(ulong clientId, string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode) {
        if (sceneName != "Game") {
            return;
        }
    }

    private void OnClientConnect(ulong obj) {
        Debug.Log($"client {obj} connected");
    }
    private void OnClientDisconnect(ulong obj) {
        Debug.Log($"client {obj} disconnected, farewell!");
    }





}
