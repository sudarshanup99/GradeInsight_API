﻿namespace GradeInsight.ViewModel
{
    public class PredictionInitialDataViewModel
    {

      public required int StudentId { get; set; }
        public required int CourseId { get; set; }
            public double InternalMarks { get; set; }  
            public double PreboardMarks { get; set; }  
        
    }
}
