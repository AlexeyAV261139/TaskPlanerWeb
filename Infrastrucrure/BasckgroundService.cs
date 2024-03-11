using Microsoft.Extentions.Hosting;

namespace Infrastructure
{

    // тут всё, что связанно с внешними сервисами
    public class BackgroundService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken) 
        { 
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}