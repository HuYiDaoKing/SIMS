//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace SIMS.Web.EntityDataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class CourseScore
    {
        public int Id { get; set; }
        public Nullable<int> CourseId { get; set; }
        public Nullable<int> CourseTeacherId { get; set; }
        public Nullable<int> MajorId { get; set; }
        public Nullable<int> StudentId { get; set; }
        public Nullable<double> Score { get; set; }
        public Nullable<System.DateTime> CreateTime { get; set; }
        public Nullable<System.DateTime> ModifyTime { get; set; }
    }
}
