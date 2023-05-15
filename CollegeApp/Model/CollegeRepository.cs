namespace CollegeApp.Model
{
    public class CollegeRepository
    {
        public static List<Student> Students { get; set; } = new List<Student>(){

                new Student {
                Id = 1,
                StudentName = "studentOne",
                Email = "student1email@gmail.com",
                Address = "beirut, Lebanon"
                }
                ,
                new Student {
                Id = 2,
                StudentName = "studentTwo",
                Email = "student2email@gmail.com",
                Address = "zahle, Lebanon"
                }
        };
    }
}
