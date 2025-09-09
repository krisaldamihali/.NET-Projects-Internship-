using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultipleURLs.Models;

namespace MultipleURLs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        // Action Method with Multiple Routes
        [HttpGet("All")]
        [HttpGet("AllEmployees")]
        [HttpGet("GetAll")]
        public ActionResult<IEnumerable<Employee>> GetAllEmployees()
        {
            var employees = EmployeeData.Employees;
            return Ok(employees);
        }
    }
}