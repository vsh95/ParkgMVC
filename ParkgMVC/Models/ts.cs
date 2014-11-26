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


    public class NumberAttribute : RegularExpressionAttribute
    {
        public NumberAttribute()
            : base("^(A|B|C|E|H|K|M|O|P|T|X|Y)+[0-9]{3,3}(A|B|C|E|H|K|M|O|P|T|X|Y){2,2}[0-9]{2,3}$")
        {
        }
    }

    public partial class ts : Statechart
    {
        public ts()
        {
            this.visit = new HashSet<visit>();
        }
    
        public long id_ts { get; set; }


        [Required(ErrorMessage = "Number Required")]
        [Number(ErrorMessage = "Not a valid number")]
        public string Number { get; set; }

        [Required(ErrorMessage = "Please, input company your car")]
        public string Company { get; set; }
        public string Mode { get; set; }
        public string Login { get; set; }
    
        public virtual usr usr { get; set; }
        public virtual ICollection<visit> visit { get; set; }
        MyParkingEntities mp = new MyParkingEntities();


        public bool CreateTS(ts t, string Log)
        {
            bool result = true;
            try
            {
                //Проблема в следующий двух строках после этого сообщения из-за них ничего не делает
                ts ExistForThisLogFalse = mp.ts.Where(x => x.Number == t.Number & x.Company == t.Company & x.Mode == t.Mode & x.Login == Log & x.Status == "False").FirstOrDefault();
                ts ExistTS = mp.ts.Where(x => x.Number == t.Number & x.Status == "True").FirstOrDefault();
                bool exist = true;
                /*if (NotExist.Status != "True" & NotExist.Status != "False")
                {
                    //послать сообщение что такое тс существует и используется в базе
                    result = false;
                }*/
                    if (ExistForThisLogFalse != null)
                    {
                        ExistForThisLogFalse.Status = "True";
                        mp.Entry(ExistForThisLogFalse).State = EntityState.Modified;
                            mp.SaveChanges();
                            exist = false;
                        result = true;
                    }
                 
                if (exist == true)
                {
                        if (ExistTS != null)
                        {
                            result = false;
                            //это тс в базе и кем-то используется
                        }
                        else if (ExistTS == null)
                        {
                            //добавить новую запись
                            t.Login = Log;
                            t.Status = "True";
                            mp.ts.Add(t);
                            mp.SaveChanges();
                            result = true;
                        }
                    }

            }
            catch
            {
                result = false;
            }
                return result;
        }

        public bool ChangeStatus(Int32 id, string Log)
        {
            bool result = true;
            try
            {
                var ts = mp.ts.Where(x => x.id_ts == id & x.Login == Log).ToList();
                var listts = mp.ts.Where(x => x.Login == Log).ToList();
                int i = 0;
                foreach (ts n in listts)
                {
                    i++;
                }
                reservation exist = mp.reservation.Where(x => x.Login == Log & x.Status == "Active").FirstOrDefault();
                if ((exist == null) || (exist != null & i > 1))
                {
                    foreach (ts n in ts)
                    {
                        n.Status = "False";
                        mp.Entry(n).State = EntityState.Modified;
                        mp.SaveChanges();
                        break;
                    }
                    result = true;
                }
                else if (exist != null & i == 1)
                {
                    result = false;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}
