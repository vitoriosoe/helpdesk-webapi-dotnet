namespace HelpDeskApi.Models { public class Setor { public int Id { get; set; } public string Nome { get; set; } = string.Empty; public ICollection<Usuario>? Usuarios { get; set; } } }
