namespace GradeInsight.Model
{
    public class Student
    {
        public int StudentId { get; set; }
        public required string StudentName { get; set; }
        public required string Address {  get; set; }   
        public required string ContactNo{  get; set; }
        public  DateTime DateCreated { get; set; }
        public required int FacultyId {  get; set; }
        public Faculty ? Faculty {  get; set; }

        public required int SemesterId { get;  set; }
        public Semester ? Semester { get; set; }
        
    }
}
