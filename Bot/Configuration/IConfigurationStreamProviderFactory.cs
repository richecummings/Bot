using System;
namespace Bot.Configuration
{
    public interface IConfigurationStreamProviderFactory
    {
        IConfigurationStreamProvider Create();
    }
}
