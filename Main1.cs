using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Raven.Client;
using System.Diagnostics;
using Raven.Client.Documents.BulkInsert;
using Raven.Client.Documents.Conventions;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Operations;
using Raven.Client.Documents.Queries;

namespace TestingRavenDB
{
    class Main1
    {
        public static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();
            Console.WriteLine("Hello!");
            X509Certificate2 clientCertificate = new X509Certificate2("D:\\IT\\mrudakov.Cluster.Settings\\admin.client.certificate.mrudakov.pfx");
            DeleteExample();
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            Console.WriteLine("Elapsed Time is {0:00}:{1:00}:{2:00}.{3}",
                            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            Console.ReadLine();
        }

        public static void CountNumberOfPaymentsSmaller()
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                // Create Table with results of Query 1.

                // Query Payment and Rental tables, sorted
                List<Payment> results = session
                    .Query<Payment>(collectionName: "Payment")
                    .Include(x => x.Rental)
                    .OrderByDescending(x => x.Amount)
                    .ToList();
                // how many payments are smaller than us
                int smaller = 0;
                // get list in ascending order
                results.Reverse();
                // iterate over all payments
                for (int i = 0; i < results.Count; i++)
                {
                    var payment = results[i];
                    // load rental data - without actually querying server! ravendb c# features - include.
                    Rental rental = session.Load<Rental>(payment.Rental);
                    // update number of payments that are smaller
                    if (i > 0 && results[i].Amount > results[i - 1].Amount)
                        smaller = i;
                    // writing data into a new collection
                    PaymentRental entry = new PaymentRental
                    {
                        PaymentId = payment.Id,
                        Amount = payment.Amount,
                        PaymentDate = payment.PaymentDate,
                        Staff = payment.Staff,
                        Customer = payment.Customer,
                        RentalId = rental.Id,
                        RentalDate = rental.RentalDate,
                        ReturnDate = rental.ReturnDate,
                        LastUpdate = rental.LastUpdate,
                        RentalStaff = rental.Staff,
                        RentalCustomer = rental.Customer,
                        Inventory = rental.Inventory,
                        Count_smaller_pay = smaller
                    };

                    session.Store(entry);
                    // set correct collection identifier 
                    session.Advanced.GetMetadataFor(entry)[Constants.Documents.Metadata.Collection] = "PaymentRental";
                }
                // save changes in the database
                session.SaveChanges();
            }
        }

        public static void BulkInsertExample()
        {
            using (BulkInsertOperation bulkInsert = DocumentStoreHolder.Store.BulkInsert())
            {
                // insert bunches of data efficiently
                for (int i = 0; i < 10000; i++)
                {
                    bulkInsert.Store(new Language
                    {
                        LastUpdate = "2006 - 02 - 15T10:02:19.0000000",
                        Name = "SampleLanguage" + i.ToString()
                    }
                    );

                }
            }
        }

        public static void UpdateExample()
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                // Query: find languages which name starts with 'SampleLanguage'
                IRavenQueryable<Language> query = session.Query<Language>()
                    .Where(x => x.Name.StartsWith("SampleLanguage"));

                var result = query.ToList();

                // Change all names to 'MarkedForDeletion'
                for (var l = 0; l < result.Count; l++)
                {
                    result[l].Name = "MarkedForDeletion";
                }

                // Apply changes
                session.SaveChanges();
            }
        }

        public static void DeleteExample()
        {
            // simple query - to delete single entry
            //var operation = DocumentStoreHolder.Store
            //.Operations
            //.Send(new DeleteByQueryOperation(new IndexQuery
            //{
            //    Query = "from Actor where startsWith(LastName, \"Rudakov\")"
            //}));
            //operation.WaitForCompletion(TimeSpan.FromSeconds(15));

            // complex query - to delete bunch of data
            var operation = DocumentStoreHolder.Store
            .Operations
            .Send(new DeleteByQueryOperation(new IndexQuery
            {
                Query = "from Language where Name = \"MarkedForDeletion\""
            }));
            operation.WaitForCompletion(TimeSpan.FromSeconds(20));
            Console.WriteLine("Ok deleted");
        }

        public static void CreateSingleEntry()
        {
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                // Storing a language
                Language entry = new Language
                {
                    Name = "Russian1",
                    LastUpdate = "2021 - 03 - 15T09:45:25.0000000",
                };

                session.Store(entry, "Language/8");
                session.Advanced.GetMetadataFor(entry)[Constants.Documents.Metadata.Collection] = "Language";

                session.SaveChanges();
            }
        }

        public static void LoadMultipleEntries()
        {
            // Querying multiple actors

            // Creating array of Id's for quering
            string[] query = new string[150];
            for (int i = 200; i > 50; i--)
            {
                query[200 - i] = "Actor/" + i.ToString();
            }
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                Dictionary<string, Actor> employees = session.Load<Actor>(query);
                foreach (KeyValuePair<string, Actor> entry in employees)
                {
                    // do something with entry.Value or entry.Key
                    Console.WriteLine(entry.Value.FirstName);
                }
            }
        }

        public static void LoadSingleEntry()
        {
            // Get single Payment
            string query = "Payment/32098";
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                Payment entry = session.Load<Payment>(query);
                Console.WriteLine(entry.Id);
                Console.WriteLine(entry.Amount);
            }
        }

        public static void LoadQuerying()
        {
            // using Query
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                List<Payment> results = session
                .Query<Payment>(collectionName: "Payment")
                .ToList();
                Console.WriteLine(results.Count);
            }
        }

    }
}
