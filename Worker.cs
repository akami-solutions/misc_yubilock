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
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("YubiLock running at: {time}", DateTimeOffset.Now);
            try
            {
                yubiLockCode.yubiLock();
                _logger.LogInformation("Started YubiLock Service Code");
            }
            catch (Exception e)
            {
                _logger.LogError("Error in YubiLock Service Code: {e}", e);
            }
            //await Task.Delay(60000, stoppingToken);
        }
    }
}