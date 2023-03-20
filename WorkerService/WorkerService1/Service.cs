using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Globalization;
using WorkerService1;
public class Service : BackgroundService
{
    private readonly Singleton _sing = Singleton.Instance;
    private readonly UdpClient _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 22220));
    private readonly Socket _s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private readonly IPEndPoint _ep = new IPEndPoint(IPAddress.Loopback, 22222);
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await _udpClient.ReceiveAsync(stoppingToken).ConfigureAwait(false);
                var message = Encoding.UTF8.GetString(result.Buffer);

                var j = 0;
                if (!Singleton.TryParseInt(message, ref j, out var sat) ||
                    !Singleton.TryParseDouble(message, ref j, out var lng) ||
                    !Singleton.TryParseDouble(message, ref j, out var lat))
                {
                    continue;
                }

                PointD pointD = new PointD();
                pointD.Set(sat, lat, lng);

                double acceptDist = Math.Pow(Math.Abs(_sing.List[0].Lat - lat), 2) +
                                    Math.Pow(Math.Abs(_sing.List[0].Lng - lng), 2), currDist;
                int res = 0;
                int count = 1;
                while (count != _sing.List.Count)
                {
                    currDist = Math.Pow(Math.Abs(_sing.List[count].Lat - lat), 2) +
                            Math.Pow(Math.Abs(_sing.List[count].Lng - lng), 2);
                    if (currDist < acceptDist)
                    {
                        res = count;
                        acceptDist = currDist;
                    }
                    count++;
                }

                await _s.SendToAsync(Encoding.ASCII.GetBytes($"{message}," +
                                                             $"{_sing.List[res].Mcc}," +
                                                             $"{_sing.List[res].Mnc}," +
                                                             $"{_sing.List[res].Lac}," +
                                                             $"{_sing.List[res].Cid}," +
                                                             $"{_sing.List[res].Lat.ToString(CultureInfo.InvariantCulture)}," +
                                                             $"{_sing.List[res].Lng.ToString(CultureInfo.InvariantCulture)}"), SocketFlags.None, _ep);
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
