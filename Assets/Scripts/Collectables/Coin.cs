using UnityEngine;
using TravesiaColombia.Core;

/// <summary>
/// Coloca este script en cada prefab de moneda.
/// La moneda debe tener un Collider con Is Trigger = true.
/// El Player debe tener el Tag "Player".
/// </summary>
public class Coin : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int _value = 1;
    [SerializeField] private RegionType _region = RegionType.Selva;

    [Header("Efecto al recoger (opcional)")]
    [SerializeField] private GameObject _collectEffect; // partículas o animación

    private bool _collected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_collected) return;
        if (!other.CompareTag("Player")) return;

        _collected = true;

        // Publicar evento — GameManager suma el score, HUD lo muestra
        EventBus.Publish(new CoinCollected(_value, _region));

        // Efecto visual opcional
        if (_collectEffect != null)
            Instantiate(_collectEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
