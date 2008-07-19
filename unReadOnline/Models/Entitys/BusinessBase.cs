#region Using

using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Specialized;
using System.Globalization;

#endregion

namespace unReadOnline.Models.Entitys
{
	/// <summary>
	/// This is the base class from which most business objects will be derived. 
	/// To create a business object, inherit from this class.
	/// </summary>
	/// <typeparam name="TYPE">The type of the derived class.</typeparam>
	/// <typeparam name="KEY">The type of the Id property.</typeparam>
	[Serializable]
	public abstract class BusinessBase<TYPE, KEY> : IDataErrorInfo, INotifyPropertyChanged, IChangeTracking, IDisposable where TYPE : BusinessBase<TYPE, KEY>, new()
	{

		#region Properties

		private KEY _Id;
		/// <summary>
		/// 标识对象的唯一的标志
		/// </summary>
		public KEY Id
		{
			get { return _Id; }
			set { _Id = value; }
		}

		private DateTime _DateCreated = DateTime.MinValue;
		/// <summary>
		/// 对象创建的时间
		/// </summary>
		public DateTime DateCreated
		{
			get
			{
				//if (_DateCreated == DateTime.MinValue)
					return _DateCreated;

				//return _DateCreated.AddHours(BlogSettings.Instance.Timezone);
			}
			set 
			{
				if (_DateCreated != value) MarkChanged("DateCreated");
				_DateCreated = value; 
			}
		}

		private DateTime _DateModified = DateTime.MinValue;
		/// <summary>
		/// 实体被修改的时间
		/// </summary>
		public DateTime DateModified
		{
			get
			{
				//if (_DateModified == DateTime.MinValue)
					return _DateModified;

				//return _DateModified.AddHours(BlogSettings.Instance.Timezone);
			}
			set { _DateModified = value; }
		}

		#endregion

		#region IsNew, IsDeleted, IsChanged

		private bool _IsNew = true;
		/// <summary>
		/// 如果是一个新的对象则为真,如果是已存在的对象则为假
		/// </summary>
		public bool IsNew
		{
			get { return _IsNew; }
		}

		private bool _IsDeleted;
		/// <summary>
		/// 如果该对象已被标识为删除的,则返回真.
		/// </summary>
		public bool IsDeleted
		{
			get { return _IsDeleted; }
		}

		private bool _IsChanged = true;
		/// <summary>
		/// 表示该对象的数据是否被更改过.
		/// </summary>
		public virtual bool IsChanged
		{
			get { return _IsChanged; }
		}

		/// <summary>
		/// 将对象标识为已删除的. 
		/// 只有在调用Save()方法的时候对象才被真正删除.
		/// </summary>
		public void Delete()
		{
			_IsDeleted = true;
			_IsChanged = true;
		}

		private StringCollection _ChangedProperties = new StringCollection();
		/// <summary>
		/// 已经被标记为脏(dirty)的属性的集合
		/// </summary>
		protected virtual StringCollection ChangedProperties
		{
			get { return _ChangedProperties; }
		}

		/// <summary>
		/// 标记一个对象已为脏的,或者已被改变的
		/// </summary>
		/// <param name="propertyName">The name of the property to mark dirty.</param>
		protected virtual void MarkChanged(string propertyName)
		{
			_IsChanged = true;
			if (!_ChangedProperties.Contains(propertyName))
			{
				_ChangedProperties.Add(propertyName);
			}

			OnPropertyChanged(propertyName);
		}

		/// <summary>
		/// 标记一个对象为干净的, 
		/// 即表示不是脏的.
		/// </summary>
		public virtual void MarkOld()
		{
			_IsChanged = false;
			_IsNew = false;
			_ChangedProperties.Clear();
		}

		///// <summary>
		///// Check whether or not the specified property has been changed
		///// </summary>
		///// <param name="propertyName">The name of the property to check.</param>
		//protected bool IsPropertyDirty(string propertyName)
		//{
		//  return DirtyProperties.Contains(propertyName.ToLowerInvariant());
		//}

