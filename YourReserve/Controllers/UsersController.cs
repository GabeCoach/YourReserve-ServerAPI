using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using YourReserve.Models;
using YourReserve.SMTP;

namespace YourReserve.Controllers
{
    public class UsersController : ApiController
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        [Authorize]
        // GET: api/Users
        public IQueryable<User> GetUsers()
        {
            return db.Users;
        }

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            var oEncrypt = new EncryptDecrypt();

            oEncrypt.HashValue = "13yH5J87m3Xx8";
            oEncrypt.SaltKey = "Gh7f8JAs308f6";
            oEncrypt.VIKey = "Hj74K45yGn28A51l";

            user.Password = oEncrypt.Decrypt(user.Password);

            return Ok(user);
        }

        [Authorize]
        // PUT: api/Users/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.UserID)
            {
                return BadRequest();
            }

            db.Entry(user).State = System.Data.Entity.EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [ResponseType(typeof(User))]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                YourReserveSMTP SMTP = new YourReserveSMTP();

                string sPassword = user.Password;

                var oEncrypt = new EncryptDecrypt();

                oEncrypt.HashValue = "13yH5J87m3Xx8";
                oEncrypt.SaltKey = "Gh7f8JAs308f6";
                oEncrypt.VIKey = "Hj74K45yGn28A51l";

                string sEncryptedPassword;

                sEncryptedPassword = oEncrypt.Encrypt(user.Password);
                user.Password = sEncryptedPassword;

                db.Users.Add(user);
                db.SaveChanges();

                SMTP.sendUserRegisterConfirmEmail(user, sPassword);

                return CreatedAtRoute("DefaultApi", new { id = user.UserID }, user);
            }
            catch(Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }
            
        }

        // DELETE: api/Users/5
        [ResponseType(typeof(User))]
        public IHttpActionResult DeleteUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.UserID == id) > 0;
        }

        [HttpPost]
        [Route("api/Users/ForgotPassword")]
        public IHttpActionResult ForgotPassword(User user)
        {
            try
            {
                var SMTP = new YourReserveSMTP();

                var searchUserName = from u in db.Users
                                     where u.UserName == user.UserName
                                     select u;

                if (!searchUserName.Any())
                {
                    throw new Exception("UserName does not exist.");
                }
                else
                {
                    var oDecrypt = new EncryptDecrypt();

                    oDecrypt.HashValue = "13yH5J87m3Xx8";
                    oDecrypt.SaltKey = "Gh7f8JAs308f6";
                    oDecrypt.VIKey = "Hj74K45yGn28A51l";

                    var User = searchUserName.FirstOrDefault();

                   string sPassword = oDecrypt.Decrypt(User.Password);
                   User.Password = sPassword;

                    SMTP.sendForgottenPasswordEmail(User);
                }
            }
            catch(Exception ex)
            {
                return Content(HttpStatusCode.BadRequest, ex.Message);
            }

            return Ok();
        }
    }
}