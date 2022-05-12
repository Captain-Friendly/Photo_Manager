
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace UsersManager.Models
{

using System;
    using System.Collections.Generic;
    
public partial class Photo
{



    public int Id { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public int VisibilityId { get; set; }

    public string GUID { get; set; }

    public System.DateTime CreationDate { get; set; }

    public double Ratings { get; set; }

    public Nullable<int> RatingsCount { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<PhotoRating> PhotoRatings { get; set; }

    public virtual User User { get; set; }

    public virtual PhotoVisibility PhotoVisibility { get; set; }

}

}
