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
    public class RestaurantTimeConfigsController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/RestaurantTimeConfigs
        public IQueryable<RestaurantTimeConfig> GetRestaurantTimeConfigs()
        {
            return db.RestaurantTimeConfigs;
        }

        // GET: api/RestaurantTimeConfigs/5

        [ResponseType(typeof(RestaurantTimeConfig))]
        public IHttpActionResult GetRestaurantTimeConfig(int id)
        {
            RestaurantTimeConfig restaurantTimeConfig = db.RestaurantTimeConfigs.Find(id);
            if (restaurantTimeConfig == null)
            {
                return NotFound();
            }

            return Ok(restaurantTimeConfig);
        }

        // PUT: api/RestaurantTimeConfigs/5

        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurantTimeConfig(int id, RestaurantTimeConfig restaurantTimeConfig)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurantTimeConfig.TimeConfigurationID)
            {
                return BadRequest();
            }

            db.Entry(restaurantTimeConfig).State = System.Data.Entity.EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantTimeConfigExists(id))
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

        // POST: api/RestaurantTimeConfigs

        [ResponseType(typeof(RestaurantTimeConfig))]
        public IHttpActionResult PostRestaurantTimeConfig(RestaurantTimeConfig restaurantTimeConfig)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var query = from t in db.RestaurantTimeConfigs
                        where t.RestaurantID == restaurantTimeConfig.RestaurantID
                        select new { Day = t.Day, OpeningTime = t.OpeningTime, ClosingTime = t.ClosingTime };

            bool isValid = false;

            try
            {
                if (!query.Any())
                {
                    db.RestaurantTimeConfigs.Add(restaurantTimeConfig);
                    db.SaveChanges();
                }
                else
                {
                    foreach (var Restraint in query)
                    {
                        if (Restraint.Day == restaurantTimeConfig.Day && Restraint.OpeningTime == restaurantTimeConfig.OpeningTime && Restraint.ClosingTime == restaurantTimeConfig.ClosingTime)
                        {
                            throw new Exception("Opening and Closing Times Can't Overlap");
                        }
                        else
                        {
                            isValid = true;
                        }
                    }

                    if (isValid == true)
                    {
                        db.RestaurantTimeConfigs.Add(restaurantTimeConfig);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }


            return CreatedAtRoute("DefaultApi", new { id = restaurantTimeConfig.TimeConfigurationID }, restaurantTimeConfig);
        }

        // DELETE: api/RestaurantTimeConfigs/5
  
        [ResponseType(typeof(RestaurantTimeConfig))]
        public IHttpActionResult DeleteRestaurantTimeConfig(int id)
        {
            RestaurantTimeConfig restaurantTimeConfig = db.RestaurantTimeConfigs.Find(id);
            if (restaurantTimeConfig == null)
            {
                return NotFound();
            }

            db.RestaurantTimeConfigs.Remove(restaurantTimeConfig);
            db.SaveChanges();

            return Ok(restaurantTimeConfig);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantTimeConfigExists(int id)
        {
            return db.RestaurantTimeConfigs.Count(e => e.TimeConfigurationID == id) > 0;
        }

      
        [HttpGet]
        [Route("api/RestaurantTimeConfigs/getTimeConfigs/{ID}")]
        public IOrderedQueryable getTimeConfigs(int ID)
        {
            var query = from t in db.RestaurantTimeConfigs
                        where t.RestaurantID == ID
                        select t;

            return (IOrderedQueryable)query;
        }
    }
}