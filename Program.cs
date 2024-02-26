using YubiLock_Service;
//System.Diagnostics.Debugger.Launch();
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options => options.ServiceName = "YubiLock Service");
builder.Services.AddHostedService<Worker>();
IHost host = builder.Build();
await host.RunAsync();



/*IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "YubiLock Service";

    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();*/