using UnityEngine;
using TravesiaColombia.Core;

/// <summary>
/// Coloca este script en el prefab del Hongo.
/// Requiere un Collider con Is Trigger = ON.
/// El Player debe tener Tag = "Player".
/// Al tocarlo activa el modo vuelo (Flappy Bird) por _flyDuration segundos.
/// </summary>
public class Mushroom : MonoBehaviour
{
    [Header("Power-Up")]
    [SerializeField] private float _flyDuration = 8f;

    [Header("Efecto visual (opcional)")]
    [SerializeField] private GameObject _collectEffect;

    private bool _collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_collected) return;
        if (!other.CompareTag("Player")) return;

        _collected = true;

        EventBus.Publish(
            new PowerUpActivated(
                PowerUpType.Vuelo,
                _flyDuration
            )
        );

        if (_collectEffect != null)
            Instantiate(
                _collectEffect,
                transform.position,
                Quaternion.identity
            );

        gameObject.SetActive(false);
    }

    public void ResetMushroom()
    {
        _collected = false;
        gameObject.SetActive(true);
    }
}