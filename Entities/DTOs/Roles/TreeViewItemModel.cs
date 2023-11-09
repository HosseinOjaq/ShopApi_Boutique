using System.Collections.Generic;

namespace Entities.DTOs.Roles
{
    public class TreeViewItemModel
    {
        public bool Enabled { get; set; }
        public bool Expanded { get; set; }

        public bool Encoded { get; set; }

        public bool Selected { get; set; }

        public string Text { get; set; }

        public string SpriteCssClass { get; set; }

        public int Id { get; set; }

        public string Url { get; set; }

        public string ImageUrl { get; set; }

        public bool HasChildren { get; set; }

        public bool Checked { get; set; }

        public List<TreeViewItemModel> Items { get; set; }

        public IDictionary<string, string> HtmlAttributes { get; set; }

        public IDictionary<string, string> ImageHtmlAttributes { get; set; }

        public IDictionary<string, string> LinkHtmlAttributes { get; set; }

        public TreeViewItemModel()
        {
            Enabled = true;
            Encoded = true;
            Items = new List<TreeViewItemModel>();
            HtmlAttributes = new Dictionary<string, string>();
            ImageHtmlAttributes = new Dictionary<string, string>();
            LinkHtmlAttributes = new Dictionary<string, string>();
        }
    }
}
