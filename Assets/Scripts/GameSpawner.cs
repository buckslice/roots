using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class GameSpawner : NetworkBehaviour {

    public GameObject nanoBotPrefab;
    public GameObject bugPrefab;
    public GameObject babyBugPrefab;

    // Start is called before the first frame update
    void Start() {
        if (!IsServer) {
            return;
        }

        for (int i = 0; i < 50; i++) {
            SpawnBug();
        }

        SpawnBotForEachPlayer();
    }

    void SpawnBug() {
        var prefab = Random.value < 0.5f ? bugPrefab : babyBugPrefab;
        var bug = SpawnPrefab(prefab, 40.0f);
        bug.GetComponent<NetworkObject>().Spawn();
    }

    void SpawnBotForEachPlayer() {
        foreach (var client in NetworkManager.Singleton.ConnectedClients.Values) {
            var go = SpawnPrefab(nanoBotPrefab, 10.0f);
            var bot = go.GetComponent<NanoBot>();
            var color = BuckNet.clientColors[client.ClientId];
            bot.SetColor_ClientRpc(color);
            go.GetComponent<NetworkObject>().SpawnWithOwnership(client.ClientId);
        }
    }

    public GameObject SpawnPrefab(GameObject spawnPrefab, float radius) {
        Vector3 pos = Vector3.up * 100 + Random.insideUnitSphere * radius;
        if (Physics.Raycast(pos, Vector3.down, out RaycastHit info, 1000)) {
            pos = info.point;
        }
        return Instantiate(spawnPrefab, pos, Quaternion.identity);
    }

}