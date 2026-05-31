using UnityEngine;

public class BananaTrigger : MonoBehaviour
{
    [SerializeField] private FallingBanana banana;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            banana.DropBanana();
        }
    }
}