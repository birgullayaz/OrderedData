namespace OrderedData.Models
{
    public class NewRegisterModel
    {
        private string? _name;
        private string? _surname;
        private string? _job;
        private string? _city;
        private string? _district;
        private long _cityId;
        private long _districtId;

        public int Id { get; set; }

        public string? Name 
        { 
            get { return _name; }
            set { _name = value?.ToUpper(); }
        }

        public string? Surname 
        { 
            get { return _surname; }
            set { _surname = value?.ToUpper(); }
        }

        public string? Job 
        { 
            get { return _job; }
            set { _job = value?.ToUpper(); }
        }

        public string? City 
        { 
            get { return _city; }
            set { _city = value; }
        }

        public long CityId 
        { 
            get { return _cityId; }
            set { _cityId = value; }
        }

        public string? District 
        { 
            get { return _district; }
            set { _district = value; }
        }

        public long DistrictId 
        { 
            get { return _districtId; }
            set { _districtId = value; }
        }
    }
} 