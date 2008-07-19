using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using System.Collections.Specialized;
using unReadOnline.Models.Entitys;

namespace unReadOnline.Models.Providers
{
    /// <summary>
    /// 所有数据提供者都应该继承的基类
    /// </summary>
    public abstract class UnReadProvider : ProviderBase
    {
        #region UnReadLink － 未读链接

        /// <summary>
        /// 获得指定ID的UnReadLink
        /// </summary>
        public abstract UnReadLink SelectUnReadLink(int id);

        /// <summary>
        /// 通过provider将一个新的UnReadLink存储到数据库中.
        /// </summary>
        public abstract void InsertUnReadLink(UnReadLink unReadLink);

        /// <summary>
        /// 通过provider更新一个存在的UnReadLink到指定的数据库中.
        /// </summary>
        public abstract void UpdateUnReadLink(UnReadLink unReadLink);

        /// <summary>
        /// 通过provider从指定的数据库中删除一条UnReadLink.
        /// </summary>
        public abstract void DeleteUnReadLink(int id);

        /// <summary>
        /// 从provider中检索指定用户下的指定分类中所有的UnReadLink并做为List返回.
        /// </summary>
        public abstract List<UnReadLink> GetUnReadLinksByUserAndCategory(int userId, int categoryId, int pageNum, int pageSiez, out int totalLinks);

        /// <summary>
        /// 从provider中检索指定用户中所有的UnReadLink并做为List返回.
        /// </summary>
        public abstract List<UnReadLink> GetUnReadLinksByUser(int userId, int pageNum, int pageSiez, out int totalLinks);

        /// <summary>
        /// 根据页码,每页显示的条数返回请求页的未读链接
        /// </summary>
        public abstract List<UnReadLink> GetUnReadLinks(int perPageCount, int pageNum, out int totalLinks);

        #endregion

        #region Category －分类

        /// <summary>
        /// 根据ID来检索分类.
        /// </summary>
        public abstract Category SelectCategory(int id);
        /// <summary>
        /// 插入一个新的分类.
        /// </summary>
        public abstract void InsertCategory(Category category);
        /// <summary>
        /// 通过provider更新数据库已经存在的一个分类.
        /// </summary>
        public abstract void UpdateCategory(Category category);
        /// <summary>
        /// 通过provider删除数据库中的一个分类.
        /// </summary>
        public abstract void DeleteCategory(int id);
        /// <summary>
        /// 根据用户ID返回指定用户的所有分类.
        /// </summary>
        public abstract List<Category> FillCategories(int userId);

        #endregion//Comment - 评论

        #region 暂时没用的

        /// <summary>
        /// 根据UnReadLink的Id来获取评论列表
        /// </summary>
        //public abstract List<Comment> GetComments(int id);


        // Settings
        /// <summary>
        /// Loads the settings from the provider.
        /// </summary>
        //public abstract StringDictionary LoadSettings();
        /// <summary>
        /// Saves the settings to the provider.
        /// </summary>
        //public abstract void SaveSettings(StringDictionary settings);

        #endregion

    }

    /// <summary>
    /// 所有注册的provider的集合.
    /// </summary>
    public class UnReadProviderCollection : ProviderCollection
    {
        /// <summary>
        /// Gets a provider by its name.
        /// </summary>
        public new UnReadProvider this[string name]
        {
            get { return (UnReadProvider)base[name]; }
        }

        /// <summary>
        /// Add a provider to the collection.
        /// </summary>
        public override void Add(ProviderBase provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            if (!(provider is UnReadProvider))
                throw new ArgumentException
                    ("无效的provider类型", "provider");

            base.Add(provider);
        }
    }
}
