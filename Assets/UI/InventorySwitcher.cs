using UnityEngine;

public class InventorySwitcher : MonoBehaviour
{
    public GameObject inventory1; // Reference to Inventory1
    public GameObject inventory2; // Reference to Inventory2

    private void Start()
    {
        // Ensure only inventory1 is active by default
        inventory1.SetActive(true);
        inventory2.SetActive(false);
    }

    private void Update()
    {
        // Check for key presses
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Activate Inventory1 and deactivate Inventory2
            inventory1.SetActive(true);
            inventory2.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Activate Inventory2 and deactivate Inventory1
            inventory1.SetActive(false);
            inventory2.SetActive(true);
        }
    }
}
