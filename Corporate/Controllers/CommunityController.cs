using FallenMoon.Library;
using FallenMoon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FallenMoon.Controllers
{
    public class CommunityController : BaseController
    {
        // GET: Community
        public ActionResult Index()
        {
            CommunityViewModel model = new CommunityViewModel();

            using (CommunityManager manager = new CommunityManager())
            {
                model.Forums = manager.GetForums();
            }

            return View(model);
        }

        [Route("Community/{forumName}", Name = "Forum")]
        public ActionResult Forum(string forumName)
        {
            CommunityViewModel model = new CommunityViewModel();

            using (CommunityManager manager = new CommunityManager())
            {
                var forum = manager.GetForum(forumName);
                if (forum == null)
                {
                    return RedirectToAction("Index");
                }

                model.Forum = forum;
                model.Categories = manager.GetCategories(model.Forum.ForumId);
            }

            return View(model);
        }

        [Route("Community/{forumName}/{subCategoryName}/NewTopic", Name = "NewTopic")]
        public ActionResult NewTopic(string forumName, string subCategoryName)
        {
            CommunityViewModel model = new CommunityViewModel();

            if (Request.Cookies["fallen_token"] == null || Session["User"] == null)
                return RedirectToAction("Index");

            using (CommunityManager manager = new CommunityManager())
            {
                var subCategory = manager.GetSubCategory(subCategoryName);
                if (subCategory == null)
                {
                    return RedirectToAction("Index");
                }

                model.SubCategory = subCategory;
                model.Forum = manager.GetForum(model.Category.ParentForumId);
                model.NewTopic = new NewTopicViewModel();
            }

            return View(model);
        }

        [Route("Community/{forumName}/{subCategoryName}/NewTopic", Name = "NewTopicSubmit")]
        public ActionResult NewTopic(CommunityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (Request.Cookies["fallen_token"] == null || Session["User"] == null)
                return RedirectToAction("Index");

            using (CommunityManager manager = new CommunityManager())
            {
            }

            return View(model);
        }

        [Route("Community/{forumName}/{subCategoryName}", Name = "SubCategory")]
        public ActionResult SubCategory(string forumName, string subCategoryName)
        {
            CommunityViewModel model = new CommunityViewModel();

            using (CommunityManager manager = new CommunityManager())
            {
                var subCategory = manager.GetSubCategory(subCategoryName);
                if (subCategory == null)
                {
                    return RedirectToAction("Index");
                }

                model.SubCategory = subCategory;
                model.Category = manager.GetCategory(subCategory.ParentCategoryId);
                model.Forum = manager.GetForum(model.Category.ParentForumId);

                model.Topics = manager.GetTopics(subCategory.SubCategoryId);
            }

            return View(model);
        }

        [Route("Community/{forumName}/{subCategoryName}/Page-{page}", Name = "SubCategoryPage")]
        public ActionResult SubCategory(string forumName, string subCategoryName, int page, int count = 10)
        {
            CommunityViewModel model = new CommunityViewModel();

            using (CommunityManager manager = new CommunityManager())
            {
                var subCategory = manager.GetSubCategory(subCategoryName);
                if (subCategory == null)
                {
                    return RedirectToAction("Index");
                }

                model.SubCategory = subCategory;
                model.Category = manager.GetCategory(subCategory.ParentCategoryId);
                model.Forum = manager.GetForum(model.Category.ParentForumId);

                model.Topics = manager.GetTopics(subCategory.SubCategoryId).Skip((page - 1) * count).Take(count).ToList();
            }

            return View(model);
        }
        
        [Route("Community/{forumName}/{subCategoryName}/Topic-{topicId}", Name = "Topic")]
        public ActionResult Topic(string forumName, string subCategoryName, int topicId)
        {
            CommunityViewModel model = new CommunityViewModel();

            using (CommunityManager manager = new CommunityManager())
            {
                var topic = manager.GetTopic(topicId);
                if(topic == null)
                {
                    return RedirectToRoute("SubCategory", new { forumName = forumName, subCategoryName = subCategoryName });
                }

                var posts = manager.GetPosts(topicId);
                if(posts.Count() == 0)
                {
                    return RedirectToRoute("SubCategory", new { forumName = forumName, subCategoryName = subCategoryName });
                }

                model.Topic = topic;
                model.Posts = posts;
                model.SubCategory = manager.GetSubCategory(subCategoryName);
                model.Category = manager.GetCategory(model.SubCategory.ParentCategoryId);
                model.Forum = manager.GetForum(model.Category.ParentForumId);
            }
            
            return View(model);
        }

        [Route("Community/{forumName}/{subCategoryName}/Topic-{topicId}/Page-{page}", Name = "TopicPage")]
        public ActionResult Topic(string forumName, string subCategoryName, int topicId, int page, int count = 10)
        {
            CommunityViewModel model = new CommunityViewModel();

            using (CommunityManager manager = new CommunityManager())
            {
                var topic = manager.GetTopic(topicId);
                if (topic == null)
                {
                    return RedirectToRoute("SubCategory", new { forumName = forumName, subCategoryName = subCategoryName });
                }

                var posts = manager.GetPosts(topicId).Skip((page - 1) * count).Take(count).ToList();
                if (posts.Count() == 0)
                {
                    return RedirectToRoute("SubCategory", new { forumName = forumName, subCategoryName = subCategoryName });
                }

                model.Topic = topic;
                model.Posts = posts;
                model.SubCategory = manager.GetSubCategory(subCategoryName);
                model.Category = manager.GetCategory(model.SubCategory.ParentCategoryId);
                model.Forum = manager.GetForum(model.Category.ParentForumId);
            }

            return View(model);
        }
    }
}