using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Championship_Control_System.ViewModels
{
    public class UpdateCoachVM
    {
        public int CoachId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public int? TeamId { get; set; }

       
        public IFormFile? ImageFile { get; set; }

        
        public string? CurrentImg { get; set; }

        // Dropdown
        public IEnumerable<SelectListItem>? Teams { get; set; }
    }
}
