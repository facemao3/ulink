using System;
using System.Web;
using System.Web.Security;
using unReadOnline.Models.Providers;

namespace unReadOnline.Models.Entitys
{
    /// <summary>
    /// 未读链接的分类,类似于收藏夹的分类
    /// </summary>
    public class Category : BusinessBase<Category,int>
    {
        #region 构造函数
        /// <summary>
        /// 构造函数,Id默认为－1，即未分配Id
        /// </summary>
        public Category()
        {
            this.Id = -1;
            DateCreated = DateTime.Now;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public Category(int id)
        {
            this.Id = id;
            DateCreated = DateTime.Now;
        }
        #endregion

        #region 属性

        /// <summary>
        /// 分类的名称
        /// </summary>
        public String Name
        {
            get;
            set;
        }

        /// <summary>
        /// 该分类所属用户的Id
        /// </summary>
        public int UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 该分类的简单介绍
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        #endregion

        #region 静态方法

        #endregion

        #region 基类的方法

        protected override void ValidationRules()
        {
            return;
        }

        protected override Category DataSelect(int id)
        {
            return UnReadService.SelectCategory(id);
        }

        protected override void DataUpdate()
        {
            if (Id != -1)
                UnReadService.UpdateCategory(this);
        }

        protected override void DataInsert()
        {
            UnReadService.InsertCategory(this);
        }

        protected override void DataDelete()
        {
            if (Id != -1)
                UnReadService.DeleteCategory(Id);
        }
        #endregion
    }
}
