using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PositionRendererSorter : MonoBehaviour
{
    [SerializeField] private int sortingOrderBase = 500;
    [SerializeField] private float offset = 0f;
    [SerializeField, Min(1)] private int sortingResolution = 100;

    private Renderer targetRenderer;
    private float previousY = float.NaN;

    private void Awake()
    {
        PositionRendererSorter[] sorters = GetComponents<PositionRendererSorter>();
        if (sorters.Length > 1 && sorters[0] != this)
        {
            enabled = false;
            return;
        }

        targetRenderer = GetComponent<Renderer>();
        ApplySortingOrder();
    }

    private void LateUpdate()
    {
        if (Mathf.Approximately(previousY, transform.position.y))
        {
            return;
        }

        ApplySortingOrder();
    }

    private void ApplySortingOrder()
    {
        if (targetRenderer == null)
        {
            return;
        }

        previousY = transform.position.y;
        targetRenderer.sortingOrder = sortingOrderBase -
            Mathf.RoundToInt((previousY + offset) * Mathf.Max(1, sortingResolution));
    }
}
