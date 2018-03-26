using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CosmosDBClient
{

    class Program
    {
        static string endpointUri = "https://localhost:8081";
        static string authKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
        string dbName = "CourseDB";
        string collectionName = "Courses";
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to run");
            Console.ReadLine();

            Run();

            Console.ReadLine();

        }
        private static async void Run()
        {
            DocumentClient documentClient = new DocumentClient(new Uri(endpointUri),
                authKey);

            Database database = await CreateDatabase(documentClient);

            Console.WriteLine("database created successfully!");

            DocumentCollection documentCollection = await CreateDocumentCollection(documentClient, database);

            Console.WriteLine("Collection created successfully!");

            await CreateCourse(documentClient, documentCollection);

            Console.WriteLine("Collection created successfully!");

            //await DeleteCourse(new Guid("2f3ffd36-6011-4e35-9d94-3955b8d4d960"), documentCollection, documentClient);


            Console.ReadLine();
        }


        private Course QueryCourse(Guid guid, String dbName, DocumentClient documentClient, string collectionName)
        {
            Course selectedCourse = documentClient.CreateDocumentQuery<Course>(
                             UriFactory.CreateDocumentCollectionUri(dbName, collectionName))
                             .Where(v => v.Name == "test")
                             .AsEnumerable()
                             .FirstOrDefault();
            return selectedCourse;
        }

        private static async Task DeleteCourse(Guid guid, DocumentCollection documentCollection, DocumentClient documentClient)
        {
            Course course = documentClient.CreateDocumentQuery<Course>(documentCollection.DocumentsLink,
                new SqlQuerySpec(string.Format("SELECT * FROM c WHERE c.CourseId = '{0}'", guid))).AsEnumerable().FirstOrDefault();

            if (course == null)
                return;

            await documentClient.DeleteDocumentAsync(course.SelfLink);
        }
        private static async Task CreateCourse(DocumentClient documentClient, DocumentCollection documentCollection)
        {
            Course course = new Course()
            {
                CourseId = Guid.NewGuid(),
                Name = "En",
                Teacher = new Teacher()
                {
                    TeacherId = Guid.NewGuid(),
                    FullName = "Scott Hanselman",
                    Age = 44
                },
                Students = new List<Student>()
                {
                    new Student(){
                         FullName = "Trump",
                         StudentId = Guid.NewGuid()
                    }
                }
                ,
                Sessions = new List<Session>(){
                    new Session(){
                        SessionId = Guid.NewGuid(),
                        Name = "CosmosDB",
                        MaterialsCount = 10
                    },
                    new Session(){
                        SessionId = Guid.NewGuid(),
                        Name = "Ch1",
                        MaterialsCount = 3
                    }
                }
            };

            Document document = await documentClient.CreateDocumentAsync(documentCollection.DocumentsLink, course);
        }
        private static async Task<DocumentCollection> CreateDocumentCollection(DocumentClient documentClient, Database database)

        {
            DocumentCollection documentCollection = documentClient.CreateDocumentCollectionQuery(database.CollectionsLink).Where(c => c.Id == "courseDocumentCollection").AsEnumerable().FirstOrDefault();

            if (documentCollection == null)
            {
                documentCollection = await documentClient.CreateDocumentCollectionAsync(database.SelfLink, new DocumentCollection()
                {
                    Id = "courseDocumentCollection"
                });
            }

            return documentCollection;
        }
        private static async Task<Database> CreateDatabase(DocumentClient documentClient)
        {
            Database database = documentClient.CreateDatabaseQuery().Where(c => c.Id == "courseDatabase").AsEnumerable().FirstOrDefault();
            if (database == null)
            {
                database = await documentClient.CreateDatabaseAsync(new Database()
                {
                    Id = "courseDatabase"
                });
            }
            return database;
        }

        private static async Task<Database> deleteDatabase(DocumentClient documentClient, String dbName)
        {
            Database database = await documentClient.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(dbName));
            return database;
        }
    }


}
