using System.Net.Sockets;
using System.Net;
using System.Text;
using WorkerService1;

namespace WorkerService2
{
    public class Receiver : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly UdpClient _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 22220));

        public Receiver(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await _udpClient.ReceiveAsync(stoppingToken).ConfigureAwait(false);
                var message = Encoding.UTF8.GetString(result.Buffer);
                _logger.LogInformation(message);
            }
        }
    }
}
