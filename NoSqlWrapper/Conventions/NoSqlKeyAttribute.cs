using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlWrapper.Conventions
{
    [Serializable()]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class NoSqlKeyAttribute : Attribute
    {
        public static PropertyInfo[] FindKeyProperty<T>()
        {
            var type = typeof(T);

            var properties = type.GetProperties()
                .Where(a => a.CustomAttributes
                    .Any(b => b.AttributeType == typeof(NoSqlKeyAttribute))).ToArray();

            return properties;
        }
    }

    [Serializable()]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class TypeSignatureAttribute : Attribute
    {
        public Guid TypeVersionId
        {
            get;
            set;
        }
        public String TypeSignature
        {
            get;
            set;
        }

        public TypeSignatureAttribute(String typeVersionId, String typeSignature)
        {
            this.TypeVersionId = new Guid(typeVersionId);
            this.TypeSignature = typeSignature;
        }

        //public static PropertyInfo[] FindKeyProperty<T>()
        //{
        //    var type = typeof(T);

        //    var properties = type.GetProperties()
        //        .Where(a => a.CustomAttributes
        //            .Any(b => b.AttributeType == typeof(TypeSignatureAttribute))).ToArray();

        //    return properties;
        //}
    } 
}
