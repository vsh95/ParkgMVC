﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParkgMVC.Models;
using System.Data;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ParkgMVC.Controllers
{
    //[HandleError(View = "~/Views/Shared/Error.aspx")]
    public class ResController : Controller
    {
        //
        // GET: /Res/
        MyParkingEntities mp = new MyParkingEntities();
        reservation r = new reservation();
        public ActionResult Agreement()
        {
            if (User.Identity.IsAuthenticated)
            {
                string Log = User.Identity.Name.ToString();
                reservation activeres = mp.reservation.Where(x => x.Login == Log & x.Status == "Active").FirstOrDefault();
                reservation formed = mp.reservation.Where(x => x.Login == Log & x.Status == "Formed").FirstOrDefault();
                if (activeres == null)
                {
                    if (formed != null)
                    {
                        reservation_tariff check = mp.reservation_tariff.Where(x => x.id_Reservation_Tariff == formed.id_Reservation_Tariff & x.Status == "available").FirstOrDefault();
                        if (check == null)
                        {
                            ViewData["NewTariff"] = "Ранее подтвержденный Вами на использование тариф закончил свое действие, пожалуйста, ознакомьтесь с условиями для нового тарифа";
                        }
                    }
                }
                else
                {
                    ViewData["NewTariff"] = "Вы уже имеете активное забронированное месторасположение!";
                }
                return View(mp.reservation_tariff.Where(x => x.Status == "available").ToList());
            }
            else
            {
                return RedirectToAction("LogOn", new { Controller = "Account" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [MultiButton(MatchFormKey = "Agreement", MatchFormValue = "Continue_Formed")]
        public ActionResult Continue_Formed(Int32 id_Reservation_Tariff)
        {
            if (User.Identity.IsAuthenticated)
            {
                reservation_tariff check = mp.reservation_tariff.Where(x => x.id_Reservation_Tariff == id_Reservation_Tariff & x.Status == "available").FirstOrDefault();
                if (check != null)
                {
                    return RedirectToAction("ZonesLevelsPlaces", new { Controller = "Home", value = "RESERVATION" });
                }
                else if (check == null)
                {
                    ViewData["NewTariff"] = "Ранее подтвержденный Вами на использование тариф закончил свое действие, пожалуйста, ознакомьтесь с условиями для нового тарифа";
                    return View(mp.reservation_tariff.Where(x => x.Status == "available").ToList());
                }
                return View(mp.reservation_tariff.Where(x => x.Status == "available").ToList());
            }
            else
            {
                return RedirectToAction("LogOn", new { Controller = "Account" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [MultiButton(MatchFormKey = "Agreement", MatchFormValue = "Next_step_formed")]
        public ActionResult Next_step_formed(Int32 id_Reservation_Tariff)
        {
            if (User.Identity.IsAuthenticated)
            {
                reservation_tariff check = mp.reservation_tariff.Where(x => x.Status == "available").FirstOrDefault();
                
                        string Log = User.Identity.Name.ToString();
                        reservation activeres = mp.reservation.Where(x => x.Login == Log & x.Status == "Active").FirstOrDefault();
                        reservation formed = mp.reservation.Where(x => x.Login == Log & x.Status == "Formed").FirstOrDefault();
                        if (activeres == null)
                        {
                            if (formed == null)
                            {
                                reservation r = new reservation();
                                r.CreateReservation("Formed", Log, 0, "Reservation wait full formed", check);
                            }
                            else if (formed != null)
                            {
                                formed.id_Reservation_Tariff = id_Reservation_Tariff;
                                mp.Entry(formed).State = EntityState.Modified;
                                mp.SaveChanges();
                                return RedirectToAction("Reservation");
                            }

                        }
                        else if (activeres != null)
                        {
                            ViewData["NewTariff"] = "Вы уже имеете активное забронированное месторасположение!";
                            return View(mp.reservation_tariff.Where(x => x.Status == "available").ToList());
                        }
                    //return RedirectToAction("ZonesLevelsPlaces", new { Controller = "Home", value = "RESERVATION" });
                
                return RedirectToAction("ZonesLevelsPlaces", new { Controller = "Home", value = "RESERVATION" });
            }
            else
            {
                return RedirectToAction("LogOn", new { Controller = "Account" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [MultiButton(MatchFormKey = "Reservation", MatchFormValue = "Connect")]
        public ActionResult Connect()
        {
            if (User.Identity.IsAuthenticated)
            {
                string Log = User.Identity.Name.ToString();
                string Date = DateTime.Now.ToString("dd.MM.yy HH:mm");
                reservation formedres = mp.reservation.Where(x => x.Login == Log & x.Status == "Formed").FirstOrDefault();
                    ts exist = mp.ts.Where(x => x.Login == Log & x.Status == "True").FirstOrDefault();
                    if (exist != null)
                    {
                        reservation_tariff tar = mp.reservation_tariff.Where(x => x.Status == "available" & x.id_Reservation_Tariff == formedres.id_Reservation_Tariff).FirstOrDefault();
                        if (tar != null)
                        {

                            usr us = mp.usr.Where(x => x.Login == Log).FirstOrDefault();
                            if (formedres.id_location_place != null)
                            {
                                place free = mp.place.Where(x => x.id_location_place == formedres.id_location_place & x.Status == "Free").FirstOrDefault();
                                if (us != null & free != null)
                                {
                                    if (us.Now_Balance >= 0)
                                    {
                                        formedres.Status = "Active";
                                        formedres.DateConnection = Date;
                                        DateTime mydate = Convert.ToDateTime(Date).AddHours(tar.ValidityPeriodFromTheTimeOfActivationInHour);//Согласно активному тарифу
                                        formedres.ApproximatelyDateOutFromActivity = Convert.ToString(mydate);
                                        mp.Entry(formedres).State = EntityState.Modified;
                                        mp.SaveChanges();
                                        place newplace = new place();
                                        newplace.ChangeStatus("In waiting visit", (long)formedres.id_location_place);
                                    }
                                    else if (us.Now_Balance < 0)
                                    {
                                        ViewData["AnswerFromReservation"] = "У Вас отрицательный баланс, активирование бронирования запрещено!";
                                    }
                                }

                                else if (free == null)
                                {
                                    ViewData["AnswerFromReservation"] = "Месторасположение занято!";
                                }
                            }
                            else if (formedres.id_location_place == null)
                            {
                                ViewData["AnswerFromReservation"] = "Месторасположение не задано!";
                            }
                        }
                        else
                        {
                            ViewData["AnswerFromReservation"] = "Данный тариф бронирования не активен. Пожалуйста, переформируйте заявку!";
                        }

                    }

                    else
                    {
                        ViewData["AnswerFromReservation"] = "Активирование бронирование без наличия ТС запрещено. Формируемая вами бронь сохранена, вы сможете подключить ее после добавления вашего ТС!";
                    }
                
                ViewData["ReservationTariff"] = mp.reservation_tariff.Where(x => x.Status == "available").ToList();
                return View(mp.reservation.Where(x => x.Login == Log & (x.Status == "Formed" || x.Status == "Active")).ToList());
            }
            else
            {
                return RedirectToAction("LogOn", new { Controller = "Account" });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [MultiButton(MatchFormKey = "Reservation", MatchFormValue = "Revoke")]
        public ActionResult Revoke(Int32 id_reservation_user)
        {
            if (User.Identity.IsAuthenticated)
            {
                string Date = DateTime.Now.ToString("dd.MM.yy HH:mm");
                string Log = User.Identity.Name.ToString();
                try
                {
                    reservation n = mp.reservation.Where(x => x.id_reservation_user == id_reservation_user).FirstOrDefault();
                    if (n.Status != "Formed")
                    {
                        //Проверяю истекла ли дата брони
                        if (Convert.ToDateTime(n.ApproximatelyDateOutFromActivity) < Convert.ToDateTime(Date))
                        {
                            n.DateOutFromActivity = n.ApproximatelyDateOutFromActivity;
                            n.Status = "Expired";
                            mp.Entry(n).State = EntityState.Modified;
                            mp.SaveChanges();
                            //При посещении или отказе (и если бронь не истекла) в кач-ве третьего параметра отправить текущее время,
                            //Здесь оа истекла и я отправляю предположительное, уже ранее рассчитанное при создании заявки брони.
                            r.Revoke("Reservation expired", n, n.ApproximatelyDateOutFromActivity);

                            //рассчитать средства и списать их со счета.
                        }
                        else if (Convert.ToDateTime(n.ApproximatelyDateOutFromActivity) >= Convert.ToDateTime(Date))
                        {
                            n.DateOutFromActivity = Date;
                            n.Status = "Revoke";
                            mp.Entry(n).State = EntityState.Modified;
                            mp.SaveChanges();
                            //При посещении или отказе (и если бронь не истекла) в кач-ве третьего параметра отправить текущее время,
                            //Здесь оа истекла и я отправляю предположительное, уже ранее рассчитанное при создании заявки брони.
                            reservation n1 = mp.reservation.Where(x => x.id_reservation_user == id_reservation_user).FirstOrDefault();
                            r.Revoke("Revoke", n1, Date);
                            //рассчитать средства и списать их со счета.
                        }
                    }
                    else if (n.Status == "Formed")
                    {
                        mp.reservation.Remove(n);
                        mp.SaveChanges();
                    }

                    ViewData["ReservationTariff"] = mp.reservation_tariff.Where(x => x.Status == "available").ToList();
                    return View(mp.reservation.Where(x => x.Login == Log & (x.Status == "Formed" || x.Status == "Active")).ToList());
                }
                catch
                {
                    ViewData["AnswerFromReservation"] = id_reservation_user;
                    ViewData["ReservationTariff"] = mp.reservation_tariff.Where(x => x.Status == "available").ToList();
                    return View(mp.reservation.Where(x => x.Login == Log & (x.Status == "Formed" || x.Status == "Active")).ToList());
                }
            }
            else
            {
                return RedirectToAction("LogOn", new { Controller = "Account" });
            }
            

        }
        public ActionResult Reservation()
        {
            if (User.Identity.IsAuthenticated)
            {
                string Date = DateTime.Now.ToString("dd.MM.yy HH:mm");
                string Log = User.Identity.Name.ToString();
                foreach (reservation n in mp.reservation.Where(x => x.Login == Log & x.Status == "Active").ToList())
                {
                    //Проверяю истекла ли дата брони
                    if (Convert.ToDateTime(n.ApproximatelyDateOutFromActivity) < Convert.ToDateTime(Date))
                    {
                        n.DateOutFromActivity = n.ApproximatelyDateOutFromActivity;
                        n.Status = "Expired";
                        mp.Entry(n).State = EntityState.Modified;
                        mp.SaveChanges();
                        //При посещении или отказе (и если бронь не истекла) в кач-ве третьего параметра отправить текущее время,
                        //Здесь оа истекла и я отправляю предположительное, уже ранее рассчитанное при создании заявки брони.
                        r.Revoke("Reservation expired", n, n.ApproximatelyDateOutFromActivity);
                        //рассчитать средства и списать их со счета.
                        break;
                    }
                }
                ViewData["ReservationTariff"] = mp.reservation_tariff.Where(x => x.Status == "available").ToList();
                return View(mp.reservation.Where(x => x.Login == Log & (x.Status == "Formed" || x.Status == "Active")).ToList());
            }

            else
            {
                return RedirectToAction("LogOn", new { Controller = "Account" });
            }

        }
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MultiButtonAttribute : ActionNameSelectorAttribute
    {
        public string MatchFormKey { get; set; }
        public string MatchFormValue { get; set; }
        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            return controllerContext.HttpContext.Request[MatchFormKey] != null &&
                controllerContext.HttpContext.Request[MatchFormKey] == MatchFormValue;
        }
    }
}