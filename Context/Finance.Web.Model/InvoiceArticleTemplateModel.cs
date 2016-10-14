using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finance.Web.Model
{
    public class InvoiceArticleTemplateModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ArticleTemplate { get; set; }
    }
}
