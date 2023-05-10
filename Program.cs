using WorkerService;
using WorkerService.Service;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<MonitorLoop>();
        services.AddHostedService<QueuedHosted>();
        services.AddSingleton<IBackGroundTaskQueue>(_ =>
        {
            return new BackGroundTaskQueue(5);
        });
    })
    .Build();

MonitorLoop monitorLoop = host.Services.GetRequiredService<MonitorLoop>()!;
monitorLoop.StartMonitorLoop();

await host.RunAsync();
