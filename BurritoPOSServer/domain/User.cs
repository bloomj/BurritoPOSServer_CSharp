using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurritoPOSServer.domain
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class User
    {
        #region Properties
        /// <summary>
        /// User's username
        /// </summary>
        public virtual String userName { get; set; }

        /// <summary>
        /// User's password
        /// </summary>
        public virtual String password { get; set; }

        /// <summary>
        /// Unique ID of User
        /// </summary>
        public virtual Int32 id { get; set; }
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public User()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_userName"></param>
        /// <param name="_password"></param>
        public User(Int32 _id, String _userName, String _password)
        {
            this.id = _id;
            this.userName = _userName;
            this.password = _password;
        }

        /// <summary>
        /// validates the object
        /// </summary>
        /// <returns>success or failure</returns>
        public virtual Boolean validate()
        {
            if (this.userName != null && this.password != null && !this.id.Equals(null))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the objects are equal
        /// </summary>
        /// <returns>success or failure</returns>
        public override Boolean Equals(Object obj)
        {
            if (this == obj)
                return true;

            if (obj == null || !this.GetType().Equals(obj.GetType()))
                return false;

            User other = (User)obj;
            if (this.userName != other.userName || this.password != other.password || this.id != other.id)
                return false;

            return true;
        }

        /// <summary>
        /// Returns base object GetHashCode
        /// </summary>
        /// <returns>Unique Hash of Object</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
