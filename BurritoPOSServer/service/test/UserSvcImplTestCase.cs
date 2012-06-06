using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
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
            factory = Factory.getInstance();
            u = new User(1, "JimB", "pass123");
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

                // First let's store the Employee
                Assert.True(ics.storeUser(u));

                // Then let's read it back in
                u = ics.getUser(u.id);
                Assert.True(u.validate());

                // Update Employee
                u.password = "pass12345";
                Assert.True(ics.storeUser(u));

                // Finally, let's cleanup the file that was created
                Assert.True(ics.deleteUser(u.id));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in testStoreUser: " + e.Message + "\n" + e.StackTrace);
                Assert.Fail(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
