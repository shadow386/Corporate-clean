using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FallenMoon.Library.Database;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Data.Entity.Validation;

namespace FallenMoon.Library
{
    public class Security : IDisposable
    {
        #region IDisposable implementation with finalizer
        // Flag: Has Dispose already been called?
        bool disposed = false;
        // Instantiate a SafeHandle instance.
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                handle.Dispose();
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }
        #endregion

        public LoginHelper LoginUser(string userEmail, string password)
        {
            LoginHelper helper = new LoginHelper();

            if (disposed)
            {
                helper.LoginSuccess = false;
                helper.LoginMessage = "Object has been disposed";

                return helper;
            }

            try
            {
                using (DatabaseContext context = new DatabaseContext())
                {
                    UserAccount user = context.UserAccounts.FirstOrDefault(u => u.UserEmail == userEmail);
                    if (user == null) throw new Exception("Email or password is invalid");

                    bool doesPasswordMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                    if (!doesPasswordMatch)
                    {
                        user.LoginAttempts += 1;
                        if (user.LoginAttempts > 3)
                        {
                            user.Locked = true;
                            user.LockDate = DateTime.Now;
                        }

                        context.SaveChanges();

                        throw new Exception("Email or password is invalid");
                    }

                    user.Token = Guid.NewGuid().ToString();
                    user.LoginAttempts = 0;
                    user.LastLoginDate = DateTime.Now;

                    context.SaveChanges();

                    helper.UserAccount = user;
                    helper.LoginSuccess = true;

                    return helper;
                }
            }
            catch (Exception e)
            {
                var logDate = DateTime.Now;
                Console.WriteLine("Error: Failed Login Attempt: {0} tried to log in on {1} at {2}.", userEmail, logDate.ToString("MM-dd-yyyy"), logDate.ToString("HH:mm:ss"));
                Console.WriteLine("Error: {0}", e.Message);

                helper.UserAccount = null;
                helper.LoginSuccess = false;
                helper.LoginMessage = e.Message;

                return helper;
            }
        }

        public LoginHelper LoginUserByToken(string token)
        {
            LoginHelper helper = new LoginHelper();

            if (disposed)
            {
                helper.LoginSuccess = false;
                helper.LoginMessage = "Object has been disposed";

                return helper;
            }

            try
            {
                UserAccount user = UserManager.GetUser(token);

                helper.UserAccount = user ?? throw new Exception("User does not exist");
                helper.LoginSuccess = true;

                return helper;
            }
            catch (Exception e)
            {
                var logDate = DateTime.Now;
                Console.WriteLine("Error: Failed Login Attempt: Someone tried to log in with token {0} on {1} at {2}.", token, logDate.ToString("MM-dd-yyyy"), logDate.ToString("HH:mm:ss"));
                Console.WriteLine("Error: {0}", e.Message);

                helper.UserAccount = null;
                helper.LoginSuccess = false;
                helper.LoginMessage = e.Message;

                return helper;
            }
        }

        public RegisterHelper RegisterUser(string userEmail, string password, string userName)
        {
            RegisterHelper helper = new RegisterHelper();

            if (disposed)
            {
                helper.RegisterSuccess = false;
                helper.RegisterMessage = "Object has been disposed";

                return helper;
            }

            try
            {
                using (DatabaseContext context = new DatabaseContext())
                {
                    UserAccount user = context.UserAccounts.FirstOrDefault(u => u.UserEmail == userEmail);
                    if (user != null) throw new Exception("Email already exists!");

                    user = new UserAccount()
                    {
                        UserEmail = userEmail
                    };

                    user.PasswordSalt = BCrypt.Net.BCrypt.GenerateSalt();
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, user.PasswordSalt);
                    user.Token = Guid.NewGuid().ToString();
                    user.UserRankId = context.UserRanks.FirstOrDefault(r => r.RankName == "Unconfirmed").UserRankId;

                    user = context.UserAccounts.Add(user);
                    context.SaveChanges();

                    UserProfile profile = new UserProfile()
                    {
                        UserId = user.UserId,
                        UserName = userName
                    };

                    profile = context.UserProfiles.Add(profile);
                    context.SaveChanges();

                    user.UserProfile = profile;

                    Confirmation confirmation = new Confirmation(user.UserId);
                    confirmation.GenerateCode();
                    confirmation.SendConfirmation(user.UserEmail);

                    helper.UserAccount = user;
                    helper.RegisterSuccess = true;

                    return helper;
                }
            }
            catch (Exception e)
            {
                var logDate = DateTime.Now;
                Console.WriteLine("Error: Failed Register Attempt: {0} tried to register on {1} at {2}.", userEmail, logDate.ToString("MM-dd-yyyy"), logDate.ToString("HH:mm:ss"));
                Console.WriteLine("Error: {0}", e.Message);

                helper.UserAccount = null;
                helper.RegisterSuccess = false;
                helper.RegisterMessage = e.Message;

                return helper;
            }
        }
    }

    public class Confirmation
    {
        public int UserId { get; private set; }
        private string Code { get; set; }

        public Confirmation(int userId)
        {
            UserId = userId;
        }

        public void SendConfirmation(string userEmail)
        {

        }

        public void GenerateCode()
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                var confirmation = new UserEmailConfirmation()
                {
                    UserId = UserId,
                    ConfirmationNumber = RandomString(8),
                    DateSent = DateTime.Now
                };

                confirmation = context.UserEmailConfirmations.Add(confirmation);

                context.SaveChanges();

                Code = confirmation.ConfirmationNumber;
            }
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
