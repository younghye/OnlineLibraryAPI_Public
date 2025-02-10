using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlineLibraryAPI.Dao;
using OlineLibraryAPI.Dao.Factory;
using OlineLibraryAPI.Models.Person_;

namespace OlineLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerDAO dao = DAOFactory.GetDAOFactory(DAOFactory.DAOFactoryType.SQL_SERVER).GetCustomerDao();

        [Authorize]
        [HttpGet("SearchByID")]
        public async Task<ActionResult<Customer>> SearchByID(int id)
        {
            Customer? customer = await dao.FindByID(id);
            return customer == null ? NotFound() : Ok(customer);
        }

        [Authorize]
        [HttpGet("SearchByLibraryCardNumber")]
        public async Task<ActionResult<Customer>> SearchByID(long libraryCardNumber)
        {
            Customer? customer = await dao.FindByLibraryCard(libraryCardNumber);
            return customer == null ? NotFound() : Ok(customer);
        }

        [Authorize]
        [HttpGet("Search")]
        public async Task<ActionResult<List<Customer>>> Search(long? libraryCardNumber, string? firstName, string? lastName, string? phoneNumber)
        {
            List<Customer> customer = await dao.Select(libraryCardNumber, firstName, lastName, phoneNumber);
            return customer.Count == 0 ? NotFound() : Ok(customer);
        }

        [Authorize]
        [HttpPut("Delete")]
        public async Task<IActionResult> DeleteByID(int id)
        {
            int result = await dao.DeleteByID(id);
            if (result == 0)
                return NotFound("No Record Deleted");
            if (result == -1)
                return Conflict("Cannot delete because the customer is referenced");
            return Ok();
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(Customer customer)
        {
            int result = await dao.Update(customer);
            return result == 0 ? NotFound("No Record Updated") : Ok();
        }

        [Authorize]
        [HttpPost("Add")]
        public async Task<ActionResult<Customer>> Insert(Customer customer)
        {
            var result = await dao.Insert(customer);
            return result == null ? Conflict("No Record Added. Check if the Customer is Already Exist") : Ok(result);

        }
    }
}
