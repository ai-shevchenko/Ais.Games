using Ais.ECS.Abstractions.Components;

namespace Ais.ECS.Abstractions.Queries;

/// <summary>
/// Запрос
/// </summary>
public interface IQuery
{
    /// <summary>
    /// Список включаемых типов компонентов
    /// </summary>
    public IReadOnlyList<Type> WithComponents { get; }
    
    /// <summary>
    /// Список исключаемых типов компонентов
    /// </summary>
    public IReadOnlyList<Type> WithoutComponents { get; }
    
    /// <summary>
    /// Отфильтровать по наличию компонента
    /// </summary>
    /// <typeparam name="T">Тип компонента</typeparam>
    /// <returns>Запрос</returns>
    IQuery With<T>()
        where T : IComponent;

    /// <summary>
    /// Отфильтровать по отсутствию компонента
    /// </summary>
    /// <typeparam name="T">Тип компонента</typeparam>
    /// <returns>Запрос</returns>
    IQuery Without<T>() 
        where T : IComponent;

    /// <summary>
    /// Получить результат выполнения запроса
    /// </summary>
    /// <returns>Результат выполнения запроса</returns>
    QueryResult GetResult();  
}