namespace Finance.Web.Models
{
    public class HtmlEditor
    {
        [System.ComponentModel.DataAnnotations.Required]
        //[System.Web.Mvc.AllowHtml]
        [System.ComponentModel.DataAnnotations.UIHint("Html_Editor_Compressed")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Page Content")]
        public string Editor { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        //[System.Web.Mvc.AllowHtml]
        [System.ComponentModel.DataAnnotations.UIHint("Html_Editor_Compressed")]
        [System.ComponentModel.DataAnnotations.Display(Name = "Template")]
        public string ArticleTemplate { get; set; }
    }
}