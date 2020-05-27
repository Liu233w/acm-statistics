using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OHunt.Web.Models
{
    [Table("submissions")]
    public class Submission
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MinLength(1)]
        [MaxLength(10)]
        public string OjName { get; set; }
    }
}
