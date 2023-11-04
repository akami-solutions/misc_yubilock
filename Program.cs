using YubiLock_Service;
//System.Diagnostics.Debugger.Launch();
/*
IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "YubiLock Service";

    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>0();
    })
    .Build();

await host.RunAsync();
*/

internal class Program
{
    private static void Main()
    {
        Console.WriteLine(@"YubiLock Worker is not able to work in a normal Command Line.\nPlease start the Service normally or re-execute the Installer.");
        System.ServiceProcess.ServiceBase.Run(new Worker());
    }
}