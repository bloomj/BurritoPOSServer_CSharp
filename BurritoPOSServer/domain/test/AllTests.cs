using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace BurritoPOSServer.domain.test
{
    class AllTests
    {
        [Suite]
        public static IEnumerable Suite
        {
            get
            {
                ArrayList suite = new ArrayList();
                suite.Add(new UserTestCase());
                return suite;
            }
        }
    }
}
