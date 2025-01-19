namespace GradeInsight.Model
{
    public class Marks
    {
        public int MarksId {  get; set; }
        public required int Mark {  get; set; }

        public required DateTime DateCreated { get; set; }
        public required bool Deleted { get; set; }

    }
}
