using UnityEngine;

public class PressureManager : MonoBehaviour
{
    public static PressureManager Instance;
    public PressurePlate[] plates; 
    public GameObject prefabToSpawn; 
    public Transform spawnPoint; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void CheckAllPlates()
    {
        foreach (PressurePlate plate in plates)
        {
            if (!plate.isPressed)
            {
                return; 
            }
        }

        if (prefabToSpawn != null && spawnPoint != null)
        {
            Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        }
    }
}