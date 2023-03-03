﻿using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data
{
   
    public class StudentSystemContext : DbContext
    {


        public StudentSystemContext()
        {

        }

        public StudentSystemContext(DbContextOptions options)
           : base(options)
        {

        }
  
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Homework> Homeworks { get; set; }
        public DbSet<StudentCourse> StudentsCourses { get; set; }







        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=StudSys;Integrated Security=true;");
            }

        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentCourse>(e => e.HasKey(sc => new { sc.CourseId, sc.StudentId }));
            base.OnModelCreating(modelBuilder);
        }
    }
}
