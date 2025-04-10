using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apigen.Models
{
    public class DatabaseTrigger : Annotatable
    {
        /// <summary>
        ///     The trigger name.
        /// </summary>
        public virtual string Name { get; set; } = null!;
    }

}
