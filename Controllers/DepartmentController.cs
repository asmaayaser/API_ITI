using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.DTO;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DepartmentController : ControllerBase
    {
        private readonly ITIEntity context;
        public DepartmentController(ITIEntity _context) {
            this.context = _context;
        
        }

        //CRUD
        //api/Department "Get"
        [HttpGet]
        public IActionResult GetDept()
        {
            List<Department> depts = context.Department.ToList();
            if (depts == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return Ok(depts);
        }



        [HttpGet("{id:int}", Name = "DepartmentDetailsRoute")]
        //[Route("{id}")]
        public IActionResult GetDeptById([FromRoute] int id)
        {
            Department dept = context.Department.Include(d => d.Employees).FirstOrDefault(d => d.Id == id);
            if (dept == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return Ok(dept);
        }



        [HttpGet("{name:alpha}")]
        public IActionResult GetDeptByName(string name)
        {
            Department dept = context.Department.Include(d => d.Employees).FirstOrDefault(d => d.Name == name);
            if (dept == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            return Ok(dept);
        }

        [HttpGet("dto/{id:int}")]
        public IActionResult GetDeptWithEmp(int id)
        {
            Department deptModel = context.Department.Include(d => d.Employees).FirstOrDefault(d => d.Id == id);
            
            //map model to dto
            DepartmentWithEmployeesDto deptDto = new DepartmentWithEmployeesDto();
            deptDto.Id = deptModel.Id;
            deptDto.Name = deptModel.Name;
            foreach (var item in deptModel.Employees)
            {
                deptDto.EmpNames.Add(new EmployeeDto { Name = item.Name, Id = item.Id });
            }
            return Ok(deptDto);

        }

        [HttpPut("{id}")]
        public IActionResult PutDept(int id, [FromBody] Department dept)
        {
            if (ModelState.IsValid)
            {
                //old reference
                Department olddept = context.Department.Include(d => d.Employees).FirstOrDefault(d => d.Id == id);
                if (olddept == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                //get from new => old
                olddept.Name = dept.Name;
                olddept.ManagerName = dept.ManagerName;
                context.SaveChanges();
                //update
                //return StatusCode(204);
                //return StatusCode(StatusCodes.Status204NoContent);
                return Ok(dept);
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveDept(int id)
        {
            Department dept = context.Department.Include(d => d.Employees).FirstOrDefault(d => d.Id == id);
            if (dept == null)
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
            try
            {
                context.Department.Remove(dept);
                context.SaveChanges();
                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost]
        public IActionResult PostDept(Department newdept)
        {
            if (ModelState.IsValid)
            {
                //save 
                context.Department.Add(newdept);
                context.SaveChanges();
                string url = Url.Link("DepartmentDetailsRoute", new { id = newdept.Id });
                //return StatusCode(StatusCodes.Status201Created);
                return Created(url, newdept);
            }
            // in companies may do that
            // mapping to modelstate => custom class "error"
            return BadRequest(ModelState);
        }
    }
}
