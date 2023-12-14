using System.ComponentModel.DataAnnotations;

public class CheckoutWithLateFeeDTO 
{
    public int Id { get; set; }
    [Required]
    public int MaterialId { get; set; }
    public MaterialDTO Material { get; set; }
    [Required]
    public int PatronId { get; set; }
    public PatronDTO Patron { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public List<CheckoutDTO> Checkouts { get; set; }
    private static decimal _lateFeePerDay = .50M;
    public decimal? LateFee 
    {
        get
        {
            DateTime dueDate = CheckoutDate.AddDays(Material.MaterialType.CheckoutDays);
            DateTime returnDate = ReturnDate ?? DateTime.Today;
            int daysLate = (returnDate - dueDate).Days;
            decimal fee = daysLate * _lateFeePerDay;
            return daysLate > 0 ? fee : 0;
        }
    }
    public bool? Paid { get; set; }
}