using System.Globalization;
namespace WorkerService1
{
    public class Singleton
    {
        private static Singleton? _instance;

        public List<LBS> List = new List<LBS>();
        public Singleton()
        {
            var sr = File.OpenText("LBS.txt");
            string? line;
            LBS lbs = new LBS();

            while ((line = sr.ReadLine()) != null)
            {
                int j = 0;
                if (!TryParseInt(line, ref j, out var mcc) ||
                    !TryParseInt(line, ref j, out var mnc) ||
                    !TryParseInt(line, ref j, out var lac) ||
                    !TryParseInt(line, ref j, out var cid) ||
                    !TryParseDouble(line, ref j, out var lat) ||
                    !TryParseDouble(line, ref j, out var lng))
                {
                    continue;
                }

                lbs.Set(mcc, mnc, lac, cid, lat, lng);
                List.Add(lbs);
            }
        }

        public static Singleton Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Singleton();
                }

                return _instance;
            }
        }
        public static bool TryParseInt(string line, ref int curr, out int value)
        {
            int next = line.IndexOf(',', curr);
            if (!int.TryParse(line.AsSpan()[curr..next], out value))
                return false;
            curr = next + 1;
            return true;
        }

        public static bool TryParseDouble(string line, ref int curr, out double value)
        {
            int next = line.IndexOf(',', curr);
            if (next == -1)
                next = line.Length;
            if (!double.TryParse(line.AsSpan()[curr..next], Style, Format, out value))
                return false;
            curr = next + 1;
            return true;
        }

        private static readonly NumberFormatInfo Format = NumberFormatInfo.InvariantInfo;
        private static readonly NumberStyles Style = NumberStyles.Number;
    }
}


