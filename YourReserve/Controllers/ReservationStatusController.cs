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
    public class ReservationStatusController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/ReservationStatus
        public IQueryable<ReservationStatu> GetReservationStatus()
        {
            return db.ReservationStatus;
        }

        // GET: api/ReservationStatus/5
        [ResponseType(typeof(ReservationStatu))]
        public IHttpActionResult GetReservationStatu(int id)
        {
            ReservationStatu reservationStatu = db.ReservationStatus.Find(id);
            if (reservationStatu == null)
            {
                return NotFound();
            }

            return Ok(reservationStatu);
        }

        // PUT: api/ReservationStatus/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutReservationStatu(int id, ReservationStatu reservationStatu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reservationStatu.ReservationStatusID)
            {
                return BadRequest();
            }

            db.Entry(reservationStatu).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationStatuExists(id))
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

        // POST: api/ReservationStatus
        [ResponseType(typeof(ReservationStatu))]
        public IHttpActionResult PostReservationStatu(ReservationStatu reservationStatu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ReservationStatus.Add(reservationStatu);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = reservationStatu.ReservationStatusID }, reservationStatu);
        }

        // DELETE: api/ReservationStatus/5
        [ResponseType(typeof(ReservationStatu))]
        public IHttpActionResult DeleteReservationStatu(int id)
        {
            ReservationStatu reservationStatu = db.ReservationStatus.Find(id);
            if (reservationStatu == null)
            {
                return NotFound();
            }

            db.ReservationStatus.Remove(reservationStatu);
            db.SaveChanges();

            return Ok(reservationStatu);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReservationStatuExists(int id)
        {
            return db.ReservationStatus.Count(e => e.ReservationStatusID == id) > 0;
        }
    }
}