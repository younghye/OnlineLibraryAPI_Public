using OlineLibraryAPI.Models.Person_;

namespace OlineLibraryAPI.Models.Loan_
{
    public class Loan
    {
        public Loan()
        {
            this.LoanDetails = [];
        }
        public int LoanID { get; set; }
        public long LibraryCardNumber { get; set; }
        public System.DateTime DateOfLoan { get; set; }
        public decimal TotalAmount { get; set; }
        public int Quantities { get; set; }

        public Customer? Customer { get; set; }
        public List<LoanDetail> LoanDetails { get; set; }
    }
}
