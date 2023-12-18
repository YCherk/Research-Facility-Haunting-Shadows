using UnityEngine;
using UnityEngine.UI; // Import the UI namespace

public class FlashlightController : MonoBehaviour
{
    public Light Flashlight; // Reference to the Spotlight component
    public Text flashlightMessage; // UI Text for the message
    public Slider flashlightBatteryBar; // UI Slider for the battery bar

    private bool isFlashlightOn = false; // Flashlight state
    private float flashlightTimer = 0; // Timer to track flashlight usage
    private float messageTimer = 0; // Timer for the message display
    private float flashlightDuration = 30f; // Duration before flashlight turns off
    private float messageDuration = 5f; // Duration to display the message

    void Start()
    {
        // Initialize the UI elements
        flashlightMessage.text = "";
        flashlightBatteryBar.maxValue = flashlightDuration;
        flashlightBatteryBar.value = flashlightDuration;
    }

    void Update()
    {
        // Check for the 'F' key press to toggle the flashlight
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }

        // Update flashlight timer and UI
        if (isFlashlightOn)
        {
            flashlightTimer += Time.deltaTime;
            flashlightBatteryBar.value = flashlightDuration - flashlightTimer;

            if (flashlightTimer >= flashlightDuration)
            {
                TurnOffFlashlight();
                ShowMessage("Damn, flashlight is out, I need to find some batteries.");
            }
        }

        // Update message timer
        if (!string.IsNullOrEmpty(flashlightMessage.text))
        {
            messageTimer += Time.deltaTime;
            if (messageTimer >= messageDuration)
            {
                flashlightMessage.text = ""; // Clear the message after 5 seconds
                messageTimer = 0; // Reset the message timer
            }
        }
    }

    void ToggleFlashlight()
    {
        isFlashlightOn = !isFlashlightOn; // Toggle the state
        Flashlight.enabled = isFlashlightOn; // Enable or disable the light

        if (isFlashlightOn)
        {
            // Removed the line that resets the timer
            flashlightMessage.text = ""; // Clear the message
            messageTimer = 0; // Reset the message timer
        }
    }

    void TurnOffFlashlight()
    {
        isFlashlightOn = false;
        Flashlight.enabled = false;
    }

    // Call this method when the player picks up a battery
    public void PickupBattery()
    {
        flashlightTimer = 0; // Reset the timer
        flashlightBatteryBar.value = flashlightDuration; // Refill the battery bar

        // If the flashlight was off when the battery was picked up, clear the message
        if (!isFlashlightOn)
        {
            flashlightMessage.text = "";
            messageTimer = 0; // Reset the message timer
        }
    }

    // Method to show a message
    private void ShowMessage(string message)
    {
        flashlightMessage.text = message;
        messageTimer = 0; // Reset the message timer when a new message is shown
    }
}
