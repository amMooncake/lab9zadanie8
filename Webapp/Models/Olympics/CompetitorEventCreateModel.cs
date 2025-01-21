using Microsoft.AspNetCore.Mvc.Rendering;

namespace Webapp.Models.Olympics;

public class CompetitorEventCreateModel
{
    public virtual Person Person { get; set; }
    public string SportName { get; set; }
    public string EventName { get; set; }
    public string OlympicsName { get; set; }
    public int Age { get; set; }
    public List<SelectListItem> Sports { get; set; } // Add this property
    public List<SelectListItem> Events { get; set; } // Add this property
    public List<SelectListItem> Olympics { get; set; } // Add this property

}