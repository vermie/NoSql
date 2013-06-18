using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace NoSqlWrapper.Repositories
{
    public interface INoSqlRepository
    {
        Guid Create<T>(T instance);

        //void Update<T>(T instance);
        void Update<T>(Guid id, T instance);

        //void Delete<T>(T instance);
        void Delete<T>(Guid id);

        T TryRetrieve<T>(Guid id);

        //T Retrieve<T>(Expression<Func<T, bool>> expression);
    }

    public class SomethingYo
    {
        public Int32 County
        {
            get;
            set;
        }

        public ChildSomething Childey
        {
            get;
            set;
        }
    }
    public class ChildSomething
    {
        public String Name
        {
            get;
            set;
        }

        public IEnumerable<ChildSomethingNote> Notes
        {
            get;
            set;
        }
    }
    public class ChildSomethingNote
    {
        public Decimal Amount
        {
            get;
            set;
        }
    }

    public class YoIt
    {
        public void Yo()
        {
            NoSqlIndex index = new NoSqlIndex();
            SomethingYo somethingYo = new SomethingYo();

            //index.Path<SomethingYo>(a => a.County);
            index
                .Path<SomethingYo>(a => a.Childey)
                .Path<ChildSomething>(a => a.Notes)
                .Path<ChildSomethingNote>(a => a.Amount);

            var result = index.GetPath();

            IndexBuilder<SomethingYo> starter = new IndexBuilder<SomethingYo>();

            var finalIndex = starter
                .Path(a => a.Childey)
                .Path(a => a.Notes)
                .Path(a => a.Amount);

            //starter.FromWtf(a=>a.
        }
    }

    public class IndexBuilder<TSource> : IIndexBuilder<TSource>
    {
        private List<PropertyInfo> _fullPath;
        public List<PropertyInfo> FullPath
        {
            get
            {
                return _fullPath.ToList();
            }
            private set
            {
                this._fullPath = value;
            }
        }

        public IndexBuilder()
            : this(new List<PropertyInfo>())
        {

        }
        public IndexBuilder(List<PropertyInfo> currentPath)
        {
            this._fullPath = currentPath;
        }

        public IIndexBuilder<T> Path<T>(Expression<Func<TSource, IEnumerable<T>>> funky)
        {
            var propertyInfo = Utility.GetPropertyInfo(funky);
            this._fullPath.Add(propertyInfo);

            return new IndexBuilder<T>(this.FullPath);
        }
        public IIndexBuilder<T> Path<T>(Expression<Func<TSource, T>> funky)
        {
            var propertyInfo = Utility.GetPropertyInfo(funky);
            this._fullPath.Add(propertyInfo);

            return new IndexBuilder<T>(this.FullPath);
        }        
    }
    public interface IIndexBuilder<T>
    {
        IIndexBuilder<B> Path<B>(Expression<Func<T, IEnumerable<B>>> funky);
        IIndexBuilder<B> Path<B>(Expression<Func<T, B>> funky);
    }


    public class NoSqlIndex
    {
        private List<Tuple<Type, LambdaExpression>> PathList
        {
            get;
            set;
        }

        public NoSqlIndex()
        {
            this.PathList = new List<Tuple<Type, LambdaExpression>>();
        }

        public NoSqlIndex Path<T>(Expression<Func<T,Object>> expression)
        {
            this.PathList.Add(new Tuple<Type, LambdaExpression>(typeof(T), expression));

            return this;
        }

        public String GetPath()
        {
            if (this.PathList.Count == 0)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            String pathDelimiter = ".";

            sb.Append(this.PathList.First().Item1.Name);

            foreach (var item in this.PathList)
            {
                sb.Append(pathDelimiter);
                sb.Append(Utility.GetPropertyInfo(item.Item2).Name);
            }

            return sb.ToString();
        }
    }
    
}
