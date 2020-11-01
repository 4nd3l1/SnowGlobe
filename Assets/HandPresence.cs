using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.XR;


public class HandPresence : MonoBehaviour
{
    public enum Orientation { Left, Right};
    public List<GameObject> controllerPrefabs;
    private GameObject targetController_spawn;
    public Orientation controller;
    private InputDevice rightController;
    private InputDevice leftController;
    private InputDeviceCharacteristics ControllerCharacteristics;

    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        if (controller.Equals(Orientation.Left))
        {
            ControllerCharacteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;
            InputDevices.GetDevicesWithCharacteristics(ControllerCharacteristics, devices);
            leftController = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name.Equals("Oculus Quest Controller - Left"));
            if (prefab)
            {
                targetController_spawn = Instantiate(prefab, transform);
            } 
        }
        else if (controller.Equals(Orientation.Right))
        {
            ControllerCharacteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;
            InputDevices.GetDevicesWithCharacteristics(ControllerCharacteristics, devices);
            rightController = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name.Equals("Oculus Quest Controller - Right"));
            if (prefab)
            {
                targetController_spawn = Instantiate(prefab, transform);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*
        //Right controller
        if (rightController!=null)
        {
            rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool button_A);
            rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool button_B);
            rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick_right);
            rightController.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger_right);
            rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool grip_right);

            if (button_A)
            {
                Debug.Log("Pressing A!");
            }
            else if (button_B)
            {
                Debug.Log("Pressing B!");
            }
            else if (trigger_right)
            {
                Debug.Log("Pressing Right Trigger!");
            }
            else if (grip_right)
            {
                Debug.Log("Pressing Right Grip!");
            }
            else if (joystick_right != null & joystick_right.x != 0 & joystick_right.y != 0)
            {
                Debug.Log("Right Joystick: (" + joystick_right.x + "," + joystick_right.y + ")");
            }
        }
        else if (leftController != null)
        {
            leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool button_X);
            leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool button_Y);
            leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick_left);
            leftController.TryGetFeatureValue(CommonUsages.triggerButton, out bool trigger_left);
            leftController.TryGetFeatureValue(CommonUsages.gripButton, out bool grip_left);

            if (button_X)
            {
                Debug.Log("Pressing X!");
            }
            else if (button_Y)
            {
                Debug.Log("Pressing Y!");
            }
            else if (trigger_left)
            {
                Debug.Log("Pressing Left Trigger!");
            }
            else if (grip_left)
            {
                Debug.Log("Pressing Left Grip!");
            }
            else if (joystick_left != null & joystick_left.x != 0 & joystick_left.y != 0)
            {
                Debug.Log("Left Joystick: (" + joystick_left.x + "," + joystick_left.y + ")");
            }
        }
        */
    }
}
