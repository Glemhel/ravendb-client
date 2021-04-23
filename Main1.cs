﻿using Raven.Client.Documents;
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
            using (IDocumentSession session = DocumentStoreHolder.Store.OpenSession())
            {
                //// Storing a language
                //Language entry = new Language
                //{
                //    Name = "Russian1",
                //    LastUpdate = "2021 - 03 - 15T09:45:25.0000000",
                //};

                //session.Store(entry, "Language/8");
                //session.Advanced.GetMetadataFor(entry)[Constants.Documents.Metadata.Collection] = "Language";

                //// send all pending operations to server, in this case only `Put` operation
                //session.SaveChanges();
                // code here

                // !!!!
                // Querying multiple actors
                //string[] query = new string[150];
                //for (int i = 200; i > 50; i--)
                //{
                //    query[200 - i] = "Actor/" + i.ToString();
                //}
                //Dictionary<string, Actor> employees = session.Load<Actor>(query);
                //foreach (KeyValuePair<string, Actor> entry in employees)
                //{
                //    // do something with entry.Value or entry.Key
                //    Console.WriteLine(entry.Value.FirstName);
                //}
                //Console.ReadLine();

                // Get single Payment
                //string query = "Payment/32098";
                //Payment entry = session.Load<Payment>(query);
                //Console.WriteLine(entry.Id);
                //Console.WriteLine(entry.Amount);
                //Console.ReadLine();

                //// Load single Payment to correct table
                //Payment entry = new Payment
                //{
                //    Id = "Payment/32099",
                //    Amount = 2.89,
                //    PaymentDate = "2007 - 05 - 14T13:44:29.9965770",
                //    Staff = "Staff/1",
                //    Customer = "Customer/263",
                //    Rental = "Rental/12736"
                //};

                //// using Query
                //List<Payment> results = session
                //    .Query<Payment>(collectionName:"Payment")
                //    .ToList();
                //Console.WriteLine(results.Count);
                //Console.ReadLine();

                // using Query with no index
                List<Payment> results = session
                    .Query<Payment>(collectionName: "Payment")
                    .Include(x => x.Rental)
                    .OrderByDescending(x => x.Amount)
                    .ToList();
                int last = 0;
                results.Reverse();
                for (int i = 0; i < results.Count; i++) {
                    var payment = results[i];
                    Rental rental = session.Load<Rental>(payment.Rental);
                    if (i > 0 && results[i].Amount > results[i - 1].Amount)
                        last = i;
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
                        Count_smaller_pay = last
                    };

                    session.Store(entry);
                    session.Advanced.GetMetadataFor(entry)[Constants.Documents.Metadata.Collection] = "PaymentRental";

                    // send all pending operations to server, in this case only `Put` operation
                }
                session.SaveChanges();
            }
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            Console.WriteLine("Elapsed Time is {0:00}:{1:00}:{2:00}.{3}",
                            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
            Console.ReadLine();
        }
      
    }
}
