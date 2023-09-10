using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scale : MonoBehaviour
{

    public float scaleIncreaseFactor = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
        obj.transform.localScale *= scaleIncreaseFactor;
        }
    }
}
