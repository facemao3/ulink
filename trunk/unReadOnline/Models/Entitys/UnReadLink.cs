using System;
using System.Web;
using System.Web.Security;
using unReadOnline.Models.Providers;
using System.Collections.Generic;

namespace unReadOnline.Models.Entitys
{
    /// <summary>
    /// 未读链接
    /// </summary>
    public class UnReadLink : BusinessBase<UnReadLink,int>
    {
        #region 构造函数
        /// <summary>
        /// 构造函数,默认Id为 -1,即未分配Id
        /// </summary>
        public UnReadLink()
        {
            this.Id = -1;
            this.IsForever = false;
            this.CategoryId = -1;
            this.DateCreated = DateTime.Now;
            this.IsPublic = true;

        }

        public UnReadLink(int id)
        {
            this.Id = id;
            this.IsForever = false;
            this.IsPublic = true;
            this.CategoryId = -1;
            this.DateCreated = DateTime.Now;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 该未读链接所属用户的Id
        /// </summary>
        public int UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 该未读链接所属的分类Id
        /// </summary>
        public int CategoryId
        {
            get;
            set;
        }

        /// <summary>
        /// 未读链接的题目,标题
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// 未读链接的Url
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        /// <summary>
        /// 未读链接的简单描述
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// 是否公开的,即其他人是否可以查看该未读链接
        /// </summary>
        public bool IsPublic
        {
            get;
            set;
        }

        /// <summary>
        /// 标识该未读链接是否永久保存,为假时超过保存期限系统会自动删除
        /// </summary>
        public bool IsForever
        {
            get;
            set;
        }
        #endregion

        #region 静态方法

        /// <summary>
        /// 根据页码,每页显示的条数返回请求页的未读链接
        /// </summary>
        /// <param name="perPageCount">每页显示的条数</param>
        /// <param name="pageNum">当前页</param>
        /// <param name="totalLinks">总条数</param>
        /// <returns></returns>
        public static List<UnReadLink> GetUnReadLinks(int perPageCount, int pageNum, out int totalLinks)
        {
            return UnReadService.GetUnReadLinks(perPageCount, pageNum, out totalLinks);
        }

        /// <summary>
        /// 从provider中检索指定用户中所有的UnReadLink并做为List返回.
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pageNum"></param>
        /// <param name="pageSiez"></param>
        /// <param name="totalLinks"></param>
        /// <returns></returns>
        public static List<UnReadLink> GetUnReadLinksByUser(int userId, int pageNum, int pageSize, out int totalLinks)
        {
            if (userId <= 0)
            {
                totalLinks = 0;
                return null;
            }

            return UnReadService.GetUnReadLinksByUser(userId, pageNum, pageSize, out totalLinks);
        }

        #endregion

        #region 基类的方法

        protected override void ValidationRules()
        {
            return;
        }

        protected override UnReadLink DataSelect(int id)
        {
            return UnReadService.SelectUnReadLink(id);
        }

        protected override void DataUpdate()
        {
            UnReadService.UpdateUnReadLink(this);
        }

        protected override void DataInsert()
        {
            UnReadService.InsertUnReadLink(this);
        }

        protected override void DataDelete()
        {
            UnReadService.DeleteUnReadLink(Id);
        }

        #endregion
    }
}
