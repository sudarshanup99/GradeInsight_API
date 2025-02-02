namespace GradeInsight.ViewModel
{
    public class MarksDataVM
    {
        public int MarksId { get; set; }
        public required int Mark { get; set; }
        public required int CourseId { get; set; }
        public required string CourseName {  get; set; }
        public int ExamTypeId { get; set; }
        public required string ExamTypeName { get; set; }
    }
}
