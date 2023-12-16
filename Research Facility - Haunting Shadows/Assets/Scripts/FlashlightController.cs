using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light Flashlight; // Reference to the Spotlight component
    private bool isFlashlightOn = false; // Flashlight state

    void Update()
    {
        // Check for the 'F' key press to toggle the flashlight
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlashlightOn = !isFlashlightOn; // Toggle the state
            Flashlight.enabled = isFlashlightOn; // Enable or disable the light based on the state
        }
    }
}
