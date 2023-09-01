using System.ComponentModel.DataAnnotations;

namespace OData_mock.Models
{
    public class Odatamock
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public Odatamock(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

    }
}
