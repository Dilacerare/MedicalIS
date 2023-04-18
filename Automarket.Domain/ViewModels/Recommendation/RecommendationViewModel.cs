using System.ComponentModel.DataAnnotations;

namespace Automarket.Domain.ViewModels.Recommendation;

public class RecommendationViewModel
{
    [Display(Name = "Id")]
    public long Id { get; set; }
    
    [Required(ErrorMessage = "Укажите имя автора")]
    [Display(Name = "Имя автора")]
    public string AuthorName { get; set; }
    
    [Required(ErrorMessage = "Укажите имя поциента")]
    [Display(Name = "Имя поциента")]
    public string PatientName { get; set; }

    [Required(ErrorMessage = "Укажите описание")]
    [Display(Name = "Описание")]
    public string Description { get; set; }
    
    public string DateCreate { get; set; }
}