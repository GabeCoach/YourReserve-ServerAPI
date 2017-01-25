/* Description: This controller handles all CRUD Operations for RestaurantLocations table.
 * Methods: GetRestaurantLocations, PostRestaurantLocations
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
    public class RestaurantLocationsController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/RestaurantLocations
        public IQueryable<RestaurantLocation> GetRestaurantLocations()
        {
            return db.RestaurantLocations;
        }

        // GET: api/RestaurantLocations/5
        [ResponseType(typeof(RestaurantLocation))]
        public IHttpActionResult GetRestaurantLocation(int id)
        {
            RestaurantLocation restaurantLocation = db.RestaurantLocations.Find(id);
            if (restaurantLocation == null)
            {
                return NotFound();
            }

            return Ok(restaurantLocation);
        }

        // PUT: api/RestaurantLocations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurantLocation(int id, RestaurantLocation restaurantLocation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurantLocation.RestaurantLocationID)
            {
                return BadRequest();
            }

            db.Entry(restaurantLocation).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantLocationExists(id))
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

        // POST: api/RestaurantLocations
        [ResponseType(typeof(RestaurantLocation))]
        public IHttpActionResult PostRestaurantLocation(RestaurantLocation restaurantLocation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RestaurantLocations.Add(restaurantLocation);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RestaurantLocationExists(restaurantLocation.RestaurantLocationID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = restaurantLocation.RestaurantLocationID }, restaurantLocation);
        }

        // DELETE: api/RestaurantLocations/5
        [ResponseType(typeof(RestaurantLocation))]
        public IHttpActionResult DeleteRestaurantLocation(int id)
        {
            RestaurantLocation restaurantLocation = db.RestaurantLocations.Find(id);
            if (restaurantLocation == null)
            {
                return NotFound();
            }

            db.RestaurantLocations.Remove(restaurantLocation);
            db.SaveChanges();

            return Ok(restaurantLocation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantLocationExists(int id)
        {
            return db.RestaurantLocations.Count(e => e.RestaurantLocationID == id) > 0;
        }
    }
}