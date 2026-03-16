using UnityEngine;

using System;

namespace TravesiaColombia.Core
{
    // ── ENUMS ────────────────────────────────────────────────────────────────

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        LevelComplete
    }

    public enum RegionType
    {
        Costa,
        EjeCafetero,
        Selva
    }

    public enum PowerUpType
    {
        Velocidad,
        Vuelo,
        VidaExtra,
        Invencibilidad
    }

    public enum EnemyType
    {
        Erizo,
        Medusa,
        Canon,
        Condor,
        HachaGiratoria,
        Zancudo,
        PlantaCarnivora
    }

    public enum CardRarity
    {
        Comun,
        Raro,
        Epico
    }

    // ── EVENTOS PLAYER ───────────────────────────────────────────────────────

    public readonly struct PlayerDied { }

    public readonly struct PlayerJumped { }

    public readonly struct PlayerLanded { }

    public readonly struct PlayerHurt
    {
        public readonly int livesRemaining;
        public PlayerHurt(int livesRemaining)
        {
            this.livesRemaining = livesRemaining;
        }
    }

    // ── EVENTOS COLECCIONABLES ───────────────────────────────────────────────

    public readonly struct CoinCollected
    {
        public readonly int amount;
        public readonly RegionType region;
        public CoinCollected(int amount, RegionType region)
        {
            this.amount = amount;
            this.region = region;
        }
    }

    public readonly struct CardFound
    {
        public readonly string cardId;
        public readonly RegionType region;
        public CardFound(string cardId, RegionType region)
        {
            this.cardId = cardId;
            this.region = region;
        }
    }

    public readonly struct PowerUpActivated
    {
        public readonly PowerUpType type;
        public readonly float duration;
        public PowerUpActivated(PowerUpType type, float duration)
        {
            this.type = type;
            this.duration = duration;
        }
    }

    // ── EVENTOS ENEMIGOS ─────────────────────────────────────────────────────

    public readonly struct EnemyDefeated
    {
        public readonly EnemyType enemyType;
        public readonly int scoreValue;
        public EnemyDefeated(EnemyType enemyType, int scoreValue)
        {
            this.enemyType = enemyType;
            this.scoreValue = scoreValue;
        }
    }

    public readonly struct EnemySpawned
    {
        public readonly EnemyType enemyType;
        public EnemySpawned(EnemyType enemyType)
        {
            this.enemyType = enemyType;
        }
    }

    // ── EVENTOS GAME STATE ───────────────────────────────────────────────────

    public readonly struct GamePaused { }

    public readonly struct GameResumed { }

    public readonly struct GameOver { }

    public readonly struct LevelCompleted
    {
        public readonly RegionType region;
        public readonly float timeElapsed;
        public LevelCompleted(RegionType region, float timeElapsed)
        {
            this.region = region;
            this.timeElapsed = timeElapsed;
        }
    }

    public readonly struct RegionUnlocked
    {
        public readonly RegionType region;
        public RegionUnlocked(RegionType region)
        {
            this.region = region;
        }
    }

    // ── EVENTOS SCORE ────────────────────────────────────────────────────────

    public readonly struct ScoreChanged
    {
        public readonly int newScore;
        public readonly int delta;
        public ScoreChanged(int newScore, int delta)
        {
            this.newScore = newScore;
            this.delta = delta;
        }
    }

    public readonly struct TimerTick
    {
        public readonly float timeElapsed;
        public TimerTick(float timeElapsed)
        {
            this.timeElapsed = timeElapsed;
        }
    }

    // ── EVENTOS TIENDA ───────────────────────────────────────────────────────

    public readonly struct ItemPurchased
    {
        public readonly string itemId;
        public readonly int cost;
        public ItemPurchased(string itemId, int cost)
        {
            this.itemId = itemId;
            this.cost = cost;
        }
    }

    public readonly struct InsufficientFunds
    {
        public readonly int required;
        public readonly int available;
        public InsufficientFunds(int required, int available)
        {
            this.required = required;
            this.available = available;
        }
    }

    // ── EVENTOS CULTURA ──────────────────────────────────────────────────────

    public readonly struct CulturalCheckpointTriggered
    {
        public readonly string questionId;
        public CulturalCheckpointTriggered(string questionId)
        {
            this.questionId = questionId;
        }
    }
}
