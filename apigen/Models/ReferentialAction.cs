using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apigen.Models
{
    public enum ReferentialAction
    {
        /// <summary>
        ///     Do nothing. That is, just ignore the constraint.
        /// </summary>
        NoAction,

        /// <summary>
        ///     Don't perform the action if it would result in a constraint violation and instead generate an error.
        /// </summary>
        Restrict,

        /// <summary>
        ///     Cascade the action to the constrained rows.
        /// </summary>
        Cascade,

        /// <summary>
        ///     Set null on the constrained rows so that the constraint is not violated after the action completes.
        /// </summary>
        SetNull,

        /// <summary>
        ///     Set a default value on the constrained rows so that the constraint is not violated after the action completes.
        /// </summary>
        SetDefault
    }

}
