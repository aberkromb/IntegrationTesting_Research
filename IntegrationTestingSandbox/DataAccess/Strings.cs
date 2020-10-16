using System.ComponentModel.DataAnnotations.Schema;

namespace IntegrationTestingSandbox.DataAccess
{
    public class Strings
    {
        [Column("string")] public string String { get; set; }
    }
}