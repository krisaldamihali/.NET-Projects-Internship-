using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CreditSimulatorAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }  

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [JsonIgnore] 
        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
