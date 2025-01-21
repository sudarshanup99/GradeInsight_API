namespace GradeInsight.Model
{
    public class Marks
    {
        public int MarksId {  get; set; }
        public required int Mark {  get; set; }
        public required int CourseId {  get; set; }
        public required int StudentId { get; set; }
        public required int ExamTypeId {  get; set; }
        public DateTime DateCreated { get; set; }
        

    }
}
