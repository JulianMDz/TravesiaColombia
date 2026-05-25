using UnityEngine;
using TravesiaColombia.Core;
using TravesiaColombia.Player;

/// <summary>
/// Coloca este script en un GameObject con Collider (Is Trigger = ON)
/// debajo del nivel. Cuando el Player cae, llama TakeDamage directamente.
/// </summary>
public class DeathZone : MonoBehaviour
{
    [Header("Punto de reaparición")]
    [SerializeField] private Transform _spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Llamar TakeDamage directamente en el PlayerController
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc != null)
            pc.TakeDamage(1, Vector3.zero);

        // Reubicar al Player en el spawn point
        if (_spawnPoint != null)
            other.transform.position = _spawnPoint.position;

        // Resetear velocidad
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }
}