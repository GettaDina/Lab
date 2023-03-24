using System.Globalization;

namespace WorkerService1
{
    public abstract class Helper
    {
        public static bool TryParseDateTime(string line, ref int current, out DateTime value)
        {
            var next = line.IndexOf(',', current);
            if (!DateTime.TryParse(line.AsSpan()[current..next], out value))
                return false;
            current = next + 1;
            return true;
        }
        public static bool TryParseInt(string line, ref int current, out int value)
        {
            var next = line.IndexOf(',', current);
            if (!int.TryParse(line.AsSpan()[current..next], out value))
                return false;
            current = next + 1;
            return true;
        }
        public static bool TryParseDouble(string line, ref int current, out double value)
        {
            int next = line.IndexOf(',', current);
            if (next == -1)
                next = line.Length;
            if (!double.TryParse(line.AsSpan()[current..next], Style, Format, out value))
                return false;
            current = next + 1;
            return true;
        }

        private static readonly NumberFormatInfo Format = NumberFormatInfo.InvariantInfo;
        private static readonly NumberStyles Style = NumberStyles.Number;
    }
}
