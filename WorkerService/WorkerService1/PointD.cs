public struct PointD
{
    public int Sat;
    public double Lat;
    public double Lng;
    public void Set(int sat, double lat, double lng)
    {
        Sat = sat;
        Lat = lat;
        Lng = lng;
    }
}
