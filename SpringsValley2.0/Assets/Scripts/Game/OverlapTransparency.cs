using System.Collections.Generic;
using UnityEngine;

public class OverlapTransparency : MonoBehaviour
{
    public SpriteRenderer sprite;
    [Range(0f, 1f)] public float transparentAlpha = 0.5f;

    private readonly HashSet<Collider2D> visualOverlaps = new HashSet<Collider2D>();
    private float originalAlpha = 1f;

    private void Awake()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        if (sprite != null)
        {
            originalAlpha = sprite.color.a;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != "visual")
        {
            return;
        }

        visualOverlaps.Add(other);
        SetTransparency(transparentAlpha);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "visual")
        {
            return;
        }

        visualOverlaps.Remove(other);
        if (visualOverlaps.Count == 0)
        {
            SetTransparency(originalAlpha);
        }
    }

    private void OnDisable()
    {
        visualOverlaps.Clear();
        SetTransparency(originalAlpha);
    }

    private void SetTransparency(float alpha)
    {
        if (sprite == null)
        {
            return;
        }

        Color colour = sprite.color;
        colour.a = Mathf.Clamp01(alpha);
        sprite.color = colour;
    }
}
