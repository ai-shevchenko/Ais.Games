using System.Diagnostics.CodeAnalysis;

namespace Ais.GameEngine.StateMachine.Abstractions;

/// <summary>
///     Контекст игрового цикла
/// </summary>
public class GameContext
{
    private readonly Dictionary<string, object?> _data = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Наименование игрового цикла
    /// </summary>
    public required string LoopName { get; init; }

    /// <summary>
    ///     Текущее состояние
    /// </summary>
    public IGameState? CurrentState { get; set; }

    /// <summary>
    ///     Игровые данные
    /// </summary>
    public IReadOnlyDictionary<string, object?> Data => _data.AsReadOnly();

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    public bool TryAdd<T>(string key, T value)
    {
        var valueKey = GetValueKey<T>(key);
        return _data.TryAdd(valueKey, value);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    public void Set<T>(string key, T value)
    {
        var valueKey = GetValueKey<T>(key);
        _data[valueKey] = value;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool TryGet<T>(string key, [NotNullWhen(true)] out T? value)
    {
        var valueKey = GetValueKey<T>(key);

        if (_data.TryGetValue(valueKey, out var item))
        {
            if (item is T typedValue)
            {
                value = typedValue;
                return true;
            }
        }

        value = default;
        return false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Get<T>(string key)
    {
        if (TryGet<T>(key, out var value))
        {
            return value;
        }

        throw new KeyNotFoundException($"The given key '{GetValueKey<T>(key)}' was not present in the game data.");
    }

    private static string GetValueKey<T>(string key)
    {
        var typedKey = string.Intern($"{key}:{typeof(T).Name}");
        return typedKey;
    }

}
