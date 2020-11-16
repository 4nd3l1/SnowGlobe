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
    public GameObject snowglobe_prefab;
    public GameObject leftController_GO;
    private InputDevice rightController;
    private InputDevice leftController;
    private GameObject sphere_rendered;
    private float distance = 0;
    private float x_trans = 0;
    private List <GameObject> snowglobe_objs = new List<GameObject>(); 
    private List<GameObject> snowglobe_orig = new List<GameObject>();


    private GameObject snowGlobe;

    private bool mode = false;
    private bool last_A = false;

    public class SnowGlobe_Tuple
    {
        public GameObject copy, original;
    }


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

        snowGlobe = Instantiate(snowglobe_prefab);
        snowGlobe.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        // snow globe sphere //////////////////////////////////////

        Vector3 last_pos = snowGlobe.transform.position;
        snowGlobe.transform.position = new Vector3(leftController_GO.transform.position.x,
            leftController_GO.transform.position.y + 0.2f, leftController_GO.transform.position.z);
        Vector3 mov = last_pos - snowGlobe.transform.position;
        
        foreach (var item in snowglobe_objs)
        {
            item.transform.position = item.transform.position - mov;
        }

        
        rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out bool grip_right);

        if (grip_right)         
        {
            sphere_rendered.SetActive(true);
            line.enabled = true;

            // Raycast drawing //////////////////////////////////////
            rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick_right);
            distance += joystick_right.y * 0.1f;
            if (distance < 0)
            {
                distance = 0;
            }
            else if (distance > 1000)
            {
                distance = 1000;
            }
            sphere_rendered.transform.position = transform.position + transform.TransformDirection(Vector3.forward) * distance;
            leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick_left);
            if (joystick_left.y > 0)
            {
                sphere_rendered.transform.localScale += new Vector3(0.05f, 0.05f, 0.05f);
            }
            else if (joystick_left.y < 0)
            {
                if (sphere_rendered.transform.localScale.x > 0)
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
                snowglobe_objs.Clear();
                selection = col_script.selection;
                foreach (var item in selection)
                {
                    if (!snowglobe_objs.Contains(item))
                    {
                        float radius = sphere_rendered.transform.localScale.x * sphere_rendered.GetComponent<SphereCollider>().radius;
                        float scaler = 0.3f / (sphere_rendered.transform.localScale.x);
                        Debug.Log(radius);
                        GameObject copy = Instantiate(item);
                        Destroy(copy.GetComponent<Rigidbody>());
                        copy.transform.localScale = new Vector3(scaler, scaler, scaler);
                        float x = snowGlobe.transform.position.x + scaler * (copy.transform.position.x - sphere_rendered.transform.position.x);
                        float y = snowGlobe.transform.position.y + scaler * (copy.transform.position.y - sphere_rendered.transform.position.y);
                        float z = snowGlobe.transform.position.z + scaler * (copy.transform.position.z - sphere_rendered.transform.position.z);
                        copy.transform.position = new Vector3(x, y, z);
                        snowglobe_objs.Add(copy);
                        snowglobe_orig.Add(item);
                    }

                }
            }
        }
        else
        {
            sphere_rendered.SetActive(false);
            line.enabled = false;
        }


    }

   
}
