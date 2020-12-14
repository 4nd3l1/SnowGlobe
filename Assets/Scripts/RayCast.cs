using Boo.Lang.Runtime.DynamicDispatching;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using static UnityEditor.BaseShaderGUI;

public class RayCast : MonoBehaviour
{
    public LineRenderer line;
    public GameObject sphere;
    public GameObject snowglobe_prefab;
    public GameObject colliderVis_prefab;
    public GameObject leftController_GO;
    private InputDevice rightController;
    private InputDevice leftController;
    private GameObject sphere_rendered;
    private GameObject collider_visual;
    private float distance = 0;
    private float x_trans = 0;
    private List<GameObject> snowglobe_objs = new List<GameObject>();
    private List<GameObject> snowglobe_orig = new List<GameObject>();

    bool interacting = false;

    private GameObject snowGlobe;

    private bool mode = false;
    private bool last_A = false;

    public bool right_trigger_selecting = false;

    private float sc = 1.0f;

    private Vector3 dist;
    private bool calc_distance = true;

    private int rotation_mode = 1;

    private bool joystick_pressed = false;


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

        //Instantitate Collider Representation
        collider_visual = Instantiate(colliderVis_prefab, transform);
        collider_visual.transform.position = new Vector3(transform.position.x  - 0.008f, transform.position.y - 0.0024f, transform.position.z + 0.0304f);
        collider_visual.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out bool grip_right);
        rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool trigger_right);
        leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool trigger_left);

        // snow globe sphere //////////////////////////////////////
        Vector3 last_pos = snowGlobe.transform.position;
        if (!trigger_right)
        {
           snowGlobe.transform.position = new Vector3(leftController_GO.transform.position.x,
            leftController_GO.transform.position.y + 0.2f, leftController_GO.transform.position.z);
        }
        
        Vector3 mov = last_pos - snowGlobe.transform.position;

        


        if (grip_right) //Selection mode
        {
            //Show the bubble and ray
            sphere_rendered.SetActive(true);
            line.enabled = true;

            // Bubble movement along z //////////////////////////////////////
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

            // Bubble scaling //////////////////////////////////////
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

            //Ray drawing
            line.SetPosition(0, transform.position);
            line.SetPosition(1, transform.TransformDirection(Vector3.forward) * 1000);
            line.enabled = true;

            // Raycast selection //////////////////////////////////////
            leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool button_X);
            leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool button_Y);

            collision col_script = (GameObject.FindGameObjectsWithTag("Sphere")[0]).GetComponent<collision>();

            List<GameObject> selection = col_script.selection;

            // Solving the on-click problem
            if (interacting)
            {
                interacting = false;
                col_script.selection.Clear();
            }

            // what happens upon copy instruction (X button)
            if (button_X)
            {
                //clean previous selection
                foreach (var item in snowglobe_objs)
                {
                    GameObject.Destroy(item);
                }
                snowglobe_objs.Clear();
                snowglobe_orig.Clear();
                selection = col_script.selection;

                //create the replics
                foreach (var item in selection)
                {
                    if (!snowglobe_objs.Contains(item))
                    {
                        float radius = sphere_rendered.transform.localScale.x * sphere_rendered.GetComponent<SphereCollider>().radius;
                        float scaler = 0.3f / (sphere_rendered.transform.localScale.x);
                        GameObject copy = Instantiate(item);
                        Destroy(copy.GetComponent<Rigidbody>());
                        Destroy(copy.GetComponent<BoxCollider>());
                        MeshCollider mc = copy.AddComponent<MeshCollider>();
                        copy.transform.localScale = new Vector3(scaler, scaler, scaler);
                        float x = snowGlobe.transform.position.x + scaler * (copy.transform.position.x - sphere_rendered.transform.position.x);
                        float y = snowGlobe.transform.position.y + scaler * (copy.transform.position.y - sphere_rendered.transform.position.y);
                        float z = snowGlobe.transform.position.z + scaler * (copy.transform.position.z - sphere_rendered.transform.position.z);
                        copy.transform.position = new Vector3(x, y, z);
                        copy.transform.parent = snowGlobe.transform;
                        snowglobe_objs.Add(copy);
                        snowglobe_orig.Add(item);
                    }

                }
            }
        }

        else
        {
            interacting = true;

            //Scaling the snowglobe
            leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick_left);
            if (joystick_left.y > 0 & snowGlobe.transform.localScale.x < 0.8f)
            {
                snowGlobe.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
            }
            else if (joystick_left.y < 0 & snowGlobe.transform.localScale.x > 0.3f)
            {
                snowGlobe.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);

            }

            if (trigger_right & !trigger_left) // grab an object from the snow globe
            {
                //Change the color of the collider representation to be related to the current rotation axis
                switch (rotation_mode)
                {
                    case 1:
                        {
                            collider_visual.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.red);
                            break;
                        }


                    case 2:
                        {
                            collider_visual.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.green);
                            break;
                        }
                        

                    case 3:
                        {
                            collider_visual.GetComponent<Renderer>().material.SetColor("_BaseColor", Color.blue);
                            break;
                        }
                        

                }

                collision_rightC col_script_right = collider_visual.GetComponent<collision_rightC>();
                if (col_script_right.selection != null)
                {
                    right_trigger_selecting = true;
                }
                for (int i = 0; i < snowglobe_objs.Count; i++)
                {
                    //make the original object suffer the same transformations as the one inside the snow globe
                    if (snowglobe_objs[i].Equals(col_script_right.selection))
                    {
                        // maintaining the same distance between object and controller
                        if (calc_distance)
                        {
                            dist = snowglobe_objs[i].transform.position - transform.position;
                            calc_distance = false;
                        }

                        //Change original and copy positions in relation to the sphere center
                        Vector3 difference = transform.position - (snowglobe_objs[i].transform.position - dist);
                        difference /= 0.3f / (sphere_rendered.transform.localScale.x);

                        snowglobe_objs[i].transform.position = transform.position + dist;

                        // Grabbed object rotation XYZ
                        rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick_right);

                        rightController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool joystick_right_click);
                        if (joystick_right_click)
                        {
                            if (joystick_pressed == false)
                            {
                                rotation_mode++;
                                if (rotation_mode > 3)
                                {
                                    rotation_mode = 1;
                                }
                                joystick_pressed = true;
                            }

                        }
                        else
                        {
                            joystick_pressed = false;
                        }

                        if (rotation_mode == 2)
                        {
                            snowglobe_objs[i].transform.rotation = snowglobe_objs[i].transform.rotation * Quaternion.Euler(0, -joystick_right.y, 0);
                            snowglobe_orig[i].transform.rotation = snowglobe_orig[i].transform.rotation * Quaternion.Euler(0, -joystick_right.y, 0);
                        }
                        else if (rotation_mode == 1)
                        {
                            snowglobe_objs[i].transform.rotation = snowglobe_objs[i].transform.rotation * Quaternion.Euler(-joystick_right.y, 0, 0);
                            snowglobe_orig[i].transform.rotation = snowglobe_orig[i].transform.rotation * Quaternion.Euler(-joystick_right.y, 0, 0);
                        }
                        else
                        {
                            snowglobe_objs[i].transform.rotation = snowglobe_objs[i].transform.rotation * Quaternion.Euler(0, 0, -joystick_right.y);
                            snowglobe_orig[i].transform.rotation = snowglobe_orig[i].transform.rotation * Quaternion.Euler(0, 0, -joystick_right.y);
                        }

                        // Join translation and rotation into a singular transform
                        snowglobe_orig[i].transform.position += Quaternion.Inverse(snowGlobe.transform.rotation) * difference;
                    }
                }

            }
            else
            {
                //Rotate the snow globe

                calc_distance = true;
                rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystick_right);
                snowGlobe.transform.rotation = snowGlobe.transform.rotation * Quaternion.Euler(0, -joystick_right.x, 0);
                right_trigger_selecting = false;
            }

            sphere_rendered.SetActive(false);
            line.enabled = false;
        }


    }


}
