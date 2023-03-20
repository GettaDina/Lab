
IHost host = Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{
    services.AddHostedService<Generator>();
    services.AddHostedService<Receiver>();
    services.AddHostedService<Service>();
    //services.AddSingleton<ICounter, Singleton>();
})
.Build();

await host.RunAsync();