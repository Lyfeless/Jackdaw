namespace Jackdaw;

public interface IDisplayObjectLines {
    public float LineWeight { get; set; }
}

public interface IDisplayObjectDashedLines : IDisplayObjectLines {
    public float DashLength { get; set; }
    public float OffsetPercent { get; set; }
}