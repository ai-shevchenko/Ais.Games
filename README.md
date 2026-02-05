## Ais.Games – модульный игровой движок на ECS

Данный репозиторий содержит простой, но расширяемый игровой движок с поддержкой:

- **ECS‑архитектуры** (`Ais.ECS`, `Ais.ECS.Abstractions`)
- **модульной системы** (`GameEngineModule` + загрузка модулей по сборкам)
- **асинхронных хуков игрового цикла** (обновление, рендер, инициализация и т.д.)
- **шины сигналов (event bus)** (`SignalBus`) с синхронной и асинхронной обработкой
- **командной системы** (шаблон Command с undo/redo)
- **системы состояний игрового цикла и таймеров**

В качестве примера использования есть игра `Snake` в `Samples/Ais.Games.RogueLike`.

---

## Структура решения

- **`ECS Framework`**
  - **`Ais.ECS.Abstractions`** – интерфейсы мира, сущностей, компонентов, систем и запросов:
    - `IWorld`, `IEntity`, `IComponent`, `ISystem`, `IQuery`, `QueryResult` и т.п.
  - **`Ais.ECS`** – реализация ECS:
    - `World`, `Entity`, `ComponentStore`, `ComponentRegistry`, `SystemManager`, `QueryBuilder`, `QueryProvider`.
- **`Game Engine`**
  - **`Ais.GameEngine.Core`** – ядро движка:
    - `GameEngineBuilder`, `GameEngine`, `GameLoop`, состояния (`InitializeState`, `RunningState`, `PauseState`, `StoppingState`),
      система таймеров (`TimerController`, `GameTimer`, `FrameTimer`), хуки (`HooksProvider`) и перехватчики состояний.
  - **`Ais.GameEngine.Core.Abstractions`**, **`Ais.GameEngine.StateMachine.Abstractions`**, **`Ais.GameEngine.Hooks.Abstractions`**,
    **`Ais.GameEngine.TimeSystem.Abstractions`** – абстракции для ядра и стейт‑машины.
  - **`Ais.GameEngine.Modules.Abstractions`** – базовые типы модулей:
    - `GameEngineModule`, `IModuleLoader`, `IKeyedModuleLoader`, расширения `ServiceCollectionExtensions`.
- **`Modules`**
  - **`Ais.GameEngine.Extensions.Ecs`**
    - модуль, подключающий ECS к игровому циклу:
      - `EcsModule` регистрирует `IEcsWorldBuilder`, создаёт `World` и вешает ECS‑хуки на обновление/рендер.
      - `EcsWorldBuilder` позволяет декларативно описывать мир и набор систем.
  - **`Ais.GameEngine.Extensions.Commands`**
    - `CommandsModule` регистрирует `CommandQueue` как `ICommandQueue` и `ICommandExecutor`;
    - `CommandQueue` реализует выполнение команд, а также undo/redo.
  - **`Ais.GameEngine.Extensions.SignalBus`**
    - `SignalBusModule` регистрирует `ISignalBus`, `ISignalPublisher`, `ISignalSubscriber`;
    - `SignalBus` реализует подписку/отписку и публикацию сигналов (синхронно и асинхронно) с потокобезопасным хранением обработчиков.
- **`Samples/Ais.Games.RogueLike`**
  - простая игра `Snake`, демонстрирующая:
    - настройку `GameEngineBuilder`;
    - конфигурацию сервисов/логирования;
    - создание основного игрового цикла;
    - использование ECS для хранения состояния и логики;
    - использование команд для спавна еды.

---

## Как это работает на высоком уровне

1. **`GameEngineBuilder`** создаёт DI‑контейнер, загружает конфигурацию (`gamesettings.json` + окружение + командная строка) и настраивает базовые сервисы.
2. Через **модули** (`GameEngineModule`) в контейнер добавляются подсистемы (ECS, команды, SignalBus и др.).
3. **`GameEngine`** создаёт один или несколько **игровых циклов** (`IGameLoop`), каждый со своей конфигурацией.
4. Игровой цикл запускает **стейт‑машину** (Initialize → Running → Pause → Stopping), вызывает хуки (`IUpdate`, `IRender`, `IAsyncUpdate` и т.п.) и управляет **таймингом** кадров.
5. Модуль ECS создаёт **мир** (`World`) и связывает его с игровым циклом: на каждом кадре вызываются зарегистрированные системы.

---

## Быстрый старт

Ниже приведён типичный минимальный пример на основе `Samples/Ais.Games.RogueLike`.

### 1. Подключение движка к вашему приложению

1. Создайте консольный проект `.NET` (например, `net8.0`).
2. Добавьте ссылки на нужные проекты/пакеты:
   - обязательно: `Ais.GameEngine.Core`, `Ais.GameEngine.Core.Abstractions`,
   - по необходимости: `Ais.ECS`, `Ais.ECS.Abstractions`,
   - модули: `Ais.GameEngine.Extensions.Ecs`, `Ais.GameEngine.Extensions.Commands`, `Ais.GameEngine.Extensions.SignalBus`.
3. Добавьте файл конфигурации `gamesettings.json` в корень проекта (пример см. в `Samples/Ais.Games.RogueLike/gamesettings.json`).

### 2. Настройка `Program.cs`

Простейший пример запуска движка с одним игровым циклом и ECS‑миром:

