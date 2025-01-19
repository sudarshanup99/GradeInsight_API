namespace GradeInsight.Model
{
    public class TeacherxCourse
    {
        public int TeacherXcourseId {  get; set; }
        public required int TeacherId {  get; set; }
        public required int CourseId {  get; set; }
        public required DateTime DateCreated { get; set; }
        public required bool Deleted { get; set; }
    }
}
