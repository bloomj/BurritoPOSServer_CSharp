using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurritoPOSServer.domain;

namespace BurritoPOSServer.service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUserSvc : IService
    {
        /// <summary>
        /// 
        /// </summary>
        String NAME { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        Boolean storeUser(User u);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        User getUser(Int32 id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Boolean deleteUser(Int32 id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<User> getAllUsers();
    }
}
