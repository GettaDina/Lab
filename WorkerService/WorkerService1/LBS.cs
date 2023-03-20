public struct LBS
{
    public int Mcc;
    public int Mnc;
    public int Lac;
    public int Cid;
    public double Lat;
    public double Lng;
    public void Set(int mcc, int mnc, int lac, int cid, double lat, double lng)
    {
        Mcc = mcc;
        Mnc = mnc;
        Lac = lac;
        Cid = cid;
        Lat = lat;
        Lng = lng;
    }

}