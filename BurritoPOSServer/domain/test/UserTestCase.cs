using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using BurritoPOSServer.domain;

namespace BurritoPOSServer.domain.test
{
    [TestFixture]
    class UserTestCase
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        [SetUp]
        protected void SetUp()
        {

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
                u.password = "pass123";

                Assert.True(u.validate());
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in testValidateUser: " + e.Message + "\n" + e.StackTrace);
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
                Console.WriteLine("Exception in testInvalidUser: " + e.Message + "\n" + e.StackTrace);
                Assert.Fail(e.Message + "\n" + e.StackTrace);
            }
        }

        [Test]
        public void testEqualsUser()
        {
            try
            {
                User u = new User(1, "JimB", "pass123");
                User x = u;

                Assert.True(u.Equals(x));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in testEqualsUser: " + e.Message + "\n" + e.StackTrace);
                Assert.Fail(e.Message + "\n" + e.StackTrace);
            }
        }

        [Test]
        public void testNotEqualsUser()
        {
            try
            {
                User u = new User(1, "JimB", "pass123");
                User x = new User();

                Assert.False(u.Equals(x));
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in testNotEqualsUser: " + e.Message + "\n" + e.StackTrace);
                Assert.Fail(e.Message + "\n" + e.StackTrace);
            }
        }
    }
}