		///// <summary>
		///// Check whether or not the specified properties has been changed
		///// </summary>
		///// <param name="propertyNames">The names of the properties to check.</param>
		///// <returns>True if all of the specified properties have been changed.</returns>
		//protected bool IsPropertyDirty(string[] propertyNames)
		//{
		//  foreach (string name in propertyNames)
		//  {
		//    if (!DirtyProperties.Contains(name.ToUpperInvariant()))
		//    {
		//      return false;
		//    }
		//  }

		//  return true;
		//}

		#endregion

		#region Validation

		private StringDictionary _BrokenRules = new StringDictionary();

		/// <summary>
		/// 添加或者删除一条失效规则.
		/// </summary>
		/// <param name="propertyName">The name of the property.</param>
		/// <param name="errorMessage">The description of the error</param>
		/// <param name="isBroken">True if the validation rule is broken.</param>
		protected virtual void AddRule(string propertyName, string errorMessage, bool isBroken)
		{
			if (isBroken)
			{
				_BrokenRules[propertyName] = errorMessage;
			}
			else
			{
				if (_BrokenRules.ContainsKey(propertyName))
				{
					_BrokenRules.Remove(propertyName);
				}
			}
		}

		/// <summary>
		/// 通过添加额外的规则到失效规则中来增强业务规则
		/// </summary>
		protected abstract void ValidationRules();

		/// <summary>
		/// 返回该对象是否有效.
		/// </summary>
		public bool IsValid
		{
			get
			{
				ValidationRules();
				return this._BrokenRules.Count == 0;
			}
		}

		/// /// <summary>
		/// If the object has broken business rules, use this property to get access
		/// to the different validation messages.
		/// </summary>
		public virtual string ValidationMessage
		{
			get
			{
				if (!IsValid)
				{
					StringBuilder sb = new StringBuilder();
					foreach (string messages in this._BrokenRules.Values)
					{
						sb.AppendLine(messages);
					}

					return sb.ToString();
				}

				return string.Empty;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Loads an instance of the object based on the Id.
		/// </summary>
		/// <param name="id">The unique identifier of the object</param>
		public static TYPE Load(KEY id)
		{
			TYPE instance = new TYPE();
			instance = instance.DataSelect(id);

			if (instance != null)
			{
                instance.Id = id;
				instance.MarkOld();
				return instance;
			}

			return null;
		}

		/// <summary>
		/// Saves the object to the data store (inserts, updates or deletes).
		/// </summary>
		virtual public SaveAction Save()
		{
			if (!IsValid && !IsDeleted)
				throw new InvalidOperationException(ValidationMessage);

			if (IsDisposed && !IsDeleted)
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "You cannot save a disposed {0}", this.GetType().Name));

			if (IsChanged)
			{
				return Update();
			}

			return SaveAction.None;
		}

		/// <summary>
		/// 当对象是old和dirty 的时候Save方法会调用此方法来更新对象
		/// </summary>
		private SaveAction Update()
		{
			SaveAction action = SaveAction.None;

			if (this.IsDeleted)
			{
				if (!this.IsNew)
				{
					action = SaveAction.Delete;
					OnSaving(this, action);
					DataDelete();
				}
			}
			else
			{
				if (this.IsNew)
				{
					if (_DateCreated == DateTime.MinValue)
						_DateCreated = DateTime.Now;

					_DateModified = DateTime.Now;
					action = SaveAction.Insert;
					OnSaving(this, action);
					DataInsert();
				}
				else
				{
					this._DateModified = DateTime.Now; ;
					action = SaveAction.Update;
					OnSaving(this, action);
					DataUpdate();
				}

				MarkOld();
			}

			OnSaved(this, action);
			return action;
		}

		#endregion

		#region Data access

		/// <summary>
		/// Retrieves the object from the data store and populates it.
		/// </summary>
		/// <param name="id">The unique identifier of the object.</param>
		/// <returns>True if the object exists and is being populated successfully</returns>
		protected abstract TYPE DataSelect(KEY id);

		/// <summary>
		/// Updates the object in its data store.
		/// </summary>
		protected abstract void DataUpdate();

		/// <summary>
		/// Inserts a new object to the data store.
		/// </summary>
		protected abstract void DataInsert();

