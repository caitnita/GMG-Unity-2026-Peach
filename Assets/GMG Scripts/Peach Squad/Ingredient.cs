using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredients", menuName = "Scriptable Objects/Ingredients")]
public class Ingredient : ScriptableObject
{
    public string ingredientName;
    public Sprite sprite;
}
