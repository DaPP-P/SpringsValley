using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets instance;
    private static bool loadAttempted;

    public static GameAssets i
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindFirstObjectByType<GameAssets>();
            if (instance != null || loadAttempted)
            {
                return instance;
            }

            loadAttempted = true;
            GameObject prefab = Resources.Load<GameObject>("GameAssets");
            if (prefab == null)
            {
                Debug.LogError("Resources/GameAssets is missing.");
                return null;
            }

            GameObject assetsObject = Instantiate(prefab);
            instance = assetsObject.GetComponent<GameAssets>();
            if (instance == null)
            {
                Debug.LogError("Resources/GameAssets needs a GameAssets component.", assetsObject);
                Destroy(assetsObject);
            }

            return instance;
        }
    }

    public Transform pfDamagePopup;
    public Transform pfTextBox;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            loadAttempted = false;
        }
    }
}
