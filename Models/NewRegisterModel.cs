namespace OrderedData.Models
{
    public class NewRegisterModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Job { get; set; }
        public string? City { get; set; }
        public long CityId { get; set; }  // int yerine long
        public string? District { get; set; }
        public long DistrictId { get; set; }  // int yerine long
    }
} 