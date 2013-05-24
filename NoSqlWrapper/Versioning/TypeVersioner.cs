using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlWrapper.Versioning
{
    

    public class TypeVersioner : ITypeVersioner
    {
        public String GetTypeSignature<T>()
        {
            return this.VersionType(typeof(T));
        }
        private String VersionType(Type type)
        {
            StringBuilder sb = new StringBuilder();
            List<Type> typeList = new List<Type>();

            this.VersionType(type, sb, typeList);

            return sb.ToString();
        }
        private void VersionType(Type type, StringBuilder signatureBuilder, List<Type> typeList)
        {
            //already got this type included? pass if we do
            if (typeList.Contains(type))
            {
                return;
            }
            else
            {
                typeList.Add(type);
            }


            //list of type properties ordered by name
            var properties = type.GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance)
                .OrderBy(a => a.Name);

            //generate the signature 
            signatureBuilder.AppendLine(type.FullName);
            properties.ForEach(a => signatureBuilder.AppendLine(this.VersionTypeProperty(a)));
            //signatureBuilder.AppendLine();

            foreach (var item in properties)
            {
                VersionType(item.PropertyType, signatureBuilder, typeList);
            }
        }
        private String VersionTypeProperty(PropertyInfo propertyInfo)
        {
            Type propertyType = propertyInfo.PropertyType;

            return propertyInfo.Name 
                + ":" 
                + propertyType.FullName;
        }

        //private Dictionary<Type, String> TypeSignatureDictionary
        //{
        //    get;
        //    set;
        //}

        //public String GetVersion<T>()
        //{
        //    return this.GetVersion(typeof(T));
        //}
        //public String GetVersion(Type type)
        //{

        //}
        //private IEnumerable<Type> GetAllLinkedTypes(Type 
    }
    
}
