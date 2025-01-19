namespace GradeInsight.Model
{
    public class TeacherxCourse
    {
        public int TeacherXcourseId {  get; set; }
        public required int TeacherId {  get; set; }
        public required int CourseId {  get; set; }
        public  DateTime DateCreated { get; set; }
       
    }
}
