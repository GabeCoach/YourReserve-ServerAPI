/* Description: This controller handles all CRUD Operations for ReservationStatus table.
 * Methods: GetRestaurantReservationStatus, PostRestaurantReservationStatus
 * Author: Gabriel Coach 
 * Email: gsctca@gmail.com
 */
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
    public class RestaurantReservationStatusController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/RestaurantReservationStatus
        public IQueryable<RestaurantReservationStatu> GetRestaurantReservationStatus()
        {
            return db.RestaurantReservationStatus;
        }

        // GET: api/RestaurantReservationStatus/5
        [ResponseType(typeof(RestaurantReservationStatu))]
        public IHttpActionResult GetRestaurantReservationStatu(int id)
        {
            RestaurantReservationStatu restaurantReservationStatu = db.RestaurantReservationStatus.Find(id);
            if (restaurantReservationStatu == null)
            {
                return NotFound();
            }

            return Ok(restaurantReservationStatu);
        }

        // PUT: api/RestaurantReservationStatus/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurantReservationStatu(int id, RestaurantReservationStatu restaurantReservationStatu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurantReservationStatu.ReservationStatusID)
            {
                return BadRequest();
            }

            db.Entry(restaurantReservationStatu).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantReservationStatuExists(id))
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

        // POST: api/RestaurantReservationStatus
        [ResponseType(typeof(RestaurantReservationStatu))]
        public IHttpActionResult PostRestaurantReservationStatu(RestaurantReservationStatu restaurantReservationStatu)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RestaurantReservationStatus.Add(restaurantReservationStatu);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RestaurantReservationStatuExists(restaurantReservationStatu.ReservationStatusID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = restaurantReservationStatu.ReservationStatusID }, restaurantReservationStatu);
        }

        // DELETE: api/RestaurantReservationStatus/5
        [ResponseType(typeof(RestaurantReservationStatu))]
        public IHttpActionResult DeleteRestaurantReservationStatu(int id)
        {
            RestaurantReservationStatu restaurantReservationStatu = db.RestaurantReservationStatus.Find(id);
            if (restaurantReservationStatu == null)
            {
                return NotFound();
            }

            db.RestaurantReservationStatus.Remove(restaurantReservationStatu);
            db.SaveChanges();

            return Ok(restaurantReservationStatu);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantReservationStatuExists(int id)
        {
            return db.RestaurantReservationStatus.Count(e => e.ReservationStatusID == id) > 0;
        }
    }
}