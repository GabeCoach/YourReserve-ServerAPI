using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using YourReserve.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using YourReserve.SMTP;

namespace YourReserve.Controllers
{
    public class ReservationsController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();
        YourReserveSMTP smtp = new YourReserveSMTP();

        // GET: api/Reservations
        public IQueryable<Reservation> GetReservations()
        {
            return db.Reservations;
        }

        // GET: api/Reservations/5
        [ResponseType(typeof(Reservation))]
        public IHttpActionResult GetReservation(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }


        // PUT: api/Reservations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutReservation(int id, Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reservation.ReservationID)
            {
                return BadRequest();
            }

            db.Entry(reservation).State = System.Data.Entity.EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPut]
        [Route("api/Reservations/calculateDuration/{ID}")]
        public TimeSpan calculateDuration(int ID, Reservation reservation)
        {
            if(ID != reservation.ReservationID)
            {
                throw new Exception();
            }

            var startTime = reservation.StartTime.ToString();
            var endTime = reservation.EndTime.ToString();
            TimeSpan duration = DateTime.Parse(endTime).Subtract(DateTime.Parse(startTime));

            reservation.Duration = duration;

            db.Entry(reservation).State = System.Data.Entity.EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return duration;
        }

        // POST: api/Reservations
        [ResponseType(typeof(Reservation))]
        public IHttpActionResult PostReservation(Reservation reservation)
        {
            

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //int TableAssignment = assignTable(Convert.ToInt32(reservation.PartyNumber));

            //reservation.RestaurantTableConfigID = TableAssignment;

            db.Reservations.Add(reservation);
            db.SaveChanges();

            smtp.sendReservationConfirmEmail(reservation);

            return CreatedAtRoute("DefaultApi", new { id = reservation.ReservationID }, reservation);
        }


        // DELETE: api/Reservations/5
        [ResponseType(typeof(Reservation))]
        public IHttpActionResult DeleteReservation(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return NotFound();
            }

            db.Reservations.Remove(reservation);
            db.SaveChanges();

            return Ok(reservation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReservationExists(int id)
        {
            return db.Reservations.Count(e => e.ReservationID == id) > 0;
        }

        [HttpGet]
        [Route("api/Reservations/getCompletedReservations/{ID}")]
        public IOrderedQueryable getCompletedReservations(int ID)
        {
            var query = from r in db.Reservations
                        where r.ReservationStatusID == 9 && r.RestaurantID == ID
                        select r;

            return (IOrderedQueryable)query;
        }

        [HttpGet]
        [Route("api/Reservations/getReservationsByCurrentDate/{ID}")]
        public IOrderedQueryable getReservationsByCurrentDate(int ID)
        {
            var currentDate = DateTime.Now.Date;

            var reservations = db.Reservations
                .Where(res => res.RestaurantID == ID && res.ReservationStatusID != 9)
                .Select(res => res);

            return (IOrderedQueryable)reservations;
        }


        [HttpGet]
        [Route("api/Reservations/getReservationsByID/{ID}")]
        public IOrderedQueryable getReservationsByID(int ID)
        {
            var query = from r in db.Reservations
                        where r.RestaurantID == ID
                        select r;

            return (IOrderedQueryable)query;
        }


        [HttpGet]
        [Route("api/Reservations/getByID/{ID}")]
        public IOrderedQueryable getByID(int ID)
        {
            var query = from r in db.Reservations
                        where r.ReservationID == ID
                        select r;

            return (IOrderedQueryable)query;
        }


        [HttpGet]
        [Route("api/Reservations/getTotalDayReservations/{ID}")]
        public int getTotalDayReservations(int ID)
        {
            DateTime today = DateTime.Today;

            var query = (from r in db.Reservations
                         where r.ReservationDate == today && r.RestaurantID == ID
                         select r).Count();

            return (int)query;
        }


        [HttpGet]
        [Route("api/Reservations/getTotalWeekReservations/{ID}")]
        public int getTotalWeekReservations(int ID)
        {
            DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek));
            DateTime endOfWeek = DateTime.Today.AddDays(1 * (int)(DateTime.Today.DayOfWeek));

            var query = (from r in db.Reservations
                         where r.ReservationDate >= startOfWeek && r.ReservationDate < endOfWeek && r.RestaurantID == ID
                         select r).Count();

            return (int)query;
        }


        [HttpGet]
        [Route("api/Reservations/getTotalMonthReservations/{ID}")]
        public int getTotalMonthReservations(int ID)
        {
            int iMonthEnd;
            string sEndDate;
            string sStartDate;
            DateTime dtStartDate;
            DateTime dtEndDate;

            int iMonthStart = Convert.ToInt32(DateTime.Now.Month.ToString());

            if(iMonthStart == 12)
            {
                iMonthEnd = 12;
            }
            else
            {
                iMonthEnd = Convert.ToInt32(DateTime.Now.Month.ToString()) + 1;
            }

            int iYearStart = Convert.ToInt32(DateTime.Now.Year.ToString());
            int iYearEnd = Convert.ToInt32(DateTime.Now.Year.ToString());

            if (iMonthStart > 12)
            {
                iMonthEnd = 1;
                iYearEnd = iYearEnd + 1;
            }

            if(iMonthStart == 12)
            {
                sEndDate = iMonthEnd.ToString() + "/31/" + iYearEnd.ToString();
            }
            else
            {
                sEndDate = iMonthEnd.ToString() + "/01/" + iYearEnd.ToString();
            }

            sStartDate = iMonthStart.ToString() + "/01/" + iYearStart.ToString();

            dtStartDate = Convert.ToDateTime(sStartDate);
            dtEndDate = Convert.ToDateTime(sEndDate);

            var query = (from r in db.Reservations
                         where r.ReservationDate >= dtStartDate && r.ReservationDate < dtEndDate && r.RestaurantID == ID
                         select r).Count();

            return (int)query;
        }

        

        [HttpGet]
        [Route("api/Reservations/getAverageReservationsPerDay/{ID}")]
        public int getAverageReservationsPerDay(int ID)
        {

            var startDate = from r in db.Restaurants
                            select r.DateRegistered;

            var itemDates = startDate.FirstOrDefault();

            int avgReservationsPerDay = 0;

            if (itemDates != null)
            {

                var endDate = DateTime.Now;

                var numDays = ((TimeSpan)(endDate - itemDates)).TotalDays;

                var totalReservations = (from r in db.Reservations
                                         where r.RestaurantID == ID
                                         select r).Count();

                avgReservationsPerDay = totalReservations / Convert.ToInt32(numDays);
            }
            return avgReservationsPerDay;
        }


        [HttpGet]
        [Route("api/Reservations/getAverageReservationsPerWeek/{ID}")]
        public int getAverageReservationsPerWeek(int ID)
        {
            var dtRegistedDate = from r in db.Restaurants
                                 where r.RestaurantID == ID
                                 select r.DateRegistered;

            var totalReservations = (from r in db.Reservations
                                     where r.RestaurantID == ID
                                     select r).Count();

            var itemDates = dtRegistedDate.FirstOrDefault();

            int avgReservationsPerWeek = 0;

            if (itemDates != null)
            {
                var dtToday = DateTime.Now;

                var NumOfDays = ((TimeSpan)(dtToday - itemDates)).TotalDays;

                double numWeeks = NumOfDays / 7.0;

                avgReservationsPerWeek = totalReservations / Convert.ToInt32(numWeeks);
            }


            return avgReservationsPerWeek;
        }


        [HttpGet]
        [Route("api/Reservations/getAverageReservationPerMonth/{ID}")]
        public int getAverageReservationPerMonth(int ID)
        {
            var oRegistedDate = from r in db.Restaurants
                                where r.RestaurantID == ID
                                select r.DateRegistered;

            var totalReservations = (from r in db.Reservations
                                     where r.RestaurantID == ID
                                     select r).Count();

            var itemDates = oRegistedDate.FirstOrDefault();

            DateTime dtDateRegistered = Convert.ToDateTime(itemDates);

            var numOfMonths = 0;

            if (itemDates != null)
            {
                var thisMonth = DateTime.Now;

                var Span = thisMonth - dtDateRegistered;

                DateTime Age = DateTime.MinValue + Span;

                int monthDiff = Age.Month;

                numOfMonths = totalReservations / monthDiff;
            }

            return numOfMonths;
        }

        [HttpGet]
        [Route("api/Reservations/getTotalReservationsEachMonth/{ID}")]
        public IOrderedQueryable getTotalReservationsEachMonth(int ID)
        {
            var monthReservationCount = from r in db.Reservations
                                        where r.ReservationDate != null && r.RestaurantID == ID
                                        group r by r.ReservationDate.Value.Month into rDate
                                        orderby rDate.Key ascending
                                        select new { Month = rDate.Key, Cnt = rDate.Count() };

            return (IOrderedQueryable)monthReservationCount;
        }


        [HttpGet]
        [Route("api/Reservations/getPeekTimes/{ID}")]
        public IOrderedQueryable getPeekTimes(int ID)
        {

            var query = (from r in db.Reservations
                         where r.RestaurantID == ID
                         group r by r.ReservationTime into rTime
                         select rTime.Key).Take(5);

            return (IOrderedQueryable)query;
        }


        [HttpGet]
        [Route("api/Reservations/getPartyNames/{ID}")]
        public IOrderedQueryable<Reservation> getPartyNames(int ID)
        {

            var query = (from r in db.Reservations
                         where r.ReservationDate == DateTime.Now && r.RestaurantID == ID
                         select r);

            return (IOrderedQueryable<Reservation>)query;
        }


        [HttpGet]
        [Route("api/Reservations/getTopReservationPartyName/{ID}")]
        public IOrderedQueryable getTopReservationPartyName(int ID)
        {
            var query = (from customer in db.Reservations
                         where customer.RestaurantID == ID
                         group customer by new { customer.FirstName, customer.LastName } into cust
                         select new { FirstName = cust.Key.FirstName, LastName = cust.Key.LastName, Cnt = cust.Count() }).Take(10);

            return (IOrderedQueryable)query;
        }

        [HttpPost]
        [Route("api/Reservations/getAvailableTimes")]
        public List<TimeSpan> getAvailableTimes()
        {

            try
            {
                Task<string> data = Request.Content.ReadAsStringAsync();

                var sData = data.Result;

                DataModel oData = JsonConvert.DeserializeObject<DataModel>(sData);

                int RestaurantID = Convert.ToInt32(oData.RestaurantID);
                DateTime dateSelected = Convert.ToDateTime(oData.Date);
                var day = dateSelected.DayOfWeek.ToString();

                var timesOpen = from t in db.RestaurantTimeConfigs
                                where t.RestaurantID == RestaurantID && t.Day == day
                                select t;

                if (!timesOpen.Any())
                {
                    throw new Exception("Restaurant is not open on this day.");
                }

                List<TimeSpan> availableTimesList = new List<TimeSpan>();

                TimeSpan incrementTime = TimeSpan.Zero;

                foreach (var time in timesOpen)
                {
                    incrementTime = (TimeSpan)time.OpeningTime;

                    while (incrementTime < time.ClosingTime)
                    {
                        if (availableTimesList.Contains(incrementTime))
                        {
                            incrementTime = incrementTime + TimeSpan.FromMinutes(30);
                        }
                        else if (incrementTime == time.ClosingTime)
                        {
                            availableTimesList.Add(incrementTime);
                        }
                        else
                        {
                            availableTimesList.Add(incrementTime);
                            incrementTime = incrementTime + TimeSpan.FromMinutes(30);
                        }
                    }
                }

                return (List<TimeSpan>)availableTimesList;
            }
            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        //[Authorize]
        //[HttpPost]
        //[Route("api/Reservations/PrintReport")]
        //public void PrintReport()
        //{
        //    Task<string> data = Request.Content.ReadAsStringAsync();

        //    var sData = data.Result;

        //    DataModel oData = JsonConvert.DeserializeObject<DataModel>(sData);

        //    List<string> Items = new List<string>();

        //    Items.Add(oData.ToString());

        //    ReservationReport oReport = new ReservationReport();

        //    oReport.printPDF(oData.FirstName, oData.LastName, oData.Date, oData.Time, oData.PartyAmount, oData.Email);
        //}

        private int assignTable(int PartyAmount)
        {
            int ConfigNumber = 0;

            var reservation = from r in db.Reservations
                              select r;

            foreach (var res in reservation)
            {
                var TableQuery = from b in db.RestaurantTableConfigs
                                 where b.NumberOfSeats >= PartyAmount && b.TableConfigID != res.RestaurantTableConfigID
                                 orderby b.NumberOfSeats ascending
                                 select b.TableConfigID;

                ConfigNumber = TableQuery.FirstOrDefault();
            }
            return ConfigNumber;
        }


        [HttpPost]
        [Route("api/Reservations/getTableAssignments/{ID}")]
        public IOrderedQueryable getTableAssignments(int ID)
        {
            Task<string> data = Request.Content.ReadAsStringAsync();
            string sData = data.Result;
            DataModel oData = JsonConvert.DeserializeObject<DataModel>(sData);

            TimeSpan tsTime = TimeSpan.Parse(oData.Time);
            DateTime dtDate = Convert.ToDateTime(oData.Date);

            var getAssignments = from r in db.Reservations
                                 where r.ReservationTime == tsTime && r.ReservationDate == dtDate && r.RestaurantID == ID
                                 join d in db.RestaurantTableConfigs on r.RestaurantTableConfigID equals d.TableConfigID
                                 select new { Name = r.FirstName + " " + r.LastName, TableNumber = d.TableNumber, Time = r.ReservationTime };

            return (IOrderedQueryable)getAssignments;
        }


        [HttpGet]
        [Route("api/Reservations/getCustomerReservations/{UserID}")]
        public IOrderedQueryable getCustomerReservations(int UserID)
        {
            var query = from r in db.Reservations
                        where r.UserID == UserID
                        join t in db.Restaurants on r.RestaurantID equals t.RestaurantID
                        select new { ReservationID = r.ReservationID, Restaurant = t.RestaurantName, Date = r.ReservationDate, Time = r.ReservationTime, PartyAmount = r.PartyNumber };

            return (IOrderedQueryable)query;
        }

        [HttpGet]
        [Route("api/Reservations/getUpComingReservations/{RestID}")]
        public IOrderedQueryable getUpComingReservations(int RestID)
        {
            var query = from r in db.Reservations
                        where r.RestaurantID == RestID && r.ReservationStatusID != 9 && r.ReservationDate > DateTime.Now
                        select r;

            return (IOrderedQueryable)query;
        }
    }
}

public class DataModel
{
    public string Date { get; set; }
    public string Time { get; set; }
    public string PartyAmount { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string RestaurantID { get; set; }
}