namespace LangBot.Web.Models
{
    public class EditDialogModel
    {
        public string ResponseUrl { get; set; }
        public string TemplateId { get; set; }
        public string UserId { get; set; }
        public bool Anonymous { get; set; }
    }
}
