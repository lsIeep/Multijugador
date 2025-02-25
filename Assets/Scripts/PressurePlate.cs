using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public bool isPressed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPressed = true;
            PressureManager.Instance.CheckAllPlates();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPressed = false;
            PressureManager.Instance.CheckAllPlates();
        }
    }
}
