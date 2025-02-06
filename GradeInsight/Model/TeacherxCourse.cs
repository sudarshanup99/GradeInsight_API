namespace GradeInsight.Model
{
    public class TeacherxCourse
    {
        public int TeacherxCourseId {  get; set; }
        public required int TeacherId {  get; set; }
        public required int CourseId {  get; set; }
        public  DateTime DateCreated { get; set; }

        public Teacher? Teacher { get; set; }
        public Course? Course { get; set; }


    }
}
