using UnityEngine;

public class OverlapTransparency : MonoBehaviour
{
    public SpriteRenderer spriteBottom;
    public SpriteRenderer spriteTop;
    public float transparentAlpha;
    private void OnTriggerEnter2D(Collider2D other)
    {
            // Set transparency
            SetTransparency(spriteBottom, transparentAlpha);
            SetTransparency(spriteTop, transparentAlpha);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Reset transparency to fully opaque
        SetTransparency(spriteBottom, 1f);
        SetTransparency(spriteTop, 1f);
    }

    private void SetTransparency(SpriteRenderer spriteRend, float alpha)
    {
        Color color = spriteRend.color;
        color.a = alpha;
        spriteRend.color = color;
    }
}
