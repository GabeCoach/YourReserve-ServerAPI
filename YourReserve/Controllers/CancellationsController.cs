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

namespace YourReserve.Controllers
{
    public class CancellationsController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/Cancellations
        public IQueryable<Cancellation> GetCancellations()
        {
            return db.Cancellations;
        }

        // GET: api/Cancellations/5
        [ResponseType(typeof(Cancellation))]
        public IHttpActionResult GetCancellation(int id)
        {
            Cancellation cancellation = db.Cancellations.Find(id);
            if (cancellation == null)
            {
                return NotFound();
            }

            return Ok(cancellation);
        }

        // PUT: api/Cancellations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCancellation(int id, Cancellation cancellation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cancellation.CancellationID)
            {
                return BadRequest();
            }

            db.Entry(cancellation).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CancellationExists(id))
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

        // POST: api/Cancellations
        [ResponseType(typeof(Cancellation))]
        public IHttpActionResult PostCancellation(Cancellation cancellation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Cancellations.Add(cancellation);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = cancellation.CancellationID }, cancellation);
        }

        // DELETE: api/Cancellations/5
        [ResponseType(typeof(Cancellation))]
        public IHttpActionResult DeleteCancellation(int id)
        {
            Cancellation cancellation = db.Cancellations.Find(id);
            if (cancellation == null)
            {
                return NotFound();
            }

            db.Cancellations.Remove(cancellation);
            db.SaveChanges();

            return Ok(cancellation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CancellationExists(int id)
        {
            return db.Cancellations.Count(e => e.CancellationID == id) > 0;
        }

        [HttpGet]
        [Route("api/Cancellations/getRestCancellations/{RestID}")]
        public IOrderedQueryable getRestCancellations(int RestID)
        {
            var query = from c in db.CancellationsViews
                        where c.RestaurantID == RestID
                        select c;

            return (IOrderedQueryable)query;
        }

        [HttpGet]
        [Route("api/Cancellations/getAverageCancellationsPerDay/{ID}")]
        public int getAverageCancellationsPerDay(int ID)
        {
            var startDate = from r in db.Restaurants
                            select r.DateRegistered;

            var itemDates = startDate.FirstOrDefault();

            int avgReservationsPerDay = 0;

            if (itemDates != null)
            {

                var endDate = DateTime.Now;

                var numDays = ((TimeSpan)(endDate - itemDates)).TotalDays;

                var totalCancellations = (from c in db.Cancellations
                                          join r in db.Reservations on c.ReservationID equals r.ReservationID
                                          where r.RestaurantID == ID
                                          select r).Count();

                avgReservationsPerDay = totalCancellations / Convert.ToInt32(numDays);
            }
            return avgReservationsPerDay;
        }

        [HttpGet]
        [Route("api/Cancellations/getAverageCancellationsPerWeek/{ID}")]
        public int getAverageCancellationsPerWeek(int ID)
        {
            var dtRegistedDate = from r in db.Restaurants
                                 where r.RestaurantID == ID
                                 select r.DateRegistered;

            var totalCancellations = (from c in db.Cancellations
                                      join r in db.Reservations on c.ReservationID equals r.ReservationID
                                      where r.RestaurantID == ID
                                      select r).Count();

            var itemDates = dtRegistedDate.FirstOrDefault();

            int avgReservationsPerWeek = 0;

            if (itemDates != null)
            {
                var dtToday = DateTime.Now;

                var NumOfDays = ((TimeSpan)(dtToday - itemDates)).TotalDays;

                double numWeeks = NumOfDays / 7.0;

                avgReservationsPerWeek = totalCancellations / Convert.ToInt32(numWeeks);
            }


            return avgReservationsPerWeek;
        }

        [HttpGet]
        [Route("api/Cancellations/getAverageCancellationsPerMonth/{ID}")]
        public int getAverageCancellationsPerMonth(int ID)
        {
            var oRegistedDate = from r in db.Restaurants
                                where r.RestaurantID == ID
                                select r.DateRegistered;

            var totalCancellations = (from c in db.Cancellations
                                      join r in db.Reservations on c.ReservationID equals r.ReservationID
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

                numOfMonths = totalCancellations / monthDiff;
            }

            return numOfMonths;
        }
    }
}