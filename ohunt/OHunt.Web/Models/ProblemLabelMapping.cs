using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OHunt.Web.Models
{
    [Table("problem_label_mappings")]
    public class ProblemLabelMapping
    {
        /// <summary>
        /// The id of the problem
        /// </summary>
        [Key]
        public long ProblemId { get; set; }

        /// <summary>
        /// The id of online judge
        /// </summary>
        [Key]
        [Required]
        public MappingOnlineJudge OnlineJudgeId { get; set; }

        /// <summary>
        /// The label of the problem
        /// </summary>
        [MinLength(1)]
        [MaxLength(25)]
        public string? ProblemLabel { get; set; }
    }
}
