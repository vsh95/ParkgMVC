﻿//------------------------------------------------------------------------------
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
    using ParkgMVC.Models;

    public partial class visit : Statechart
    {
        public long Num_vis { get; set; }
        public long id_ts { get; set; }
        public long id_location_place { get; set; }
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string FirstAttemptGoOut { get; set; }
        public string NextAttemptGoOut { get; set; }
        public long id_vis_param { get; set; }
        public Nullable<long> id_reservation_user { get; set; }
        public Nullable<long> id_abonement { get; set; }
        
        public virtual place place { get; set; }
        public virtual ts ts { get; set; }
        public virtual visitparameters visitparameters { get; set; }

        public bool RegisterIn()
        {
            bool Result = true;
            return Result;
        }
    }
}
