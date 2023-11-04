using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;

namespace YubiLock_Service;

public class Worker : ServiceBase
{
    /// <summary>
    /// Script was written with the help of [this](https://github.com/asciimo71/Rider-WindowsService/blob/master/ConsoleApplication/ConsoleApplication/WinService.cs) repository
    /// </summary>
    
    public const string Servicename = "YubiLock";

    public Worker()
    {
        this.ServiceName = Servicename;
        this.CanStop = true;
        this.CanPauseAndContinue = false;
        this.AutoLog = false;
    }

    private BackgroundWorker _worker;

    /// <summary>
    /// On start methode.
    /// Includes the Start Script of our Custom YubiLock System. 
    /// </summary>
    /// <param name="args"></param>
    protected override void OnStart(string[] args)
    {
        EventLog.WriteEntry("Service entered onStart.", EventLogEntryType.Warning);
        base.OnStart(args);
        // If the worker is not null and is Busy Log a Warning message into the Event Log entries and return
        if (_worker != null && _worker.IsBusy)
        {
            EventLog.WriteEntry("Service: YubiLock Worker already running.", EventLogEntryType.Warning);
            return;
        }

        // Define _worker as new BackgroundWorker
        _worker = new BackgroundWorker();
        // Need to learn what this do
        _worker.DoWork += (sender, eventArgs) =>
        {
            EventLog.WriteEntry("YubiLock do work enetered", EventLogEntryType.Information);
            // Create a i variable that is 0;
            var i = 0;
            // While theres no Cancellation pending for the Worker execute the class yubiLock() every 3 Seconds and Write a new Entrie into the EventLogs
            while (!_worker.CancellationPending)
            {
                yubiLockCode.yubiLock();
                EventLog.WriteEntry("YubiLock Worker, working on" + i++, EventLogEntryType.Information);
                Thread.Sleep(3000);
            }
            // As soon as a Cancellation is pending write a EventLog 
            EventLog.WriteEntry("YubiLock exit", EventLogEntryType.Information);
        };
        // Say the worker that it Supports a Cancellation
        _worker.WorkerSupportsCancellation = true;
        // Disables the Worker Reports Progrss
        _worker.WorkerReportsProgress = false;
        // Run the Worker Async
        _worker.RunWorkerAsync();
        EventLog.WriteEntry("Service: YubiLock Worker started", EventLogEntryType.Information);
    }

    /// <summary>
    /// A methode as soon as the Worker stopps
    /// </summary>
    protected override void OnStop()
    {
        base.OnStop();
        // If the worker is not busy return 
        if (!_worker.IsBusy) return;
        EventLog.WriteEntry("Service: YubiLock Worker is stopping...", EventLogEntryType.Information);
        // Cancel and stop the asyncron Worker
        _worker.CancelAsync();
        // Wait 3 Seconds
        Thread.Sleep(3000);


        EventLog.WriteEntry("Service: YubiLock Worker stopped.", EventLogEntryType.Information);
        // Set _worker to null
        _worker = null;
    }


    /*private readonly ILogger<Worker> _logger;

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
    }*/
}   