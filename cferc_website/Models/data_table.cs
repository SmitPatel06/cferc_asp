//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace cferc_website.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class data_table
    {
        public long valID { get; set; }
        public string seriesID { get; set; }
        public Nullable<int> year { get; set; }
        public string period { get; set; }
        public Nullable<long> value { get; set; }
    
        public virtual series series { get; set; }
    }
}
