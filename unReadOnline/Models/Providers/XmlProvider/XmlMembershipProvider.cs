#region Using

using System;
using System.Xml;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Web.Security;
using System.Globalization;
using System.Web.Hosting;
using System.Web.Management;
using System.Security.Permissions;
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Linq;

#endregion

namespace unReadOnline.Models.Providers.XmlProvider
{
    /// <summary>
    /// 
    /// </summary>
    public class XmlMembershipProvider : MembershipProvider {
        //private Dictionary<string, MembershipUser> _Users;
        private XElement _data;
        private string _XmlFileName;

        #region Properties

        // MembershipProvider Properties
        /// <summary>
        /// 
        /// </summary>
        public override string ApplicationName {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool EnablePasswordRetrieval {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool EnablePasswordReset {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int MaxInvalidPasswordAttempts {
            get { return 5; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int MinRequiredNonAlphanumericCharacters {
            get { return 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int MinRequiredPasswordLength {
            get { return 6; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int PasswordAttemptWindow {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override MembershipPasswordFormat PasswordFormat {
            get { return MembershipPasswordFormat.Clear; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string PasswordStrengthRegularExpression {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool RequiresQuestionAndAnswer {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool RequiresUniqueEmail {
            get { return false; }
        }

        #endregion

        #region Supported methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(string name, NameValueCollection config) {
            if (config == null)
                throw new ArgumentNullException("config");

            if (String.IsNullOrEmpty(name))
                name = "XmlMembershipProvider";

            if (Type.GetType("Mono.Runtime") != null) {
                // Mono dies with a "Unrecognized attribute: description" if a description is part of the config.
                if (!string.IsNullOrEmpty(config["description"])) {
                    config.Remove("description");
                }
            } else {
                if (string.IsNullOrEmpty(config["description"])) {
                    config.Remove("description");
                    config.Add("description", "XML membership provider");
                }
            }

            base.Initialize(name, config);

            // Initialize _XmlFileName and make sure the path
            // is app-relative
            string path = config["xmlFileName"];

            if (String.IsNullOrEmpty(path))
                //path = UnReadSettings.Instance.StorageLocation + "users.xml";

            if (!VirtualPathUtility.IsAppRelative(path))
                throw new ArgumentException
                    ("xmlFileName must be app-relative");

            string fullyQualifiedPath = VirtualPathUtility.Combine
                (VirtualPathUtility.AppendTrailingSlash
                (HttpRuntime.AppDomainAppVirtualPath), path);

            _XmlFileName = HostingEnvironment.MapPath(fullyQualifiedPath);
            config.Remove("xmlFileName");

            // Make sure we have permission to read the XML data source and
            // throw an exception if we don't
            FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.Write, _XmlFileName);
            permission.Demand();

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0) {
                string attr = config.GetKey(0);
                if (!String.IsNullOrEmpty(attr))
                    throw new ProviderException("Unrecognized attribute: " + attr);
            }
        }

        /// <summary>
        /// Returns true if the username and password match an exsisting user.
        /// </summary>
        public override bool ValidateUser(string username, string password) {
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
                return false;

            try {
                ReadMembershipDataStore();

                // Validate the user name and password
                var users = from p in _data.Descendants("User")
                           where (p.Element("UserName").Value == username)
                           select new
                           {
                               //Name = p.Element("UserName").Value,
                               Password = p.Element("Password").Value
                           };
                var user = users.First();
               
                if (user != null) {
                    if (user.Password == password) // Case-sensitive
                    {
                        //user.LastLoginDate = DateTime.Now;
                        //UpdateUser(user);
                        return true;
                    }
                }

                return false;
            } catch (Exception) {
                return false;
            }
        }

        /// <summary>
        /// Retrieves a user based on his/hers username.
        /// the userIsOnline parameter is ignored.
        /// </summary>
        public override MembershipUser GetUser(string username, bool userIsOnline) {
            if (String.IsNullOrEmpty(username))
                return null;

            //ReadMembershipDataStore();

            //// Retrieve the user from the data source
            //MembershipUser user;
            //if (_Users.TryGetValue(username, out user))
            //    return user;

            //return null;
            return GetUser(username);
        }

        /// <summary>
        /// Retrieves a collection of all the users.
        /// This implementation ignores pageIndex and pageSize,
        /// and it doesn't sort the MembershipUser objects returned.
        /// </summary>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
            ReadMembershipDataStore();
            MembershipUserCollection users = new MembershipUserCollection();

            //foreach (KeyValuePair<string, MembershipUser> pair in _Users) {
            //    users.Add(pair.Value);
            //}

            var allUsers = from p in _data.Descendants("User")
                           select new
                           {
                               UserName = p.Element("UserName").Value
                           };
            foreach (var user in allUsers)
            {
                var u = GetUser(user.UserName);
                if (null != u)
                    users.Add(u);
            }

            totalRecords = users.Count;
            return users;
        }

        /// <summary>
        /// Changes a users password.
        /// </summary>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            #region 原来的非Linq的
            //XmlDocument doc = new XmlDocument();
            //doc.Load(_XmlFileName);
            //XmlNodeList nodes = doc.GetElementsByTagName("User");
            //foreach (XmlNode node in nodes) {
            //    if (node["UserName"].InnerText.Equals(username, StringComparison.OrdinalIgnoreCase)
            //      || node["Password"].InnerText.Equals(oldPassword, StringComparison.OrdinalIgnoreCase)) {
            //        node["Password"].InnerText = newPassword;
            //        doc.Save(_XmlFileName);

            //                            _Users = null;
            //                            ReadMembershipDataStore();
            //        return true;
            //    }
            //}
            #endregion

            MembershipUser user = GetUser(username);
            if (user.Comment == oldPassword)
            {
                var element = _data.Descendants("User").Where(p => p.Element("UserName").Value == username).First();
                element.SetElementValue("Password", newPassword);
                SaveToXml();
            }

            return false;
        }

        /// <summary>
        ///  创建一个新用户并保存到XML文件中。Creates a new user store he/she in the XML file
        /// </summary>
        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {
            ReadMembershipDataStore();
            int newid = Convert.ToInt32(_data.Attribute("LastId").Value) + 1;
            _data.SetAttributeValue("LastId",newid);
            XElement newUser =
                new XElement("User",
                    new XAttribute("Id", newid),
                    new XElement("UserName", username),
                    new XElement("Password", password),
                    new XElement("Email", email),
                    new XElement("CreateTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)),
                    new XElement("LastLoginTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture))
                );
            _data.Add(newUser);
            SaveToXml();

            #region 原来的非Linq的方法
            //XmlDocument doc = new XmlDocument();
            //doc.Load(_XmlFileName);

            //XmlNode xmlUserRoot = doc.CreateElement("User");
            //XmlNode xmlUserName = doc.CreateElement("UserName");
            //XmlNode xmlPassword = doc.CreateElement("Password");
            //XmlNode xmlEmail = doc.CreateElement("Email");
            //XmlNode xmlLastLoginTime = doc.CreateElement("LastLoginTime");

            //xmlUserName.InnerText = username;
            //xmlPassword.InnerText = password;
            //xmlEmail.InnerText = email;
            //xmlLastLoginTime.InnerText = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

            //xmlUserRoot.AppendChild(xmlUserName);
            //xmlUserRoot.AppendChild(xmlPassword);
            //xmlUserRoot.AppendChild(xmlEmail);
            //xmlUserRoot.AppendChild(xmlLastLoginTime);

            //doc.SelectSingleNode("Users").AppendChild(xmlUserRoot);
            //doc.Save(_XmlFileName);
            #endregion

            status = MembershipCreateStatus.Success;
            MembershipUser user = new MembershipUser(Name, username, username, email, passwordQuestion, password, isApproved, false, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.MaxValue);
            //_Users.Add(username, user);
            return user;
        }

        /// <summary>
        /// Deletes the user from the XML file and 
        /// removes him/her from the internal cache.
        /// </summary>
        public override bool DeleteUser(string username, bool deleteAllRelatedData) {
            if (string.IsNullOrEmpty(username))
                return false;
            ReadMembershipDataStore();
            var element = _data.Descendants("User").Where(p => p.Element("UserName").Value == username).First();
            if (element != null)
            {
                element.Remove();
                SaveToXml();
                return true;
            }

            #region 原来的非Linq 的
            //XmlDocument doc = new XmlDocument();
            //doc.Load(_XmlFileName);

            //foreach (XmlNode node in doc.GetElementsByTagName("User")) {
            //    if (node.ChildNodes[0].InnerText.Equals(username, StringComparison.OrdinalIgnoreCase)) {
            //        doc.SelectSingleNode("Users").RemoveChild(node);
            //        doc.Save(_XmlFileName);
            //        _Users.Remove(username);
            //        return true;
            //    }
            //}
            #endregion

            return false;
        }

        /// <summary>
        /// Get a user based on the username parameter.
        /// the userIsOnline parameter is ignored.
        /// </summary>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
            if (providerUserKey == null)
                throw new ArgumentNullException("providerUserKey");
            MembershipUser user = GetUser(Convert.ToInt32(providerUserKey));
            if (user != null)
                return user;

            #region 原来的非Linq的
            //XmlDocument doc = new XmlDocument();
            //doc.Load(_XmlFileName);

            //foreach (XmlNode node in doc.SelectNodes("//User")) {
            //    if (node.ChildNodes[0].InnerText.Equals(providerUserKey.ToString(), StringComparison.OrdinalIgnoreCase)) {
            //        string userName = node.ChildNodes[0].InnerText;
            //        string password = node.ChildNodes[1].InnerText;
            //        string email = node.ChildNodes[2].InnerText;
            //        DateTime lastLoginTime = DateTime.Parse(node.ChildNodes[3].InnerText, CultureInfo.InvariantCulture);
            //        return new MembershipUser(Name, providerUserKey.ToString(), providerUserKey, email, string.Empty, password, true, false, DateTime.Now, lastLoginTime, DateTime.Now, DateTime.Now, DateTime.MaxValue);
            //    }
            //}
            #endregion

            return default(MembershipUser);
        }

        /// <summary>
        /// Retrieves a username based on a matching email.
        /// </summary>
        public override string GetUserNameByEmail(string email) {
            if (email == null)
                throw new ArgumentNullException("email");
            ReadMembershipDataStore();
            var element = _data.Descendants("User")
                .Where(p => p.Element("Email").Value.ToLower() == email.ToLower())
                .First();
            if (element != null)
            {
                return element.Element("Email").Value;
            }

            #region 原来的非Linq的
            //XmlDocument doc = new XmlDocument();
            //doc.Load(_XmlFileName);

            //foreach (XmlNode node in doc.GetElementsByTagName("User")) {
            //    if (node.ChildNodes[2].InnerText.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase)) {
            //        return node.ChildNodes[0].InnerText;
            //    }
            //}
            #endregion

            return null;
        }

        /// <summary>
        /// Updates a user. The username will not be changed.
        /// </summary>
        public override void UpdateUser(MembershipUser user) {
            ReadMembershipDataStore();
            var element = _data.Descendants("User")
                .Where(p => Convert.ToInt32(p.Attribute("Id").Value) == Convert.ToInt32(user.ProviderUserKey.ToString()))
                .First();
            element.SetElementValue("Email", user.Email);
            element.SetElementValue("LastLoginTime", user.LastLoginDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            if (user.Comment.Length > 30)
                element.SetElementValue("Password", user.Comment);
            SaveToXml();

            #region 原来的非Linq 的
            //XmlDocument doc = new XmlDocument();
            //doc.Load(_XmlFileName);

            //foreach (XmlNode node in doc.GetElementsByTagName("User")) {
            //    if (node.ChildNodes[0].InnerText.Equals(user.UserName, StringComparison.OrdinalIgnoreCase)) {
            //        if (user.Comment.Length > 30) {
            //            node.ChildNodes[1].InnerText = user.Comment;
            //        }
            //        node.ChildNodes[2].InnerText = user.Email;
            //        node.ChildNodes[3].InnerText = user.LastLoginDate.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            //        doc.Save(_XmlFileName);
            //        _Users[user.UserName] = user;
            //    }
            //}
            #endregion
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Builds the internal cache of users.
        /// </summary>
        private void ReadMembershipDataStore() {
            lock (this)
            {
                #region 原来的非Linq的
                //if (_Users == null)
                //{
                //    _Users = new Dictionary<string, MembershipUser>(16, StringComparer.OrdinalIgnoreCase);
                //    XmlDocument doc = new XmlDocument();
                //    doc.Load(_XmlFileName);
                //    XmlNodeList nodes = doc.GetElementsByTagName("User");

                //    foreach (XmlNode node in nodes)
                //    {
                //        MembershipUser user = new MembershipUser(
                //            Name,                       // Provider name
                //            node["UserName"].InnerText, // Username
                //            node["UserName"].InnerText, // providerUserKey
                //            node["Email"].InnerText,    // Email
                //            String.Empty,               // passwordQuestion
                //            node["Password"].InnerText, // Comment
                //            true,                       // isApproved
                //            false,                      // isLockedOut
                //            DateTime.Now,               // creationDate
                //            DateTime.Parse(node["LastLoginTime"].InnerText, CultureInfo.InvariantCulture), // lastLoginDate
                //            DateTime.Now,               // lastActivityDate
                //            DateTime.Now, // lastPasswordChangedDate
                //            new DateTime(1980, 1, 1)    // lastLockoutDate
                //        );
                //        _Users.Add(user.UserName, user);
                //    }
                //}
                #endregion

                if (_data == null)
                    _data = XElement.Load(_XmlFileName);
            }
        }

        /// <summary>
        /// 根据用户名检索并返回用户
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private MembershipUser GetUser(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            ReadMembershipDataStore();
            var users = from p in _data.Descendants("User")
                        where p.Element("UserName").Value.ToLower(CultureInfo.InvariantCulture) == name.ToLower(CultureInfo.InvariantCulture)
                        select new
                        {
                            Id = Convert.ToInt32(p.Attribute("Id").Value),
                            UserName = p.Element("UserName").Value,
                            Password = p.Element("Password").Value,
                            Email = p.Element("Email").Value,
                            LastLoginTime = DateTime.Parse(p.Element("LastLoginTime").Value),
                            CreateTime = p.Element("CreateTime").Value
                        };
            
            if (users.Count() > 0)
            {
                var user = users.First();
                return new MembershipUser(Name, user.UserName, user.Id, user.Email, string.Empty, user.Password, true, false, DateTime.Parse(user.CreateTime), user.LastLoginTime, DateTime.Now, DateTime.Now, new DateTime(1980, 1, 1));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 根据用户Id检索并返回用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private MembershipUser GetUser(int id)
        {
            ReadMembershipDataStore();
            var users = from p in _data.Descendants("User")
                        where Convert.ToInt32( p.Attribute("Id").Value) == id
                        select new
                        {
                            Id = Convert.ToInt32(p.Attribute("Id").Value),
                            UserName = p.Element("UserName").Value,
                            Password = p.Element("Password").Value,
                            Email = p.Element("Email").Value,
                            LastLoginTime = DateTime.Parse(p.Element("LastLoginTime").Value),
                            CreateTime = p.Element("CreateTime").Value
                        };
            var user = users.First();
            if (null != user)
            {
                return new MembershipUser(Name, user.UserName, user.Id, user.Email, string.Empty, user.Password, true, false, DateTime.Parse(user.CreateTime), user.LastLoginTime, DateTime.Now, DateTime.Now, new DateTime(1980, 1, 1));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 保存修改的结果到XML文件中
        /// </summary>
        private void SaveToXml()
        {
            _data.Save(_XmlFileName);
        }

        ///// <summary>
        ///// Encrypts a string using the SHA256 algorithm.
        ///// </summary>
        //private static string Encrypt(string plainMessage)
        //{
        //  byte[] data = Encoding.UTF8.GetBytes(plainMessage);
        //  using (HashAlgorithm sha = new SHA256Managed())
        //  {
        //    byte[] encryptedBytes = sha.TransformFinalBlock(data, 0, data.Length);
        //    return Convert.ToBase64String(sha.Hash);
        //  }
        //}

        #endregion

        #region Unsupported methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public override string ResetPassword(string username, string answer) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public override bool UnlockUser(string userName) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="usernameToMatch"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetNumberOfUsersOnline() {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="newPasswordQuestion"></param>
        /// <param name="newPasswordAnswer"></param>
        /// <returns></returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
            throw new NotSupportedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public override string GetPassword(string username, string answer) {
            throw new NotSupportedException();
        }

        #endregion

    }
}