using System.Collections;
using System.Security.Cryptography;
using Unity.Profiling;
using UnityEngine;

public class ReplaceSprite : MonoBehaviour
{
    public float waitTime;
    private Sprite defaultSprite;
    public Sprite changedSprite;

    private SpriteRenderer sprite;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        defaultSprite = sprite.sprite;
    }

    public void Replace()
    {
        sprite.sprite = changedSprite;
        StartCoroutine(WaitToReturn());
    }

    IEnumerator WaitToReturn()
    {
        yield return new WaitForSeconds(waitTime);
        sprite.sprite = defaultSprite;
    }
}
