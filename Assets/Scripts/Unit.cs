using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class Unit : NetworkBehaviour {

    NavMeshAgent agent;

    // Start is called before the first frame update
    protected virtual void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start() {
        if (!IsServer) { // only server controls this
            Destroy(agent);
        }
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

    }
}
