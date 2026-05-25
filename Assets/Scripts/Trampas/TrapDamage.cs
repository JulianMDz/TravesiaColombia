using UnityEngine;
using TravesiaColombia.Player;

/// <summary>
/// Agrega este script a cualquier trampa que deba quitar vida al Player.
/// El objeto debe estar en el Layer "Trap".
/// Usa OnCollisionEnter para troncos (sólidos) o OnTriggerEnter para triggers.
/// </summary>
public class TrapDamage : MonoBehaviour
{
    [Header("Daño")]
    [SerializeField] private int _damage = 1;
    [SerializeField] private float _knockbackForce = 5f;

    // ── Colisión física (troncos sólidos) ────────────────────────────────────
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        ApplyDamage(collision.transform);
    }

    // ── Trigger (trampas con Is Trigger = ON) ────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        ApplyDamage(other.transform);
    }

    private void ApplyDamage(Transform playerTransform)
    {
        PlayerController pc = playerTransform.GetComponent<PlayerController>();
        if (pc == null)
            pc = playerTransform.GetComponentInParent<PlayerController>();
        if (pc == null) return;

        Vector3 hitDir = (playerTransform.position - transform.position).normalized;
        pc.TakeDamage(_damage, hitDir);
    }
}