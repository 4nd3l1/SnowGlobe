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

    public bool right_trigger_selecting = false;
    
    private float sc = 1.0f;


    public class SnowGlobe_Tuple
    {
        public GameObject copy, original;
    }


    // Start is called before the first frame update
    void Start()
    {
        //Get the controllers
        List<InputDevice> devices_right = new List<InputDevice>();
        List<InputDevice> devices_left = new List<InputDevice>();
        InputDeviceCharacteristics ControllerCharacteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;
        InputDevices.GetDevicesWithCharacteristics(ControllerCharacteristics, devices_right);
        rightController = devices_right[0];
        ControllerCharacteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;
        InputDevices.GetDevicesWithCharacteristics(ControllerCharacteristics, devices_left);
        leftController = devices_left[0];

        //Instantiate de selection bubble
        sphere_rendered = Instantiate(sphere, transform.position, Quaternion.identity);

        //Instantiate the snow globe
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

        /*
        foreach (var item in snowglobe_objs)
        {
            item.transform.position = item.transform.position - mov;
        }
        */
        
        rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out bool grip_right);
        rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool trigger_right);
        leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool trigger_left);

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
                sc = 1;
                foreach (var item in snowglobe_objs)
                {
                    GameObject.Destroy(item);
                }
                snowglobe_objs.Clear();
                snowglobe_orig.Clear();
                selection = col_script.selection;
                foreach (var item in selection)
                {
                    if (!snowglobe_objs.Contains(item))
                    {
                        float radius = sphere_rendered.transform.localScale.x * sphere_rendered.GetComponent<SphereCollider>().radius;
                        float scaler = 0.3f / (sphere_rendered.transform.localScale.x);
                        GameObject copy = Instantiate(item);
                        Debug.Log(copy);
                        Destroy(copy.GetComponent<Rigidbody>());
                        copy.transform.localScale = new Vector3(scaler, scaler, scaler);
                        float x = snowGlobe.transform.position.x + scaler * (copy.transform.position.x - sphere_rendered.transform.position.x);
                        float y = snowGlobe.transform.position.y + scaler * (copy.transform.position.y - sphere_rendered.transform.position.y);
                        float z = snowGlobe.transform.position.z + scaler * (copy.transform.position.z - sphere_rendered.transform.position.z);
                        copy.transform.position = new Vector3(x, y, z);
                        copy.transform.parent = snowGlobe.transform;
                        copy.AddComponent<MaskedObject>();
                        snowglobe_objs.Add(copy);
                        snowglobe_orig.Add(item);
                    }

                }
            } 
        }

        else
        {
            //Grap an object inside the snow globe
            leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick_left);
            if (joystick_left.y > 0 & snowGlobe.transform.localScale.x < 0.8f)
            {
                snowGlobe.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
            }
            else if (joystick_left.y < 0 & snowGlobe.transform.localScale.x > 0.3f)
            {
                snowGlobe.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);

            }

            if (trigger_right & !trigger_left)
            {
                collision_rightC col_script_right = this.GetComponent<collision_rightC>();
                if(col_script_right.selection != null)
                {
                    right_trigger_selecting = true;
                }
                for (int i = 0; i < snowglobe_objs.Count; i++)
                {
                    //make the original object suffer the same transformations as the one inside the snow globe
                    if (snowglobe_objs[i].Equals(col_script_right.selection))
                    {
                        Vector3 difference = transform.position - snowglobe_objs[i].transform.position;
                        difference /= 0.3f / (sphere_rendered.transform.localScale.x);
                        snowglobe_objs[i].transform.position = transform.position;
                        rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick_right);
                        snowglobe_objs[i].transform.rotation = snowglobe_objs[i].transform.rotation * Quaternion.Euler(0, -joystick_right.x, 0);
                        snowglobe_orig[i].transform.rotation = snowglobe_orig[i].transform.rotation * Quaternion.Euler(0, -joystick_right.x, 0);
                        snowglobe_orig[i].transform.position += Quaternion.Inverse(snowGlobe.transform.rotation) * difference; 
                    }
                }

            }
            else
            {
                rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick_right);
                snowGlobe.transform.rotation = snowGlobe.transform.rotation * Quaternion.Euler(0, -joystick_right.x, 0);
                right_trigger_selecting = false;
            }

          /*   
           leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick_left);
            float sens = 0.005f; 
            
            // Scale the snow globe - Main Problem: Release
            if (joystick_left.y > 0 & snowGlobe.transform.localScale.magnitude < 1)
            {
                foreach (var item in snowglobe_objs)
                {
                    item.transform.localScale += new Vector3(sens, sens, sens);
                    item.transform.position = snowGlobe.transform.position + (item.transform.position - snowGlobe.transform.position) * sens;
                }
                snowGlobe.transform.localScale += new Vector3(sens, sens, sens);
            }
            else if (joystick_left.y < 0 & snowGlobe.transform.localScale.magnitude > 0.1)
            {
                foreach (var item in snowglobe_objs)
                {
                    if((item.transform.localScale.magnitude - sens) > 0)
                    {
                        item.transform.localScale -= new Vector3(sens, sens, sens);
                        item.transform.position = snowGlobe.transform.position + (item.transform.position - snowGlobe.transform.position) * sens;
                    }
                }
                snowGlobe.transform.localScale -= new Vector3(sens, sens, sens);
                
            }
       */
            sphere_rendered.SetActive(false);
            line.enabled = false;
        }


    }

   
}
