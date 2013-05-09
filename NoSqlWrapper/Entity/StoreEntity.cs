using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using NoSqlWrapper.Interfaces;

namespace NoSqlWrapper.Entity
{
    [Table("Store")]
    public class StoreEntity
    {
        public Guid Id { get; set; }

        [Required]
        public string TypeName { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
