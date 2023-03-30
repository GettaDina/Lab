using WorkerService1;
IHost host = Host.CreateDefaultBuilder(args)
.ConfigureServices(services =>
{
    var appSettings = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
    services.Configure<AppSettings>(appSettings.GetSection("Info"));
    services.AddSingleton<LbsService>();
    services.AddHostedService<Generator>();
    services.AddHostedService<Receiver>();
})
.Build();

await host.RunAsync();

public class AppSettings
{
    public string? Host { get; set; }
    public int Port { get; set; }
}