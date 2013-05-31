using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoSqlWrapper.Migration
{

    public interface IMigration
    {
        Guid SourceTypeVersionId
        {
            get;
        }
        Guid TargetTypeVersionId
        {
            get;
        }
        String Apply(String source);
    }

    public class Migration : IMigration
    {
        private IMigration Decoratee
        {
            get;
            set;
        }

        public Migration(IMigration iMigration)
        {
            this.Decoratee = iMigration;
        }



        public Guid SourceTypeVersionId
        {
            get { throw new NotImplementedException(); }
        }

        public Guid TargetTypeVersionId
        {
            get { throw new NotImplementedException(); }
        }

        public string Apply(string source)
        {
            return this.Decoratee.Apply(source);
        }
    }

    public class TestMigration:IMigration
    {

        public Guid SourceTypeVersionId
        {
            get;
            set;
        }

        public Guid TargetTypeVersionId
        {
            get;
            set;
        }

        public string Apply(string source)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return String.Format("[{0]} -> [{1}]", this.SourceTypeVersionId, this.TargetTypeVersionId);
        }
    }

    [Serializable()]
    public struct MigrationKey
    {
        public Guid SourceTypeVersionId;
        public Guid TargetTypeVersionId;


        public MigrationKey(Guid sourceTypeVersionId, Guid targetTypeVersionId)
        {
            this.SourceTypeVersionId = sourceTypeVersionId;
            this.TargetTypeVersionId = targetTypeVersionId;
        }

        public override string ToString()
        {
            return String.Format("[{0} -> [{1}]", this.SourceTypeVersionId, this.TargetTypeVersionId);
        }
    }

    public class Migrations
    {
        private static readonly Migrations _instance = new Migrations();
        public static Migrations Instance
        {
            get
            {
                return _instance;
            }
        }

        private Object _synchronizingObject = new Object();
        private Dictionary<MigrationKey, IMigration> MigrationDictionary
        {
            get;
            set;
        }
        private Migrations()
        {
            this.MigrationDictionary = new Dictionary<MigrationKey, IMigration>();
        }

        public void RegisterMigration(IMigration migration)
        {
            lock (this._synchronizingObject)
            {
                if (this.MigrationDictionary.ContainsKey(migration.MigrationKey()))
                {
                    throw new Exceptions.NoSqlWrapperException("Migration already registered! cannot register migration with same key twice!");
                }

                this.MigrationDictionary.Add(migration.MigrationKey(), migration);
            }
        }
        public Boolean IsMigrationRegistered(IMigration migration)
        {
            lock (this._synchronizingObject)
            {
                return this.MigrationDictionary.ContainsKey(migration.MigrationKey());
            }
        }
        public IMigration TryRetrieveMigration(MigrationKey migrationKey)
        {
            lock (this._synchronizingObject)
            {
                IMigration migrationValue;

                if (this.MigrationDictionary.TryGetValue(migrationKey, out migrationValue))
                {
                    return migrationValue;
                }
            }

            return null;
        }
        private List<IMigration> GetMigrationListCopy()
        {
            var migrationKeyList = new List<IMigration>();

            lock (this._synchronizingObject)
            {
                //safely copy into a new list
                migrationKeyList.AddRange(this.MigrationDictionary.Values);
            }

            return migrationKeyList;
        }
        
        public IMigration[] FindMigrationPath(Guid typeVersionSourceId, Guid typeVersionTargetId)
        {
            //find a path from source to target? avoid recursions???
            //a-b,   b-c,   c-d straight upgrade makes gets a-d
            //a1-b1, c1,    a1-d1 hopping upgrade
            //a2-b2, b2-c2, c2-a2, c2-d2 could this create a loop?
            var migrationListCopy = this.GetMigrationListCopy();
            List<MigrationKey> migrationKeyList = migrationListCopy.Select(a=>a.MigrationKey()).ToList();
            List<MigrationKey> traversedKeyList = new List<MigrationKey>();
            Stack<MigrationKey> migrationPath = new Stack<MigrationKey>();
            Boolean matchFound = false;

            this.FindMigrationPath(migrationKeyList, traversedKeyList, migrationPath,
                typeVersionSourceId, typeVersionTargetId, ref matchFound);

            var path = migrationPath.ToArray();
            List<IMigration> migrationsToRun = new List<IMigration>();

            migrationPath.Reverse().ForEach(a => 
                migrationsToRun.Add(migrationListCopy.Where(b => b.MigrationKey().Equals(a)).Single()));

            return migrationsToRun.ToArray();
        }

        //a to b, b to c, c to d
        //a to d
        //find all links from source to next...
        private void FindMigrationPath(
            List<MigrationKey> migrationKeyList,
            List<MigrationKey> traversedKeyList,
            Stack<MigrationKey> migrationPath, Guid sourceId, Guid targetId, ref Boolean matchFound)
        {
            if (matchFound)
            {
                return;
            }

            //find all the source links..
            var nextLeads = migrationKeyList.Where(a => a.SourceTypeVersionId == sourceId).ToList();
            foreach (var lead in nextLeads)
            {
                if (traversedKeyList.Contains(lead) == false)
                {
                    traversedKeyList.Add(lead);

                    migrationPath.Push(lead);

                    this.FindMigrationPath(migrationKeyList, traversedKeyList, migrationPath, 
                        lead.TargetTypeVersionId, targetId, ref matchFound);

                    if (migrationPath.Peek().TargetTypeVersionId == targetId)
                    {
                        matchFound = true;
                        return;
                    }
                    else
                    {
                        migrationPath.Pop();
                    }
                }
            }

        }


    }

}
