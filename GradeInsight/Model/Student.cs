namespace GradeInsight.Model
{
    public class Student
    {
        public int StudentId { get; set; }
        public required string StudentName { get; set; }
        public required string Address {  get; set; }   
        public required string ContactNo{  get; set; }
   
        public required DateTime DateCreated { get; set; }
        public required bool Deleted { get; set; }
    }
}
