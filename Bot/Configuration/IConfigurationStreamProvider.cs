using System;
using System.IO;
using System.Threading.Tasks;

namespace Bot.Configuration
{
    public interface IConfigurationStreamProvider : IDisposable
    {
        Task<Stream> GetStreamAsync();
    }
}
