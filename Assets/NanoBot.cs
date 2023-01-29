using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class NanoBot : NetworkBehaviour {

    public NavMeshAgent agent;

    // Start is called before the first frame update
    void Start() {
        agent = GetComponent<NavMeshAgent>();
        if (!IsServer) { // only server controls this?????
            Destroy(agent);
        }


    }

    // Update is called once per frame
    void Update() {
        // find random hair follicle
        // protec


    }
}
