using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlineLibraryAPI.Dao;
using OlineLibraryAPI.Dao.Factory;
using OlineLibraryAPI.Models.Loan_;

namespace OlineLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly ILoanDAO dao = DAOFactory.GetDAOFactory(DAOFactory.DAOFactoryType.SQL_SERVER).GetRentDao();

        [Authorize]
        [HttpGet("SearchCustomerLoans")]
        public async Task<ActionResult<List<LoanDetail>>> SearchByLibraryCardNumber(long libraryCardNumber)
        {
            List<LoanDetail> loanDetails = await dao.FindByLibraryCardNumber(libraryCardNumber);
            return loanDetails == null || loanDetails.Count() == 0 ? NotFound() : Ok(loanDetails);
        }

        [Authorize]
        [HttpGet("SearchItemCopyLoans")]
        public async Task<ActionResult<List<LoanDetail>>> SearchByBarcode(long barcode)
        {
            List<LoanDetail> loanDetails = await dao.FindByBarcode(barcode);
            return loanDetails == null || loanDetails.Count() == 0 ? NotFound() : Ok(loanDetails);
        }

        [Authorize]
        [HttpGet("SearchItemCheckout")]
        public async Task<ActionResult<LoanDetail>> SearchActiveLoanByBarcode(long barcode)
        {
            LoanDetail? loanDetail = await dao.FindActiveLoanByBarcode(barcode);
            return loanDetail == null ? NotFound() : Ok(loanDetail);
        }

        [Authorize]
        [HttpPost("AddLoan")]
        public async Task<IActionResult> Loan(Loan loan)
        {
            int newID = await dao.Insert(loan);
            return newID <= 0 ? Conflict("No Record Added") : Ok(newID);
        }

        [Authorize]
        [HttpPut("UpdateLoan")]
        public async Task<IActionResult> UpdateLoan(List<LoanDetail> loanDetail)
        {
            int result = await dao.Update(loanDetail);
            return Ok(result + " rows updated");
        }
    }
}
