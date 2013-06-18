using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NoSqlWrapper.Conventions;
using NoSqlWrapper.Serialization;
using System.Reflection;
using System.Linq.Expressions;

namespace NoSqlWrapper
{
    internal static class Utility
    {
        public static String FormatString(this String theString, Object arg0)
        {
            return string.Format(theString, arg0);
        }
        public static String FormatString(this String theString, Object arg0, Object arg1)
        {
            return string.Format(theString, arg0, arg1);
        }
        public static String FormatString(this String theString, Object arg0, Object arg1, Object arg2)
        {
            return string.Format(theString, arg0, arg1, arg2);
        }
        public static String FormatString(this String theString, params Object[] parameters)
        {
            return string.Format(theString, parameters);
        }


        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
        public static void TryDispose(this IDisposable disposable)
        {
            if (disposable != null)
                disposable.Dispose();
        }

        public static ISerializer<T> Get<T>(this ISerializerFactory serializer, Options options)
        {
            var serialy = serializer.Get<T>();
            serialy.Options = options;
            return serialy;
        }


        public static T TryGetCustomAttribute<T>(this Type type, Boolean inherit = false)
            where T : Attribute
        {
            return type.GetCustomAttribute<T>(inherit) as T;
        }

        public static Migration.MigrationKey MigrationKey(this Migration.IMigration migration)
        {
            return new Migration.MigrationKey(migration.SourceTypeVersionId, migration.TargetTypeVersionId);
        }

        //internal static PropertyInfo GetPropertyInfo<TObject, TProperty>(Expression<Func<TObject, TProperty>> propertyRefExpr)
        //{
        //    return GetPropertyInfo(propertyRefExpr);
        //}
        internal static PropertyInfo GetPropertyInfo(LambdaExpression expression)
        {
            var body = expression.Body;
            var expr = expression.Body as MemberExpression;

            // includes things like:
            //   casts
            //   implicit/explicit conversion operators
            //   VB's CType
            //   boxed value types
            // probably should not support these, instead strictly enforce member access
            while (expr == null && body.NodeType == ExpressionType.Convert)
            {
                var convert = (UnaryExpression)body;
                expr = convert.Operand as MemberExpression;
                body = expr;
            }

            if (expr == null || !(expr.Member is PropertyInfo))
                throw new ArgumentException("expression '{0}' must be a property-access expression".FormatString(expression), "expression");

            return (PropertyInfo)expr.Member;
        }
    }

}
