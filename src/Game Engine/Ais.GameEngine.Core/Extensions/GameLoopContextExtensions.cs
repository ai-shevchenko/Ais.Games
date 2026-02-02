using Ais.GameEngine.Core.Abstractions;

namespace Ais.GameEngine.Core.Extensions;

public static class GameLoopContextExtensions
{
    public static void Set<T>(this GameLoopContext context, T value)
    {
        Set(context, typeof(T).Name, value);
    }

    public static void Set<T>(this GameLoopContext context, string name, T value)
    {
        context.GameData[name] = value;
    }

    public static T? Get<T>(this GameLoopContext context)
    {
        return Get<T>(context, typeof(T).Name);
    }

    public static T? Get<T>(this GameLoopContext context, string name)
    {
        if (context.GameData.TryGetValue(name, out var item) && item is T typedItem)
        {
            return typedItem;
        }

        return default;
    }
}