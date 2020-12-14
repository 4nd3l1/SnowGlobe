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

    // Main Function: Change the visual representation of the left/right controllers
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
}
