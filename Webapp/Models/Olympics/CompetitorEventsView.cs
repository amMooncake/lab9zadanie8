namespace Webapp.Models.Olympics;

public class CompetitorEventsView
{
    public Person Person { get; set; }
    public string Name { get; set; }
    public string SportName { get; set; }
    public string? EventName { get; set; }
    public string CitiName { get; set; }
    public string Season { get; set; }
    public int? Age { get; set; }
    public string Medal { get; set; }
}