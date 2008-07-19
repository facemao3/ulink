//#region Using

//using System;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.Data.SqlTypes;
//using System.Text;
//using System.Globalization;

//#endregion

//namespace unReadOnline.Models.Providers
//{
//  /// <summary>
//  /// Microsoft SQL Server Implementation of UnReadProvider
//  /// </summary>
//    public class MSSQLProvider : UnReadProvider, IDisposable
//  {
//    private string connStringName;
//    private SqlConnection providerConn;

//    #region Notes
//    /// <summary>
//    /// Retrieves a note based on the specified Id.
//    /// </summary>
//    public override Note SelectNote(int id)
//    {
//      bool connClose = OpenConnection();

//      Note note = new Note();
//      string sqlQuery = "SELECT NoteID, Title, Description, NoteContent, DateCreated, " +
//                          "DateModified, Author, IsPublished, IsCommentEnabled, Raters, Rating, Slug " +
//                          "FROM be_Notes " +
//                          "WHERE NoteID = @id";
//      SqlCommand cmd = new SqlCommand(sqlQuery, providerConn);
//      cmd.Parameters.Add(new SqlParameter("@id", id.ToString()));
//      SqlDataReader rdr = cmd.ExecuteReader();
//      rdr.Read();

//      note.Id = rdr.Getint(0);
//      note.Title = rdr.GetString(1);
//      note.Content = rdr.GetString(3);
//      if (!rdr.IsDBNull(2))
//        note.Description = rdr.GetString(2);
//      if (!rdr.IsDBNull(4))
//        note.DateCreated = rdr.GetDateTime(4);
//      if (!rdr.IsDBNull(5))
//        note.DateModified = rdr.GetDateTime(5);
//      if (!rdr.IsDBNull(6))
//        note.Author = rdr.GetString(6);
//      if (!rdr.IsDBNull(7))
//        note.IsPublished = rdr.GetBoolean(7);
//      if (!rdr.IsDBNull(8))
//        note.IsCommentsEnabled = rdr.GetBoolean(8);
//      if (!rdr.IsDBNull(9))
//        note.Raters = rdr.GetInt32(9);
//      if (!rdr.IsDBNull(10))
//        note.Rating = rdr.GetFloat(10);
//      if (!rdr.IsDBNull(11))
//        note.Slug = rdr.GetString(11);
//      else
//        note.Slug = "";

//      rdr.Close();

//      // Tags
//      sqlQuery = "SELECT Tag " +
//                  "FROM be_NoteTag " +
//                  "WHERE NoteID = @id";
//      cmd.CommandText = sqlQuery;
//      rdr = cmd.ExecuteReader();

//      while (rdr.Read())
//      {
//        if (!rdr.IsDBNull(0))
//          note.Tags.Add(rdr.GetString(0));
//      }

//      rdr.Close();
//            note.Tags.MarkOld();

//      // Categories
//      sqlQuery = "SELECT CategoryID " +
//                  "FROM be_NoteCategory " +
//                  "WHERE NoteID = @id";
//      cmd.CommandText = sqlQuery;
//      rdr = cmd.ExecuteReader();

//      while (rdr.Read())
//      {
//        int key = rdr.Getint(0);
//        if (Category.GetCategory(key) != null)
//            note.Categories.Add(Category.GetCategory(key));
//      }

//      rdr.Close();

//      // Comments
//      sqlQuery = "SELECT NoteCommentID, CommentDate, Author, Email, Website, Comment, Country, Ip, IsApproved " +
//                  "FROM be_NoteComment " +
//                  "WHERE NoteID = @id";
//      cmd.CommandText = sqlQuery;
//      rdr = cmd.ExecuteReader();

//      while (rdr.Read())
//      {
//        Comment comment = new Comment();
//        comment.Id = rdr.Getint(0);
//        comment.IsApproved = true;
//        comment.Author = rdr.GetString(2);
//        if (!rdr.IsDBNull(4))
//        {
//          Uri website;
//          if (Uri.TryCreate(rdr.GetString(4), UriKind.Absolute, out website))
//            comment.Website = website;
//        }
//        comment.Email = rdr.GetString(3);
//        comment.Content = rdr.GetString(5);
//        comment.DateCreated = rdr.GetDateTime(1);
//        comment.Parent = note;

