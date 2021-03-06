//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FallenMoon.Library.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class BlogPostComment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BlogPostComment()
        {
            this.BlogPostCommentReports = new HashSet<BlogPostCommentReport>();
        }
    
        public int BlogPostCommentId { get; set; }
        public int BlogPostId { get; set; }
        public int UserId { get; set; }
        public string CommentText { get; set; }
        public System.DateTime CommentDate { get; set; }
        public bool Deleted { get; set; }
        public bool Removed { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BlogPostCommentReport> BlogPostCommentReports { get; set; }
        public virtual BlogPost BlogPost { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}
