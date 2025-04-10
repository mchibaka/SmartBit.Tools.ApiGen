using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apigen.Models
{
    public class DatabasePrimaryKey : Annotatable
    {
        /// <summary>
        ///     The table on which the primary key is defined.
        /// </summary>
        public virtual DatabaseTable? Table { get; set; }

        /// <summary>
        ///     The name of the primary key.
        /// </summary>
        public virtual string? Name { get; set; }

        /// <summary>
        ///     The ordered list of columns that make up the primary key.
        /// </summary>
        public virtual IList<DatabaseColumn> Columns { get; } = new List<DatabaseColumn>();

        /// <inheritdoc />
        public override string ToString()
            => Name ?? "<UNKNOWN>";
    }

}
