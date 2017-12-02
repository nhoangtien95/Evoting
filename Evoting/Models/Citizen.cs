namespace Evoting.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Citizen")]
    public partial class Citizen
    {
        [StringLength(50)]
        public string ID { get; set; }

        public int? Account { get; set; }
    }
}
