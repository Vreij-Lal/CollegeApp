using CollegeApp.Model;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace CollegeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        //default built in logger interface
        private readonly ILogger<StudentController> _logger;
        public StudentController(ILogger <StudentController> logger)
        {
            _logger = logger;
        }


        //Get all students
        [HttpGet]
        [Route("all", Name = "GetAllStudents")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public ActionResult<IEnumerable<Student>> GetStudents()
        {
            _logger.LogInformation("Get Students method Started");

            var students = CollegeRepository.Students.Select(s => new StudentDTO()
            {
                Id = s.Id,
                StudentName = s.StudentName,
                Address = s.Address,
                Email = s.Email,
            }
            );

            // ok 200 success
            return Ok(students);
        }

        //Get student by id
        [HttpGet]
        [Route("{id:int}", Name = "GetStudentsById")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<StudentDTO> GetStudentById(int id)
        {

            //BadRequest - 400 - client error
            if (id <= 0)
            {
                _logger.LogWarning("bad request");
                return BadRequest();
            }

            var student = CollegeRepository.Students.Where(n => n.Id == id).FirstOrDefault();

            //NotFound - 404 - client error
            if (student == null)
            {
                _logger.LogError("Student not found with the given id");
                return NotFound($"the student with the id {id} is not found");
            }

            var studentDTO = new StudentDTO
            {
                Id = student.Id,
                StudentName = student.StudentName,
                Email = student.Email,
                Address = student.Address
            };

            //ok - 200 - success
            return Ok(studentDTO);

        }

        //Get student by name
        [HttpGet]
        [Route("{name:alpha}", Name = "GetStudentsByName")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<Student> GetStudentByName(string name)
        {

            //BadRequest - 400 - client error
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest();
            }

            var student = CollegeRepository.Students.Where(n => n.StudentName == name).FirstOrDefault();

            //NotFound - 404 - client error
            if (student == null)
            {

                return NotFound($"the student with the name {name} is not found");
            }


            var studentDTO = new StudentDTO
            {
                Id = student.Id,
                StudentName = student.StudentName,
                Email = student.Email,
                Address = student.Address
            };


            //ok - 200 - success
            return Ok(studentDTO);
        }


        //Create new student
        [HttpPost]
        [Route("create")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<StudentDTO> CreateStudent([FromBody] StudentDTO model)
        {

            if (model == null)
            {
                return BadRequest();
            }


            int newId = CollegeRepository.Students.LastOrDefault().Id + 1;

            Student student = new Student
            {
                Id = newId,
                StudentName = model.StudentName,
                Address = model.Address,
                Email = model.Email
            };
            CollegeRepository.Students.Add(student);
            model.Id = student.Id;
            //return Ok(model);
            return CreatedAtRoute("GetStudentsById", new { id = model.Id }, model);
        }


        //to update student
        [HttpPut]
        [Route("update")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(204)]

        public ActionResult UpdateStudent([FromBody] StudentDTO model)
        {
            if (model == null || model.Id <= 0)
            {
                BadRequest();
            }

            var existingStudent = CollegeRepository.Students.Where(s => s.Id == model.Id).FirstOrDefault();

            if (existingStudent == null)
            {
                return NotFound();
            }

            existingStudent.StudentName = model.StudentName;
            existingStudent.Email = model.Email;
            existingStudent.Address = model.Address;

            return NoContent();

        }



        //to update by patching student
        [HttpPatch]
        [Route("{id:int}updatepartial")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(204)]


        public ActionResult UpdateStudentPartial(int id, [FromBody] JsonPatchDocument<StudentDTO> patchDocument)
        {
            if (patchDocument == null || id <= 0)
            {
                BadRequest();
            }

            var existingStudent = CollegeRepository.Students.Where(s => s.Id == id).FirstOrDefault();

            if (existingStudent == null)
            {
                return NotFound();
            }

            var studentDTO = new StudentDTO
            {
                Id = existingStudent.Id,
                StudentName = existingStudent.StudentName,
                Email = existingStudent.Email,
                Address = existingStudent.Address,
            };

            patchDocument.ApplyTo(studentDTO, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            existingStudent.StudentName = studentDTO.StudentName;
            existingStudent.Email = studentDTO.Email;
            existingStudent.Address = studentDTO.Address;

            return NoContent();

        }




        //Delete student by id
        [HttpDelete("delete/{id:min(1):max(100)}", Name = "DeleteStudentById")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public ActionResult<bool> DeleteStudent(int id)
        {

            //BadRequest - 400 - client error
            if (id <= 0)
            {
                return BadRequest();
            }

            var student = CollegeRepository.Students.Where(n => n.Id == id).FirstOrDefault();

            //NotFound - 404 - client error
            if (student == null)
            {

                return NotFound($"the student with the name {id} is not found");
            }


            CollegeRepository.Students.Remove(student);
            return true;
        }

    }
}
