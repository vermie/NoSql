using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace NoSqlWrapper.Data.Entity
{
    [Table("Store")]
    public class StoreEntity : IStoreEntity
    {
        [Key()]
        [Column(Order=0)]
        public Guid StoreId { get; set; }
        [Key()]
        [Column(Order = 1)]
        public Guid TypeVersionId { get; set; }

        [Required]
        public string Value { get; set; }

        public TypeVersionEntity TypeVersion { get; set; }
    }
}
