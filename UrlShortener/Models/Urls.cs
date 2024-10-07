using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models;

public class Urls
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(6)]
    public string CodigoEncurtador { get; set; }
    [Required]
    public string UrlDestino { get; set; }
    public DateTimeOffset DataCriacao { get; set; } = DateTimeOffset.UtcNow;
    public int QuantidadeDeAcessos { get; set; } = 0;
    public DateTimeOffset? DataAtualizacao { get; set; }
}
