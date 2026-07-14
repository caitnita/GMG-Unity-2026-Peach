using UnityEngine;

[CreateAssetMenu(fileName = "Customer", menuName = "Scriptable Objects/Customer")]
public class Customer : ScriptableObject
{
    public string customerName;
    public Sprite sprite;
    public Animator animator;

    public AudioClip spawnSound;
    public AudioClip successSound;
    public AudioClip failureSound;
    public AudioClip waitSound;
}
