using OrderedData.Models;

namespace OrderedData.Data.Seeder
{
    public static class DatabaseSeeder
    {
        private static readonly string[] Names = { 
            "Ahmet", "Mehmet", "Ayşe", "Fatma", "Ali", "Veli", "Zeynep", "Elif", "Can", "Cem",
            "Deniz", "Ece", "Emre", "Gül", "Hakan", "İrem", "Kemal", "Leyla", "Murat", "Nur"
        };
        
        private static readonly string[] Surnames = { 
            "Yılmaz", "Demir", "Kaya", "Çelik", "Şahin", "Yıldız", "Özdemir", "Arslan", "Doğan", "Kılıç",
            "Aydın", "Bulut", "Çetin", "Erdoğan", "Güneş", "Koç", "Kurt", "Öztürk", "Şen", "Yalçın"
        };
        
        private static readonly string[] Jobs = { 
            "Software Engineer", "Teacher", "Doctor", "Lawyer", "Architect", 
            "Designer", "Manager", "Accountant", "Chef", "Writer",
            "Data Scientist", "Product Manager", "UX Designer", "DevOps Engineer", 
            "Business Analyst", "Marketing Specialist", "HR Manager", "Sales Executive",
            "System Administrator", "Quality Assurance"
        };

        public static void Seed(ApplicationDbContext context)
        {
            // Eğer veritabanında veri yoksa ekleme yap
            if (!context.UsersInfo.Any())
            {
                var users = new List<UsersInfoModel>();
                var random = new Random();

                for (int i = 1; i <= 100; i++)
                {
                    users.Add(new UsersInfoModel
                    {
                        Id = i,
                        Name = Names[random.Next(Names.Length)],
                        Surname = Surnames[random.Next(Surnames.Length)],
                        Job = Jobs[random.Next(Jobs.Length)]
                    });
                }

                context.UsersInfo.AddRange(users);
                context.SaveChanges();
            }
        }
    }
} 