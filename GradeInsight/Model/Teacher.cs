﻿namespace GradeInsight.Model
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public required string TeacherName { get; set; }
        public required string ContactNo {  get; set; } 
        public required string Email { get; set; }
        public  DateTime DateCreated { get; set; }
        
    }
}
