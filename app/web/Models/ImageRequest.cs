using System.ComponentModel.DataAnnotations;

namespace LangBot.Web.Models
{
    public class ImageRequest
    {
        [Required] public string Image { get; set; }
        [Required] public string Hash { get; set; }
    }
}
