using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class InventoryToggleController : MonoBehaviour
{
    [Header("Inventario")]
    [SerializeField] private GameObject inventoryCanvas;

    [Header("Opciones")]
    [SerializeField] private bool pauseGameWhenOpen = false;

    private bool isOpen = false;

    private void Start()
    {
        if (inventoryCanvas != null)
        {
            inventoryCanvas.SetActive(false);
        }

        isOpen = false;
    }

    private void Update()
    {
        bool inventoryPressed = false;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            inventoryPressed = true;
        }
#else
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryPressed = true;
        }
#endif

        if (inventoryPressed)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (isOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    public void OpenInventory()
    {
        isOpen = true;

        if (inventoryCanvas != null)
        {
            inventoryCanvas.SetActive(true);
        }

        if (pauseGameWhenOpen)
        {
            Time.timeScale = 0f;
        }
    }

    public void CloseInventory()
    {
        isOpen = false;

        if (inventoryCanvas != null)
        {
            inventoryCanvas.SetActive(false);
        }

        if (pauseGameWhenOpen)
        {
            Time.timeScale = 1f;
        }
    }
}