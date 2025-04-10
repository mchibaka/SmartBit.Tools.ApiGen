using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apigen.Models
{
    public class DatabaseForeignKey : Annotatable
    {
        /// <summary>
        ///     The table that contains the foreign key constraint.
        /// </summary>
        public virtual DatabaseTable Table { get; set; } = null!;

        /// <summary>
        ///     The table to which the columns are constrained.
        /// </summary>
        public virtual DatabaseTable PrincipalTable { get; set; } = null!;

        /// <summary>
        ///     The ordered list of columns that are constrained.
        /// </summary>
        public virtual IList<DatabaseColumn> Columns { get; } = new List<DatabaseColumn>();

        /// <summary>
        ///     The ordered list of columns in the <see cref="PrincipalTable" /> to which the <see cref="Columns" />
        ///     of the foreign key are constrained.
        /// </summary>
        public virtual IList<DatabaseColumn> PrincipalColumns { get; } = new List<DatabaseColumn>();

        /// <summary>
        ///     The foreign key constraint name.
        /// </summary>
        public virtual string? Name { get; set; }

        /// <summary>
        ///     The action performed by the database when a row constrained by this foreign key
        ///     is deleted, or <see langword="null" /> if there is no action defined.
        /// </summary>
        public virtual ReferentialAction? OnDelete { get; set; }

        /// <inheritdoc />
        public override string ToString()
            => Name ?? "<UNKNOWN>";
    }

}
