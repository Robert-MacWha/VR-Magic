using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SpellDrawer : MonoBehaviour
{
    [Header("Line used when casting spells")]
    public GameObject defaultLine;
    [Header("How hard the user has to press on the trigger to cast a spell")]
    public float gripThreshold = 0.8f;

    private SpellManager spellManager;
    private HandPresence myHand;
    // Grabed from handPresence object, used to handle inputs
    private InputDevice targetDevice;

    private bool casting = false;
    private LineRenderer stroke;
    private int pointCounter = 0;

    private List<Vector3> positions;

    private void Start()
    {
        // Grab the hand and input device
        myHand = GetComponent<HandPresence>();
        targetDevice = myHand.targetDevice;

        // Grab the spell manager
        spellManager = GameObject.FindGameObjectWithTag("SpellManager").GetComponent<SpellManager>();
    }

    private void Update()
    {
        // See if the player has pressed the grip button
        float gripValue;
        targetDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.grip, out gripValue);

        if (gripValue >= gripThreshold)
        {
            // See if the user is currently casting
            if (casting)
            {
                // Add the current point to the stroke
                AddPointToStroke();
            }
            else
            {
                // Reset the points list
                positions = new List<Vector3>();

                // Create a new linerenderer object
                GameObject newRenderer = Instantiate(defaultLine);
                stroke = newRenderer.GetComponent<LineRenderer>();

                pointCounter = 0;

                // Start casting and add the starting point to the stroke
                casting = true;
                AddPointToStroke();
            }

        }
        else if (casting)
        {
            // If the user was casting, stop casting and send the points to the spellManager
            casting = false;

            int spell = spellManager.ParseSpell(positions);
            Destroy(stroke.gameObject);

            if (spell != -1)
            {
                spellManager.CastSpell(spell, AverageStrokePosition());
            }
        }
    }

    private void AddPointToStroke()
    {
        // Add the position to the positions and add them to the line
        positions.Add(transform.position);

        stroke.positionCount = pointCounter;
        stroke.SetPositions(positions.ToArray());

        pointCounter += 1;
    }

    private Vector3 AverageStrokePosition()
    {
        Vector3 avg = Vector3.zero;
        foreach(Vector3 v in positions)
        {
            avg += v;
        }

        avg /= positions.Count;

        return avg;
    }
}
