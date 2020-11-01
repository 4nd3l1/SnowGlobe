using Boo.Lang.Runtime.DynamicDispatching;
using System.Collections;
using System.Collections.Generic;
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
    private Camera _cam1;
    private float distance = 0;
 

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
        if (button_X)
        {
            List<GameObject> selection = (GameObject.FindGameObjectsWithTag("Sphere")[0]).GetComponent<collision>().selection;

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
