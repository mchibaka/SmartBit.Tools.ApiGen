
namespace SmartBit.Tools.ApiGen.Models;

using DotLiquid;
using System.Collections.Generic;

internal class TableDto: ILiquidizable
{
    internal string Name;
    internal  ICollection<ColumnDto> Columns;
    public object ToLiquid()
    {
        return new { Name, Columns };
    }
}
