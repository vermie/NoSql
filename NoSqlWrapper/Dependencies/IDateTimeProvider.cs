using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlWrapper.Dependencies
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
    public class DefaultDateTimeProvider:IDateTimeProvider 
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}
