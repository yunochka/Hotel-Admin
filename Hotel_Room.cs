//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HotelAdm2App
{
    using System;
    using System.Collections.Generic;
    
    public partial class Hotel_Room
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Hotel_Room()
        {
            this.Booking = new HashSet<Booking>();
        }
    
        public int Hotel_Room_ID { get; set; }
        public int Staff_ID { get; set; }
        public string Room_Number { get; set; }
        public decimal Room_Price { get; set; }
        public string Room_Status { get; set; }
        public string Room_Description { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Booking> Booking { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