//        if (!rdr.IsDBNull(6))
//          comment.Country = rdr.GetString(6);
//        if (!rdr.IsDBNull(7))
//          comment.IP = rdr.GetString(7);
//        if (!rdr.IsDBNull(8))
//          comment.IsApproved = rdr.GetBoolean(8);
//        else
//          comment.IsApproved = true;

//        note.Comments.Add(comment);
//      }

//      note.Comments.Sort();

//      rdr.Close();

//      // Email Notification
//      sqlQuery = "SELECT NotifyAddress " +
//                  "FROM be_NoteNotify " +
//                  "WHERE NoteID = @id";
//      cmd.CommandText = sqlQuery;
//      rdr = cmd.ExecuteReader();

//      while (rdr.Read())
//      {
//        if (!rdr.IsDBNull(0))
//          note.NotificationEmails.Add(rdr.GetString(0));
//      }

//      rdr.Close();

//      if (connClose)
//        providerConn.Close();

//      return note;
//    }

//    /// <summary>
//    /// Inserts a new Note to the data store.
//    /// </summary>
//    public override void InsertNote(Note note)
//    {
//      OpenConnection();

//      string sqlQuery = "INSERT INTO " +
//                          "be_Notes (NoteID, Title, Description, NoteContent, DateCreated, " +
//                          "DateModified, Author, IsPublished, IsCommentEnabled, Raters, Rating, Slug)" +
//                          "VALUES (@id, @title, @desc, @content, @created, @modified, " +
//                          "@author, @published, @commentEnabled, @raters, @rating, @slug)";
//      SqlCommand cmd = new SqlCommand(sqlQuery, providerConn);
//      cmd.Parameters.Add(new SqlParameter("@id", note.Id.ToString()));
//      cmd.Parameters.Add(new SqlParameter("@title", note.Title));
//      if (note.Description == null)
//        cmd.Parameters.Add(new SqlParameter("@desc", ""));
//      else
//        cmd.Parameters.Add(new SqlParameter("@desc", note.Description));
//      cmd.Parameters.Add(new SqlParameter("@content", note.Content));
//      cmd.Parameters.Add(new SqlParameter("@created", new SqlDateTime(note.DateCreated.AddHours(-NoteSettings.Instance.Timezone))));
//      if (note.DateModified == new DateTime())
//        cmd.Parameters.Add(new SqlParameter("@modified", new SqlDateTime()));
//      else
//        cmd.Parameters.Add(new SqlParameter("@modified", new SqlDateTime(note.DateModified.AddHours(-NoteSettings.Instance.Timezone))));
//      if (note.Author == null)
//        cmd.Parameters.Add(new SqlParameter("@author", ""));
//      else
//        cmd.Parameters.Add(new SqlParameter("@author", note.Author));
//      cmd.Parameters.Add(new SqlParameter("@published", note.IsPublished));
//      cmd.Parameters.Add(new SqlParameter("@commentEnabled", note.IsCommentsEnabled));
//      cmd.Parameters.Add(new SqlParameter("@raters", note.Raters.ToString(CultureInfo.InvariantCulture)));
//      cmd.Parameters.Add(new SqlParameter("@rating", note.Rating.ToString(System.Globalization.CultureInfo.InvariantCulture)));
//      if (note.Slug == null)
//        cmd.Parameters.Add(new SqlParameter("@slug", ""));
//      else
//        cmd.Parameters.Add(new SqlParameter("@slug", note.Slug));

//      cmd.ExecuteNonQuery();

//      // Tags
//      UpdateTags(note);

//      // Categories
//      UpdateCategories(note);

//      // Comments
//      UpdateComments(note);

//      // Email Notification
//      UpdateNotify(note);

//      providerConn.Close();
//    }

//    /// <summary>
//    /// Updates a Note.
//    /// </summary>
//    public override void UpdateNote(Note note)
//    {
//      OpenConnection();

