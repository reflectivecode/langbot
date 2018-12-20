using DbUp.Engine.Output;
using Microsoft.Extensions.Logging;

namespace LangBot.Web
{
    public class UpgradeLog : IUpgradeLog
    {
        private readonly ILogger<UpgradeLog> _logger;

        public UpgradeLog(ILogger<UpgradeLog> logger) => _logger = logger;

        public void WriteInformation(string format, params object[] args) => _logger.LogInformation(format, args);

        public void WriteError(string format, params object[] args) => _logger.LogError(format, args);

        public void WriteWarning(string format, params object[] args) => _logger.LogWarning(format, args);
    }
}