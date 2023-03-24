namespace WorkerService1;
public struct PointD
{
    public DateTime Time;
    public int Sat;
    public double Lat;
    public double Lng;
    public int Mcc;
    public int Mnc;
    public int Lac;
    public int Cid;

    public PointD(DateTime time, int sat, double lat, double lng, int mcc, int mnc, int lac, int cid)
    {
        Time = time;
        Sat = sat;
        Lat = lat;
        Lng = lng;
        Mcc = mcc;
        Mnc = mnc;
        Lac = lac;
        Cid = cid;
    }

    public override string ToString()
    {
        return Time + " " + Sat + " " + Lat + " " + Lng + " " + Mcc + " " + Mnc + " " + Lac + " " + Cid;
    }
}