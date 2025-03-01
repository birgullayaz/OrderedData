using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderedData.Models
{
    [Table("districts")]
    public class DistrictModel
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("city_id")]
        public int CityId { get; set; }

        [ForeignKey("CityId")]
        public CityModel? City { get; set; }
    }
} 