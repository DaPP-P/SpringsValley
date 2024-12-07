using UnityEngine;

public class OverlapTransparency : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float transparentAlpha;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Set transparency
        if (other.CompareTag("visual")) {
        SetTransparency(sprite, transparentAlpha);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Reset transparency to fully opaque
        SetTransparency(sprite, 1f);
    }

    private void SetTransparency(SpriteRenderer spriteRend, float alpha)
    {
        Color color = spriteRend.color;
        color.a = alpha;
        spriteRend.color = color;
    }
}
