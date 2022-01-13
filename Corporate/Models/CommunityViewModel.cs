using FallenMoon.Library.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FallenMoon.Models
{
    public class CommunityViewModel
    {
        public List<CommunityForum> Forums { get; set; }
        public List<CommunityCategory> Categories { get; set; }
        public List<CommunitySubCategory> SubCategories { get; set; }
        public List<CommunityTopic> Topics { get; set; }
        public List<CommunityPost> Posts { get; set; }

        public CommunityForum Forum { get; set; }
        public CommunityCategory Category { get; set; }
        public CommunitySubCategory SubCategory { get; set; }
        public CommunityTopic Topic { get; set; }

        public NewTopicViewModel NewTopic { get; set; }
    }

    public class NewTopicViewModel
    {
        [Required]
        [Display(Name = "Topic Title")]
        public string NewTopicTitle { get; set; }

        [Required]
        [Display(Name = "Topic Text")]
        public string NewTopicText { get; set; }
    }
}