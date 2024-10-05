using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameHelperScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Vector3 returnMouseDirection(GameObject item)
    {
        // Converts the mouse coordinates to in game coordinates and finds the difference between
        // there and the weapon.hand.
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - item.transform.position;
        difference.z = 0;
        difference.Normalize();

        return difference;
    }

    public void WaitForSecondsAndResume(float waitTime)
    {
        StartCoroutine(WaitCoroutine(waitTime));
    }

    private IEnumerator WaitCoroutine(float waitTime)
    {
        // Wait for the specified number of seconds
        yield return new WaitForSeconds(waitTime);
    }
}
