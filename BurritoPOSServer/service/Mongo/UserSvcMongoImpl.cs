﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using log4net;
using log4net.Config;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Wrappers;
using BurritoPOSServer.domain;

namespace BurritoPOSServer.service.Mongo
{
    /// <summary>
    /// This service implementation uses MongoDB Driver to do basic CRUD operations with MongoDB for Employee objects.
    /// </summary>
    class UserSvcMongoImpl : IUserSvc
    {
        private static ILog dLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        String IUserSvc.NAME
        {
            get { return "IUserSvc"; }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public UserSvcMongoImpl()
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

            try
            {
                MongoServer server = MongoServer.Create();
                //MongoCredentials credentials = new MongoCredentials("username", "password");
                MongoDatabase db = server.GetDatabase("neatoBurrito");
                //MongoDatabase db = server.GetDatabase("neatoBurrito", credentials);

                using (server.RequestStart(db))
                {
                    MongoCollection<BsonDocument> coll = db.GetCollection("user");
                    var query = new QueryDocument("id", id);

                    BsonDocument myDoc = coll.FindOne(query);

                    //ensure we were passed a valid object before attempting to read
                    if (myDoc != null)
                    {
                        dLog.Debug("myDoc: " + myDoc.ToString());

                        #region Read Fields
                        u.id = id;
                        u.userName = myDoc["username"].AsString;
                        u.password = myDoc["password"].AsString;
                        #endregion
                    }
                    dLog.Debug("Finishing setting user");
                }
            }
            catch (Exception e2)
            {
                dLog.Error("Exception in getUser: " + e2.Message + "\n" + e2.StackTrace);
                u = new User();
            }
            finally
            {
                //using statement above already calls RequestDone()
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

            try
            {
                MongoServer server = MongoServer.Create();
                //MongoCredentials credentials = new MongoCredentials("username", "password");
                MongoDatabase db = server.GetDatabase("neatoBurrito");
                //MongoDatabase db = server.GetDatabase("neatoBurrito", credentials);

                using (server.RequestStart(db))
                {
                    MongoCollection<BsonDocument> coll = db.GetCollection("user");
                    var query = new QueryDocument("id", u.id);

                    dLog.Debug("Finding if user exists");
                    BsonDocument myDoc = coll.FindOne(query);

                    query.Add("username", u.userName);
                    query.Add("password", u.password);

                    //ensure we were passed a valid object before attempting to write
                    if (myDoc == null)
                    {
                        dLog.Debug("Inserting user");
                        coll.Insert(query);

                        result = true;
                    }
                    else
                    {
                        var update = new UpdateDocument();
                        update.Add(query.ToBsonDocument());
                        dLog.Debug("Updating user");
                        dLog.Debug("myDoc: " + myDoc.ToString());
                        dLog.Debug("update Query: " + update.ToString());

                        SafeModeResult wr = coll.Update(new QueryDocument("id", u.id), update, SafeMode.True);

                        dLog.Debug("SafeModeResult: " + wr.Ok);
                        if (wr.LastErrorMessage == null && wr.Ok)
                        {
                            result = true;
                        }
                        else
                        {
                            dLog.Debug("SafeModeResult: " + wr.LastErrorMessage);
                        }
                    }
                }
            }
            catch (Exception e2)
            {
                dLog.Error("Exception in storeEUser: " + e2.Message);
            }
            finally
            {
                //using statement above already calls RequestDone()
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

            try
            {
                MongoServer server = MongoServer.Create();
                //MongoCredentials credentials = new MongoCredentials("username", "password");
                MongoDatabase db = server.GetDatabase("neatoBurrito");
                //MongoDatabase db = server.GetDatabase("neatoBurrito", credentials);

                using (server.RequestStart(db))
                {
                    MongoCollection<BsonDocument> coll = db.GetCollection("user");
                    var query = new QueryDocument("id", id);

                    SafeModeResult wr = coll.Remove(query, SafeMode.True);

                    dLog.Debug("SafeModeResult: " + wr.Ok);
                    if (wr.LastErrorMessage == null && wr.Ok)
                    {
                        result = true;
                    }
                    else
                    {
                        dLog.Debug("SafeModeResult: " + wr.LastErrorMessage);
                    }
                }
            }
            catch (Exception e2)
            {
                dLog.Error("Exception in deleteUser: " + e2.Message);
            }
            finally
            {
                //using statement above already calls RequestDone()
            }

            return result;
        }

        //TODO: come back and reduce number of reads on DB
        /// <summary>
        /// This method returns all users.
        /// </summary>
        /// <returns>List of users objects</returns>
        public List<User> getAllUsers()
        {
            dLog.Info("Entering method getAllUsers");
            List<User> result = new List<User>();

            try
            {
                MongoServer server = MongoServer.Create();
                //MongoCredentials credentials = new MongoCredentials("username", "password");
                MongoDatabase db = server.GetDatabase("neatoBurrito");
                //MongoDatabase db = server.GetDatabase("neatoBurrito", credentials);

                using (server.RequestStart(db))
                {
                    MongoCollection<BsonDocument> coll = db.GetCollection("user");

                    MongoCursor cur = coll.FindAll();

                    foreach (var doc in cur)
                    {
                        result.Add(getUser(((BsonDocument)doc)["id"].AsInt32));
                    }
                }
            }
            catch (Exception e2)
            {
                dLog.Error("Exception in getAllUsers: " + e2.Message);
            }
            finally
            {
                //using statement above already calls RequestDone()
            }

            return result;
        }
    }
}
