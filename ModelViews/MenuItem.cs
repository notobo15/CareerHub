using System.Collections.Generic;

namespace RecruitmentApp.ModelViews
{
    public class MenuItem
    {
        public string Type { get; set; } // "category", "item", "parent"
        public string Title { get; set; }
        public string Url { get; set; }         // Dành cho "item"
        public string Icon { get; set; }        // Feather icon name
        public string SubId { get; set; }       // Collapse ID for "parent"
        public string Controller { get; set; }  // Nếu muốn dùng asp-controller
        public string Action { get; set; }      // Nếu muốn dùng asp-action
        public string Area { get; set; }        // Nếu có area
        public List<MenuItem> Children { get; set; } = new(); // Submenu


        public bool IsActive { get; set; }
        public bool IsParentActive { get; set; }    
    }
}
