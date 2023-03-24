using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Options;

namespace WorkerService1
{
    public class Generator : BackgroundService
    {
        private readonly Singleton _sing = Singleton.Instance;
        private readonly AppSettings _config;
        public Generator(IOptions<AppSettings> config)
        {
            _config = config.Value;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                if (IPAddress.TryParse(_config.Host, out var ip))
                {
                    var ep = new IPEndPoint(ip, _config.Port);
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        using var sr = File.OpenText("Point.txt");
                        while (await sr.ReadLineAsync() is { } line)
                        {
                            var j = 0;

                            if (Helper.TryParseDateTime(line, ref j, out var time) && 
                                Helper.TryParseInt(line, ref j, out var sat) &&
                                Helper.TryParseDouble(line, ref j, out var lng) &&
                                Helper.TryParseDouble(line, ref j, out var lat))
                            {
                                FindCeil(lat, lng, out var lbs);
                                if (sat < 3)
                                {
                                    if (TryGetCeil(lbs, out var point))
                                    {
                                        lat = point.Lat;
                                        lng = point.Lng;
                                    }
                                }

                                await s.SendToAsync(Encoding.ASCII.GetBytes(new PointD(time, sat, lat, lng, lbs.Mcc, lbs.Mnc, lbs.Lac, lbs.Cid).ToString()), SocketFlags.None, ep);
                            }
                        }
                        await Task.Delay(1000, stoppingToken);
                    }
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
        private void FindCeil(double lat, double lng, out Lbs lbs)
        {
            var numLbs = 0;
            var res = double.MaxValue;

            for (var i = 0; i < _sing.Dictionary.Count; i++)
            {
                var current = Math.Pow(_sing.Dictionary.ElementAt(i).Value.Lat - lat, 2)
                           + Math.Pow(_sing.Dictionary.ElementAt(i).Value.Lng - lng, 2);
                if (!(current > res)) continue;
                res = current;
                numLbs = i;
            }

            lbs = _sing.Dictionary.ElementAt(numLbs).Key;
        }

        private bool TryGetCeil(Lbs lbs, out Point point)
        {
            return _sing.Dictionary.TryGetValue(lbs, out point);
        }
    }
}


