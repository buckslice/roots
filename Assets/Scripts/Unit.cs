using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class Unit : NetworkBehaviour {

    public NavMeshAgent agent;

    public float health;

    public string enemyTag;

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
    protected virtual void Update() {
        if (Input.GetKeyDown(KeyCode.V)) {
            findEnemy();
        }
    }

    void findEnemy() {
        var colliders = Physics.OverlapSphere(transform.position, 10, 1 << Layers.Unit);
        Debug.Log(colliders.Length);
    }
}
