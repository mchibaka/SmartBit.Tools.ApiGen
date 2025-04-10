using DotLiquid;
using System.Net.Http.Headers;

public class ColumnDto : ILiquidizable
{
    public string Name { get; internal set; }
    public string Type { get; internal set; }

    public object ToLiquid()
    {
        return new { Name, Type } ;
    }
}