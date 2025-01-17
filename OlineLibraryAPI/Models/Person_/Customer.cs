using OlineLibraryAPI.Models.Loan_;

namespace OlineLibraryAPI.Models.Person_
{
    public class Customer
    {
        public Customer()
        {
            this.Rents = [];
        }
        public int CustomerID { get; set; }
        public long LibraryCardNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime? Deleted { get; set; }
        public required virtual Person Person { get; set; }
        public virtual List<Loan> Rents { get; set; }
    }
}
