using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class UnitControl : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {

    }

    List<NanoBot> selected = new List<NanoBot>();

    // Update is called once per frame
    void Update() {

        var id = NetworkManager.Singleton.LocalClientId;

        // left click select nanobot
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, 1000, 1 << Layers.Unit)) {
                if (hitInfo.collider.CompareTag(Tags.NanoBot)) {
                    selected.Add(hitInfo.collider.GetComponentInParent<NanoBot>());
                    Debug.Log("wat");
                }
            }
        }

        // right click tell them to move
        if (Input.GetMouseButtonDown(1)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, 1000, 1 << Layers.Scalp)) {
                Debug.Log("hello?);");
                foreach (var bot in selected) {
                    Debug.Log(bot.OwnerClientId + " " + id);
                    if (bot.OwnerClientId == id) {
                        bot.agent.destination = hitInfo.point;
                    }
                }
            }
        }

    }
}
