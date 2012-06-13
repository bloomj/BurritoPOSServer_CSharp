using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using log4net.Config;
using NUnit.Framework;
using BurritoPOSServer.domain;

namespace BurritoPOSServer.service.test
{
    /// <summary>
    /// Unit test fixture for user service implementation unit tests
    /// </summary>
    [TestFixture]
    class UserSvcImplTestCase
    {
        private Factory factory;
        private User u;
        private static ILog dLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        [SetUp]
        protected void SetUp()
        {
            XmlConfigurator.Configure(new FileInfo("config/log4net.properties"));
            factory = Factory.getInstance();
            u = new User(1, "JimB", BCrypt.HashPassword("pass123", BCrypt.GenerateSalt()));
        }

        /// <summary>
        /// 
        /// </summary>
        [TearDown]
        protected void tearDown()
        {

        }

        /// <summary>
        /// Unit test for user service implementation
        /// </summary>
        [Test]
        public void testStoreUser()
        {
            try
            {
                //week 3
                //IUserSvc ics = factory.getUserSvc();

                //week 4
                IUserSvc ics = (IUserSvc)factory.getService("IUserSvc");

                // First let's store the user
                Assert.True(ics.storeUser(u));

                // Then let's read it back in
                u = ics.getUser(u.id);
                Assert.True(u.validate());

                // Update user
                Assert.True(BCrypt.CheckPassword("pass123", u.password));

                u.password = BCrypt.HashPassword("pass12345", BCrypt.GenerateSalt());
                Assert.True(ics.storeUser(u));

                // Finally, let's cleanup the file that was created
                Assert.True(ics.deleteUser(u.id));
            }
            catch (Exception e)
            {
                dLog.Error("Exception in testStoreUser: " + e.Message + "\n" + e.StackTrace);
                Assert.Fail(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
