using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shamia_Rev2.A.Class
{
    /// <summary>
    /// this class used BANTIME 
    /// </summary>
    public static class SBanlist
    {
        #region declare
        /// <summary>
        /// list contains temp user auto kick
        /// </summary>
        public static List<string> List = new List<string>();

        /// <summary>
        /// add user in banlist
        /// </summary>
        /// <param name="user">user owner</param>
        public static void AddBanList(string user)
        {
            List.Add(user);
        }
        #endregion
        /// <summary>
        /// clear all owner(s) in list auto kick
        /// </summary>
        public static void ClearList()
        {
            List.Clear();
        }
        /// <summary>
        /// remove owner in list
        /// </summary>
        /// <param name="user">owner remove</param>
        public static void RemoveBanList (string user)
        {
            if (!UserInList(user)) { return;}
            List.Remove(user);
        }
        /// <summary>
        /// check owner is list autokick
        /// </summary>
        /// <param name="user">user</param>
        /// <returns></returns>
        public static bool UserInList(string user)
        {
            return List.Contains(user);
        }
      
    }
}
