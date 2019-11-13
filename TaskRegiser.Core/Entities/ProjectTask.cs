using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TaskRegiser.Core.Entities
{
    public class ProjectTask
    {
        [Key]
        public int ID{get;set;}
        [MaxLength(20)]
        public string Name { get; set; }
        [ForeignKey("Project")]
        public Project project { get; set; }
        public bool Approved { get; set; }
        public DateTime DateEnd { get; set; }
        [ForeignKey("EmployeeFK")]
        public Employee Employee { get; set; }
        public string EmployeeFK { get; set; }
        public DateTime DateStart { get; set; }
    }
}
