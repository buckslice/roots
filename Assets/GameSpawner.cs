using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class GameSpawner : NetworkBehaviour {

    public GameObject spawnPrefab;

    // Start is called before the first frame update
    void Start() {
        if (!IsServer) {
            return;
        }

        StartCoroutine(SpawnBotsForever());
    }

    IEnumerator SpawnBotsForever() {
        while (true) {
            foreach (var client in NetworkManager.Singleton.ConnectedClients.Values) {
                Vector3 pos = Vector3.up * 100 + Random.insideUnitSphere * 10;
                if (Physics.Raycast(pos, Vector3.down, out RaycastHit info, 1000)) {
                    pos = info.point;
                }
                var go = Instantiate(spawnPrefab, pos, Quaternion.identity);
                var bot = go.GetComponent<NanoBot>();
                var color = BuckNet.clientColors[client.ClientId];
                go.transform.position = pos;
                go.GetComponent<NetworkObject>().SpawnWithOwnership(client.ClientId);
                bot.SetColor_ClientRpc(color);
            }

            yield return new WaitForSeconds(5.0f);
        }
    }

}
;