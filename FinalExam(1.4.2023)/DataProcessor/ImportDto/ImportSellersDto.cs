using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Boardgames.DataProcessor.ImportDto
{
    public class ImportSellersDto
    {

        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(30)]

        public string Address { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [RegularExpression(@"^www\.[a-zA-Z0-9-]+\.com$")]
        public string Website { get; set; }

        public virtual int[] Boardgames { get;set; }
    }
}
