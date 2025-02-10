using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlineLibraryAPI.Dao;
using OlineLibraryAPI.Dao.Factory;
using OlineLibraryAPI.Models;
using OlineLibraryAPI.Models.Person_;

namespace OlineLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeDAO dao = DAOFactory.GetDAOFactory(DAOFactory.DAOFactoryType.SQL_SERVER).GetEmployeeDao();
        private readonly IAuthDAO tokenDao = DAOFactory.GetDAOFactory(DAOFactory.DAOFactoryType.SQL_SERVER).GetTokenDao();

        [HttpGet("Login")]
        public async Task<ActionResult<Auth>> Login(string userName, string password)
        {
            Employee? employee = await dao.FindLoginUser(userName, password);
            if (employee != null)
            {
                var token = tokenDao.GenerateJWTToken(employee);
                return Ok(new Auth { Token = token, Employee = employee });
            }
            return NotFound();
        }

        [HttpGet("ForgotPassword")]
        public async Task<ActionResult<Employee>> ForgotPassword(string email)
        {
            Employee? employee = await dao.FindByEmail(email);
            return employee == null ? NotFound() : Ok(employee);
        }

        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(int id, string password)
        {
            int result = await dao.UpdatePassword(id, password);
            return result == 0 ? NotFound("No password Updated") : Ok();
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Insert(Employee employee)
        {
            int newID = await dao.Insert(employee);
            if (newID == -1)
                return Conflict("The Employee is Already Exist");
            else if (newID == -2)
                return Conflict("UserName not Available");
            else if (newID > 0)
                return Ok(newID);
            else
                return Conflict("No Record Added");
        }

        [Authorize]
        [HttpGet("SearchByID")]
        public async Task<ActionResult<Employee>> SearchByID(int id)
        {
            Employee? employee = await dao.FindByID(id);
            return employee == null ? NotFound() : Ok(employee);
        }

        [Authorize]
        [HttpGet("Search")]
        public async Task<ActionResult<List<Employee>>> Search(string? firstName, string? lastName, string? role, string? phoneNumber)
        {
            List<Employee> employees = await dao.Select(firstName, lastName, role, phoneNumber);
            return employees.Count == 0 ? NotFound() : Ok(employees);
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> Update(Employee employee)
        {
            int result = await dao.Update(employee);
            return result == 0 ? NotFound("No Record Updated") : Ok();
        }

        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteByID(int id)
        {
            int result = await dao.DeleteByID(id);
            return result == 0 ? NotFound("No Record Deleted") : Ok();
        }
    }
}
