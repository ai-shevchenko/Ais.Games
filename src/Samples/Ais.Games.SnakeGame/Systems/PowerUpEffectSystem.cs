using Ais.ECS.Abstractions.Entities;
using Ais.ECS.Extensions;
using Ais.GameEngine.Extensions.Ecs;
using Ais.Games.SnakeGame.Components;

namespace Ais.Games.SnakeGame.Systems;

/// <summary>
///     Обновляет активные эффекты паверапов (таймеры, множитель очков).
/// </summary>
internal sealed class PowerUpEffectSystem : EcsSystem
{
    public override void Update(float deltaTime)
    {
        // Ищем голову змейки
        var segments = World.CreateQuery()
            .With<SnakeSegment>()
            .With<Position>()
            .GetResult()
            .Entities;

        if (segments.Length == 0)
        {
            return;
        }

        IEntity? head = null;
        foreach (var e in segments)
        {
            var seg = e.GetComponent<SnakeSegment>(World);
            if (seg.IsHead)
            {
                head = e;
                break;
            }
        }

        if (head is null)
        {
            return;
        }

        // Обновляем активный эффект на голове
        if (World.GetStore<ActivePowerUpEffect>().Contains(head))
        {
            ref var effect = ref head.GetComponent<ActivePowerUpEffect>(World);
            effect.RemainingSeconds -= deltaTime;

            if (effect.RemainingSeconds <= 0f)
            {
                // Эффект закончился — сбрасываем множитель очков
                ResetScoreMultiplier();
                World.GetStore<ActivePowerUpEffect>().Remove(head);
                return;
            }

            // Если эффект удвоения очков — поддерживаем множитель
            if (effect.Type == PowerUpType.DoubleScore)
            {
                EnsureScoreMultiplier(2);
            }
        }
        else
        {
            // Нет активного эффекта — множитель по умолчанию
            ResetScoreMultiplier();
        }
    }

    private void EnsureScoreMultiplier(int multiplier)
    {
        var scores = World.CreateQuery()
            .With<Score>()
            .GetResult()
            .Entities;

        if (scores.Length == 0)
        {
            return;
        }

        foreach (var entity in scores)
        {
            ref var score = ref entity.GetComponent<Score>(World);
            score.ScoreMultiplier = multiplier;
        }
    }

    private void ResetScoreMultiplier()
    {
        var scores = World.CreateQuery()
            .With<Score>()
            .GetResult()
            .Entities;

        foreach (var entity in scores)
        {
            ref var score = ref entity.GetComponent<Score>(World);
            score.ScoreMultiplier = 1;
        }
    }
}