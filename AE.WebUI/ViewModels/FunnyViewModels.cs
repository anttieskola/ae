using System.Collections.Generic;

namespace AE.WebUI.ViewModels
{
    public class PostModel
    {
        public PostModel()
        {
            Comments = new List<string>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string SourceUrl { get; set; }
        public List<string> Comments { get; set; }
    }
}