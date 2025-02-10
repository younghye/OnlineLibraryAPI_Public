using OlineLibraryAPI.Dao.Base;
using OlineLibraryAPI.Models.Loan_;
namespace OlineLibraryAPI.Dao;

public interface ILoanDAO : IBaseDAO
{
    Task<int> Insert(Loan loan);

    Task<int> Update(List<LoanDetail> loanDetail);

    Task<List<LoanDetail>> FindByLibraryCardNumber(long libraryCardNumber);

    Task<List<LoanDetail>> FindByBarcode(long barcode);

    Task<LoanDetail?> FindActiveLoanByBarcode(long barcode);
}