using System;

[Serializable]
public class CommonData
{
    // Row index / record ID
    public int No;

    // UNIX timestamp in UTC (milliseconds since epoch)
    public long timestamp;

    // Windows FILETIME timestamp (100-nanosecond intervals since 1601-01-01 UTC)
    public long filetimeTS;

    // Participant identifier
    public string partID;

    public string taskID;
}
