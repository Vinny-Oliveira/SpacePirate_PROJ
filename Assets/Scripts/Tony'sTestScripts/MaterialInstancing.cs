using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInstancing: MonoBehaviour
{
    public GameObject highlightQuad;
    public Material highlightMaterial;
    void Start()
    {
        highlightQuad = this.gameObject;
        highlightMaterial = highlightQuad.GetComponent<MeshRenderer>().material;
    }

}
