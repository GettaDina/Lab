using System.Net.Sockets;
using System.Net;
using System.Text;
using WorkerService1;
public class Receiver : BackgroundService
{
    private readonly ILogger<Generator> _logger;
    private readonly UdpClient _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 22222));
    private Dictionary<LBS, PointD> _dictionary = new Dictionary<LBS, PointD>();
    public Receiver(ILogger<Generator> logger)
    {
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                var result = await _udpClient.ReceiveAsync(stoppingToken).ConfigureAwait(false);
                var message = Encoding.UTF8.GetString(result.Buffer);
                int j = 0;
                if (!Singleton.TryParseInt(message, ref j, out var sat) ||
                    !Singleton.TryParseDouble(message, ref j, out var lng) ||
                    !Singleton.TryParseDouble(message, ref j, out var lat) ||
                    !Singleton.TryParseInt(message, ref j, out var mcc) ||
                    !Singleton.TryParseInt(message, ref j, out var mnc) ||
                    !Singleton.TryParseInt(message, ref j, out var lac) ||
                    !Singleton.TryParseInt(message, ref j, out var cid) ||
                    !Singleton.TryParseDouble(message, ref j, out var lat2) ||
                    !Singleton.TryParseDouble(message, ref j, out var lng2))
                {
                    continue;
                }

                LBS lbs = new LBS();
                lbs.Set(mcc, mnc, lac, cid, lat2, lng2);
                PointD pointD = new PointD();
                pointD.Set(sat, lat, lng);
                //_dictionary.Add(lbs, pointD);
                _logger.LogInformation(message);
            }
            _udpClient.Dispose();
        }
        catch (Exception e)
        {
            if (stoppingToken.IsCancellationRequested)
                throw new OperationCanceledException();
            Console.WriteLine(e);
            throw;
        }
        
    }
}


