using Ais.GameEngine.Core.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Ais.GameEngine.Extensions.Abstractions;

public interface IGameLoopModule
{
    void Configure(GameLoopBuilderSettings settings, IConfiguration configuration);
}