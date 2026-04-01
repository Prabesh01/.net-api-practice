using Microsoft.AspNetCore.Mvc;
namespace StudentAPI;

[ApiController]
[Route("api/[controller]")]
public class StudentController: ControllerBase
{
    private static readonly List<Student> students = new List<Student>
    {
    new Student { Id = 1, Name = "Alice Johnson", Age = 20, Faculty = "Engineering", Course = "Computer Science", Grade = "A" },
    new Student { Id = 2, Name = "Bob Smith", Age = 22, Faculty = "Business", Course = "Marketing", Grade = "B+" },
    new Student { Id = 3, Name = "Charlie Brown", Age = 19, Faculty = "Arts", Course = "Graphic Design", Grade = "A-" },
    new Student { Id = 4, Name = "Diana Prince", Age = 21, Faculty = "Science", Course = "Biology", Grade = "B" },
    new Student { Id = 5, Name = "Ethan Hunt", Age = 23, Faculty = "Engineering", Course = "Mechanical Engineering", Grade = "A" },
    new Student { Id = 6, Name = "Fiona Gallagher", Age = 20, Faculty = "Law", Course = "International Law", Grade = "B+" },
    new Student { Id = 7, Name = "George Martin", Age = 22, Faculty = "Music", Course = "Composition", Grade = "A-" },
    new Student { Id = 8, Name = "Hannah Lee", Age = 19, Faculty = "Medicine", Course = "Nursing", Grade = "B" },
    new Student { Id = 9, Name = "Ian Somerhalder", Age = 21, Faculty = "Drama", Course = "Theatre Arts", Grade = "A" },
    new Student { Id = 10, Name = "Jane Doe", Age = 20, Faculty = "Education", Course = "Elementary Education", Grade = "B+" }
    };

    [HttpGet()]
    [Route("getall")]
    public ActionResult<List<Student>> Get()
    {
        return Ok(students);
    }

    [HttpGet("{id}")]
    public ActionResult<Student> Get(int id)
    {
        var student=students.FirstOrDefault(p=>p.Id==id);
        if(student==null){
            return NotFound(
                "Student not found with ID: "+id
            );
        }
        return Ok(student);
    }

    [HttpPost]
    public ActionResult Post(Student student)
    {
        student.Id=students.Max(p=>p.Id)+1;
        students.Add(student);
        return CreatedAtAction(nameof(Get), new {id=student.Id},student);
    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, Student student)
    {
        var existingStudent = students.FirstOrDefault(p => p.Id == id);
        if (existingStudent == null)
        {
            return NotFound();
        }
        students.Remove(existingStudent);
        student.Id = id;
        students.Add(student);
        return Ok(student);
    }
    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var student = students.FirstOrDefault(p => p.Id == id);
        if (student == null)
        {
            return NotFound();
        }
        students.Remove(student);
        return NoContent();
    }
}