using Ais.GameEngine.Core.Abstractions;
using Ais.GameEngine.Hooks.Abstractions;
using Ais.Games.SnakeGame.Components;
using Ais.Games.SnakeGame.Events;

using Microsoft.Extensions.Logging;

namespace Ais.Games.SnakeGame.Hooks;

internal sealed class LogSignalsHook : BaseHook, IInitialize, IDestroy
{
    private readonly IGameLoopEventBus _eventBus;
    private readonly ILogger<LogSignalsHook> _logger;
    private readonly List<IDisposable> _subscriptions = new();

    public LogSignalsHook(ILogger<LogSignalsHook> logger, IGameLoopEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
    }

    public void OnDestroy()
    {
        foreach (var subscription in _subscriptions)
        {
            subscription?.Dispose();
        }

        _subscriptions.Clear();
    }

    public void Initialize()
    {
        if (!_logger.IsEnabled(LogLevel.Debug))
        {
            return;
        }

        SubscribeToGameStarted();
        SubscribeToFoodEaten();
        SubscribeToSnakeGrowth();
        SubscribeToDirectionChanged();
        SubscribeToWallCollision();
        SubscribeToTailCollision();
        SubscribeToGameOver();
        SubscribeToPowerUpCollected();
        SubscribeToPowerUpExpired();

        SubscribeToSnakeInvalidMovement();
        SubscribeToEntityClipping();
        SubscribeToOutOfBounds();
        SubscribeToSnakeLengthMismatch();
        SubscribeToMultipleHeads();
        SubscribeToBrokenChain();
        SubscribeToDuplicateFood();
        SubscribeToScoreInconsistency();
        SubscribeToPowerUpDurationMismatch();
        SubscribeToPositionSyncError();
        SubscribeToInvalidGameStateTransition();
        SubscribeToSystemExecutionTimeAnomaly();
    }

