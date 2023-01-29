using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class NanoBot : NetworkBehaviour {

    NavMeshAgent agent;
    GameObject selectionLight;
    MeshRenderer mr;

    // Start is called before the first frame update
    void Awake() {
        agent = GetComponent<NavMeshAgent>();
        mr = GetComponentInChildren<MeshRenderer>();
        selectionLight = transform.Find("SelectionLight").gameObject;
        selectionLight.SetActive(false);
    }
    void Start() {
        if (!IsServer) { // only server controls this
            Destroy(agent);
        }
    }

    [ClientRpc]
    public void SetColor_ClientRpc(Color color) {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        mpb.SetColor("_Color", color);
        mr.SetPropertyBlock(mpb);
    }

    public void SetSelected(bool selected) {
        selectionLight.SetActive(selected);
    }

    [ServerRpc]
    public void MoveUnit_ServerRpc(Vector3 destination) {
        Debug.Log($"Moving Unit here {destination}");
        agent.destination = destination;


        //Vector3 look = destination - transform.position;
        //look.y = 0;
        //look.Normalize();
        //transform.LookAt(look);
    }

    // Update is called once per frame
    void Update() {
        // find random hair follicle
        // protec
    }
}
