/* Description: This controller handles all CRUD Operations for Restaurant Table Bookings table.
 * Methods: GetRestaurantTableBookings
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
    public class RestaurantTableBookingsController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/RestaurantTableBookings
        public IQueryable<RestaurantTableBooking> GetRestaurantTableBookings()
        {
            return db.RestaurantTableBookings;
        }

        // GET: api/RestaurantTableBookings/5
        [ResponseType(typeof(RestaurantTableBooking))]
        public IHttpActionResult GetRestaurantTableBooking(int id)
        {
            RestaurantTableBooking restaurantTableBooking = db.RestaurantTableBookings.Find(id);
            if (restaurantTableBooking == null)
            {
                return NotFound();
            }

            return Ok(restaurantTableBooking);
        }

        // PUT: api/RestaurantTableBookings/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurantTableBooking(int id, RestaurantTableBooking restaurantTableBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurantTableBooking.TableBookingID)
            {
                return BadRequest();
            }

            db.Entry(restaurantTableBooking).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantTableBookingExists(id))
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

        // POST: api/RestaurantTableBookings
        [ResponseType(typeof(RestaurantTableBooking))]
        public IHttpActionResult PostRestaurantTableBooking(RestaurantTableBooking restaurantTableBooking)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RestaurantTableBookings.Add(restaurantTableBooking);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RestaurantTableBookingExists(restaurantTableBooking.TableBookingID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = restaurantTableBooking.TableBookingID }, restaurantTableBooking);
        }

        // DELETE: api/RestaurantTableBookings/5
        [ResponseType(typeof(RestaurantTableBooking))]
        public IHttpActionResult DeleteRestaurantTableBooking(int id)
        {
            RestaurantTableBooking restaurantTableBooking = db.RestaurantTableBookings.Find(id);
            if (restaurantTableBooking == null)
            {
                return NotFound();
            }

            db.RestaurantTableBookings.Remove(restaurantTableBooking);
            db.SaveChanges();

            return Ok(restaurantTableBooking);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantTableBookingExists(int id)
        {
            return db.RestaurantTableBookings.Count(e => e.TableBookingID == id) > 0;
        }
    }
}