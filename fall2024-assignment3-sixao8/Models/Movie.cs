namespace fall2024_assignment3_sixao8.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? IMDB { get; set; }
        public string? Genre { get; set; }
        public string? releaseYear { get; set; }
        public byte[]? Media { get; set; }
        public string[]? Reviews { get; set; }
        public double reviewSentiment { get; set; }
    }
}