//      string sqlQuery = "UPDATE be_Notes " +
//                          "SET Title = @title, Description = @desc, NoteContent = @content, " +
//                          "DateCreated = @created, DateModified = @modified, Author = @Author, " +
//                          "IsPublished = @published, IsCommentEnabled = @commentEnabled, " +
//                          "Raters = @raters, Rating = @rating, Slug = @slug " +
//                          "WHERE NoteID = @id";
//      SqlCommand cmd = new SqlCommand(sqlQuery, providerConn);
//      cmd.Parameters.Add(new SqlParameter("@title", note.Title));
//      if (note.Description == null)
//        cmd.Parameters.Add(new SqlParameter("@desc", ""));
//      else
//        cmd.Parameters.Add(new SqlParameter("@desc", note.Description));
//      cmd.Parameters.Add(new SqlParameter("@content", note.Content));
//      cmd.Parameters.Add(new SqlParameter("@created", new SqlDateTime(note.DateCreated.AddHours(-NoteSettings.Instance.Timezone))));
//      if (note.DateModified == new DateTime())
//        cmd.Parameters.Add(new SqlParameter("@modified", new SqlDateTime()));
//      else
//        cmd.Parameters.Add(new SqlParameter("@modified", new SqlDateTime(note.DateModified.AddHours(-NoteSettings.Instance.Timezone))));
//      if (note.Author == null)
//        cmd.Parameters.Add(new SqlParameter("@author", ""));
//      else
//        cmd.Parameters.Add(new SqlParameter("@author", note.Author));
//      cmd.Parameters.Add(new SqlParameter("@published", note.IsPublished));
//      cmd.Parameters.Add(new SqlParameter("@commentEnabled", note.IsCommentsEnabled));
//      cmd.Parameters.Add(new SqlParameter("@id", note.Id.ToString()));
//      cmd.Parameters.Add(new SqlParameter("@raters", note.Raters.ToString(CultureInfo.InvariantCulture)));
//      cmd.Parameters.Add(new SqlParameter("@rating", note.Rating.ToString(CultureInfo.InvariantCulture)));
//      if (note.Slug == null)
//        cmd.Parameters.Add(new SqlParameter("@slug", ""));
//      else
//        cmd.Parameters.Add(new SqlParameter("@slug", note.Slug));

//      cmd.ExecuteNonQuery();

//      // Tags
//      UpdateTags(note);

//      // Categories
//      UpdateCategories(note);

//      // Comments
//      UpdateComments(note);

//      // Email Notification
//      UpdateNotify(note);

//      providerConn.Close();

//    }

//    /// <summary>
//    /// Deletes a note from the data store.
//    /// </summary>
//    public override void DeleteNote(Note note)
//    {
//      OpenConnection();

//      string sqlQuery =   "DELETE FROM be_NoteTag WHERE NoteID = @id;" +
//                          "DELETE FROM be_NoteCategory WHERE NoteID = @id;" +
//                          "DELETE FROM be_NoteNotify WHERE NoteID = @id;" +
//                          "DELETE FROM be_NoteComment WHERE NoteID = @id;" +
//                          "DELETE FROM be_Notes WHERE NoteID = @id;";
//      SqlCommand cmd = new SqlCommand(sqlQuery, providerConn);
//      cmd.Parameters.Add(new SqlParameter("@id", note.Id.ToString()));

//      cmd.ExecuteNonQuery();

//      providerConn.Close();
//    }

//    /// <summary>
//    /// Retrieves all notes from the data store
//    /// </summary>
//    /// <returns>List of Notes</returns>
//    public override List<Note> FillNotes()
//    {
//      List<Note> notes = new List<Note>();

//      OpenConnection();

//      string sqlQuery = "SELECT NoteID FROM be_Notes ";
//      SqlDataAdapter sa = new SqlDataAdapter(sqlQuery, providerConn);
//      DataTable dtNotes = new DataTable();
//      dtNotes.Locale = CultureInfo.InvariantCulture;
//      sa.Fill(dtNotes);

//      foreach (DataRow dr in dtNotes.Rows)
//      {
//        notes.Add(Note.Load(new int(dr[0].ToString())));
//      }

//      providerConn.Close();

//      notes.Sort();
//      return notes;
//    }

