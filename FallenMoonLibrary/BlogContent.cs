using FallenMoon.Library.Database;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FallenMoon.Library
{
    public class BlogContent : IDisposable
    {
        #region IDisposable implementation with finalizer
        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
        #endregion

        public BlogHelper GetStories(int count = 0, int skip = 0)
        {
            BlogHelper helper = new BlogHelper();

            if (disposed)
            {
                helper.ErrorMessage = "Object has been disposed";
                helper.Success = false;
                return helper;
            }

            try
            {
                using (DatabaseContext context = new DatabaseContext())
                {
                    var stories = context.BlogPosts.Where(b => b.Published && b.DatePublished <= DateTime.Now)
                        .OrderBy(b => b.DatePublished)
                        .Skip(skip)
                        .Take(count)
                        .ToList();

                    helper.Stories = stories;
                    helper.Success = true;

                    helper.HasPrevPage = skip > 0;
                    helper.HasNextPage = context.BlogPosts.Count() > (count + skip);

                    return helper;
                }
            }
            catch (Exception e)
            {
                var logDate = DateTime.Now;
                Console.WriteLine("Error: {0}", e.Message);

                helper.Stories = null;
                helper.Story = null;
                helper.Success = false;
                helper.ErrorMessage = e.Message;

                return helper;
            }
        }

        public BlogHelper GetStory(int storyId)
        {
            BlogHelper helper = new BlogHelper();

            if (disposed)
            {
                helper.ErrorMessage = "Object has been disposed";
                return helper;
            }

            try
            {
                using (DatabaseContext context = new DatabaseContext())
                {
                    var story = context.BlogPosts.FirstOrDefault(b => b.BlogPostId == storyId && b.Published && b.DatePublished <= DateTime.Now);
                    if (story == null) throw new Exception("Post does not exist");
                    if(!story.Published || story.DatePublished >= DateTime.Now) throw new Exception("Post is not available");
                    if (story.Deleted || story.DateDeleted <= DateTime.Now) throw new Exception("Post has been deleted");

                    helper.Story = story;
                    helper.Success = true;

                    return helper;
                }
            }
            catch (Exception e)
            {
                var logDate = DateTime.Now;
                Console.WriteLine("Error: {0}", e.Message);

                helper.Stories = null;
                helper.Story = null;
                helper.ErrorMessage = e.Message;

                return helper;
            }
        }

        public BlogHelper AddStory(BlogPost post)
        {
            BlogHelper helper = new BlogHelper();

            if (disposed)
            {
                helper.ErrorMessage = "Object has been disposed";
                return helper;
            }

            try
            {
                using (DatabaseContext context = new DatabaseContext())
                {
                    var story = context.BlogPosts.Add(post);

                    context.SaveChanges();

                    helper.Story = story;
                    helper.ErrorMessage = "Success";

                    return helper;
                }
            }
            catch (Exception e)
            {
                var logDate = DateTime.Now;
                Console.WriteLine("Error: {0}", e.Message);

                helper.Stories = null;
                helper.Story = null;
                helper.ErrorMessage = e.Message;

                return helper;
            }
        }

        public List<BlogPostComment> GetBlogComments(int blogPostId)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var comments = context.BlogPostComments.Where(b => b.BlogPostId == blogPostId).ToList();
                foreach (var comment in comments)
                {
                    if (comment.UserAccount.Deleted)
                        comment.UserAccount.UserProfile.UserName = "Deleted";

                    if (comment.Deleted)
                        comment.CommentText = "Deleted";

                    if (comment.Removed)
                        comment.CommentText = "Removed by admin";

                    comment.BlogPostCommentReports = null;
                }
                return comments;
            }
        }

        public BlogHelper PostComment(int blogPostId, int userId, string commentText)
        {
            BlogHelper helper = new BlogHelper();

            if (disposed)
            {
                helper.ErrorMessage = "Object has been disposed";
                return helper;
            }

            try
            {
                using (DatabaseContext context = new DatabaseContext())
                {
                    var comment = new BlogPostComment()
                    {
                        BlogPostId = blogPostId,
                        UserId = userId,
                        CommentText = commentText,
                        CommentDate = DateTime.Now
                    };

                    comment = context.BlogPostComments.Add(comment);

                    context.SaveChanges();

                    helper.Comment = comment;
                    helper.Success = true;

                    return helper;
                }
            }
            catch (Exception e)
            {
                var logDate = DateTime.Now;
                Console.WriteLine("Error: {0}", e.Message);

                helper.Stories = null;
                helper.Story = null;
                helper.ErrorMessage = e.Message;

                return helper;
            }
        }

        public void PublishStory(int storyId)
        {

        }

        public void DeleteStory(int storyId)
        {

        }
    }
}
