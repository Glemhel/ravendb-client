using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace TestingRavenDB
{
    // 'DocumentStore' is a main-entry point for client API.
    // It is responsible for managing and establishing connections
    // between your application and RavenDB server/cluster
    // and is capable of working with multiple databases at once.
    // Due to it's nature, it is recommended to have only one
    // singleton instance per application
    public class DocumentStoreHolder
    {
        // Use Lazy<IDocumentStore> to initialize the document store lazily. 
        // This ensures that it is created only once - when first accessing the public `Store` property.
        private static Lazy<IDocumentStore> store = new Lazy<IDocumentStore>(CreateStore);

        public static IDocumentStore Store => store.Value;

        private static IDocumentStore CreateStore()
        {
            IDocumentStore store = new DocumentStore()
            {
                // Define the cluster node URLs (required)
                Urls = new[] { "https://a.mrudakov.ravendb.community:8080", 
                           /*some additional nodes of this cluster*/ },

                // Set conventions as necessary (optional)
                Conventions =
            {
                MaxNumberOfRequestsPerSession = 1000,
                UseOptimisticConcurrency = true
            },

                // Define a default database (optional)
                Database = "Test",

                // Define a client certificate (optional)
                Certificate = new X509Certificate2(
                    "D:\\IT\\mrudakov.Cluster.Settings\\admin.client.certificate.mrudakov.pfx"
                    ),

                // Initialize the Document Store
            }.Initialize();

            return store;
        }
    }
}
