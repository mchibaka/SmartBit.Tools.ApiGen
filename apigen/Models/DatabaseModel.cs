using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apigen.Models
{
    public class DatabaseModel 
    {
        /// <summary>
        ///     The database name, or <see langword="null" /> if none is set.
        /// </summary>
        public virtual string? DatabaseName { get; set; }

        /// <summary>
        ///     The database schema, or <see langword="null" /> to use the default schema.
        /// </summary>
        public virtual string? DefaultSchema { get; set; }

        /// <summary>
        ///     The database collation, or <see langword="null" /> if none is set.
        /// </summary>
        public virtual string? Collation { get; set; }

        /// <summary>
        ///     The list of tables in the database.
        /// </summary>
        public virtual IList<DatabaseTable> Tables { get; } = new List<DatabaseTable>();

        /// <summary>
        ///     The list of sequences in the database.
        /// </summary>
        public virtual IList<DatabaseSequence> Sequences { get; } = new List<DatabaseSequence>();
    }

}
