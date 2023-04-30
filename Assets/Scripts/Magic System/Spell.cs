using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="newSpell", menuName="ScriptableObjects/Spell")]
public class Spell : ScriptableObject
{
    public string name;
    public SpellManager.Directions[] directions;
    public GameObject prefab;
}
