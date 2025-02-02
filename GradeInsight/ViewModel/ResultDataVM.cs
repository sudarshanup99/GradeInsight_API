namespace GradeInsight.ViewModel
{
    public class ResultDataVM
    {
        public int StudentId { get; set; }
        public required string StudentName { get; set; }
        public int FacultyId { get; set; }
        public required string FacultyName { get; set; }
        public int SemesterId { get; set; }
        public required string SemesterName { get; set; }
     


        public List< MarksDataVM> ?Marks { get; set; }
    }
}
