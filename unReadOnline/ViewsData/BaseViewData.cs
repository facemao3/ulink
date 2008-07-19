using unReadOnline.Models.Entitys;

namespace unReadOnline.ViewsData
{
    public abstract class BaseViewData
    {
        /// <summary>
        /// 当前用户名
        /// </summary>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// 用户是否已验证
        /// </summary>
        public bool IsAuthenticated
        {
            get;
            set;
        }

        /// <summary>
        /// 分类列表
        /// </summary>
        public Category[] Categories
        {
            get;
            set;
        }

        /// <summary>
        /// 搜索字符串
        /// </summary>
        public string SearchQuery
        {
            get;
            set;
        }
    }
}
