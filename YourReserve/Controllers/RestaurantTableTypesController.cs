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
    public class RestaurantTableTypesController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/RestaurantTableTypes
        public IQueryable<RestaurantTableType> GetRestaurantTableTypes()
        {
            return db.RestaurantTableTypes;
        }

        // GET: api/RestaurantTableTypes/5
        [ResponseType(typeof(RestaurantTableType))]
        public IHttpActionResult GetRestaurantTableType(int id)
        {
            RestaurantTableType restaurantTableType = db.RestaurantTableTypes.Find(id);
            if (restaurantTableType == null)
            {
                return NotFound();
            }

            return Ok(restaurantTableType);
        }

        // PUT: api/RestaurantTableTypes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurantTableType(int id, RestaurantTableType restaurantTableType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurantTableType.TableTypeID)
            {
                return BadRequest();
            }

            db.Entry(restaurantTableType).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantTableTypeExists(id))
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

        // POST: api/RestaurantTableTypes
        [ResponseType(typeof(RestaurantTableType))]
        public IHttpActionResult PostRestaurantTableType(RestaurantTableType restaurantTableType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.RestaurantTableTypes.Add(restaurantTableType);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (RestaurantTableTypeExists(restaurantTableType.TableTypeID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = restaurantTableType.TableTypeID }, restaurantTableType);
        }

        // DELETE: api/RestaurantTableTypes/5
        [ResponseType(typeof(RestaurantTableType))]
        public IHttpActionResult DeleteRestaurantTableType(int id)
        {
            RestaurantTableType restaurantTableType = db.RestaurantTableTypes.Find(id);
            if (restaurantTableType == null)
            {
                return NotFound();
            }

            db.RestaurantTableTypes.Remove(restaurantTableType);
            db.SaveChanges();

            return Ok(restaurantTableType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantTableTypeExists(int id)
        {
            return db.RestaurantTableTypes.Count(e => e.TableTypeID == id) > 0;
        }
    }
}