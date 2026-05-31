using UnityEngine;
using TravesiaColombia.Player;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            player.SetCheckpoint(transform);
        }
    }
}