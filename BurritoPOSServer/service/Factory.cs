﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using log4net;
using log4net.Config;
using BurritoPOSServer.exception;
using BurritoPOSServer.service.Hibernate;

namespace BurritoPOSServer.service
{
    /// <summary>
    /// This class is a service Factory that provides concrete service implementations.
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// 
        /// </summary>
        private static ILog dLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 
        /// </summary>
        private Dictionary<String, String> fProperties = new Dictionary<String, String>();

        /// <summary>
        /// Default Constructor
        /// </summary>
        private Factory()
        {
            try
            {
                XmlConfigurator.Configure(new FileInfo("config/log4net.properties"));
                // parse the file on load
                this.parse("config/properties.xml");
            }
            catch (Exception e)
            {
                dLog.Error("Exception in parse: " + e.Message);
            }
        }

        private readonly static Factory factory = new Factory();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Factory getInstance() { return factory; }

        /// <summary>
        /// This method returns a concrete class based on current configuration
        /// </summary>
        /// <param name="name">Name of the service</param>
        /// <returns>Concrete service class</returns>
        public IService getService(String name)
        {
            dLog.Debug("In getServicename: " + name);
            try
            {
                Object o = Activator.CreateInstance(Type.GetType(getImplName(name)));
                return (IService)o;
            }
            catch (TypeLoadException e1)
            {
                dLog.Error("TypeLoadException in getService: " + e1.Message);
                throw new ServiceLoadException(name + " not loaded");
            }
            catch (Exception e2)
            {
                dLog.Error("Exception in getService: " + e2.Message);
                throw new ServiceLoadException(name + " not loaded");
            }
        }

        /// <summary>
        /// This method gets the required implementation name for service requested.
        /// </summary>
        /// <param name="name">Name of service</param>
        /// <returns>Concrete class to instantiate</returns>
        private String getImplName(String name)
        {
            String retVal = "";

            try
            {
                //read out of HashMap populated on load
                retVal = fProperties[name];

                dLog.Debug("Got " + retVal + " from properties.xml file for name: " + name);
            }
            catch (Exception e)
            {
                dLog.Debug("Current directory: " + Directory.GetCurrentDirectory());
                dLog.Error("Exception in getImplName: " + e.Message);
            }

            return retVal;
        }

        //week 3 specific implementation calls (prior to Factory or IoC implementation)
        /// <summary>
        /// Static call to Serializable implementation of User service layer
        /// </summary>
        /// <returns></returns>
        public IUserSvc getUserSvc()
        {
            return new UserHibernateImpl();
        }

        /// <summary>
        /// This method parses the configuration xml file
        /// </summary>
        /// <param name="filePath">File path to configuration xml</param>
        public void parse(String filePath)
        {
            XmlDocument xmlDoc;

            try
            {
                xmlDoc = new XmlDocument(); // create an xml document object.
                xmlDoc.Load(filePath); // load the XML document from the specified file.

                // Get elements.
                XmlNodeList serviceEntries = xmlDoc.GetElementsByTagName("entry");

                foreach (XmlNode svcEntry in serviceEntries)
                {
                    fProperties.Add(svcEntry.Attributes["key"].Value, svcEntry.InnerXml.ToString());
                    dLog.Debug("curKey: " + svcEntry.Attributes["key"].Value + "value: " + svcEntry.InnerXml.ToString() + "hashmap size: " + fProperties.Count);
                }

            }
            catch (Exception e)
            {
                dLog.Debug("Current diretory: " + Directory.GetCurrentDirectory());
                dLog.Error("Exception in getImplName: " + e.Message);
            }
            finally
            {

            }
        }
    }
}
