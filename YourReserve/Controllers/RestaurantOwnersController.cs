/* Description: This controller handles all CRUD Operations for RestaurantOwners table.
 * Methods: GetRestaurantOwners, PostRestaurantOwners
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
    public class RestaurantOwnersController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        // GET: api/RestaurantOwners
        public IQueryable<RestaurantOwner> GetRestaurantOwners()
        {
            return db.RestaurantOwners;
        }

        // GET: api/RestaurantOwners/5
        [ResponseType(typeof(RestaurantOwner))]
        public IHttpActionResult GetRestaurantOwner(int id)
        {
            RestaurantOwner restaurantOwner = db.RestaurantOwners.Find(id);
            if (restaurantOwner == null)
            {
                return NotFound();
            }

            return Ok(restaurantOwner);
        }

        /* Description: This method gets the RestaurantOwners by the UserName.
         * Params: string UserName,
         */
        [HttpGet]
        [Route("api/RestaurantOwners/getByUserName/{UserName}")]
        public IOrderedQueryable getByUserName(string UserName)
        {
            var query = from u in db.RestaurantOwners
                        where u.UserName == UserName
                        select u;

            return (IOrderedQueryable)query;
        }

        // PUT: api/RestaurantOwners/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurantOwner(int id, RestaurantOwner restaurantOwner)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != restaurantOwner.RestaurantOwnerID)
            {
                return BadRequest();
            }

            db.Entry(restaurantOwner).State = System.Data.Entity.EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantOwnerExists(id))
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

        // POST: api/RestaurantOwners
        [ResponseType(typeof(RestaurantOwner))]
        public IHttpActionResult PostRestaurantOwner(RestaurantOwner restaurantOwner)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Encrypt password to be entered in the database
            var oEncrypt = new EncryptDecrypt();
            oEncrypt.HashValue = "13yH5J87m3Xx8";
            oEncrypt.SaltKey = "Gh7f8JAs308f6";
            oEncrypt.VIKey = "Hj74K45yGn28A51l";
            string sEncryptedPassword = restaurantOwner.Password;

            sEncryptedPassword = oEncrypt.Encrypt(restaurantOwner.Password);
            restaurantOwner.Password = sEncryptedPassword;

            db.RestaurantOwners.Add(restaurantOwner);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = restaurantOwner.RestaurantOwnerID }, restaurantOwner);
        }

        // DELETE: api/RestaurantOwners/5
        [ResponseType(typeof(RestaurantOwner))]
        public IHttpActionResult DeleteRestaurantOwner(int id)
        {
            RestaurantOwner restaurantOwner = db.RestaurantOwners.Find(id);
            if (restaurantOwner == null)
            {
                return NotFound();
            }

            db.RestaurantOwners.Remove(restaurantOwner);
            db.SaveChanges();

            return Ok(restaurantOwner);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool RestaurantOwnerExists(int id)
        {
            return db.RestaurantOwners.Count(e => e.RestaurantOwnerID == id) > 0;
        }

        /* Description: This method gets the RestaurantID using the RestaurantOwners UserName.
         * Params: string UserName
         */
        [HttpGet]
        [Route("api/RestaurantOwners/getRestaurantID/{UserName}")]
        public int getRestaurantID(string UserName)
        {
            int returnID = 0;

            try
            {
                var query = from r in db.RestaurantOwners
                            where r.UserName == UserName
                            select r.RestaurantID;

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return (int)returnID;
        }

    }
}