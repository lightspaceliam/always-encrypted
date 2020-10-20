using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Threading.Tasks;

namespace AlwaysEncypted.Poc
{
    public abstract class BaseStub
    {
        protected readonly ILogger<BaseStub> Logger;
        protected readonly IConfiguration Configuration;
        protected readonly CultureInfo Culture = CultureInfo.CreateSpecificCulture("en-AU");

        protected BaseStub(
            IConfiguration configuration,
            ILogger<AdoStub> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        internal abstract Task Run();
    }
}
