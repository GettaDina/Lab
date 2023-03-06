using WorkerService1;
using WorkerService2;

IHost host = Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{
    services.AddHostedService<Worker>();
    services.AddHostedService<Receiver>();
})
.Build();

await host.RunAsync();