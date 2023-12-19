using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.DTO;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly ITIEntity context;
        public DepartmentController(ITIEntity _context) {
            this.context = _context;
        
        }
        [HttpGet("{id:int}")]
        public IActionResult GetDept(int id)
        {
            Department deptModel = context.Department.Include(d => d.Employees).FirstOrDefault(d => d.Id == id);
            
            //map model to dto
            DepartmentWithEmployeesDto deptDto = new DepartmentWithEmployeesDto();
            deptDto.Id = deptModel.Id;
            deptDto.Name = deptModel.Name;
            foreach( var item in deptModel.Employees)
            {
                deptDto.EmpNames.Add(new  EmployeeDto { Name = item.Name,Id = item.Id});
            }
            return Ok(deptDto);

        }
    }
}
