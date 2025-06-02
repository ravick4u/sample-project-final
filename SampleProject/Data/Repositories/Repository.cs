using System;
using System.Collections.Generic;
using System.Linq;
using BusinessEntities;
using Common;
//using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
//using Raven.Client.Indexes;

namespace Data.Repositories
{
    [AutoRegister]
    public class Repository<T> : IRepository<T> where T : IdObject
    {
        private readonly IDocumentSession _documentSession;

        public Repository(IDocumentSession documentSession)
        {
            _documentSession = documentSession;
        }

        public void Save(T entity)
        {
            _documentSession.Store(entity);
        }

        public void Delete(T entity)
        {
            _documentSession.Delete(entity);
        }

        public T Get(Guid id)
        {
            return _documentSession.Load<T>(id.ToString());
        }

        protected void DeleteAll<TIndex>() where TIndex : AbstractIndexCreationTask<T>
        {
            var indexName = typeof(TIndex).Name;
            var operation = new DeleteByQueryOperation(new IndexQuery { Query = $"FROM INDEX '{indexName}'" });
            _documentSession.Advanced.DocumentStore.Operations.Send(operation);
        }
    }
}