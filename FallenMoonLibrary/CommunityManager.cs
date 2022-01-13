using FallenMoon.Library.Database;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FallenMoon.Library
{
    public class CommunityManager : IDisposable
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

        public List<CommunityForum> GetForums()
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var forums = context.CommunityForums.Where(c => c.Visible)
                                                    .OrderBy(c => c.Order)
                                                    .ToList();

                return forums;
            }
        }

        public List<CommunityCategory> GetCategories(int forumId)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var categories = context.CommunityCategories.Where(c => c.ParentForumId == forumId)
                                                            .Include(c => c.CommunityForum)
                                                            .Include(c => c.CommunitySubCategories)
                                                            .OrderBy(c => c.Order)
                                                            .ToList();

                return categories;
            }
        }

        public List<CommunitySubCategory> GetSubCategories(int categoryId)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var subCategories = context.CommunitySubCategories.Where(c => c.ParentCategoryId == categoryId)
                                                                  .Include(c => c.CommunityCategory)
                                                                  .Include(c => c.CommunityTopics)
                                                                  .OrderBy(c => c.Order)
                                                                  .ToList();

                return subCategories;
            }
        }

        public List<CommunityTopic> GetTopics(int subCategoryId)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var topics = context.CommunityTopics.Where(p => p.SubCategoryId == subCategoryId && !p.Deleted)
                                                    .Include(t => t.CommunitySubCategory)
                                                    .Include(t => t.CommunityPosts)
                                                    .OrderBy(p => p.TopicDate)
                                                    .ToList();

                return topics;
            }
        }

        public List<CommunityPost> GetPosts(int topicId)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var posts = context.CommunityPosts.Where(p => p.ParentTopicId == topicId)
                                                  .OrderBy(p => p.PostDate)
                                                  .ToList();

                return posts;
            }
        }
        public List<CommunityPost> GetPosts(int topicId, int page, int count)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var posts = context.CommunityPosts.Where(p => p.ParentTopicId == topicId)
                                                  .OrderBy(p => p.PostDate)
                                                  .Skip((page - 1) * count).Take(count)
                                                  .ToList();

                return posts;
            }
        }


        public CommunityForum GetForum(string forumName)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var forum = context.CommunityForums.FirstOrDefault(f => f.ForumName == forumName);
                return forum;
            }
        }

        public CommunityCategory GetCategory(string categoryName)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var category = context.CommunityCategories.FirstOrDefault(c => c.CategoryName == categoryName);
                return category;
            }
        }

        public CommunitySubCategory GetSubCategory(string subCategoryName)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var subCategory = context.CommunitySubCategories.FirstOrDefault(c => c.SubCategoryName == subCategoryName);
                return subCategory;
            }
        }

        public CommunityForum GetForum(int forumId)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var forum = context.CommunityForums.FirstOrDefault(f => f.ForumId == forumId);
                return forum;
            }
        }

        public CommunityCategory GetCategory(int categoryId)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var category = context.CommunityCategories.FirstOrDefault(c => c.CategoryId == categoryId);
                return category;
            }
        }

        public CommunitySubCategory GetSubCategory(int subCategoryId)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var subCategory = context.CommunitySubCategories.FirstOrDefault(c => c.SubCategoryId == subCategoryId);
                return subCategory;
            }
        }

        public CommunityTopic GetTopic(int topicId)
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var topic = context.CommunityTopics.FirstOrDefault(t => t.TopicId == topicId);
                return topic;
            }
        }
    }
}