//    private void UpdateTags(Note note)
//    {
//      SqlCommand cmd = new SqlCommand();
//      cmd.Connection = providerConn;
//      cmd.CommandText = "DELETE FROM be_NoteTag WHERE NoteID = @id";
//      cmd.Parameters.Clear();
//      cmd.Parameters.Add(new SqlParameter("@id", note.Id.ToString()));
//      cmd.ExecuteNonQuery();

//      foreach (string tag in note.Tags)
//      {
//        cmd.CommandText = "INSERT INTO be_NoteTag (NoteID, Tag) VALUES (@id, @tag)";
//        cmd.Parameters.Clear();
//        cmd.Parameters.Add(new SqlParameter("@id", note.Id.ToString()));
//        cmd.Parameters.Add(new SqlParameter("@tag", tag));
//        cmd.ExecuteNonQuery();
//      }
//    }

//    private void UpdateCategories(Note note)
//    {
//      SqlCommand cmd = new SqlCommand();
//      cmd.Connection = providerConn;
//      cmd.CommandText = "DELETE FROM be_NoteCategory WHERE NoteID = @id";
//      cmd.Parameters.Clear();
//      cmd.Parameters.Add(new SqlParameter("@id", note.Id.ToString()));
//      cmd.ExecuteNonQuery();

//      foreach (Category cat in note.Categories)
//      {
//        //if (Category.GetCategory(key) != null)
//        //{
//        cmd.CommandText = "INSERT INTO be_NoteCategory (NoteID, CategoryID) VALUES (@id, @cat)";
//        cmd.Parameters.Clear();
//        cmd.Parameters.Add(new SqlParameter("@id", note.Id.ToString()));
//        cmd.Parameters.Add(new SqlParameter("@cat", cat.Id));
//        cmd.ExecuteNonQuery();
//        //}
//      }
//    }

//    private void UpdateComments(Note note)
//    {
//      SqlCommand cmd = new SqlCommand();
//      cmd.Connection = providerConn;
//      cmd.CommandText = "DELETE FROM be_NoteComment WHERE NoteID = @id";
//      cmd.Parameters.Clear();
//      cmd.Parameters.Add(new SqlParameter("@id", note.Id.ToString()));
//      cmd.ExecuteNonQuery();

//      foreach (Comment comment in note.Comments)
//      {
//        cmd.CommandText = "INSERT INTO be_NoteComment (NoteCommentID, NoteID, CommentDate, Author, Email, Website, Comment, Country, Ip, IsApproved) " +
//                            "VALUES (@notecommentid, @id, @date, @author, @email, @website, @comment, @country, @ip, @isapproved)";
//        cmd.Parameters.Clear();
//        cmd.Parameters.Add(new SqlParameter("@notecommentid", comment.Id.ToString()));
//        cmd.Parameters.Add(new SqlParameter("@id", note.Id.ToString()));
//        cmd.Parameters.Add(new SqlParameter("@date", new SqlDateTime(comment.DateCreated)));
//        cmd.Parameters.Add(new SqlParameter("@author", comment.Author));
//        cmd.Parameters.Add(new SqlParameter("@email", comment.Email));
//        if (comment.Website == null)
//          cmd.Parameters.Add(new SqlParameter("@website", ""));
//        else
//          cmd.Parameters.Add(new SqlParameter("@website", comment.Website.ToString()));
//        cmd.Parameters.Add(new SqlParameter("@comment", comment.Content));
//        if (comment.Country == null)
//          cmd.Parameters.Add(new SqlParameter("@country", ""));
//        else
//          cmd.Parameters.Add(new SqlParameter("@country", comment.Country));
//        if (comment.IP == null)
//          cmd.Parameters.Add(new SqlParameter("@ip", ""));
//        else
//          cmd.Parameters.Add(new SqlParameter("@ip", comment.IP));
//        cmd.Parameters.Add(new SqlParameter("@isapproved", comment.IsApproved));
//        cmd.ExecuteNonQuery();
//      }
//    }

//    private void UpdateNotify(Note note)
//    {
//      SqlCommand cmd = new SqlCommand();
//      cmd.Connection = providerConn;
//      cmd.CommandText = "DELETE FROM be_NoteNotify WHERE NoteID = @id";
//      cmd.Parameters.Clear();
//      cmd.Parameters.Add(new SqlParameter("@id", note.Id.ToString()));
//      cmd.ExecuteNonQuery();

