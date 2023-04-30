using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public enum Directions { NORTH, EAST, SOUTH, WEST };

    [Header("List of all usable spells, preferable in order of least to most complex")]
    public Spell[] spells;

    [Header("Minimum length of a intentional stroke")]
    [Tooltip("Mimimum advisable size is arround 2")]
    public float minStrokeLength;

    // VR camera, used to project path from countroller into 2d
    [HideInInspector]
    public Camera vrCamera;

    public int ParseSpell(List<Vector3> points)
    {
        // Grab the camera if necessary
        if (vrCamera == null)
        {
            vrCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        // Project the points into view space (from world space)
        List<Vector2> points2D = WorldToScreenSpace(points);

        // Calculate the primary directions the user was moving in
        List<Vector2> directions = IsolatePrimaryDirections(points2D, minStrokeLength);

        // Match the directions to a spell
        int spellIndex = DirectionsToSpell(directions);

        return spellIndex;
    }

    public void CastSpell(int index, Vector3 position)
    {
        // Instantiate the spell at the given position facing the direction of the camera
        Quaternion spellRotation = vrCamera.transform.rotation;
        Instantiate(spells[index].prefab, position, spellRotation);
    }

    private List<Vector2> WorldToScreenSpace(List<Vector3> points)
    {
        // Convert all points to screen space
        List<Vector2> points2D = new List<Vector2>();
        foreach (Vector3 v in points)
        {
            Vector3 screenPos = vrCamera.WorldToScreenPoint(v);
            points2D.Add(new Vector2(screenPos.x, screenPos.y));
        }

        return points2D;
    }

    private List<Vector2> IsolatePrimaryDirections(List<Vector2> points, float minDirectionLength)
    {
        // Convert the path of points to a list of vectors
        List<Vector2> vectors = new List<Vector2>();

        // Loop over all adjacent pairs of points and calculate the difference vector
        for (int i = 0; i < points.Count - 1; i++)
        {
            vectors.Add(
                points[i + 1] - points[i]
            );
        }

        // Loop over each vector and snap it to one of the four cardinal directions
        for (int i = 0; i < vectors.Count; i++)
        {
            // Store the magnitude
            float magnitude = vectors[i].magnitude;

            // Calculate the angle
            float angle = Mathf.Atan2(vectors[i].y, vectors[i].x) * Mathf.Rad2Deg;

            // Divide by 90
            angle /= 90;

            // Round to nearest int
            angle = Mathf.Round(angle);

            // Fix special case 4 (360* = 0*)
            if (angle == 4)
            {
                angle = 0;
            }

            // Convert the angle back to 0 --- 315 (45 x ( 0-7 ))
            angle *= 90;

            // Convert the angle back to radians
            angle = angle * Mathf.Deg2Rad;

            // Convert the angle to a vector
            vectors[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // Remember to set the magnitude to the correct value
            vectors[i] *= magnitude;
        }

        // Loop over the list of vectors and find the directions
        List<Vector2> primaryDirections = new List<Vector2>();

        for (int i = 0; i < vectors.Count; i++)
        {
            Vector2 currentDirection = vectors[i].normalized;
            float directionLength = 0;

            while (i < vectors.Count && vectors[i].normalized == currentDirection)
            {
                // Add the length of the current vector to the directionLength
                directionLength += vectors[i].magnitude;

                // Increment I because I want to look at the next vector
                i += 1;
            }

            // If the direction lasted for long enough, add it to the primaryDirections list
            if (directionLength / 100 >= minDirectionLength)
            {
                if (primaryDirections.Count == 0)
                {
                    primaryDirections.Add(currentDirection);
                }
                else
                {
                    // Only add it if the previous direction was different
                    if (primaryDirections[primaryDirections.Count - 1] != currentDirection)
                    {
                        primaryDirections.Add(currentDirection);
                    }
                }
            }
        }

        // Return the primary directions 
        return primaryDirections;
    }
    
    private int DirectionsToSpell(List<Vector2> directions)
    {
        // Convert each direction into a item from the directions index
        List<Directions> simplifiedDirections = new List<Directions>();
        foreach(Vector2 v in directions)
        {
            // Calculate the angle
            float angle = Mathf.Atan2(v.x, v.y) * Mathf.Rad2Deg;

            // Divide by 90
            angle /= 90;

            // Round to nearest int
            angle = Mathf.Round(angle);

            // Fix negatives
            if (angle == -2) { angle = 2; }
            if (angle == -1) { angle = 3; }

            simplifiedDirections.Add((Directions)angle);
        }

        // Loop over each spell and find what one matches the directions
        for(int i = 0; i < spells.Length; i ++)
        {
            // Make sure they are the same length
            if (spells[i].directions.Length != simplifiedDirections.Count) { continue; }

            bool matching = true;

            for(int j = 0; j < spells[i].directions.Length; j ++)
            {
                // See if any of the directions don't match
                if (simplifiedDirections[j] != spells[i].directions[j])
                {
                    matching = false;
                    break;
                }
            }

            // Return the index of the spell if it matches
            if (matching)
            {
                return i;
            }
        }

        return -1;
    }
}
