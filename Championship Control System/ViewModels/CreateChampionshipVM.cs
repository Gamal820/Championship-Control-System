using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Championship_Control_System.ViewModels
{
    public class CreateChampionshipVM
    {
        [Required]
        public string ChampionshipName { get; set; } = null!;

        public string? Season { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public string? Country { get; set; }

        public IFormFile? LogoFile { get; set; }

        // Teams
        public List<int> TeamIds { get; set; } = new List<int>();

        public IEnumerable<SelectListItem>? Teams { get; set; }
    }
}
