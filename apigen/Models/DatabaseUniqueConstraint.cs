using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apigen.Models
{
    public class DatabaseUniqueConstraint : Annotatable
    {
        /// <summary>
        ///     The table on which the unique constraint is defined.
        /// </summary>
        public virtual DatabaseTable Table { get; set; } = null!;

        /// <summary>
        ///     The name of the constraint.
        /// </summary>
        public virtual string? Name { get; set; }

        /// <summary>
        ///     The ordered list of columns that make up the constraint.
        /// </summary>
        public virtual IList<DatabaseColumn> Columns { get; } = new List<DatabaseColumn>();

        /// <inheritdoc />
        public override string ToString()
            => Name ?? "<UNKNOWN>";
    }
}
