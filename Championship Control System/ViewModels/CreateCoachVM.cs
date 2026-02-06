using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Championship_Control_System.ViewModels
{
    public class CreateCoachVM
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(3)]
        [MaxLength(100)]
        public string? Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }  

        public int? TeamId { get; set; }
        
        public IFormFile ImageFile { get; set; }

        // Dropdown items
        public IEnumerable<SelectListItem>? Teams { get; set; }
    }
}
