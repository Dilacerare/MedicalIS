using System.ComponentModel.DataAnnotations;

namespace Automarket.Domain.ViewModels.PerfectHealth;

public class PerfectHealthViewModel
{
    public long Id { get; set; }
    
    [Display(Name = "Температура")]
    [Required(ErrorMessage = "Введите температуру")]
    public double Temperature { get; set; }
    
    [Display(Name = "Давление")]
    [Required(ErrorMessage = "Введите давление")]
    [RegularExpression(@"\d{1,3}\/\d{1,3}", 
        ErrorMessage = "Должна соответствовать формату ***/***, * - цифры")]
    public string BloodPressure { get; set; }
    
    [Display(Name = "Общий анализ мочи")]
    [Required(ErrorMessage = "Введите анализ мочи")]
    public string GUrineAnalysis { get; set; }
    
    [Display(Name = "Общий анализ крови")]
    [Required(ErrorMessage = "Введите анализ крови")]
    public string GBloodTest { get; set; }
    
    [Display(Name = "Холестерин")]
    [Required(ErrorMessage = "Введите холестерин")]
    public double Cholesterol { get; set; }
}