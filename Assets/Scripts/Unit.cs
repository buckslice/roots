using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class Unit : NetworkBehaviour {

    [HideInInspector] public NavMeshAgent agent;

    public float health;

    public string enemyTag;

    public static Collider[] colliders = new Collider[32];
    public static RaycastHit[] hits = new RaycastHit[32];

    // Start is called before the first frame update
    protected virtual void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start() {
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
            FindEnemy();
        }
    }

    void FindEnemy() {
        var colliders = Physics.OverlapSphere(transform.position, 10, 1 << Layers.Unit);
        Debug.Log(colliders.Length);
    }
}
