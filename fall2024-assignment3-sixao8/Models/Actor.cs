using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI.Chat;
using System.ClientModel;
using System.Numerics;

namespace fall2024_assignment3_sixao8.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Gender { get; set; }
        public int? Age { get; set; }
        public string? IMDBlink { get; set; }
        public byte[]? ProfilePic { get; set; }
        public string[]? Tweets { get; set; }
        public double? TweetSentiment { get; set; }
    }
}
