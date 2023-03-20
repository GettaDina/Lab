using System.Net.Sockets;
using System.Net;
using System.Text;

public class Generator : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            var ep = new IPEndPoint(IPAddress.Loopback, 22220);
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
                var sr = File.OpenText("PointD.txt");
                string? line;
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    await s.SendToAsync(Encoding.ASCII.GetBytes($"{line}"), SocketFlags.None, ep);
                }
            }
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

