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
    
    public partial class BlogPostCommentReport
    {
        public int BlogPostCommentReportId { get; set; }
        public int BlogPostCommentId { get; set; }
        public int UserId { get; set; }
    
        public virtual BlogPostComment BlogPostComment { get; set; }
        public virtual UserAccount UserAccount { get; set; }
    }
}