using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using webApi.Models.Data;
using webApi.Models.Entity;

namespace webApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbcontext _tables;
        public EmployeeController(AppDbcontext tables)
        {
            _tables = tables;
        }
        //now we need to define the type of fucnto that we are usign for
        [HttpGet] //for getting data from database 
      
        public IActionResult GetAllData()
        {
            var lst = _tables.Employees.ToList();
            return Ok(lst);
        }
        //partical get - get the details for a only id 
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _tables.Employees.Find(id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        [HttpPost]
       
        public IActionResult Add_emp(Adder y)
        {
            var obj_emp = new Employee()
            {
                Name = y.Name,
                Surname = y.Surname,
                Email = y.Email,
                Phone = y.Phone,
                Address = y.Address,
                Position = y.Position
            };
            _tables.Employees.Add(obj_emp);
            _tables.SaveChanges();
            //return Ok(obj_emp);
            return CreatedAtAction(nameof(GetById), new { id = obj_emp.Id }, obj_emp);
        }

        [HttpDelete("{id}")]
       
        public IActionResult Delete_emp(int id)
        {
            var emp = _tables.Employees.Find(id);
            if(emp == null) return NotFound();
            _tables.Employees.Remove(emp);
            _tables.SaveChanges();
            return NoContent(); ;
        }

        [HttpPut("{id}")]

        public IActionResult Update_emp(int id, Employee emp)
        {
            var item
                = _tables.Employees.Find(id);
            if(item == null) return NotFound();
            item.Name = emp.Name;
            item.Surname = emp.Surname;
            item.Email = emp.Email;
            item.Phone = emp.Phone;
            item.Address = emp.Address;
            item.Position = emp.Position;
            _tables.SaveChanges();
            return NoContent(); // 204 No Content
        }
    }
}
