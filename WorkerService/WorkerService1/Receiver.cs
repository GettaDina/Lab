using System.Net.Sockets;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;

namespace WorkerService1
{
    public class Receiver : BackgroundService
    {
        private readonly AppSettings _config;
        public Receiver(IOptions<AppSettings> config, ILogger<Receiver> logger)
        {
            _config = config.Value;
            _logger = logger;
        }
        private readonly ILogger<Receiver> _logger;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using UdpClient udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, _config.Port));
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = await udpClient.ReceiveAsync(stoppingToken).ConfigureAwait(false);
                    var message = Encoding.UTF8.GetString(result.Buffer);
                    //int j = 0;
                    //if (!Helper.TryParseInt(message, ref j, out var sat) ||
                    //    !Helper.TryParseDouble(message, ref j, out var lng) ||
                    //    !Helper.TryParseDouble(message, ref j, out var lat) ||
                    //    !Helper.TryParseInt(message, ref j, out var mcc) ||
                    //    !Helper.TryParseInt(message, ref j, out var mnc) ||
                    //    !Helper.TryParseInt(message, ref j, out var lac) ||
                    //    !Helper.TryParseInt(message, ref j, out var cid) ||
                    //    !Helper.TryParseDouble(message, ref j, out var lat2) ||
                    //    !Helper.TryParseDouble(message, ref j, out var lng2))
                    //{
                    //    continue;
                    //}
                    _logger.LogInformation(message);
                }
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



