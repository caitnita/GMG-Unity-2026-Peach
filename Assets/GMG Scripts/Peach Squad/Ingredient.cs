using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "Scriptable Objects/Ingredient")]
public class Ingredient : ScriptableObject
{
    public string ingredientName;
    public Sprite sprite;
    public AudioClip sound;

    [Header("Note: Currently, these three settings cannot be combined")]

    [Header("Required ingredients will always appear once, at the beginning of the order")]
    public bool isRequired;
    [Header("Unique items will appear once if at all, anywhere in the order")]
    public bool isUnique;
    [Header("Cursed ingredients will only be ordered by cursed customers")]
    public bool isCursed;
}
