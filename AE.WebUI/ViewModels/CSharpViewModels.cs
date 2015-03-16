using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AE.WebUI.ViewModels
{
    public class CSharpSnippletListItem
    {
        public int Id { get; set; }
        public string Headline { get; set; }
    }

    public class CSharpSnippletContentItem
    {
        public string Headline { get; set; }
        public string Content { get; set; }
    }
}