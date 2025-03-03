﻿namespace GradeInsight.Model
{
    public class User
    {
        public int UserId { get; set; }
        public required int UserTypeId { get; set; }
        public DateTime DateCreated { get; set; }
      
        public required string UserName { get; set; }
        public  string? UserPassword { get; set; }
        public required string UserFullName { get; set; }
        public required string UserEmail { get; set; }
    }
}
