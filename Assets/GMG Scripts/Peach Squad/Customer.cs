using UnityEngine;

[CreateAssetMenu(fileName = "Customer", menuName = "Scriptable Objects/Customer")]
public class Customer : ScriptableObject
{
    public string customerName;
    public Sprite sprite;
    public Animator animator;

    [Header("1=Cute, 2=Creepy, 3=Cursed")]
    public int customerType;

    [Header("Sound Effects")]
    public AudioClip spawnSound;
    public AudioClip successSound;
    public AudioClip failureSound;
    public AudioClip waitSound;
}
