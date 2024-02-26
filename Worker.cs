using System.Net.Mime;

namespace YubiLock_Service;


public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }
    
    
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(Timeout.Infinite, stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("YubiLock running at: {time}", DateTimeOffset.Now);
            try
            {
                _logger.LogInformation("Started YubiLock Service Code");
                await Task.Run(() =>
                {
                    yubiLockCode.yubiLock();
                });
                await Task.Delay(1_000, stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in YubiLock Service Code: {e}", e);
            }
            //await Task.Delay(60000, stoppingToken);
        }
    }
}