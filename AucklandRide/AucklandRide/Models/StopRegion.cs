using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AucklandRide.Updater.Models
{
    public class StopRegion
    {
        [Key]
        public int Id { get; set; }

        [StringLength(20)]
        public string StopId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
