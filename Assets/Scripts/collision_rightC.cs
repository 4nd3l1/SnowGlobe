using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collision_rightC : MonoBehaviour
{
    public GameObject selection;
    void OnCollisionEnter(Collision col)
    {
        // When target is hit   
        RayCast raycast_script = GameObject.FindGameObjectWithTag("RightController").GetComponent<RayCast>();
        if (col.gameObject.tag == "Selectable" & !raycast_script.right_trigger_selecting)
        {
            selection = col.gameObject;
        }
    }

    void OnCollisionExit(Collision col)
    {
        //grap only one object at a time
        RayCast raycast_script = GameObject.FindGameObjectWithTag("RightController").GetComponent<RayCast>();
        if (col.gameObject.tag == "Selectable" & !raycast_script.right_trigger_selecting & col.gameObject.Equals(selection))
        {
            selection = null;
        }
    }
}


