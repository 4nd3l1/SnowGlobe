
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class collision : MonoBehaviour
{
    public List<GameObject> selection = new List<GameObject>();
    void OnCollisionEnter(Collision col)
    {
        // When target is hit      
        if (col.gameObject.tag == "Selectable" & !selection.Contains(col.gameObject))
        {
            selection.Add(col.gameObject);
            List<InputDevice> devices_right = new List<InputDevice>();
            InputDeviceCharacteristics ControllerCharacteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;
            InputDevices.GetDevicesWithCharacteristics(ControllerCharacteristics, devices_right);
            InputDevice rightController = devices_right[0];
            rightController.SendHapticImpulse(0, 1, 0.1f);
            
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
