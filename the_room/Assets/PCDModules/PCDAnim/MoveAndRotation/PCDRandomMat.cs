using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCDRandomMat : MonoBehaviour
{
    public List<RandomWeightInfo<Material>> randMatList;
    void OnEnable() {
        GetComponent<Renderer>().sharedMaterial = RandomWeightChoose.Choose(randMatList);
    }

}
