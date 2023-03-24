namespace WorkerService1
{
    public class Singleton
    {
        public static Singleton Instance => _instance.Value;
        public readonly Dictionary<Lbs, Point> Dictionary = new();

        private static readonly Lazy<Singleton> _instance =
            new(() => new Singleton());
        public Singleton()
        {
            using var sr = File.OpenText("LBS.txt");
            while (sr.ReadLine() is { } line)
            {
                int j = 0;
                if (!Helper.TryParseInt(line, ref j, out var mcc) ||
                    !Helper.TryParseInt(line, ref j, out var mnc) ||
                    !Helper.TryParseInt(line, ref j, out var lac) ||
                    !Helper.TryParseInt(line, ref j, out var cid) ||
                    !Helper.TryParseDouble(line, ref j, out var lat) ||
                    !Helper.TryParseDouble(line, ref j, out var lng))
                {
                    continue;
                } 
                Dictionary.Add(new Lbs(mcc, mnc, lac, cid), new Point(lat, lng));
            }
        }
    }
}