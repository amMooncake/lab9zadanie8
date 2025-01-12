using Microsoft.AspNetCore.Mvc.Rendering;

namespace Webapp.Models.Olympics;

public class CreateCompetitorEvent
{
    public Person Person { get; set; }
    
    public List<SelectListItem> Sports { get; set; }
    public List<SelectListItem> Events { get; set; }
    public List<SelectListItem> GameNames {set; get;} 
    public int? Age { get; set; }
}