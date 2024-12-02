using UnityEngine;

public class CabinControl : MonoBehaviour
{

    public Animator doorAnimator;
    public Animator roofAnimator;

    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        doorAnimator.SetBool("open", true);
        roofAnimator.SetBool("open", true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        doorAnimator.SetBool("open", false);
        roofAnimator.SetBool("open", false);
    }
}

