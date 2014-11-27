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
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using System.Data;
    using System.Data.Entity;
    using ParkgMVC.Models;
    using System.ComponentModel.DataAnnotations;
    public partial class reservation : ConnectedTariffPlan
    {
        public long id_reservation_user { get; set; }
        public long id_Reservation_Tariff { get; set; }
        public Nullable<long> id_location_place { get; set; }
        public Nullable<int> id_alternative_location_place { get; set; }
        public string ApproximatelyDateOutFromActivity { get; set; }
        public string Description { get; set; }

        public virtual place place { get; set; }
        public virtual reservation_tariff reservation_tariff { get; set; }
        public virtual usr usr { get; set; }

        MyParkingEntities mp = new MyParkingEntities();

        public bool CreateReservation(string Describe, string Log, reservation_tariff tar)
        {
            bool Result = false;
            try
            {
                reservation r = new reservation();
                r.id_Reservation_Tariff = tar.id_Reservation_Tariff;
                r.Login = Log;
                r.Status = "Formed";
                r.Description = Describe;
                mp.reservation.Add(r);
                mp.SaveChanges();
                Result = true;
            }
            catch
            {
                Result = false;
            }
            return Result;
        }


        public void Edit(reservation formedres, Int32 id_loc_place)
        {
            formedres.id_location_place = id_loc_place;
            formedres.id_alternative_location_place = id_loc_place;
            mp.Entry(formedres).State = EntityState.Modified;
            mp.SaveChanges();
        }

        public bool Revoke(string Describe, reservation obj, string Date)
        {
            bool result = false;
            try
            {
                //format my date have view:  string d = "21.11.14 20:00";
                long span = 0;
                place exemp = new place();
                if (Describe == "Reservation was expired" | Describe == "Reservation was revoke")
                {
                    span = Convert.ToDateTime(Date).Ticks - Convert.ToDateTime(obj.DateConnection).Ticks;
                    //Перевод места в Free состояние
                    exemp.ChangeStatus("Free", (long)obj.id_location_place);
                }
                else if (Describe == "Reservation was used")
                {
                    DateTime mydate = Convert.ToDateTime(obj.DateConnection).AddMinutes(obj.reservation_tariff.FirstFreeTimeInMinutes);
                    span = Convert.ToDateTime(Date).Ticks - mydate.Ticks;
                }
                decimal hour = 60;
                decimal priceinmin = (decimal)(obj.reservation_tariff.PriceInRubForHourHightFreeTime) / hour;
                decimal price = (decimal)TimeSpan.FromTicks(span).TotalMinutes * priceinmin;

                usr ur = mp.usr.Where(x => x.Login == obj.Login).FirstOrDefault();
                    ur.Now_Balance = ur.Now_Balance - price;
                    mp.Entry(ur).State = EntityState.Modified;
                    mp.SaveChanges();
                //Да и еще, расчеты делаются точно. Но не округлить ли баланс на выходе в лучшую сторону для автомобилиста, до копейки(сотых) хотя бы.

                    balance bl = new balance();
                    bl.Operation("Debit", price, (decimal)ur.Now_Balance, ur.Login, Describe, Date);

                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool FindOnExpired()
        {
            foreach (reservation n in mp.reservation.Where(x=>x.Status == "Active").ToList())
            {
                string Date = DateTime.Now.ToString("dd.MM.yy HH:mm");
                //Проверяю истекла ли дата брони
                if (Convert.ToDateTime(n.ApproximatelyDateOutFromActivity) < Convert.ToDateTime(Date))
                {
                    n.DateOutFromActivity = n.ApproximatelyDateOutFromActivity;
                    n.Status = "Closed";
                    n.Description = "Reservation was expired";
                    mp.Entry(n).State = EntityState.Modified;
                    mp.SaveChanges();
                    //При посещении или отказе (и если бронь не истекла) в кач-ве третьего параметра отправить текущее время,
                    //Здесь оа истекла и я отправляю предположительное, уже ранее рассчитанное при создании заявки брони.
                    reservation r = new reservation();
                    r.Revoke("Reservation was expired", n, n.ApproximatelyDateOutFromActivity);
                    //рассчитать средства и списать их со счета.
                    break;
                }
            }
            return true;
        }

    }
}
