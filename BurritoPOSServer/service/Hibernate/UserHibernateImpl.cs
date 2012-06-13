using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using log4net;
using log4net.Config;
using NHibernate;
using NHibernate.Cfg;
using BurritoPOSServer.domain;

namespace BurritoPOSServer.service.Hibernate
{
    /// <summary>
    /// This service implementation uses NHibernate framework to do basic CRUD operations with MS SQL Server 2008 for User objects.
    /// </summary>
    class UserHibernateImpl : BaseSvcHibernateImpl, IUserSvc
    {
        private static ILog dLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        String IUserSvc.NAME
        {
            get { return "IUserSvc"; }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public UserHibernateImpl()
        {
            XmlConfigurator.Configure(new FileInfo("config/log4net.properties"));
        }

        /// <summary>
        /// This method retrieves a user.
        /// </summary>
        /// <param name="id">Unique ID of user to retrieve</param>
        /// <returns>user object</returns>
        public User getUser(Int32 id)
        {
            dLog.Info("Entering method getUser | ID: " + id);
            User u = new User();
            ISession session = null;

            try {
                using (session = getSession()) {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        IQuery query = session.CreateQuery(@"FROM User WHERE id = :id");
                        query.SetParameter("id", id);

                        u = query.List<User>()[0];
                    }

                }
            }
            catch (Exception e2)
            {
                dLog.Error("Exception in getUser: " + e2.Message + "\n" + e2.StackTrace);
                u = new User();
            }
            finally
            {
                //ensure that session is close regardless of the errors in try/catch
                if (session != null && session.IsOpen)
                    session.Close();
            }

            return u;
        }

        /// <summary>
        /// This method stores a user.
        /// </summary>
        /// <param name="u">The user object to store</param>
        /// <returns>Success/Failure</returns>
        public Boolean storeUser(User u)
        {
            dLog.Info("Entering method storeUser | ID: " + u.id);
            Boolean result = false;
            ISession session = null;

            try
            {
                using (session = getSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.Save(u);
                        transaction.Commit();

                        if (transaction.WasCommitted)
                            result = true;
                    }
                }
            }
            catch (Exception e2)
            {
                dLog.Error("Exception in storeUser: " + e2.Message);
            }
            finally
            {
                //ensure that session is close regardless of the errors in try/catch
                if (session != null && session.IsOpen)
                    session.Close();
            }

            return result;
        }

        /// <summary>
        /// This method deletes a user.
        /// </summary>
        /// <param name="id">Unique ID of the user to delete</param>
        /// <returns>Success/Failure</returns>
        public Boolean deleteUser(Int32 id)
        {
            dLog.Info("Entering method deleteUser | ID:" + id);
            Boolean result = false;
            ISession session = null;

            try
            {
                using (session = getSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        IQuery query = session.CreateQuery(@"FROM User WHERE id = :id");
                        query.SetParameter("id", id);
                        User e = query.List<User>()[0];
                        session.Delete(e);
                        transaction.Commit();

                        if (transaction.WasCommitted)
                            result = true;
                    }
                }
            }
            catch (Exception e2)
            {
                dLog.Error("Exception in deleteUser: " + e2.Message);
            }
            finally
            {
                //ensure that session is close regardless of the errors in try/catch
                if (session != null && session.IsOpen)
                    session.Close();
            }

            return result;
        }

        /// <summary>
        /// This method returns all users.
        /// </summary>
        /// <returns>List of user objects</returns>
        public List<User> getAllUsers()
        {
            dLog.Info("Entering method getAllUsers");
            List<User> result = new List<User>();
            ISession session = null;

            try
            {
                using (session = getSession())
                {
                    session.Clear();

                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        IQuery query = session.CreateQuery(@"FROM User");

                        foreach (User u in query.List<User>())
                        {
                            result.Add(u);
                        }

                        transaction.Commit();
                        session.Flush();
                        //session.evict(User.class);
                    }
                }
            }
            catch (Exception e2)
            {
                dLog.Error("Exception in getAllUsers: " + e2.Message);
            }
            finally
            {
                //ensure that session is close regardless of the errors in try/catch
                if (session != null && session.IsOpen)
                {
                    session.Close();
                }
            }

            return result;
        }
    }
}
