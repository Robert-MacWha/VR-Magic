using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    [Header("Settings used to select controllers")]
    public InputDeviceCharacteristics controllerCharacteristics;
    [Header("Controller models")]
    [Tooltip("Used if specific controller is not found")]
    public GameObject defaultController;
    public List<GameObject> controllerPrefabs;

    [HideInInspector]
    public InputDevice targetDevice;
    private GameObject spawnedController;

    private void Awake()
    {
        // Get all the input devices
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        // Find and instantiate the correct models for the controller
        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);

            if (prefab)
            {
                spawnedController = Instantiate(prefab, transform);
            }
            else
            {
                // If the controller is not recognized, just spawn in the default controller
                Debug.LogError("Did not find controller model");
                Debug.Log(targetDevice.name);
                spawnedController = Instantiate(defaultController, transform);
            }
        }
    }
}
