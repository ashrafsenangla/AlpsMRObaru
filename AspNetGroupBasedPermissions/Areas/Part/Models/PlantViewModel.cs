using System;

namespace AspNetGroupBasedPermissions.Areas.Part.Models
{
    public class PlantViewModel
    {
        public int PlantID { get; set; }
        public string PlantName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public bool ISActive { get; set; }

    }
}