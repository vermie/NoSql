using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlWrapper.Data.Entity
{

    [Table("TypeVersion")]
    public class TypeVersionEntity : NoSqlWrapper.Data.Entity.ITypeVersionEntity
    {
        [Key()]
        public Guid TypeVersionId
        {
            get;
            set;
        }  
        [MaxLength(1000)]
        public String AssemblyName
        {
            get;
            set;
        }
        [MaxLength(1000)]
        public String TypeName 
        {
            get;
            set;
        }
        public String TypeSignature
        {
            get;
            set;
        }

        public DateTime DateCreated
        {
            get;
            set;
        }
        

        public virtual ICollection<StoreEntity> StoreItems
        {
            get;
            set;
        }
    }
}
