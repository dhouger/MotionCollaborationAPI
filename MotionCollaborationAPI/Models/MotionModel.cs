using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace MotionCollaborationAPI.Models
{
	public class MotionModel : IConvertibleToBsonDocument
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string _id { get; set; }

		[BsonRepresentation(BsonType.String)]
		public string Name { get; set; }

		public List<string> Library { get; set; }

		[BsonRepresentation(BsonType.String)]
		public string ClassName { get; set; }

		[BsonRepresentation(BsonType.String)]
		public string Duration { get; set; }

		[BsonRepresentation(BsonType.String)]
		public string Delta { get; set; }

		[BsonRepresentation(BsonType.String)]
		public string Interpolation { get; set; }

		[BsonRepresentation(BsonType.String)]
		public string Tags { get; set; }

		[BsonRepresentation(BsonType.String)]
		public string Notes { get; set; }

		[BsonRepresentation(BsonType.String)]
		public string CodeSample { get; set; }

		[BsonRepresentation(BsonType.DateTime)]
		public DateTime CreationDate { get; set; }

		[BsonRepresentation(BsonType.DateTime)]
		public DateTime EditDate { get; set; }

		[BsonRepresentation(BsonType.String)]
		public string Author { get; set; }

		[BsonRepresentation(BsonType.String)]
		public string Editor { get; set; }

		[BsonConstructor]
		public MotionModel()
		{
			_id = ObjectId.GenerateNewId(DateTime.Now).ToString();
			CreationDate = DateTime.Now;
			EditDate = DateTime.Now;
		}

		public BsonDocument ToBsonDocument()
		{
			BsonArray lib = new BsonArray(Library);

			BsonDocument document = new BsonDocument()
			{
				{ "_id", _id },
				{ "Name", Name },
				{ "Library", lib },
				{ "ClassName", ClassName },
				{ "Duration", Duration },
				{ "Delta", Delta },
				{ "Interpolation", Interpolation },
				{ "Tags", Tags },
				{ "Notes", Notes },
				{ "CodeSample", CodeSample },
				{ "CreationDate", CreationDate.ToString("o") },
				{ "EditDate", EditDate.ToString("o") },
				{ "Author", Author },
				{ "Editor", Editor }
			};

			return document;
		}
	}
}