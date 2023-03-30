namespace WorkerService1;
public struct PointD
{
    public Lbs Lbs;
    public double Lat;
    public double Lng;
    public DateTime Time;
    public int Sat;
    public override string ToString()
    {
        return $"{Lbs},{Lat},{Lng},{Time.ToString(@"hh\:mm\:ss")},{Sat}";
    }

    public static bool TryParse(string str, out PointD point)
    {
        var j = 0;
        if (Helper.TryParseInt(str, ref j, out var mcc) &&
            Helper.TryParseInt(str, ref j, out var mnc) &&
            Helper.TryParseInt(str, ref j, out var lac) &&
            Helper.TryParseInt(str, ref j, out var cid) &&
            Helper.TryParseDouble(str, ref j, out var lng) &&
            Helper.TryParseDouble(str, ref j, out var lat) &&
            Helper.TryParseDateTime(str, ref j, out var time) && 
            Helper.TryParseInt(str, ref j, out var sat)) 
        {
            point.Lbs = new Lbs(mcc, mnc, lac, cid);
            point.Lat = lat;
            point.Lng = lng;
            point.Time = time.ToUniversalTime();
            point.Sat = sat;
            return true;
        }
        point = default; 
        return false;
    }
}