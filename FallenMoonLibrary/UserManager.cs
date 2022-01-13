using FallenMoon.Library.Database;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FallenMoon.Library
{
    public class UserManager
    {
        public static UserAccount GetUser(int userId)
        {
            if (userId == 0) return null;

            using (DatabaseContext context = new DatabaseContext())
            {
                var user = context.UserAccounts.FirstOrDefault(u => u.UserId == userId);
                if (user.Deleted && user.DeletedDate <= DateTime.Now)
                    user = null;

                return user;
            }
        }

        public static UserAccount GetUser(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;

            using (DatabaseContext context = new DatabaseContext())
            {
                var user = context.UserAccounts.FirstOrDefault(u => u.Token == token);
                if (user.Deleted && user.DeletedDate <= DateTime.Now)
                    user = null;

                return user;

            }
        }

        public static AccountHelper GetProfile(int userId)
        {
            AccountHelper helper = new AccountHelper();

            try
            {
                if (userId == 0) throw new Exception("Invalid user id");

                using (DatabaseContext context = new DatabaseContext())
                {
                    var profile = context.UserProfiles.FirstOrDefault(u => u.UserId == userId);

                    helper.Profile = profile;
                    helper.Account = GetUser(userId);
                    helper.Success = true;

                    return helper;
                }
            }
            catch(Exception e)
            {
                helper.Success = false;
                helper.ErrorMessage = e.Message;

                return helper;
            }
        }

        public static AccountHelper GetProfile(string userName)
        {
            AccountHelper helper = new AccountHelper();

            try
            {
                if (string.IsNullOrEmpty(userName)) throw new Exception("Username is invalid");

                using (DatabaseContext context = new DatabaseContext())
                {
                    var profile = context.UserProfiles.FirstOrDefault(u => u.UserName == userName);

                    helper.Profile = profile;
                    helper.Account = GetUser(profile.UserId);
                    helper.Success = true;

                    return helper;
                }
            }
            catch (Exception e)
            {
                helper.Success = false;
                helper.ErrorMessage = e.Message;

                return helper;
            }
        }

        public static UserRank GetRankFromUserId(int userId)
        {
            try
            {
                using (DatabaseContext context = new DatabaseContext())
                {
                    var user = context.UserAccounts.FirstOrDefault(u => u.UserId == userId);
                    if (user == null) throw new Exception("User does not exist");

                    return (UserRank)user.UserRankId;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return UserRank.Invalid;
            }
        }

        public static bool IsFounder(int userId)
        {
            var rank = GetRankFromUserId(userId);
            if (rank == 0) return false;

            return rank <= UserRank.Founder;
        }

        public static bool IsAdmin(int userId)
        {
            var rank = GetRankFromUserId(userId);
            if (rank == 0) return false;

            return rank <= UserRank.Administrator;
        }

        public static bool IsModerator(int userId)
        {
            var rank = GetRankFromUserId(userId);
            if (rank == 0) return false;

            return rank <= UserRank.Moderator;
        }

        public static bool IsBlogger(int userId)
        {
            var rank = GetRankFromUserId(userId);
            if (rank == 0) return false;

            return rank <= UserRank.Blogger;
        }

        public static bool IsUser(int userId)
        {
            var rank = GetRankFromUserId(userId);
            if (rank == 0) return false;

            return rank <= UserRank.User;
        }

        public static bool IsUnconfirmed(int userId)
        {
            var rank = GetRankFromUserId(userId);
            if (rank == 0) return false;

            return rank == UserRank.Unconfirmed;
        }

        public static bool IsBanned(int userId)
        {
            var rank = GetRankFromUserId(userId);
            if (rank == 0) return false;

            return rank == UserRank.Banned;
        }
    }

    public enum UserRank
    {
        Invalid = 0,
        Founder = 1,
        Administrator = 2,
        Moderator = 3,
        Blogger = 4,
        User = 5,
        Unconfirmed = 6,
        Banned = 7
    }
}
