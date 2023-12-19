using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ITIEntity context;

        public EmployeeController(ITIEntity _context)
        {
            context = _context;
        }

        //CRUD
        //api/Empolyee "Get"
        [HttpGet]
        public IActionResult GetEmployee()
        {
            List<Employee> emps = context.Employees.ToList();
            if (emps == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return Ok(emps);
        }

        [HttpGet("{id:int}",Name ="EmpolyeeDetailsRoute")]
        //[Route("{id}")]
        public IActionResult GetById([FromRoute]int id)
        {
            Employee emp =context.Employees.FirstOrDefault(e => e.Id == id);
            if(emp == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return Ok(emp);
        }



        [HttpGet("{name:alpha}")] 
        public IActionResult GetByName(string name)
        {
            Employee emp = context.Employees.FirstOrDefault(e => e.Name == name);
            if (emp == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return Ok(emp);
        }

        [HttpGet("dept/{id}")]
        public IActionResult GetEmpWithDeptName(int id)
        {
            Employee emp = context.Employees.Include(e => e.Department).FirstOrDefault(e => e.Id == id);
            if (emp == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return Ok(emp);
        }

        [HttpGet("dto/{id}")]
        public IActionResult GetEmpWithDeptName1(int id)
        {
            Employee emp = context.Employees.Include(e => e.Department).FirstOrDefault(e => e.Id == id);
            if (emp == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            EmployeeNameWithDepartmentNameDto EmpDto = new EmployeeNameWithDepartmentNameDto();
            EmpDto.EmpName = emp.Name;
            EmpDto.DeptName = emp.Department.Name;
            EmpDto.EmpId = emp.Id;  
            return Ok(EmpDto);
        }

        [HttpPut("{id}")]
        public IActionResult PutEmployee(int id ,[FromBody] Employee emp)
        {
            if(ModelState.IsValid)
            {
                //old reference
                Employee oldEmp = context.Employees.FirstOrDefault(e => e.Id == id);
                if(oldEmp==null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                //get from new => old
                oldEmp.Name = emp.Name;
                oldEmp.Age = emp.Age;
                oldEmp.Salary = emp.Salary;
                oldEmp.Address = emp.Address;
                context.SaveChanges();
                //update
                //return StatusCode(204);
                return StatusCode(StatusCodes.Status204NoContent);


            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveEmployee(int id)
        {
            Employee emp = context.Employees.FirstOrDefault(e => e.Id == id);
            if(emp==null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            try
            {
                context.Employees.Remove(emp);
                context.SaveChanges();
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
          
        }
        [HttpPost]
        public IActionResult PostEmployee(Employee newEmp)
        {
            if(ModelState.IsValid)
            {
                //save 
                context.Employees.Add(newEmp);
                context.SaveChanges();
                string url = Url.Link("EmpolyeeDetailsRoute",new {id =newEmp.Id});
                //return StatusCode(StatusCodes.Status201Created);
                return Created(url , newEmp);
            }
            // in companies may do that
            // mapping to modelstate => custom class "error"
            return BadRequest(ModelState);
        }

    }
}
