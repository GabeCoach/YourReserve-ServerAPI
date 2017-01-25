/* Description: This class handles all of the SMTP functionality
 * Methods: sendRestConfirmationEmail, sendUserRegisterConfirmEmail
 * Author: Gabriel Coach 
 * Email: gsctca@gmail.com
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YourReserve.Models;
using System.Net;
using System.Net.Mail;

namespace YourReserve.SMTP
{
    public class YourReserveSMTP
    {
        private DB_9D2D33_YourReserveDBEntities db = new DB_9D2D33_YourReserveDBEntities();

        /* Description: This method builds and sends a confirmation to the registered restaurant
         * Params: RestaurantOwner Object
         */
        public void sendRestConfirmationEmail(RestaurantOwner oRestaurantOwner)
        {
            string firstName = oRestaurantOwner.FirstName.ToString();
            string lastName = oRestaurantOwner.LastName.ToString();
            string userName = oRestaurantOwner.UserName.ToString();
            string password = oRestaurantOwner.Password.ToString();

            string to = oRestaurantOwner.Email.ToString();
            string from = "info@entellication.com";
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Welcome To YourReserve";
            message.Body = @"Hello " + firstName + " " + lastName + " Welcome to the YourReserve Restaurant Reservation Management System" +
            "\r\n" + "You can now log in to Your Reserves Admin Panel with your following credentials:" +
            "\r\n" + userName + "\r\n" + password + "\r\n" + "Login Here http://www.yourreserveadmin.entellication.com." +
            "\r\n" + "You can also book a reservation with your Restaurant on http://www.yourreserve.entellication.com.";

            SmtpClient client = new SmtpClient("mail.entellication.com");
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential("info@entellication.com", "qxwcs34RT$");
            client.Send(message);
        }

        /* Description: This method builds and sends a confirmation to the registered User
         * Params: User Object, string Password
         */
        public void sendUserRegisterConfirmEmail(User oUser, string sPassword)
        {
            string firstName = oUser.FirstName.ToString();
            string lastName = oUser.LastName.ToString();
            string userName = oUser.UserName.ToString();
            string password = sPassword;

            string to = oUser.Email.ToString();
            string from = "info@entellication.com";
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Welcome To YourReserve";
            message.Body = @"Hello " + firstName + " " + lastName + " Welcome to the YourReserve Reservation System" +
            "\r\n" + "You can now log in to Your Reserve with your following credentials:" +
            "\r\n" + userName + "\r\n" + password + "\r\n" + "Login Here http://www.yourreserve.entellication.com." +
            "\r\n" + "We hope that you enjoy making reservations at some of the finest restaurants around the country.";

            SmtpClient client = new SmtpClient("mail.entellication.com");
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential("info@entellication.com", "qxwcs34RT$");
            client.Send(message);
        }

        /* Description: This method builds and sends a ForgottenPassword email to the user
         * Params: User Object
         */
        public void sendForgottenPasswordEmail(User oUser)
        {
            string firstName = oUser.FirstName.ToString();
            string lastName = oUser.LastName.ToString();
            string userName = oUser.UserName.ToString();
            string password = oUser.Password.ToString();

            string to = oUser.Email.ToString();
            string from = "info@entellication.com";
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Welcome To YourReserve";
            message.Body = @"Hello " + firstName + " " + lastName + " If you are recieving this message, it is because you have submitted a change password request." +
            "\r\n" + "Your Credentials are:" +
            "\r\n" + userName + "\r\n" + password + "\r\n" + "Login Here at http://www.yourreserve.entellication.com." +
            "\r\n" + "If you wish to change your password please go to the Edit Profile section of your account.";

            SmtpClient client = new SmtpClient("mail.entellication.com");
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential("info@entellication.com", "qxwcs34RT$");
            client.Send(message);
        }

        /* Description: This method builds and sends a confirmation to the user 
         * who reserved a reservation
         * Params: Reservation Object
         */
        public void sendReservationConfirmEmail(Reservation oReservation)
        {
            string firstName = oReservation.FirstName.ToString();
            string lastName = oReservation.LastName.ToString();
            string Time = oReservation.ReservationTime.ToString();
            string Date = oReservation.ReservationDate.Value.ToLongDateString();

            var restaurant = from r in db.Restaurants
                             where r.RestaurantID == oReservation.RestaurantID
                             select r.RestaurantName;

            string to = oReservation.Email.ToString();
            string from = "info@entellication.com";
            MailMessage message = new MailMessage(from, to);
            message.Subject = "Your Reservation Details";
            message.Body = @"Hello " + firstName + " " + lastName + " Here are your reservation details:" +
            "\r\n" + Time + "\r\n" + Date + "\r\n" + "\r\n" + restaurant + "\r\n" +
            "Login Here at http://www.yourreserve.entellication.com. to view all of your reservations";


            SmtpClient client = new SmtpClient("mail.entellication.com");
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential("info@entellication.com", "qxwcs34RT$");
            client.Send(message);
        }
    }
}