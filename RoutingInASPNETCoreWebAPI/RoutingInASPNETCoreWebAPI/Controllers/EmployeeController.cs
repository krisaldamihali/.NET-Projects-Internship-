using Microsoft.AspNetCore.Mvc;
using RoutingInASPNETCoreWebAPI.Models;
using RoutingInASPNETCoreWebAPI.Repositories;

namespace RoutingInASPNETCoreWebAPI.Controllers
{
    [ApiController] 
    [Route("api/[controller]")] //sets the base route to api/employee
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository _repository;

        //Injects IEmployeeRepository to manage data operations.
        public EmployeeController(IEmployeeRepository repository)
        {
            _repository = repository;
        }

        //  Retrieves all employees (GET api/employee).
        [HttpGet]
        public ActionResult<IEnumerable<Employee>> GetAllEmployees()
        {
            var employees = _repository.GetAll();
            return Ok(employees);
        }

        //Retrieves a specific employee by ID(GET api/employee/{id}).
        [HttpGet("{id}")]
        public ActionResult<Employee> GetEmployeeById(int id)
        {
            var employee = _repository.GetById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        //Adds a new employee(POST api/employee).
        [HttpPost]
        public ActionResult<Employee> CreateEmployee([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _repository.Add(employee);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }

        // Updates an existing employee entirely (PUT api/employee/{id}).
        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.Id)
            {
                return BadRequest("Employee ID mismatch.");
            }

            if (!_repository.Exists(id))
            {
                return NotFound();
            }

            _repository.Update(employee);
            return NoContent();
        }

        // Partially updates an existing employee (PATCH api/employee/{id}).
        [HttpPatch("{id}")]
        public IActionResult PatchEmployee(int id, [FromBody] Employee employee)
        {
            var existingEmployee = _repository.GetById(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            // For simplicity, updating all fields. In real scenarios, use JSON Patch.
            existingEmployee.Name = employee.Name ?? existingEmployee.Name;
            existingEmployee.Position = employee.Position ?? existingEmployee.Position;
            existingEmployee.Age = employee.Age != 0 ? employee.Age : existingEmployee.Age;
            existingEmployee.Email = employee.Email ?? existingEmployee.Email;

            _repository.Update(existingEmployee);
            return NoContent();
        }

        // Deletes an employee (DELETE api/employee/{id}).
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            if (!_repository.Exists(id))
            {
                return NotFound();
            }

            _repository.Delete(id);
            return NoContent();
        }
    }
}