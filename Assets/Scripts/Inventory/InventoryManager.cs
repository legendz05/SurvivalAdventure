using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    InputManager inputManager;

    public bool inventoryOpen = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        gameObject.SetActive(false);

        inputManager = FindAnyObjectByType<InputManager>();
    }

    public void OpenInventory()
    {
        gameObject.SetActive(true);
        inputManager.EnableInventoryControls();
        inventoryOpen = true;
    }

    public void CloseInventory()
    {
        inputManager.DisableInventoryControls();
        gameObject.SetActive(false);
        inventoryOpen = false;
    }
}
