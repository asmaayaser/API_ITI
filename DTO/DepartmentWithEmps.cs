using System.Text.Json.Serialization;

namespace WebAPI.DTO
{
    public class DepartmentWithEmployeesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<EmployeeDto> EmpNames { get; set; } = new List<EmployeeDto>();

    }
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
