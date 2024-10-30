namespace fall2024_assignment3_sixao8.Models
{
    public class ActorDetail
    {
        public int Id { get; set; }
        public Actor Actor { get; set; }
        public IEnumerable<Movie> Movies { get; set; }
        
        public ActorDetail(Actor actor, IEnumerable<Movie> movies)
        {
            Actor = actor;
            Movies = movies;
        }
    }
}
