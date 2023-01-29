using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameSpawner : NetworkBehaviour {

    public GameObject spawnPrefab;

    // Start is called before the first frame update
    void Start() {
        if (IsServer) {
            foreach (var client in NetworkManager.Singleton.ConnectedClients.Values) {
                var go = Instantiate(spawnPrefab, Vector3.up * 100 + Random.insideUnitSphere * 10, Quaternion.identity);
                go.GetComponent<NetworkObject>().SpawnWithOwnership(client.ClientId);
            }
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
