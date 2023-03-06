using System.Net.Sockets;
using System.Net;
using System.Text;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            var ep = new IPEndPoint(IPAddress.Loopback, 22220);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
                await s.SendToAsync(Encoding.ASCII.GetBytes($"Worker running at: {DateTimeOffset.Now}"), SocketFlags.None, ep);
            }
        }
    }
}