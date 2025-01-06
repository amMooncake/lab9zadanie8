using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;

[Table("Contacts")]
public class ContactEntity
{
    [Key]
    public int Id{ get; set; }
    [Required]
    [MaxLength(length: 20)]
    public string FirstName { get; set; }
    
    [Required]
    [MaxLength(length: 50)]
    public string LastName { get; set; }
    
    public string Email { get; set; }
    
    [Column("Phone")]
    public string phoneNumber { get; set; }
    
    public DateOnly Birthday { get; set; }
    
    public DateTime Created { get; set; }
    
    public int OrganizationId { get; set; }


}