using MongoDB.Bson;
using MongoDB.Driver;
using MotionCollaborationAPI.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Web.Http;

namespace MotionCollaborationAPI.Controllers
{
	public class DeletedController : ApiController
    {
		#region Private Methods
		/// <summary>
		/// Returns the MongoDB Client using the Azure URL storred in the project settings
		/// </summary>
		/// <returns>MongoClient</returns>
		private MongoClient GetMongoClient()
		{
			MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(Properties.Settings.Default.MongoDB));
			settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
			MongoClient client = new MongoClient(settings);
			return client;
		}

		/// <summary>
		/// Returns a MongoDB Database Object
		/// </summary>
		/// <param name="DatabaseName">The name of the database to return</param>
		/// <returns>IMongoDatabase</returns>
		private IMongoDatabase GetMongoDatabase(string DatabaseName)
		{
			MongoClient client = GetMongoClient();
			IMongoDatabase db = client.GetDatabase(DatabaseName);
			return db;
		}

		/// <summary>
		/// Returns the default MongoDB database and the DeletedCollection storred in the project settings
		/// </summary>
		/// <typeparam name="T">DataModel for use with the Documents returned in the collection</typeparam>
		/// <returns>IMongoCollection</returns>
		private IMongoCollection<T> GetMongoCollection<T>()
		{
			return GetMongoCollection<T>(Properties.Settings.Default.DefaultDBName, Properties.Settings.Default.DeletedCollection);
		}

		/// <summary>
		/// Returns a custom set database/collection from the default MongoDB in the project settings
		/// </summary>
		/// <typeparam name="T">DataModel for use with the Documents returned in the collection</typeparam>
		/// <param name="DatabaseName">The name of the database to retrieve from the MongoDB client</param>
		/// <param name="CollectionName">The name of the collection to retrieve from the database</param>
		/// <returns>IMongoCollection</returns>
		private IMongoCollection<T> GetMongoCollection<T>(string DatabaseName, string CollectionName)
		{
			IMongoDatabase db = GetMongoDatabase(DatabaseName);
			IMongoCollection<T> collection = db.GetCollection<T>(CollectionName);
			return collection;
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Return all of the rules from the deleted collection and the default database/MongoClient in the project settings
		/// </summary>
		/// <returns>Formatted JSON string with data or exception details</returns>
		[HttpGet]
		public HttpResponseMessage Get()
		{
			try
			{
				var collection = GetMongoCollection<MotionModel>();
				var docs = collection.Find(new BsonDocument()).ToList();

				return Request.CreateResponse(HttpStatusCode.OK, docs);
			}
			catch (Exception e)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, e);
			}
		}

		/// <summary>
		/// Update a MotionModel in the DeletedCollection using data from the modelJson
		/// </summary>
		/// <param name="modelJson">JSON from an HTTP PUT request</param>
		/// <returns>A done status or a formatted JSON response</returns>
		[HttpPut]
		public HttpResponseMessage Put([FromBody]MotionModel modelJson)
		{
			try
			{
				var collection = GetMongoCollection<MotionModel>();

				var update = Builders<MotionModel>.Update
					.Set("Name", modelJson.Name)
					.Set("Library", modelJson.Library)	
					.Set("ClassName", modelJson.ClassName)
					.Set("Duration", modelJson.Duration)
					.Set("Delta", modelJson.Delta)
					.Set("Interpolation", modelJson.Interpolation)
					.Set("Tags", modelJson.Tags)
					.Set("Notes", modelJson.Notes)
					.Set("CodeSample", modelJson.CodeSample)
					.CurrentDate("EditDate");

				var filter = new FilterDefinitionBuilder<MotionModel>().Eq("_id", modelJson._id);

				collection.FindOneAndUpdate(filter, update);

				return Request.CreateResponse(HttpStatusCode.OK, "Done");
			}
			catch (Exception e)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, e);
			}
		}

		/// <summary>
		/// Add a new record to the DeletedCollection using data from modelJson
		/// </summary>
		/// <param name="modelJson">JSON from an HTTP POST request</param>
		/// <returns>A done status or a formatted JSON response</returns>
		[HttpPost]
		public HttpResponseMessage Post([FromBody]MotionModel modelJson)
		{
			try
			{
				var collection = GetMongoCollection<MotionModel>();

				collection.InsertOne(modelJson);

				return Request.CreateResponse(HttpStatusCode.OK, "Done");
			}
			catch (Exception e)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, e);
			}
		}

		/// <summary>
		/// Delete a record from the DeletedCollection using a provided id string
		/// </summary>
		/// <param name="id"></param>
		/// <returns>A done status or a formatted JSON response</returns>
		[HttpDelete]
		public HttpResponseMessage Delete(string id)
		{
			try
			{
				var collection = GetMongoCollection<MotionModel>();

				var filter = new FilterDefinitionBuilder<MotionModel>().Eq("_id", id);

				collection.DeleteOne(filter);

				return Request.CreateResponse(HttpStatusCode.OK, "Done");
			}
			catch (Exception e)
			{
				return Request.CreateResponse(HttpStatusCode.BadRequest, e);
			}
		}
		#endregion
    }
}
