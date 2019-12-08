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
        [Required]
        public string Name { get; set; }
        [ForeignKey("ProjectFK")]
        [Required]
        public Project project { get; set; }
        public bool Approved { get; set; }
        public DateTime DateEnd { get; set; }
        [ForeignKey("EmployeeFK")]
        [Required]
        public Employee Employee { get; set; }
        public string EmployeeFK { get; set; }
        [Required]
        public int ProjectFK { get; set; }
        [Required]
        public DateTime DateStart { get; set; }
    }
}
