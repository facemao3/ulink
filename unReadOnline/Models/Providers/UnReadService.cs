using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration.Provider;
using System.Web.Configuration;
using unReadOnline.Models.Entitys;

namespace unReadOnline.Models.Providers
{
    /// <summary>
    /// 数据入口服务类，提供统一的接口
    /// </summary>
    public static class UnReadService
    {

        #region Provider model

        private static UnReadProvider _provider;
        private static UnReadProviderCollection _providers;
        private static object _lock = new object();

        /// <summary>
        /// 获取当前的provider.
        /// </summary>
        public static UnReadProvider Provider
        {
            get { return _provider; }
        }

        /// <summary>
        /// 获取所有已注册providers的集合.
        /// </summary>
        public static UnReadProviderCollection Providers
        {
            get { return _providers; }
        }

        /// <summary>
        /// 从web.config加载providers.
        /// </summary>
        private static void LoadProviders()
        {
            // Avoid claiming lock if providers are already loaded
            if (_provider == null)
            {
                lock (_lock)
                {
                    // Do this again to make sure _provider is still null
                    if (_provider == null)
                    {
                        // Get a reference to the <UnReadProvider> section
                        UnReadProviderSection section = (UnReadProviderSection)WebConfigurationManager.GetSection("unReadOnline/unReadProvider");

                        // Load registered providers and point _provider
                        // to the default provider
                        _providers = new UnReadProviderCollection();
                        ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(UnReadProvider));
                        _provider = _providers[section.DefaultProvider];

                        if (_provider == null)
                            throw new ProviderException("无法加载默认的UnReadProvider");
                    }
                }
            }
        }

        #endregion

        #region UnReadLinks

        /// <summary>
        /// Returns a UnReadLink based on the specified id.
        /// </summary>
        public static UnReadLink SelectUnReadLink(int id)
        {
            LoadProviders();
            return _provider.SelectUnReadLink(id);
        }

        /// <summary>
        /// Persists a new UnReadLink in the current provider.
        /// </summary>
        public static void InsertUnReadLink(UnReadLink unReadLink)
        {
            LoadProviders();
            _provider.InsertUnReadLink(unReadLink);
        }

        /// <summary>
        /// Updates an exsiting UnReadLink.
        /// </summary>
        public static void UpdateUnReadLink(UnReadLink unReadLink)
        {
            LoadProviders();
            _provider.UpdateUnReadLink(unReadLink);
        }

        /// <summary>
        /// Deletes the specified UnReadLink from the current provider.
        /// </summary>
        public static void DeleteUnReadLink(int id)
        {
            LoadProviders();
            _provider.DeleteUnReadLink(id);
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
            LoadProviders();
            return _provider.GetUnReadLinksByUser(userId, pageNum, pageSize, out totalLinks);
        }

        /// <summary>
        /// 根据页码,每页显示的条数返回请求页的未读链接
        /// </summary>
        /// <param name="perPageCount">每页显示的条数</param>
        /// <param name="pageNum">当前页</param>
        /// <param name="totalLinks">总条数</param>
        /// <returns></returns>
        public static List<UnReadLink> GetUnReadLinks(int perPageCount, int pageNum, out int totalLinks)
        {
            LoadProviders();
            return _provider.GetUnReadLinks(perPageCount, pageNum, out totalLinks);
        }

        #endregion

        #region Categorys

        /// <summary>
        /// Returns a Category based on the specified id.
        /// </summary>
        public static Category SelectCategory(int id)
        {
            LoadProviders();
            return _provider.SelectCategory(id);
        }

        /// <summary>
        /// Persists a new Category in the current provider.
        /// </summary>
        public static void InsertCategory(Category category)
        {
            LoadProviders();
            _provider.InsertCategory(category);
        }

        /// <summary>
        /// Updates an exsiting Category.
        /// </summary>
        public static void UpdateCategory(Category category)
        {
            LoadProviders();
            _provider.UpdateCategory(category);
        }

        /// <summary>
        /// Deletes the specified Category from the current provider.
        /// </summary>
        public static void DeleteCategory(int id)
        {
            LoadProviders();
            _provider.DeleteCategory(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<Category> FillCategorys(int userId)
        {
            LoadProviders();
            return _provider.FillCategories(userId);
        }

        #endregion

        //#region Comment

        ///// <summary>
        ///// 根据Note的Id来获取评论列表
        ///// </summary>
        ///// <param name="id">Note的Id</param>
        //public static List<Comment> GetComments(int id)
        //{
        //    LoadProviders();
        //    return new List<Comment>();
        //}

        //#endregion

        //#region Settings

        ///// <summary>
        ///// Loads the settings from the provider and returns
        ///// them in a StringDictionary for the NoteSettings class to use.
        ///// </summary>
        //public static System.Collections.Specialized.StringDictionary LoadSettings()
        //{
        //    LoadProviders();
        //    return _provider.LoadSettings();
        //}

        ///// <summary>
        ///// Save the settings to the current provider.
        ///// </summary>
        //public static void SaveSettings(System.Collections.Specialized.StringDictionary settings)
        //{
        //    LoadProviders();
        //    _provider.SaveSettings(settings);
        //}

        //#endregion
    }
}
