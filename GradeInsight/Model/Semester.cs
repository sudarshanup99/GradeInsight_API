namespace GradeInsight.Model
{
    public class Semester
    {
        public int SemesterId {  get; set; }
        public required string SemesterName { get; set; }
       public required int FacultyId {  get; set; }

         public  DateTime DateCreated { get; set; }
        public Faculty ? Faculty { get; set; }

        
    }
}
