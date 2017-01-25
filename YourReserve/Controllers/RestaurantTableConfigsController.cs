/* Description: This controller handles all CRUD Operations for Restaurants table.
 * Methods: SearchRestaurant, registerRestaurant
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
    public class RestaurantTableConfigsController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/RestaurantTableConfigs
 
        public IQueryable<RestaurantTableConfig> GetRestaurantTableConfigs()
        {
            return db.RestaurantTableConfigs;
        }

        // GET: api/RestaurantTableConfigs/5

        [ResponseType(typeof(RestaurantTableConfig))]
        public IHttpActionResult GetRestaurantTableConfig(int id)
        {
            RestaurantTableConfig restaurantTableConfig = db.RestaurantTableConfigs.Find(id);
            if (restaurantTableConfig == null)
            {
                return NotFound();
            }

            return Ok(restaurantTableConfig);
        }

        // PUT: api/RestaurantTableConfigs/5

        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurantTableConfig(int id, RestaurantTableConfig restaurantTableConfig)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurantTableConfig.TableConfigID)
            {
                return BadRequest();
            }

            db.Entry(restaurantTableConfig).State = System.Data.Entity.EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantTableConfigExists(id))
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

        // POST: api/RestaurantTableConfigs

        [ResponseType(typeof(RestaurantTableConfig))]
        public IHttpActionResult PostRestaurantTableConfig(RestaurantTableConfig restaurantTableConfig)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //query table configurations
            var query = from t in db.RestaurantTableConfigs
                        where t.RestaurantID == restaurantTableConfig.RestaurantID
                        select new { ConfigID = t.TableConfigID, TableNumber = t.TableNumber };

            bool isValid = false;

            try
            {
                //determine if table has been created
                if (!query.Any())
                {
                    db.RestaurantTableConfigs.Add(restaurantTableConfig);
                    db.SaveChanges();
                }
                else
                {
                    foreach (var restraint in query)
                    {
                        if (restraint.TableNumber != restaurantTableConfig.TableNumber)
                        {
                            isValid = true;
                        }
                        else
                        {
                            isValid = false;
                            throw new Exception("Table Number has already been created");
                        }
                    }

                    if (isValid == true)
                    {
                        db.RestaurantTableConfigs.Add(restaurantTableConfig);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }


            return CreatedAtRoute("DefaultApi", new { id = restaurantTableConfig.TableConfigID }, restaurantTableConfig);
        }

        // DELETE: api/RestaurantTableConfigs/5

        [ResponseType(typeof(RestaurantTableConfig))]
        public IHttpActionResult DeleteRestaurantTableConfig(int id)
        {
            RestaurantTableConfig restaurantTableConfig = db.RestaurantTableConfigs.Find(id);
            if (restaurantTableConfig == null)
            {
                return NotFound();
            }

            db.RestaurantTableConfigs.Remove(restaurantTableConfig);
            db.SaveChanges();

            return Ok(restaurantTableConfig);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantTableConfigExists(int id)
        {
            return db.RestaurantTableConfigs.Count(e => e.TableConfigID == id) > 0;
        }

        /* Description: This method queries a table configuration.
         * Params: int ID
         */
        [HttpGet]
        [Route("api/RestaurantTableConfigs/getTableConfiguration/{ID}")]
        public IOrderedQueryable getTableConfiguration(int ID)
        {
            var query = from t in db.RestaurantTableConfigs
                        join type in db.RestaurantTableTypes on t.TableTypeID equals type.TableTypeID
                        select new { TableNumber = t.TableNumber, NumSeats = t.NumberOfSeats, TableType = type.TableType };

            return (IOrderedQueryable)query;
        }
    }
}