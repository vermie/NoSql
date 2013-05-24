using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlWrapper.Conventions
{

    //enable versioning: default = false
    //allow lossy deserialization: default = true
    //TODO:
    //enable custom migrations???

    //enable implicit none-lossy migrations (deserialize and if all fields get deserialed be cool with it) 
    //allow 'best effort' deserializations
    //
    public class Options
    {
        public Boolean ArchiveVersionChanges
        {
            get;
            set;
        }
        public Boolean VersioningEnabled
        {
            get;
            set;
        }
        public Boolean StrictDeserializationEnabled
        {
            get;
            set;
        }

        public Dependencies.IDateTimeProvider DateTimeProvider
        {
            get;
            set;
        }

        public Options()
        {
            this.ArchiveVersionChanges = false;
            this.VersioningEnabled = false;
            this.StrictDeserializationEnabled = false;
            this.DateTimeProvider = new Dependencies.DefaultDateTimeProvider();
        }
    }
}
