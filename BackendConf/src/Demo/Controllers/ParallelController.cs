using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0", Deprecated = true)]
    [ApiVersion("3.0")]
    [Route("parallel")]
    public sealed class ParallelController
    {
        private readonly RemoteService _remoteService;

        public ParallelController(RemoteService remoteService)
        {
            _remoteService = remoteService;
        }

        [HttpGet]
        [Obsolete, MapToApiVersion("1.0")]
        public async Task<IEnumerable<string>> Demo1()
        {
            var data = await _remoteService.IOBoundOperationAsync(timeoutInSec: 1);
            var result = new string[data.Count];

            Parallel.ForEach(
                data,
                async (item, loopState, index) =>
                    {
                        var details = await _remoteService.IOBoundOperationAsync(timeoutInSec: 5);
                        result[index] = string.Join(", ", details);
                    });

            return result;
        }

        [HttpGet]
        [Obsolete, MapToApiVersion("2.0")]
        public async Task<IEnumerable<string>> Demo2()
        {
            var data =
             await _remoteService.IOBoundOperationAsync(timeoutInSec: 1);
            var result = new string[data.Count];

            Parallel.ForEach(
                data,
                (item, loopState, index) =>
                    {
                        var details = _remoteService.IOBoundOperationAsync(timeoutInSec: 5);
                        result[index] = string.Join(", ", details.Result);
                    });

            return result;
        }

        [HttpGet]
        [MapToApiVersion("3.0")]
        public async Task<IEnumerable<string>> Demo3()
        {
            var data = await _remoteService.IOBoundOperationAsync(timeoutInSec: 1);
            var result = new string[data.Count];

            var tasks = data.Select(
                async (item, index) =>
                    {
                        var details = await _remoteService.IOBoundOperationAsync(timeoutInSec: 5);
                        result[index] = string.Join(", ", details);
                    });

            await Task.WhenAll(tasks);

            return result;
        }
    }
}