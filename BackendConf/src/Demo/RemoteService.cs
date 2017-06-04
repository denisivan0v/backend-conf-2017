using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Demo
{
    public sealed class RemoteService
    {
        private readonly ILogger<RemoteService> _logger;

        public RemoteService(ILogger<RemoteService> logger)
        {
            _logger = logger;
        }

        // ReSharper disable once InconsistentNaming
        public async Task<IReadOnlyCollection<string>> IOBoundOperationAsync(int timeoutInSec)
        {
            _logger.LogInformation("Remote service call started. Delay is {delay} second(s)", timeoutInSec);
            await Task.Delay(timeoutInSec * 1000);
            _logger.LogInformation("Remote service call finished.", timeoutInSec);

            return new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
        }
    }
}
