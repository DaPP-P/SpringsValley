using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRendererSorter : MonoBehaviour
{
    [SerializeField]
    private int sortingOrderBase = 500;
    [SerializeField]
    private int offset = 0;
    private Renderer myRenderer;


    private void Awake() {
        myRenderer = gameObject.GetComponent<Renderer>();
    }

    private void LateUpdate() {
        if (gameObject.name == "Lower") {
            // Calculate the sorting order for the Lower sprite
            myRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
        }
        else if (gameObject.name == "Upper") {
            // Find the Lower GameObject in the parent (or scene if necessary)
            GameObject lowerObject = transform.parent.Find("Lower").gameObject;

            if (lowerObject != null) {
                Renderer lowerRenderer = lowerObject.GetComponent<Renderer>();
                // Set Upper's sorting order equal to Lower's
                myRenderer.sortingOrder = lowerRenderer.sortingOrder;
            }
        } else {
            myRenderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
        }
    }

}
