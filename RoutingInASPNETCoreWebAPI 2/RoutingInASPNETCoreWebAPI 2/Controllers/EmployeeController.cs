using Microsoft.AspNetCore.Mvc;
using RoutingInASPNETCoreWebAPI.Data;

using RoutingInASPNETCoreWebAPI_2.Models;

namespace RoutingInASPNETCoreWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        // GET api/Employee/Gender/Male?Department=IT&City=Los Angeles
        [HttpGet("Gender/{gender}")]
        public ActionResult<IEnumerable<Employee>> GetEmployeesByGender([FromRoute] string gender, [FromQuery] string? department, [FromQuery] string? city)
        {
            var filteredEmployees = EmployeeData.Employees
                .Where(e => e.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(department))
                filteredEmployees = filteredEmployees.Where(e => e.Department.Equals(department, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(city))
                filteredEmployees = filteredEmployees.Where(e => e.City.Equals(city, StringComparison.OrdinalIgnoreCase));

            var result = filteredEmployees.ToList();

            if (!result.Any())
                return NotFound("No employees match the provided search criteria.");

            return Ok(result);
        }
    }
}