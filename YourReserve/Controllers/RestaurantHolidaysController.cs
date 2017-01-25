/* Description: This controller handles all CRUD Operations for RestaurantHolidays table.
 * Methods: GetRestaurantHolidays, PostRestaurantHolidays
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
    public class RestaurantHolidaysController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/RestaurantHolidays
        public IQueryable<RestaurantHoliday> GetRestaurantHolidays()
        {
            return db.RestaurantHolidays;
        }

        // GET: api/RestaurantHolidays/5
        [ResponseType(typeof(RestaurantHoliday))]
        public IHttpActionResult GetRestaurantHoliday(int id)
        {
            RestaurantHoliday restaurantHoliday = db.RestaurantHolidays.Find(id);
            if (restaurantHoliday == null)
            {
                return NotFound();
            }

            return Ok(restaurantHoliday);
        }

        // PUT: api/RestaurantHolidays/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurantHoliday(int id, RestaurantHoliday restaurantHoliday)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurantHoliday.RestaurantHolidayID)
            {
                return BadRequest();
            }

            db.Entry(restaurantHoliday).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantHolidayExists(id))
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

        // POST: api/RestaurantHolidays
        [ResponseType(typeof(RestaurantHoliday))]
        public IHttpActionResult PostRestaurantHoliday(RestaurantHoliday restaurantHoliday)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RestaurantHolidays.Add(restaurantHoliday);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RestaurantHolidayExists(restaurantHoliday.RestaurantHolidayID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = restaurantHoliday.RestaurantHolidayID }, restaurantHoliday);
        }

        // DELETE: api/RestaurantHolidays/5
        [ResponseType(typeof(RestaurantHoliday))]
        public IHttpActionResult DeleteRestaurantHoliday(int id)
        {
            RestaurantHoliday restaurantHoliday = db.RestaurantHolidays.Find(id);
            if (restaurantHoliday == null)
            {
                return NotFound();
            }

            db.RestaurantHolidays.Remove(restaurantHoliday);
            db.SaveChanges();

            return Ok(restaurantHoliday);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantHolidayExists(int id)
        {
            return db.RestaurantHolidays.Count(e => e.RestaurantHolidayID == id) > 0;
        }
    }
}