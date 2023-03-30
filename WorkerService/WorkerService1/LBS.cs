namespace WorkerService1;
public readonly struct Lbs
{
    private readonly int _mcc;
    private readonly int _mnc;
    private readonly int _lac;
    private readonly int _cid;
    public Lbs(int mcc, int mnc, int lac, int cid)
    {
        _mcc = mcc;
        _mnc = mnc;
        _lac = lac;
        _cid = cid;
    }
    public override string ToString()
    {
        return $"{_mcc},{_mnc},{_lac},{_cid}";
    }
}


