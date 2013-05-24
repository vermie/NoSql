using System;


namespace NoSqlWrapper.Versioning
{
    public interface ITypeVersioner
    {
        string GetTypeSignature<T>();
    }
}
