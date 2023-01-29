using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class GameSpawner : NetworkBehaviour {

    public GameObject spawnPrefab;

    // Start is called before the first frame update
    void Start() {
        if (IsServer) {
            foreach (var client in NetworkManager.Singleton.ConnectedClients.Values) {

                Vector3 pos = Vector3.up * 100 + Random.insideUnitSphere * 10;
                if (Physics.Raycast(pos, Vector3.down, out RaycastHit info, 1000)) {
                    pos = info.point;
                }
                var go = Instantiate(spawnPrefab, pos, Quaternion.identity);

                go.transform.position = pos;
                go.GetComponent<NetworkObject>().SpawnWithOwnership(client.ClientId);
                //go.GetComponent<NetworkObject>().Spawn();

            }
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
;