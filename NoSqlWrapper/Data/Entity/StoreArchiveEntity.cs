using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlWrapper.Data.Entity
{
    [Table("StoreArchive")]
    public class StoreArchiveEntity : IStoreArchiveEntity
    {
        [Key()]
        [Column(Order = 0)]
        public Guid StoreArchiveId { get; set; }

        public Guid StoreId { get; set; }
        public Guid TypeVersionId { get; set; }

        [Required]
        public string Value { get; set; }

        public DateTime DateCreated
        {
            get;
            set;
        }
        public DateTime LastUpdated
        {
            get;
            set;
        }
        public DateTime DateArchived
        {
            get;
            set;
        }

        public TypeVersionEntity TypeVersion { get; set; }
    }

    public interface IStoreArchiveEntity : IStoreEntity
    {
        Guid StoreArchiveId { get; set; }
        DateTime DateArchived { get; set; }
    }
}
