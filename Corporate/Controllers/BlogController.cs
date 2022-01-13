using FallenMoon.Library;
using FallenMoon.Library.Database;
using FallenMoon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FallenMoon.Controllers
{
    public class BlogController : BaseController
    {
        public ActionResult Index(int page = 1)
        {
            using (BlogContent content = new BlogContent())
            {
                BlogViewModel model = new BlogViewModel();

                var count = page * 5;
                var skip = count - 5;

                var helper = content.GetStories(count, skip);
                if (helper != null && helper.Success)
                    model.Stories = helper.Stories;
                else
                    model.Stories = new List<BlogPost>();

                model.HasNextPage = helper.HasNextPage;
                model.HasPrevPage = helper.HasPrevPage;
                model.CurrentPage = page;

                using (SocialMedia social = new SocialMedia())
                {
                    model.Tweets = social.GetTweets("thefallenmoon", 3);
                }

                return View(model);
            }
        }

        [Route("Blog/Story/{storyId}")]
        public ActionResult Story(int storyId)
        {
            using (BlogContent content = new BlogContent())
            {
                BlogViewModel model = new BlogViewModel();

                var helper = content.GetStory(storyId);
                if (helper != null && helper.Success)
                    model.Story = helper.Story;
                else
                    return RedirectToAction("Index");

                using (SocialMedia social = new SocialMedia())
                {
                    model.Tweets = social.GetTweets("thefallenmoon", 3);
                }

                return View(model);
            }
        }

        public ActionResult Post(string message)
        {
            using (BlogContent content = new BlogContent())
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult Comment(int blogPostId, string commentText)
        {
            if (Request.UrlReferrer == null) return RedirectToAction("Index", "Home");

            var urlReferral = new Uri(Request.UrlReferrer.ToString()).AbsolutePath;
            if (Session["User"] == null) return RedirectToLocal(urlReferral);

            var userId = (Session["User"] as UserAccount).UserId;

            using (BlogContent content = new BlogContent())
            {
                var helper = content.PostComment(blogPostId, userId, commentText);
                if (helper.Success && helper.Comment != null)
                    return RedirectToLocal(urlReferral);
                else
                    return RedirectToAction("Index", "Home");
            }
        }
    }
}