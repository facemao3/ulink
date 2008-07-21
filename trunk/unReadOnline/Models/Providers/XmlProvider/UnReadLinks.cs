using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using unReadOnline.Models.Entitys;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Web.Hosting;

namespace unReadOnline.Models.Providers.XmlProvider
{
    /// <summary>
    /// 
    /// </summary>
    public partial class XmlProvider : UnReadProvider
    {
        #region 帮助方法
        private XElement unReadLinksXml;
        /// <summary>
        /// 加载XML文件
        /// </summary>
        private void LoadUnReadLinksXml()
        {
            if (unReadLinksXml == null)
            {
                string path = "~/App_Data/XML/UnReadLinks.xml";
                string fullyQualifiedPath = VirtualPathUtility.Combine
                (VirtualPathUtility.AppendTrailingSlash
                (HttpRuntime.AppDomainAppVirtualPath), path);

                string _XmlFileName = HostingEnvironment.MapPath(fullyQualifiedPath);
                FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.Write, _XmlFileName);
                permission.Demand();
                unReadLinksXml = XElement.Load(_XmlFileName);
            }
        }

        /// <summary>
        /// 保存结果到XML文件
        /// </summary>
        private void SaveToUnReadLinksXml()
        {
            if (unReadLinksXml != null)
            {
                string path = "~/App_Data/XML/UnReadLinks.xml";
                string fullyQualifiedPath = VirtualPathUtility.Combine
                (VirtualPathUtility.AppendTrailingSlash
                (HttpRuntime.AppDomainAppVirtualPath), path);

                string _XmlFileName = HostingEnvironment.MapPath(fullyQualifiedPath);
                FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.Write, _XmlFileName);
                permission.Demand();
                unReadLinksXml.Save(_XmlFileName);
            }
        }

        /// <summary>
        /// UnReadLink枚举,标识一些Xml文件用到的字段,预防写错
        /// </summary>
        enum uEnum
        {
            UnReadLink,
            LastId,
            TotalCount,
            Id,
            UserId,
            CategoryId,
            Title,
            Url,
            Description,
            CreateTime,
            IsForever,
            IsPublic
        }

        #endregion

        #region 未读链接 - 基本的CRUD

        /// <summary>
        /// 根据Id获取一条未读链接
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override UnReadLink SelectUnReadLink(int id)
        {
            try
            {
                LoadUnReadLinksXml();
                var tempUnReadLink = unReadLinksXml.Elements(uEnum.UnReadLink.ToString())
                    .Where(p => Convert.ToInt32(p.Attribute("Id").Value) == id)
                    .Select(p =>
                        new UnReadLink
                        {
                            Id = Convert.ToInt32(p.Attribute(uEnum.Id.ToString()).Value),
                            UserId = Convert.ToInt32(p.Element(uEnum.UserId.ToString()).Value),
                            CategoryId = Convert.ToInt32(p.Element(uEnum.CategoryId.ToString()).Value),
                            Title = p.Element(uEnum.Title.ToString()).Value,
                            Url = p.Element(uEnum.Url.ToString()).Value,
                            Description = p.Element(uEnum.Description.ToString()).Value,
                            DateCreated = DateTime.Parse(p.Element(uEnum.CreateTime.ToString()).Value),
                            IsForever = (bool)p.Element(uEnum.IsForever.ToString()),
                            IsPublic = (bool)p.Element(uEnum.IsPublic.ToString())
                        })
                     .First();
                if (tempUnReadLink != null)
                {
                    tempUnReadLink.MarkOld();
                    return tempUnReadLink;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return null;
        }


        /// <summary>
        /// 插入一条未读链接
        /// </summary>
        /// <param name="unReadLink"></param>
        public override void InsertUnReadLink(UnReadLink unReadLink)
        {
            LoadUnReadLinksXml();
            int newId = Convert.ToInt32(unReadLinksXml.Attribute(uEnum.LastId.ToString()).Value) + 1;
            XElement newUnReadLink = new XElement(uEnum.UnReadLink.ToString(),
                new XAttribute("Id", newId),
                new XElement(uEnum.UserId.ToString(), unReadLink.UserId),
                new XElement(uEnum.CategoryId.ToString(), unReadLink.CategoryId),
                new XElement(uEnum.Title.ToString(), unReadLink.Title),
                new XElement(uEnum.Url.ToString(), unReadLink.Url),
                new XElement(uEnum.Description.ToString(), unReadLink.Description),
                new XElement(uEnum.CreateTime.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")),
                new XElement(uEnum.IsForever.ToString(), unReadLink.IsForever),
                new XElement(uEnum.IsPublic.ToString(), unReadLink.IsPublic)
                );
            unReadLinksXml.SetAttributeValue(uEnum.LastId.ToString(), newId);
            unReadLinksXml.SetAttributeValue(uEnum.TotalCount.ToString(),
                Convert.ToInt32(unReadLinksXml.Attribute(uEnum.TotalCount.ToString()).Value) + 1);
            unReadLinksXml.AddFirst(newUnReadLink);
            SaveToUnReadLinksXml();
        }

        /// <summary>
        /// 更新一条未读链接
        /// </summary>
        /// <param name="unReadLink"></param>
        public override void UpdateUnReadLink(UnReadLink unReadLink)
        {
            LoadUnReadLinksXml();
            var tempUnReadLink = unReadLinksXml.Elements(uEnum.UnReadLink.ToString())
                .Where(p => Convert.ToInt32(p.Attribute("Id").Value) == unReadLink.Id)
                .First();
            tempUnReadLink.SetElementValue(uEnum.CategoryId.ToString(), unReadLink.CategoryId);
            tempUnReadLink.SetElementValue(uEnum.Title.ToString(), unReadLink.Title);
            tempUnReadLink.SetElementValue(uEnum.Url.ToString(), unReadLink.Url);
            tempUnReadLink.SetElementValue(uEnum.Description.ToString(), unReadLink.Description);
            tempUnReadLink.SetElementValue(uEnum.IsForever.ToString(), unReadLink.IsForever.ToString());
            tempUnReadLink.SetElementValue(uEnum.IsPublic.ToString(), unReadLink.IsPublic.ToString());

            SaveToUnReadLinksXml();
            unReadLink.MarkOld();
        }

        /// <summary>
        /// 删除一条未读链接
        /// </summary>
        /// <param name="id"></param>
        public override void DeleteUnReadLink(int id)
        {
            LoadUnReadLinksXml();
            var tempUnReadLink = unReadLinksXml.Elements(uEnum.UnReadLink.ToString())
                .Where(p => Convert.ToInt32(p.Attribute("Id").Value) == id)
                .First();
            tempUnReadLink.Remove();
            unReadLinksXml.SetAttributeValue(uEnum.TotalCount.ToString(),
                Convert.ToInt32(unReadLinksXml.Attribute(uEnum.TotalCount.ToString()).Value) - 1);
            SaveToUnReadLinksXml();
        }

        public override List<UnReadLink>  GetUnReadLinksByUserAndCategory(int userId, int categoryId, int pageNum, int pageSiez, out int totalLinks)
        {
            totalLinks = 0;
            return null;
        }

        #endregion

        #region 根据条件查找

        /// <summary>
        /// 显示在首页，必需IsPublic的。根据页码,每页显示的条数返回请求页的未读链接
        /// </summary>
        /// <param name="perPageCount">每页显示的条数</param>
        /// <param name="pageNum">当前页</param>
        /// <param name="totalLinks">总条数</param>
        /// <returns></returns>
        public override List<UnReadLink> GetUnReadLinks(int perPageCount, int pageNum, out int totalLinks)
        {
            LoadUnReadLinksXml();
            var tempUnReadLinks = unReadLinksXml.Elements(uEnum.UnReadLink.ToString())
                .Where(p => Convert.ToBoolean(p.Element(uEnum.IsPublic.ToString()).Value) == true)
                .Skip(perPageCount * (pageNum - 1))
                .Take(perPageCount)
                .Select(p => new UnReadLink
                {
                    Id = Convert.ToInt32(p.Attribute(uEnum.Id.ToString()).Value),
                    UserId = Convert.ToInt32(p.Element(uEnum.UserId.ToString()).Value),
                    CategoryId = Convert.ToInt32(p.Element(uEnum.CategoryId.ToString()).Value),
                    Title = p.Element(uEnum.Title.ToString()).Value,
                    Url = p.Element(uEnum.Url.ToString()).Value,
                    Description = p.Element(uEnum.Description.ToString()).Value,
                    DateCreated = DateTime.Parse(p.Element(uEnum.CreateTime.ToString()).Value),
                    IsForever = Convert.ToBoolean(p.Element(uEnum.IsForever.ToString()).Value),
                    IsPublic = Convert.ToBoolean(p.Element(uEnum.IsPublic.ToString()).Value)
                });
            totalLinks = Convert.ToInt32(unReadLinksXml.Attribute(uEnum.TotalCount.ToString()).Value);

            List<UnReadLink> list = new List<UnReadLink>();
            foreach (var unReadLink in tempUnReadLinks)
            {
                list.Add(unReadLink);
            }

            return list;
        }

        /// <summary>
        /// 从provider中检索指定用户中所有的UnReadLink并做为List返回.
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="pageNum"></param>
        /// <param name="pageSiez"></param>
        /// <param name="totalLinks"></param>
        /// <returns></returns>
        public override List<UnReadLink> GetUnReadLinksByUser(int userId, int pageNum, int pageSize, out int totalLinks)
        {
            LoadUnReadLinksXml();
            var tempUnReadLinks = unReadLinksXml.Elements(uEnum.UnReadLink.ToString())
                .Where(p => Convert.ToInt32(p.Element(uEnum.UserId.ToString()).Value) == userId)
                .Skip(pageSize * (pageNum - 1))
                .Take(pageSize)
                .Select(p => new UnReadLink
                {
                    Id = Convert.ToInt32(p.Attribute(uEnum.Id.ToString()).Value),
                    UserId = Convert.ToInt32(p.Element(uEnum.UserId.ToString()).Value),
                    CategoryId = Convert.ToInt32(p.Element(uEnum.CategoryId.ToString()).Value),
                    Title = p.Element(uEnum.Title.ToString()).Value,
                    Url = p.Element(uEnum.Url.ToString()).Value,
                    Description = p.Element(uEnum.Description.ToString()).Value,
                    DateCreated = DateTime.Parse(p.Element(uEnum.CreateTime.ToString()).Value),
                    IsForever = Convert.ToBoolean(p.Element(uEnum.IsForever.ToString()).Value),
                    IsPublic = Convert.ToBoolean(p.Element(uEnum.IsPublic.ToString()).Value)
                });

            totalLinks = unReadLinksXml.Elements(uEnum.UnReadLink.ToString())
                .Where(p => Convert.ToInt32(p.Element(uEnum.UserId.ToString()).Value) == userId)
                .Count();

            List<UnReadLink> list = new List<UnReadLink>();
            foreach (var unReadLink in tempUnReadLinks)
            {
                list.Add(unReadLink);
            }

            return list;
        }

        #endregion
    }
}
