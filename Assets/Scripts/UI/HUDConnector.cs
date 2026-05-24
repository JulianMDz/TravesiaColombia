using UnityEngine;
using TravesiaColombia.Core;

/// <summary>
/// Conecta el EventBus con el HUDController.
/// Coloca este script en el mismo GameObject que HUDController.
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
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<CoinCollected>(OnCoinCollected);
        EventBus.Unsubscribe<ScoreChanged>(OnScoreChanged);
        EventBus.Unsubscribe<PlayerHurt>(OnPlayerHurt);
        EventBus.Unsubscribe<PlayerDied>(OnPlayerDied);
    }

    private void OnCoinCollected(CoinCollected e)
    {
        // Suma los puntos al HUD directamente
        _hud.AddScore(e.amount * 10); // cada moneda vale 10 puntos
    }

    private void OnScoreChanged(ScoreChanged e)
    {
        _hud.UpdateScore(e.newScore);
    }

    private void OnPlayerHurt(PlayerHurt e)
    {
        _hud.UpdateLives(e.livesRemaining);
    }

    private void OnPlayerDied(PlayerDied e)
    {
        _hud.UpdateLives(0);
        _hud.StopTimer();
    }
}