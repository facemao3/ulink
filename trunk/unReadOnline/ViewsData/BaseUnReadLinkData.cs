using System;
using unReadOnline.Models.Entitys;
using System.Collections.Generic;

namespace unReadOnline.ViewsData
{
    public class BaseUnReadLinkData : BaseViewData
    {
        /// <summary>
        /// 每页显示未读链接的条数
        /// </summary>
        public int UnReadLinkPerPage
        {
            get;
            set;
        }

        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage
        {
            get;
            set;
        }

        /// <summary>
        /// 未读链接的列表
        /// </summary>
        public List<UnReadLink> UnReadLinks
        {
            get;
            set;
        }

        /// <summary>
        /// 未读链接的总条数
        /// </summary>
        public int TotalUnReadLinks
        {
            get;
            set;
        }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                if ((TotalUnReadLinks == 0) || (UnReadLinkPerPage == 0))
                {
                    return 1;
                }

                if ((TotalUnReadLinks % UnReadLinkPerPage) == 0)
                {
                    return (TotalUnReadLinks / UnReadLinkPerPage);
                }
                else
                {
                    double result = (TotalUnReadLinks / UnReadLinkPerPage);

                    result = Math.Ceiling(result);

                    return (Convert.ToInt32(result) + 1);
                }
            }
        }
    }
}
