//#region Using

//using System;
//using System.Xml;
//using System.IO;
//using System.Globalization;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using ingNote.Core;

//#endregion

//namespace unReadOnline.Models.Providers.XmlProvider
//{
//    /// <summary>
//    /// A storage provider for ingNote that uses XML files.
//    /// <remarks>
//    /// To build another provider, you can just copy and modify
//    /// this one. Then add it to the web.config's ingNote section.
//    /// </remarks>
//    /// </summary>
//    public partial class XmlProvider : UnReadProvider
//    {
//        //private static string _Folder = System.Web.HttpContext.Current.Server.MapPath(NoteSettings.Instance.StorageLocation);

//        internal static string _Folder
//        {
//            get
//            {
//                string p = NoteSettings.Instance.StorageLocation.Replace("~/", "");
//                return System.IO.Path.Combine(System.Web.HttpRuntime.AppDomainAppPath, p);
//            }
//        }

//        /// <summary>
//        /// Retrieves a note based on the specified Id.
//        /// </summary>
//        public override Note SelectNote(int id)
//        {
//            string fileName = _Folder + "notes" + Path.DirectorySeparatorChar + id.ToString() + ".xml";
//            Note note = new Note();
//            XmlDocument doc = new XmlDocument();
//            doc.Load(fileName);

//            note.Title = doc.SelectSingleNode("note/title").InnerText;
//            note.Description = doc.SelectSingleNode("note/description").InnerText;
//            note.Content = doc.SelectSingleNode("note/content").InnerText;
//            note.DateCreated = DateTime.Parse(doc.SelectSingleNode("note/pubDate").InnerText, CultureInfo.InvariantCulture);

//            if (doc.SelectSingleNode("note/lastModified") != null)
//                note.DateModified = DateTime.Parse(doc.SelectSingleNode("note/lastModified").InnerText, CultureInfo.InvariantCulture);

//            if (doc.SelectSingleNode("note/author") != null)
//                note.Author = doc.SelectSingleNode("note/author").InnerText;

//            if (doc.SelectSingleNode("note/ispublished") != null)
//                note.IsPublished = bool.Parse(doc.SelectSingleNode("note/ispublished").InnerText);

//            if (doc.SelectSingleNode("note/iscommentsenabled") != null)
//                note.IsCommentsEnabled = bool.Parse(doc.SelectSingleNode("note/iscommentsenabled").InnerText);

//            if (doc.SelectSingleNode("note/raters") != null)
//                note.Raters = int.Parse(doc.SelectSingleNode("note/raters").InnerText, CultureInfo.InvariantCulture);

//            if (doc.SelectSingleNode("note/rating") != null)
//                note.Rating = float.Parse(doc.SelectSingleNode("note/rating").InnerText, System.Globalization.CultureInfo.GetCultureInfo("en-gb"));

//            if (doc.SelectSingleNode("note/slug") != null)
//                note.Slug = doc.SelectSingleNode("note/slug").InnerText;

//            // Tags
//            foreach (XmlNode node in doc.SelectNodes("note/tags/tag"))
//            {
//                if (!string.IsNullOrEmpty(node.InnerText))
//                    note.Tags.Add(node.InnerText);
//            }

//            // comments
//            foreach (XmlNode node in doc.SelectNodes("note/comments/comment"))
//            {
//                Comment comment = new Comment();
//                comment.Id = new int(node.Attributes["id"].InnerText);
//                comment.Author = node.SelectSingleNode("author").InnerText;
//                comment.Email = node.SelectSingleNode("email").InnerText;
//                comment.Parent = note;

//                if (node.SelectSingleNode("country") != null)
//                    comment.Country = node.SelectSingleNode("country").InnerText;

//                if (node.SelectSingleNode("ip") != null)
//                    comment.IP = node.SelectSingleNode("ip").InnerText;

//                if (node.SelectSingleNode("website") != null)
//                {
//                    Uri website;
//                    if (Uri.TryCreate(node.SelectSingleNode("website").InnerText, UriKind.Absolute, out website))
//                        comment.Website = website;
//                }

//                if (node.Attributes["approved"] != null)
//                    comment.IsApproved = bool.Parse(node.Attributes["approved"].InnerText);
//                else
//                    comment.IsApproved = true;

//                comment.Content = node.SelectSingleNode("content").InnerText;
//                comment.DateCreated = DateTime.Parse(node.SelectSingleNode("date").InnerText, CultureInfo.InvariantCulture);
//                note.Comments.Add(comment);
//            }

//            note.Comments.Sort();

//            // categories
//            foreach (XmlNode node in doc.SelectNodes("note/categories/category"))
//            {
//                int key = new int(node.InnerText);
//                Category cat = Category.GetCategory(key);
//                if (cat != null)//CategoryDictionary.Instance.ContainsKey(key))
//                    note.Categories.Add(cat);
//            }

