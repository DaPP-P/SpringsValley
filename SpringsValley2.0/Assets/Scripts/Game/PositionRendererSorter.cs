using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRendererSorter : MonoBehaviour
{
    [SerializeField]
    private int sortingOrderBase = 500;
    [SerializeField]
    private float offset = 0;
    private Renderer myRenderer;


    private void Awake() {
        myRenderer = gameObject.GetComponent<Renderer>();
    }

    private void LateUpdate() {
            myRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);     
    }

}
