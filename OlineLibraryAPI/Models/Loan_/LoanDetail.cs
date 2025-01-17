using OlineLibraryAPI.Models.Item_;

namespace OlineLibraryAPI.Models.Loan_
{
    public class LoanDetail
    {

        public int LoanID { get; set; }
        public long Barcode { get; set; }
        public System.DateTime ReturnDueDate { get; set; }
        public System.DateTime? ReturnDate { get; set; }
        public decimal Fine { get; set; }
        public string? Note { get; set; }

        public virtual ItemCopy? ItemCopy { get; set; }
        public virtual Loan? Loan { get; set; }
    }
}