//            // Notification e-mails
//            foreach (XmlNode node in doc.SelectNodes("note/notifications/email"))
//            {
//                note.NotificationEmails.Add(node.InnerText);
//            }

//            return note;
//        }

//        /// <summary>
//        /// Inserts a new Note to the data store.
//        /// </summary>
//        /// <param name="note"></param>
//        public override void InsertNote(Note note)
//        {
//            if (!Directory.Exists(_Folder + "notes"))
//                Directory.CreateDirectory(_Folder + "notes");

//            string fileName = _Folder + "notes" + Path.DirectorySeparatorChar + note.Id.ToString() + ".xml";
//            XmlWriterSettings settings = new XmlWriterSettings();
//            settings.Indent = true;

//            using (XmlWriter writer = XmlWriter.Create(fileName, settings))
//            {
//                writer.WriteStartDocument(true);
//                writer.WriteStartElement("note");

//                writer.WriteElementString("author", note.Author);
//                writer.WriteElementString("title", note.Title);
//                writer.WriteElementString("description", note.Description);
//                writer.WriteElementString("content", note.Content);
//                writer.WriteElementString("ispublished", note.IsPublished.ToString());
//                writer.WriteElementString("iscommentsenabled", note.IsCommentsEnabled.ToString());
//                writer.WriteElementString("pubDate", note.DateCreated.AddHours(-NoteSettings.Instance.Timezone).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
//                writer.WriteElementString("lastModified", note.DateModified.AddHours(-NoteSettings.Instance.Timezone).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
//                writer.WriteElementString("raters", note.Raters.ToString(CultureInfo.InvariantCulture));
//                writer.WriteElementString("rating", note.Rating.ToString(CultureInfo.InvariantCulture));
//                writer.WriteElementString("slug", note.Slug);

//                // Tags
//                writer.WriteStartElement("tags");
//                foreach (string tag in note.Tags)
//                {
//                    writer.WriteElementString("tag", tag);
//                }
//                writer.WriteEndElement();

//                // comments
//                writer.WriteStartElement("comments");
//                foreach (Comment comment in note.Comments)
//                {
//                    writer.WriteStartElement("comment");
//                    writer.WriteAttributeString("id", comment.Id.ToString());
//                    writer.WriteAttributeString("approved", comment.IsApproved.ToString());
//                    writer.WriteElementString("date", comment.DateCreated.AddHours(-NoteSettings.Instance.Timezone).ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
//                    writer.WriteElementString("author", comment.Author);
//                    writer.WriteElementString("email", comment.Email);
//                    writer.WriteElementString("country", comment.Country);
//                    writer.WriteElementString("ip", comment.IP);
//                    if (comment.Website != null)
//                        writer.WriteElementString("website", comment.Website.ToString());
//                    writer.WriteElementString("content", comment.Content);
//                    writer.WriteEndElement();
//                }
//                writer.WriteEndElement();

//                // categories
//                writer.WriteStartElement("categories");
//                foreach (Category cat in note.Categories)
//                {
//                    //if (cat.Id = .Instance.ContainsKey(key))
//                    //     writer.WriteElementString("category", key.ToString());
//                    writer.WriteElementString("category", cat.Id.ToString());

//                }
//                writer.WriteEndElement();

//                // Notification e-mails
//                writer.WriteStartElement("notifications");
//                foreach (string email in note.NotificationEmails)
//                {
//                    writer.WriteElementString("email", email);
//                }
//                writer.WriteEndElement();

//                writer.WriteEndElement();
//            }
//        }

//        /// <summary>
//        /// Updates a Note.
//        /// </summary>
//        public override void UpdateNote(Note note)
//        {
//            InsertNote(note);
//        }

//        /// <summary>
//        /// Deletes a note from the data store.
//        /// </summary>
//        public override void DeleteNote(Note note)
//        {
//            string fileName = _Folder + "notes" + Path.DirectorySeparatorChar + note.Id.ToString() + ".xml";
//            if (File.Exists(fileName))
//                File.Delete(fileName);
//        }

//        /// <summary>
//        /// Retrieves all notes from the data store
//        /// </summary>
//        /// <returns>List of Notes</returns>
//        public override List<Note> FillNotes()
//        {
//            string folder = Category._Folder + "notes" + Path.DirectorySeparatorChar;
//            List<Note> notes = new List<Note>();

//            foreach (string file in Directory.GetFiles(folder, "*.xml", SearchOption.TopDirectoryOnly))
//            {
//                FileInfo info = new FileInfo(file);
//                string id = info.Name.Replace(".xml", string.Empty);
//                //Note note = SelectNote(new int(id));
//                Note note = Note.Load(new int(id));
//                notes.Add(note);
//            }

//            notes.Sort();
//            return notes;
//        }

//    }
//}