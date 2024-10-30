using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PB102App.Models
{
    public class Work
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [NotMapped]
        public int SelectCategoryId { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public ICollection<WorkImage> Images { get; set; }
        [NotMapped]
        public List<IFormFile> UploadedImages { get; set; }
    }
}
