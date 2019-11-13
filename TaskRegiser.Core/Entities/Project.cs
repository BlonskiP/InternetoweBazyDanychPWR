using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TaskRegiser.Core.Entities
{
    public class Project
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [ForeignKey("EmployeeFK")]
        public Employee ProjectManager { get; set; }
        public string EmployeeFK { get; set; }
        public DateTime CreationDate { get; set; }

    }

}
