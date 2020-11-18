using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInstancing: MonoBehaviour
{
    public GameObject thisobject;
    public Material shader;
    void Start()
    {
        thisobject = this.gameObject;
        shader = thisobject.GetComponent<MeshRenderer>().material;
    }

}
