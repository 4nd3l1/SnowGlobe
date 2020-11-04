
using System.Collections.Generic;
using UnityEngine;

public class collision : MonoBehaviour
{
    public List<GameObject> selection = new List<GameObject>();
    void OnCollisionEnter(Collision col)
    {
        // When target is hit      
        if (col.gameObject.tag == "Selectable" & !selection.Contains(col.gameObject))
        {
            selection.Add(col.gameObject);
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "Selectable")
        {
            foreach (var item in selection)
            {
                selection.Remove(item);
            }
        }
    }
}
