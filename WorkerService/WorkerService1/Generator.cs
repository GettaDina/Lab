using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Options;
namespace WorkerService1
{
    public class Generator : BackgroundService
    {
        private readonly LbsService _lbsService;
        private readonly IPEndPoint _ep;
        private readonly List<string> _list = new();
        public Generator(IOptions<AppSettings> config, LbsService lbs)
        {
            _lbsService = lbs;
            _ep = new IPEndPoint(IPAddress.Parse(config.Value.Host!), config.Value.Port);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    using StreamReader sr = File.OpenText("Point.txt");
                    while (await sr.ReadLineAsync() is { } line)
                    {
                        var j = 0;

                        if (
                            Helper.TryParseDouble(line, ref j, out var lng) &&
                            Helper.TryParseDouble(line, ref j, out var lat))
                        {
                            _lbsService.FindCeil(lat, lng, out var lbs);
                            _list.Add($"{lbs},{line},{15}");
                            _list.Add($"{lbs},{line},{0}");
                        }
                    }

                    foreach (var t in _list)
                    {
                        await s.SendToAsync(Encoding.ASCII.GetBytes(t), SocketFlags.None, _ep);
                    }

                    await Task.Delay(1000, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                }

                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        
    }
}


