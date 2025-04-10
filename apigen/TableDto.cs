

// Change this to the path of the assembly you want to inspect
using DotLiquid;
using System.Collections.Generic;


public class TableDto: ILiquidizable
{
    public string Name;
    public List<ColumnDto> Columns = new List<ColumnDto>();

    public object ToLiquid()
    {
        return new { Name, Columns };
    }
}