```csharp
using Ais.ECS.Extensions;
using Ais.GameEngine.Core;
using Microsoft.Extensions.DependencyInjection;

var builder = GameEngineBuilder.Create(args);

// Дополнительные сервисы игры (опционально)
builder.ConfigureGameServices((context, services) =>
{
    // Пример: загрузка настроек окна
    // var settings = context.Configuration.GetRequiredSection(nameof(GameWindowSettings));
    // services.Configure<GameWindowSettings>(settings);
});

// Логирование (опционально)
builder.ConfigureGameLogging((context, logging) =>
{
    // Пример с Serilog смотрите в Samples/Ais.Games.RogueLike/Program.cs
});

var cts = new CancellationTokenSource();

using var gameEngine = builder.Build();
using var mainLoop = gameEngine.CreateGameLoop("main", settings =>
{
    // Настройка DI для конкретного игрового цикла и ECS
    settings.GameServices
        .AddEcs()
        .WithSystem<MyMovementSystem>()
        .WithSystem<MyRenderSystem>()
        .WithWorldSetup((services, world) =>
        {
            // Инициализация мира: создание сущностей, компонентов и т.п.
        });
});

gameEngine.Start(cts.Token);

// Простейший цикл ожидания
while (!cts.IsCancellationRequested)
{
}

gameEngine.Stop();
```

---

## Использование ECS

### Создание компонента

Компонент – это простой тип данных, реализующий `IComponent` (или обычный POCO, если используется конкретная реализация ECS):

```csharp
public sealed class Position // : IComponent (если требуется)
{
    public int X { get; set; }
    public int Y { get; set; }
}
```

### Создание системы

Система реализует `ISystem` и описывает логику обработки набора сущностей:

```csharp
using Ais.ECS.Abstractions.Systems;
using Ais.ECS.Abstractions.Worlds;

public sealed class MovementSystem : ISystem
{
    public void Update(IWorld world)
    {
        // Пример: пройтись по сущностям и обновить компонент Position
    }
}
```

Регистрируется система через `EcsWorldBuilder`:

```csharp
settings.GameServices
    .AddEcs()
    .WithSystem<MovementSystem>();
```

### Инициализация мира

Для создания сущностей и первичного состояния используется `WithWorldSetup`:

```csharp
settings.GameServices
    .AddEcs()
    .WithSystem<MovementSystem>()
    .WithWorldSetup((services, world) =>
    {
        var entity = world.CreateEntity();
        entity.AddComponent(world, new Position { X = 0, Y = 0 });
    });
```

---

## Командная система (Commands)

Модуль команд позволяет инкапсулировать действия в объекты и поддерживать undo/redo.

1. Подключите модуль `Ais.GameEngine.Extensions.Commands` (через ссылку на проект или DLL).
2. Реализуйте команду:

```csharp
using Ais.GameEngine.Extensions.Commands.Abstractions;

public sealed class SpawnEnemyCommand : ICommand
{
    public void Execute()
    {
        // Создать врага
    }

    public void Undo()
    {
        // Удалить врага
    }
}
```

3. В коде игры используйте `ICommandExecutor`:

```csharp
using Ais.GameEngine.Extensions.Commands.Abstractions;

public sealed class GameLogic
{
    private readonly ICommandExecutor _executor;

    public GameLogic(ICommandExecutor executor)
    {
        _executor = executor;
    }

    public void SpawnEnemy()
    {
        _executor.Execute(new SpawnEnemyCommand());
    }
}
```

Команды можно откатить или повторить через `ICommandQueue` (методы `UndoCommand`, `RedoCommand`).

---

## Шина сигналов (SignalBus)

`SignalBus` реализует слабосвязанный обмен событиями между частями системы.

### Определение сигнала

```csharp
using Ais.GameEngine.Extensions.SignalBus.Abstractions;

public sealed class PlayerDiedSignal : ISignal
{
    public int Score { get; init; }
}
```

### Подписка и публикация

```csharp
using Ais.GameEngine.Extensions.SignalBus.Abstractions;

public sealed class PlayerDeathHandler
{
    private readonly ISignalBus _bus;

    public PlayerDeathHandler(ISignalBus bus)
    {
        _bus = bus;
        _bus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
    }

    private void OnPlayerDied(PlayerDiedSignal signal)
    {
        Console.WriteLine($"Player died with score {signal.Score}");
    }
}

// Где‑то в коде игры:
// await _bus.PublishAsync(new PlayerDiedSignal { Score = 100 });
```

`SignalBus` поддерживает:

- синхронные обработчики (`Action<TSignal>`);
- асинхронные (`Func<TSignal, Task>` и `Func<TSignal, CancellationToken, Task>`).

---

## Конфигурация и модули

- Файл конфигурации по умолчанию – `gamesettings.json` (и, при наличии, `gamesettings.{ENV}.json`).
- Модули ищутся через `ModuleLoader` по типу `GameEngineModule` в загруженных сборках.
- Дополнительные модули можно подключать:
  - либо просто добавляя их в тот же exe‑проект;
  - либо через отдельные DLL и загрузку из папки (см. `ModuleLoader.LoadFromDirectory` / `LoadDll` / `LoadAssembly`).

Это позволяет расширять движок собственными модулями: физика, UI, сетевой код и т.п., не меняя ядро.

---

## Пример: Snake (`Samples/Ais.Games.RogueLike`)

Пример `Snake` демонстрирует полный цикл:

- настройка `GameEngineBuilder` и логирования через Serilog;
- конфигурация окна и параметров игры через `GameWindowSettings` и `gamesettings.json`;
- создание игрового цикла с ECS‑миром;
- использование систем `InputSystem`, `MovementSystem`, `RenderSystem` и команд `SpawnFoodCommand`;
- взаимодействие между системами через общий мир и DI.

Рекомендуется начать изучение движка именно с этого примера.
