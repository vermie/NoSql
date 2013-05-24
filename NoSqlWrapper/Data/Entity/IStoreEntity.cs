using System;
namespace NoSqlWrapper.Data.Entity
{
    public interface IStoreEntity
    {
        Guid StoreId { get; set; }
        Guid TypeVersionId { get; set; }
        string Value { get; set; }
    }
}
