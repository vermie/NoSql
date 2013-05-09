using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace NoSqlWrapper.Entity
{
    [Table("Store")]
    public class StoreEntity
    {
        public Guid Id { get; set; }

        [NotMapped]
        public object Value { get; set; }

        public string TypeName
        {
            get { return Value.GetType().FullName; }
            private set { }
        }

        [Required]
        public string Blob
        {
            get { return Value.ToString(); }
            set { Value = (object)value; }
        }
    }
}
