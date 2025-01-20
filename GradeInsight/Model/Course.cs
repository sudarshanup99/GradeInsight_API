namespace GradeInsight.Model
{
    public class Course
    {
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        public required int MarksId {  get; set; }
        public required int FacultyId {  get; set; }
        public  DateTime DateCreated { get; set; }

        public  required int TeacherId { get; set; }

        public required int SemesterId { get; set; }




    }
}
