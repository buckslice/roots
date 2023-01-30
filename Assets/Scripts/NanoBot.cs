using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class NanoBot : Unit {

    GameObject selectionLight;
    MeshRenderer mr;

    // Start is called before the first frame update
    protected override void Awake() {
        base.Awake();
        mr = GetComponentInChildren<MeshRenderer>();
        selectionLight = transform.Find("SelectionLight").gameObject;
        selectionLight.SetActive(false);
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

}