		/// <summary>
		/// Deletes the object from the data store.
		/// </summary>
		protected abstract void DataDelete();

		#endregion

		#region Equality overrides

		/// <summary>
		/// A uniquely key to identify this particullar instance of the class
		/// </summary>
		/// <returns>A unique integer value</returns>
		public override int GetHashCode()
		{
			return this.Id.GetHashCode();
		}

		/// <summary>
		/// Comapares this object with another
		/// </summary>
		/// <param name="obj">The object to compare</param>
		/// <returns>True if the two objects as equal</returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}

			if (obj.GetType() == this.GetType())
			{
				return obj.GetHashCode() == this.GetHashCode();
			}

			return false;
		}

		/// <summary>
		/// Checks to see if two business objects are the same.
		/// </summary>
		public static bool operator ==(BusinessBase<TYPE, KEY> first, BusinessBase<TYPE, KEY> second)
		{
			if (Object.ReferenceEquals(first, second))
			{
				return true;
			}

			if ((object)first == null || (object)second == null)
			{
				return false;
			}

			return first.GetHashCode() == second.GetHashCode();
		}

		/// <summary>
		/// Checks to see if two business objects are different.
		/// </summary>
		public static bool operator !=(BusinessBase<TYPE, KEY> first, BusinessBase<TYPE, KEY> second)
		{
			return !(first == second);
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the class is Saved
		/// </summary>
		public static event EventHandler<SavedEventArgs> Saved;
		/// <summary>
		/// Raises the Saved event.
		/// </summary>
		protected static void OnSaved(BusinessBase<TYPE, KEY> businessObject, SaveAction action)
		{
			if (Saved != null)
			{
				Saved(businessObject, new SavedEventArgs(action));
			}
		}

		/// <summary>
		/// Occurs when the class is Saved
		/// </summary>
		public static event EventHandler<SavedEventArgs> Saving;
		/// <summary>
		/// Raises the Saving event
		/// </summary>
		protected static void OnSaving(BusinessBase<TYPE, KEY> businessObject, SaveAction action)
		{
			if (Saving != null)
			{
				Saving(businessObject, new SavedEventArgs(action));
			}
		}

		/// <summary>
		/// Occurs when this instance is marked dirty. 
		/// It means the instance has been changed but not saved.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>
		/// Raises the PropertyChanged event safely.
		/// </summary>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		#region IDisposable

		private bool _IsDisposed;
		/// <summary>
		/// Gets or sets if the object has been disposed.
		/// <remarks>
		/// If the objects is disposed, it must not be disposed a second
		/// time. The IsDisposed property is set the first time the object
		/// is disposed. If the IsDisposed property is true, then the Dispose()
		/// method will not dispose again. This help not to prolong the object's
		/// life if the Garbage Collector.
		/// </remarks>
		/// </summary>
		protected bool IsDisposed
		{
			get { return _IsDisposed; }
		}

		/// <summary>
		/// Disposes the object and frees ressources for the Garbage Collector.
		/// </summary>
		/// <param name="disposing">If true, the object gets disposed.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (this.IsDisposed)
				return;

			if (disposing)
			{
				_ChangedProperties.Clear();
				_BrokenRules.Clear();
				_IsDisposed = true;
			}
		}

		/// <summary>
		/// Disposes the object and frees ressources for the Garbage Collector.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		#endregion

		#region IDataErrorInfo Members

		/// <summary>
		/// Gets an error message indicating what is wrong with this object.
		/// </summary>
		/// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
		public string Error
		{
			get { return ValidationMessage; }
		}

		/// <summary>
		/// Gets the <see cref="System.String"/> with the specified column name.
		/// </summary>
		public string this[string columnName]
		{
			get
			{
				if (_BrokenRules.ContainsKey(columnName))
					return _BrokenRules[columnName];

				return string.Empty;
			}
		}

		#endregion

		#region IChangeTracking Members

		/// <summary>
		/// Resets the object抯 state to unchanged by accepting the modifications.
		/// </summary>
		void IChangeTracking.AcceptChanges()
		{
			Save();
		}

		#endregion
	}
}