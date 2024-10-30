using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace fall2024_assignment3_sixao8.Models
{
    public class ActorMovie
    {
        [Key]
        public int ID { get; set; }

        [ForeignKey("Movie")]
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        [ForeignKey("Actor")]
        public int ActorId { get; set; }
        public Actor? Actor { get; set; }
    }
}
