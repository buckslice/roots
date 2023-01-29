using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class UnitControl : MonoBehaviour {

    List<NanoBot> selected = new List<NanoBot>();

    // Update is called once per frame
    void Update() {

        var id = NetworkManager.Singleton.LocalClientId;

        // left click select nanobot
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, 1000, 1 << Layers.Unit)) {
                if (hitInfo.collider.CompareTag(Tags.NanoBot)) { // did we hit a bot?
                    var bot = hitInfo.collider.GetComponentInParent<NanoBot>();
                    if (bot.OwnerClientId == id) { // player owns this bot?
                        bot.SetSelected(true);
                        selected.Add(bot);
                    }
                }
            } else { // clear selection if you click off
                foreach (var bot in selected) {
                    bot.SetSelected(false);
                }
                selected.Clear();
            }
        }        

        // right click tell them to move
        if (Input.GetMouseButtonDown(1)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hitInfo, 1000, 1 << Layers.Scalp)) {
                foreach (var bot in selected) {
                    if (bot.OwnerClientId == id) {
                        var pos = hitInfo.point;
                        //Debug.Log($"Request Unit moves here: {pos}");
                        bot.MoveUnit_ServerRpc(pos);
                    }
                }
            }
        }

    }
}
