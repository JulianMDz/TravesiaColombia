using UnityEngine;
using TravesiaColombia.Core;

/// <summary>
/// Conecta el EventBus con el HUDController.
/// Coloca este script en el mismo GameObject que HUDController.
/// Maneja: monedas/plumas, hongos, vidas y score.
/// </summary>
[RequireComponent(typeof(HUDController))]
public class HUDConnector : MonoBehaviour
{
    private HUDController _hud;

    private void Awake()
    {
        _hud = GetComponent<HUDController>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<CoinCollected>(OnCoinCollected);
        EventBus.Subscribe<ScoreChanged>(OnScoreChanged);
        EventBus.Subscribe<PlayerHurt>(OnPlayerHurt);
        EventBus.Subscribe<PlayerDied>(OnPlayerDied);
        EventBus.Subscribe<PowerUpActivated>(OnPowerUpActivated);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<CoinCollected>(OnCoinCollected);
        EventBus.Unsubscribe<ScoreChanged>(OnScoreChanged);
        EventBus.Unsubscribe<PlayerHurt>(OnPlayerHurt);
        EventBus.Unsubscribe<PlayerDied>(OnPlayerDied);
        EventBus.Unsubscribe<PowerUpActivated>(OnPowerUpActivated);
    }

    // ── Monedas/Plumas ───────────────────────────────────────────────────────
    private void OnCoinCollected(CoinCollected e)
    {
        _hud.AddPluma();           // contador de plumas en HUD
        _hud.AddScore(e.amount * 10); // 10 puntos por moneda
    }

    // ── Score ────────────────────────────────────────────────────────────────
    private void OnScoreChanged(ScoreChanged e)
    {
        _hud.UpdateScore(e.newScore);
    }

    // ── Vidas ────────────────────────────────────────────────────────────────
    private void OnPlayerHurt(PlayerHurt e)
    {
        _hud.UpdateLives(e.livesRemaining);
    }

    private void OnPlayerDied(PlayerDied e)
    {
        _hud.UpdateLives(0);
        _hud.StopTimer();
    }

    // ── Hongos (Power-Up Vuelo) ───────────────────────────────────────────────
    private void OnPowerUpActivated(PowerUpActivated e)
    {
        if (e.type != PowerUpType.Vuelo) return;
        _hud.AddHongo(); // contador de hongos en HUD
    }
}