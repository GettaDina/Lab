using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Options;

namespace WorkerService1
{
    public class Receiver : BackgroundService
    {
        private readonly AppSettings _config;
        private readonly LbsService _lbsService;
        public Receiver(IOptions<AppSettings> config, ILogger<Receiver> logger, LbsService lbs)
        {
            _config = config.Value;
            _lbsService = lbs;
            _logger = logger;
        }
        private readonly ILogger<Receiver> _logger;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using UdpClient udpClient = new UdpClient(_config.Port);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = await udpClient.ReceiveAsync(stoppingToken).ConfigureAwait(false);
                    var message = Encoding.UTF8.GetString(result.Buffer);
                    if (PointD.TryParse(message, out var pointD))
                    {
                        if (pointD.Sat < 3)
                        {
                            if (_lbsService.TryGetCeil(pointD.Lbs, out var point))
                            {
                                pointD.Lat = point.Lat;
                                pointD.Lng = point.Lng;
                            }
                        }
                        _logger.LogInformation(pointD.ToString());
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
}