//      foreach (string email in note.NotificationEmails)
//      {
//        cmd.CommandText = "INSERT INTO be_NoteNotify (NoteID, NotifyAddress) VALUES (@id, @notify)";
//        cmd.Parameters.Clear();
//        cmd.Parameters.Add(new SqlParameter("@id", note.Id.ToString()));
//        cmd.Parameters.Add(new SqlParameter("@notify", email));
//        cmd.ExecuteNonQuery();
//      }
//    }
//    #endregion

//    #region Settings

//    /// <summary>
//    /// Loads the settings from the provider.
//    /// </summary>
//    /// <returns></returns>
//    public override StringDictionary LoadSettings()
//    {
//      StringDictionary dic = new StringDictionary();
//      using (SqlConnection conn = new SqlConnection(ConnectionString))
//      {
//        string sqlQuery = "SELECT SettingName, SettingValue FROM be_Settings";
//        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
//        {
//          conn.Open();
//          using (SqlDataReader rdr = cmd.ExecuteReader())
//          {
//            while (rdr.Read())
//            {
//              string name = rdr.GetString(0);
//              string value = rdr.GetString(1);

//              dic.Add(name, value);
//            }
//          }
//        }
//      }

//      return dic;
//    }

//    /// <summary>
//    /// Saves the settings to the provider.
//    /// </summary>
//    /// <param name="settings"></param>
//    public override void SaveSettings(StringDictionary settings)
//    {
//      if (settings == null)
//        throw new ArgumentNullException("settings");

//      using (SqlConnection conn = new SqlConnection(ConnectionString))
//      {
//        string sqlQuery = "DELETE FROM be_Settings";
//        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
//        {
//          conn.Open();
//          cmd.ExecuteNonQuery();

//          foreach (string key in settings.Keys)
//          {
//            sqlQuery = "INSERT INTO be_Settings (SettingName, SettingValue) " +
//                        "VALUES (@name, @value)";
//            cmd.CommandText = sqlQuery;
//            cmd.Parameters.Clear();
//            cmd.Parameters.Add(new SqlParameter("@name", key));
//            cmd.Parameters.Add(new SqlParameter("@value", settings[key]));
//            cmd.ExecuteNonQuery();
//          }
//        }
//      }

//    }

//    #endregion


//    /// <summary>
//    /// Initializes the provider
//    /// </summary>
//    /// <param name="name">Configuration name</param>
//    /// <param name="config">Configuration settings</param>
//    public override void Initialize(string name, NameValueCollection config)
//    {
//      if (config == null)
//      {
//        throw new ArgumentNullException("config");
//      }

//      if (String.IsNullOrEmpty(name))
//      {
//        name = "MSSQLUnReadProvider";
//      }

//      if (String.IsNullOrEmpty(config["description"]))
//      {
//        config.Remove("description");
//        config.Add("description", "MSSQL Note Provider");
//      }

//      base.Initialize(name, config);

//      if (config["connectionStringName"] == null)
//      {
//        // default to ingNote
//        config["connectionStringName"] = "ingNote";
//      }

//      connStringName = config["connectionStringName"].ToString();
//      config.Remove("connectionStringName");
//    }

//    /// <summary>
//    /// Connection string
//    /// </summary>
//    public string ConnectionString
//    {
//      get
//      {
//        return ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
//      }
//    }

//    /// <summary>
//    /// Handles Opening the SQL Connection
//    /// </summary>
//    private bool OpenConnection()
//    {
//      bool result = false;

//      // Initial if needed
//      if (providerConn == null)
//        providerConn = new SqlConnection(ConnectionString);
//      // Open it if needed
//      if (providerConn.State == System.Data.ConnectionState.Closed)
//      {
//        result = true;
//        providerConn.Open();
//      }

//      return result;
//    }

//        #region IDisposable Members

//        private void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                this.providerConn.Dispose();
//            }
//        }

//        /// <summary>
//        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
//        /// </summary>
//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//        #endregion
//    }
//}
