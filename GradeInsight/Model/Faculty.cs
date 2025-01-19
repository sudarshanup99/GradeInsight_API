namespace GradeInsight.Model
{
    public class Faculty
    {
        public int FacultyId {  get; set; } 
        public required string FacultyName { get; set; }
        public required DateTime DateCreated { get; set; }
        public required bool Deleted { get; set; }
    }
}
