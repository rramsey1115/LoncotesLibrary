using System.ComponentModel.DataAnnotations;
namespace Loncotes.Models;

public class Patron 
{
    public int Id { get; set; }
    [Required]
    public string? FirstName { get; set; }
    [Required]
    public string? LastName { get; set; }
    [Required]
    public string? Address { get; set; }
    [Required]
    public string? Email { get; set; }
    [Required]
    public bool IsActive { get; set; }
    public List<Checkout> Checkouts { get; set; }
    private List<CheckoutWithLateFeeDTO> CheckoutWithLateFees { get; set; }
    public decimal? Balance {
        get
        {
            // look at all checkoutsWithLateFees
            // if lateFee exists and Paid == false, execute math for balance
            decimal? allFees = null;
            CheckoutWithLateFees.Select(clf => clf.LateFee != null && clf.Paid == false ? allFees += clf.LateFee : null);
            //  return allFees (either null or a decimal)
            return allFees;
        }
    }
}