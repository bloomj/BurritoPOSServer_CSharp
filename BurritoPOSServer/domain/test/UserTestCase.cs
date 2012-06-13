using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using log4net.Config;
using NUnit.Framework;
using BurritoPOSServer.domain;

namespace BurritoPOSServer.domain.test
{
    [TestFixture]
    class UserTestCase
    {
        private static ILog dLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        /// 
        [SetUp]
        protected void SetUp()
        {
            XmlConfigurator.Configure(new FileInfo("config/log4net.properties"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// 
        [TearDown]
        protected void tearDown()
        {

        }

        [Test]
        public void testValidateUser()
        {
            try
            {
                User u = new User();
                u.id = 1;
                u.userName = "JimB";
                u.password = BCrypt.HashPassword("pass123", BCrypt.GenerateSalt());

                Assert.True(u.validate());
            }
            catch (Exception e)
            {
                dLog.Error("Exception in testValidateUser: " + e.Message + "\n" + e.StackTrace);
                Assert.Fail(e.Message + "\n" + e.StackTrace);
            }
        }

        [Test]
        public void testInvalidUser()
        {
            try
            {
                User u = new User();

                Assert.False(u.validate());
            }
            catch (Exception e)
            {
                dLog.Error("Exception in testInvalidUser: " + e.Message + "\n" + e.StackTrace);
                Assert.Fail(e.Message + "\n" + e.StackTrace);
            }
        }

        [Test]
        public void testEqualsUser()
        {
            try
            {
                User u = new User(1, "JimB", BCrypt.HashPassword("pass123", BCrypt.GenerateSalt()));
                User x = u;

                Assert.True(u.Equals(x));
            }
            catch (Exception e)
            {
                dLog.Error("Exception in testEqualsUser: " + e.Message + "\n" + e.StackTrace);
                Assert.Fail(e.Message + "\n" + e.StackTrace);
            }
        }

        [Test]
        public void testNotEqualsUser()
        {
            try
            {
                User u = new User(1, "JimB", BCrypt.HashPassword("pass123", BCrypt.GenerateSalt()));
                User x = new User();

                Assert.False(u.Equals(x));
            }
            catch (Exception e)
            {
                dLog.Error("Exception in testNotEqualsUser: " + e.Message + "\n" + e.StackTrace);
                Assert.Fail(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
