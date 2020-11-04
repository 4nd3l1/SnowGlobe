using Boo.Lang.Runtime.DynamicDispatching;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class RayCast : MonoBehaviour
{
    public LineRenderer line;
    public GameObject sphere;
    private InputDevice rightController;
    private InputDevice leftController;
    private GameObject sphere_rendered;
    private float distance = 0;
    private float x_trans = 0;
    private List<GameObject> snowglobe_objs = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        
        List<InputDevice> devices_right = new List<InputDevice>();
        List<InputDevice> devices_left = new List<InputDevice>();
        InputDeviceCharacteristics ControllerCharacteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;
        InputDevices.GetDevicesWithCharacteristics(ControllerCharacteristics, devices_right);
        rightController = devices_right[0];
        ControllerCharacteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;
        InputDevices.GetDevicesWithCharacteristics(ControllerCharacteristics, devices_left);
        leftController = devices_left[0];
        sphere_rendered = Instantiate(sphere, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        // Raycast drawing //////////////////////////////////////
        rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick_right);
        distance += joystick_right.y * 0.1f;
        if (distance < 0)
        {
            distance = 0;
        }
        else if(distance > 1000)
        {
            distance = 1000;
        }
        sphere_rendered.transform.position = transform.position + transform.TransformDirection(Vector3.forward) * distance;
        leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick_left);
        if(joystick_left.y > 0)
        {
            sphere_rendered.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
        } 
        else if(joystick_left.y < 0)
        {
            if(sphere_rendered.transform.localScale.x > 0)
            {
                sphere_rendered.transform.localScale -= new Vector3(0.05f, 0.05f, 0.05f);
            }

        }


        line.SetPosition(0, transform.position);
        line.SetPosition(1, transform.TransformDirection(Vector3.forward) * 1000);
        line.enabled = true;

        // Raycast selection //////////////////////////////////////

        leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool button_X);
        leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool button_Y);
        collision col_script = (GameObject.FindGameObjectsWithTag("Sphere")[0]).GetComponent<collision>();
        List<GameObject> selection = col_script.selection;
        if (button_X)
        {
            foreach (var item in snowglobe_objs)
            {
                GameObject.Destroy(item);

            }
            selection = col_script.selection;
            foreach (var item in selection)
            {
                if (!snowglobe_objs.Contains(item))
                {
                    GameObject copy = Instantiate(item);
                    Destroy(copy.GetComponent<Rigidbody>());
                    copy.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    copy.transform.position = new Vector3(Camera.main.transform.position.x + copy.transform.position.x*0.1f, transform.position.y,
                        Camera.main.transform.position.z + copy.transform.position.z * 0.1f + 0.2f);
                    snowglobe_objs.Add(copy);
                }
                
            }
        } else if (button_Y)
        {
            foreach (var item in snowglobe_objs)
            {
                GameObject.Destroy(item);
            }
            col_script.selection.Clear();
        }

        /*
        int layerMask = 1 << 8;
        layerMask = ~layerMask; 
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            //Debug.Log("Did Hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            //Debug.Log("Did not Hit");
        }
        */

    }

   
}
