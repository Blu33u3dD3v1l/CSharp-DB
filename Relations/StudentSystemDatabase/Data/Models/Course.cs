﻿using System.ComponentModel.DataAnnotations;


namespace P01_StudentSystem.Data.Models
{
    public class Course
    {

       


        
        public int CourseId { get; set; }

        
        [MaxLength(80)]
       
        public string Name { get; set; }

        
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }

        
        public ICollection<Resource> Resources { get; set; }

       
        public ICollection<StudentCourse> StudentsCourses { get; set; }

       
        public ICollection<Homework> Homeworks { get; set; }







    }
}
