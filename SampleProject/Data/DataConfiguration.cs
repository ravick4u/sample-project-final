using BusinessEntities;
using Common;
using Newtonsoft.Json;
using Raven.Client.Documents;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Session;
using Raven.Client.Json.Serialization.NewtonsoftJson;
using SimpleInjector;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Data
{
    public class DataConfiguration
    {
        public static void Initialize(Container container, Lifestyle lifestyle, bool createIndexes = true)
        {
            var assembly = typeof(DataConfiguration).Assembly;

            container.RegisterSingleton<IListTypeLookup<Assembly>, ListTypeLookup<Assembly>>();

            InitializeAssemblyInstancesService.RegisterAssemblyWithSerializableTypes(container, typeof(User).Assembly);
            InitializeAssemblyInstancesService.RegisterAssemblyWithSerializableTypes(container, assembly);

            InitializeAssemblyInstancesService.Initialize(container, lifestyle, assembly);
            container.RegisterSingleton(() => InitializeDocumentStore(assembly, createIndexes));

            container.Register(() =>
            {
                var session = container.GetInstance<IDocumentStore>().OpenSession();
                session.Advanced.MaxNumberOfRequestsPerSession = 5000;
                return session;
            }, lifestyle);
        }

        private static IDocumentStore InitializeDocumentStore(Assembly assembly, bool createIndexes)
        {
            var documentStore = new DocumentStore
            {
                Urls = new[] { "http://localhost:8080" },
                Database = "SampleProject",
                Conventions = new DocumentConventions
                {
                    UseOptimisticConcurrency = true,

                    SaveEnumsAsIntegers = true,
                    Serialization = new NewtonsoftJsonSerializationConventions
                    {
                        CustomizeJsonSerializer = jsonSerializer =>
                        {
                            jsonSerializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                            jsonSerializer.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                            jsonSerializer.DefaultValueHandling = DefaultValueHandling.Ignore;
                            jsonSerializer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                            jsonSerializer.NullValueHandling = NullValueHandling.Include;
                        }
                    }
                }
            };

            documentStore.Initialize();

            if (createIndexes)
            {
                IndexCreation.CreateIndexes(assembly, documentStore);
            }

            return documentStore;
        }
    }
}
