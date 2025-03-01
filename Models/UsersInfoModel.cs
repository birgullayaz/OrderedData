using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderedData.Models;

[Table("usersinfo")]  // Tablo adını belirtiyoruz
public class UsersInfoModel
{ 
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column("name", TypeName = "varchar(255)")]
    public string? Name { get; set; }

    [Column("surname", TypeName = "varchar(255)")]
    public string? Surname { get; set; }

    [Column("job", TypeName = "varchar(255)")]
    public string? Job { get; set; }

    [Column("city", TypeName = "bigint")]
    public long? City { get; set; }

    [Column("district", TypeName = "bigint")]
    public long? District { get; set; }
}
