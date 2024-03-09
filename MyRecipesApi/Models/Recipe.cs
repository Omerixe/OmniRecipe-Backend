using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyRecipesApi.Models
{
    public class Recipe
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }

        public ICollection<string> Steps { get; set; }
        public int Version { get; set; }

        public string ImageData { get; set; }
    }
}