    private void SubscribeToGameStarted()
    {
        var sub = _eventBus.Subscribe<GameStartedEvent>(null, (_, _) =>
        {
            _logger.LogInformation("🎮 Игра начата!");
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToFoodEaten()
    {
        var sub = _eventBus.Subscribe<FoodEatenEvent>(null, (foodEvent, _) =>
        {
            _logger.LogDebug("🍎 Еда съедена! Очков: +{ScoreGained}, Всего: {TotalScore}",
                foodEvent.ScoreGained, foodEvent.TotalScore);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToSnakeGrowth()
    {
        var sub = _eventBus.Subscribe<SnakeGrowthEvent>(null, (growthEvent, _) =>
        {
            _logger.LogDebug("📏 Змейка выросла! Новая длина: {NewLength}",
                growthEvent.NewLength);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToDirectionChanged()
    {
        var sub = _eventBus.Subscribe<DirectionChangedEvent>(null, (directionEvent, _) =>
        {
            var direction = (directionEvent.DirectionX, directionEvent.DirectionY) switch
            {
                (1, 0) => "→ Вправо",
                (-1, 0) => "← Влево",
                (0, 1) => "↓ Вниз",
                (0, -1) => "↑ Вверх",
                _ => "? Неизвестно"
            };

            _logger.LogDebug("⬅️ Направление изменено: {Direction}", direction);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToWallCollision()
    {
        var sub = _eventBus.Subscribe<WallCollisionEvent>(null, (wallEvent, _) =>
        {
            _logger.LogWarning("⚠️ Столкновение со стеной в позиции ({X}, {Y})",
                wallEvent.PositionX, wallEvent.PositionY);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToTailCollision()
    {
        var sub = _eventBus.Subscribe<TailCollisionEvent>(null, (tailEvent, _) =>
        {
            _logger.LogWarning("⚠️ Столкновение с хвостом в позиции ({X}, {Y})",
                tailEvent.PositionX, tailEvent.PositionY);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToPowerUpCollected()
    {
        var sub = _eventBus.Subscribe<PowerUpCollectedEvent>(null, (powerUpEvent, _) =>
        {
            var powerUpName = powerUpEvent.PowerUpType switch
            {
                PowerUpType.SpeedBoost => "⚡ Ускорение",
                PowerUpType.DoubleScore => "💰 Двойные очки",
                _ => "❓ Неизвестный powerup"
            };

            _logger.LogInformation("✨ Power-up собран: {PowerUpName} на {Duration:F1} сек",
                powerUpName, powerUpEvent.EffectDuration);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToPowerUpExpired()
    {
        var sub = _eventBus.Subscribe<PowerUpExpiredEvent>(null, (expiredEvent, _) =>
        {
            var powerUpName = expiredEvent.PowerUpType switch
            {
                PowerUpType.SpeedBoost => "⚡ Ускорение",
                PowerUpType.DoubleScore => "💰 Двойные очки",
                _ => "❓ Неизвестный powerup"
            };

            _logger.LogDebug("⏰ Power-up истёк: {PowerUpName}", powerUpName);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToGameOver()
    {
        var sub = _eventBus.Subscribe<GameOverEvent>(null, (gameOverEvent, _) =>
        {
            if (gameOverEvent.IsWin)
            {
                _logger.LogInformation("🏆 Победа! Игра окончена успешно!");
            }
            else
            {
                _logger.LogWarning("💀 Поражение! Игра окончена.");
            }

            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToSnakeInvalidMovement()
    {
        var sub = _eventBus.Subscribe<SnakeInvalidMovementEvent>(null, (invalidEvent, _) =>
        {
            _logger.LogError("❌ Недопустимое движение: попытка ({X}, {Y}) при текущем ({CX}, {CY})",
                invalidEvent.AttemptedDirectionX, invalidEvent.AttemptedDirectionY,
                invalidEvent.CurrentDirectionX, invalidEvent.CurrentDirectionY);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToEntityClipping()
    {
        var sub = _eventBus.Subscribe<EntityClippingEvent>(null, (clipEvent, _) =>
        {
            _logger.LogError("❌ Перекрытие сущностей: {Entity1} и {Entity2} на позиции ({X}, {Y})",
                clipEvent.Entity1Type, clipEvent.Entity2Type, clipEvent.PositionX, clipEvent.PositionY);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToOutOfBounds()
    {
        var sub = _eventBus.Subscribe<OutOfBoundsEvent>(null, (boundsEvent, _) =>
        {
            _logger.LogError("❌ {EntityType} вышла за границы на позиции ({X}, {Y}), макс ({MaxX}, {MaxY})",
                boundsEvent.EntityType, boundsEvent.PositionX, boundsEvent.PositionY, boundsEvent.MaxX,
                boundsEvent.MaxY);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToSnakeLengthMismatch()
    {
        var sub = _eventBus.Subscribe<SnakeLengthMismatchEvent>(null, (mismatchEvent, _) =>
        {
            _logger.LogError("❌ Несоответствие длины змейки: ожидается {Expected}, получено {Actual}",
                mismatchEvent.ExpectedLength, mismatchEvent.ActualLength);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToMultipleHeads()
    {
        var sub = _eventBus.Subscribe<MultipleHeadsDetectedEvent>(null, (headsEvent, _) =>
        {
            _logger.LogError("❌ Обнаружено несколько голов: {Count}",
                headsEvent.HeadCount);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToBrokenChain()
    {
        var sub1 = _eventBus.Subscribe<BrokenSnakeChainEvent>(null, (chainEvent, _) =>
        {
            _logger.LogError("❌ Разорванная цепь змейки: сегментов {Count}, макс порядок {MaxOrder}",
                chainEvent.SegmentCount, chainEvent.MaxOrder);
            return Task.CompletedTask;
        });

        var sub2 = _eventBus.Subscribe<SnakeChainRepairedEvent>(null, (chain, _) =>
        {
            _logger.LogDebug("Цепь змейки восстановлена: сегментов {Count}", chain.NewLength);
            return Task.CompletedTask;
        });

        _subscriptions.AddRange(sub1, sub2);
    }

    private void SubscribeToDuplicateFood()
    {
        var sub = _eventBus.Subscribe<DuplicateFoodEvent>(null, (foodEvent, _) =>
        {
            _logger.LogError("❌ Дублирование еды: {Count} на позиции ({X}, {Y})",
                foodEvent.FoodCount, foodEvent.PositionX, foodEvent.PositionY);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToScoreInconsistency()
    {
        var sub = _eventBus.Subscribe<ScoreInconsistencyEvent>(null, (scoreEvent, _) =>
        {
            _logger.LogError("❌ Несоответствие счёта: ожидается {Expected}, получено {Actual}, съедено {Fruits}",
                scoreEvent.ExpectedScore, scoreEvent.ActualScore, scoreEvent.FruitsEaten);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToPowerUpDurationMismatch()
    {
        var sub = _eventBus.Subscribe<PowerUpDurationMismatchEvent>(null, (durationEvent, _) =>
        {
            _logger.LogError("❌ Несоответствие длительности {PowerUp}: ожидается {Expected:F1}, получено {Actual:F1}",
                durationEvent.PowerUpType, durationEvent.ExpectedDuration, durationEvent.ActualDuration);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToPositionSyncError()
    {
        var sub = _eventBus.Subscribe<PositionSyncErrorEvent>(null, (syncEvent, _) =>
        {
            _logger.LogError("❌ Ошибка синхронизации {EntityType}: ожидается ({EX}, {EY}), получено ({AX}, {AY})",
                syncEvent.EntityType, syncEvent.ExpectedX, syncEvent.ExpectedY, syncEvent.ActualX, syncEvent.ActualY);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToInvalidGameStateTransition()
    {
        var sub = _eventBus.Subscribe<InvalidGameStateTransitionEvent>(null, (stateEvent, _) =>
        {
            _logger.LogError("❌ Недопустимый переход состояния: {From} → {To}",
                stateEvent.FromState, stateEvent.ToState);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }

    private void SubscribeToSystemExecutionTimeAnomaly()
    {
        var sub = _eventBus.Subscribe<SystemExecutionTimeAnomalyEvent>(null, (timeEvent, _) =>
        {
            _logger.LogWarning("⚠️ Система {System} выполнялась дольше порога: {Actual:F2}ms > {Threshold:F2}ms",
                timeEvent.SystemName, timeEvent.ExecutionTimeMs, timeEvent.ThresholdMs);
            return Task.CompletedTask;
        });

        _subscriptions.Add(sub);
    }
}
