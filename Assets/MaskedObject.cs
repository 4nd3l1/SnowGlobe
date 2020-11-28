using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskedObject : MonoBehaviour
{
    public int RenderQueueNum;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material.renderQueue = RenderQueueNum;
;
    }
}
