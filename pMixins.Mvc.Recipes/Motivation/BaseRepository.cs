using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace pMixins.Mvc.Recipes.Motivation
{
    public interface IEntity{}
    public class ExampleEntity : IEntity{}

    public abstract class BaseRepository<T> where T : IEntity
    {
        protected abstract string ConnectionString { get; }

        protected void ExecuteQuery(Action<SqlConnection> query)
        {
            //Do Transaction Support / Error Handling / Logging

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                query(conn);
                conn.Close();
            }
        }       

        //CRUD Methods:
        public abstract T GetByID(int id);
        public abstract IEnumerable<T> GetAll(int id);
        public abstract void Add(T Entity);
        public abstract void Update(T Entity);
        public abstract void Delete(T Entity);
    }

    public abstract class SqlStoredProcedureBaseRepository<T> : BaseRepository<T>
    where T : IEntity
    {
        protected abstract string GetByIDStoredProcedureName {get;}
        protected abstract T MapDataReaderToEntity(SqlDataReader reader);

        protected abstract string AddStoredProcedureName { get; }
        protected abstract IEnumerable<SqlParameter> MapEntityToSqlParameters(T entity); 

        public override T GetByID(int id)
        {
            var entity = default(T);

            ExecuteQuery(conn =>
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = GetByIDStoredProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("Id", id));

                    entity = MapDataReaderToEntity(cmd.ExecuteReader());
                }
            });

            return entity;
        }

        public override void Add(T Entity)
        {
            ExecuteQuery(conn =>
            {
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = AddStoredProcedureName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (var p in MapEntityToSqlParameters(Entity))
                        cmd.Parameters.Add(p);

                    cmd.ExecuteNonQuery();
                }
            });
        }

        //Other CRUD methods omitted for brevity
    }

}
