//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ParkgMVC.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using ParkgMVC.Models;
    using System.Data;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations;

    public partial class place : Statechart
    {
        public place()
        {
            this.reservation = new HashSet<reservation>();
            this.visit = new HashSet<visit>();
        }
        public long id_location_place { get; set; }
        public long id_location_level { get; set; }
        public int NumberPlace { get; set; }
        public long id_tariff_on_place { get; set; }
    
        public virtual levelzone levelzone { get; set; }
        public virtual tariffonplace tariffonplace { get; set; }
        public virtual ICollection<reservation> reservation { get; set; }
        public virtual ICollection<visit> visit { get; set; }

        MyParkingEntities mp = new MyParkingEntities();

        public bool ChangeStatus(string newstatus, long id_location_place)
        {
            place ForChangeStatus = mp.place.Where(x => x.id_location_place == id_location_place).FirstOrDefault();

            ForChangeStatus.Status = newstatus;
            mp.Entry(ForChangeStatus).State = EntityState.Modified;
            mp.SaveChanges();
            return true;

        }
    }
}
