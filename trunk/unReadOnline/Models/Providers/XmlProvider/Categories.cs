#region Using

using System;
using System.Xml;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Collections.Specialized;

using unReadOnline.Models.Entitys;
using System.Linq;
using System.Xml.Linq;

#endregion

namespace unReadOnline.Models.Providers.XmlProvider
{
    /// <summary>
    /// A storage provider for unReadOnline that uses XML files.
    /// <remarks>
    /// To build another provider, you can just copy and modify
    /// this one. Then add it to the web.config's ingNote section.
    /// </remarks>
    /// </summary>
    public partial class XmlProvider : UnReadProvider
    {
        #region 帮助方法
        private XElement categoryXml;
        /// <summary>
        /// 加载XML文件
        /// </summary>
        private void LoadcategoryXml()
        {
            if (categoryXml == null)
            {
                string path = System.Web.HttpRuntime.AppDomainAppPath + "/App_Data/XML/Categorys.xml";
                categoryXml = XElement.Load(path);
            }
        }

        /// <summary>
        /// 保存结果到XML文件
        /// </summary>
        private void SaveToXml()
        {
            if (categoryXml != null)
            {
                string path = System.Web.HttpRuntime.AppDomainAppPath + "/App_Data/XML/Categorys.xml";
                categoryXml.Save(path);
            }
        }
        #endregion

        #region Categories

        /// <summary>
        /// 通过分类的Id来获取一个分类
        /// </summary>
        /// <param name="id">The category's int.</param>
        /// <returns>A matching Category</returns>
        public override Category SelectCategory(int id)
        {
            LoadcategoryXml();
            var tempCategory = categoryXml.Elements("Category")
                .Where(p => Convert.ToInt32(p.Attribute("Id").Value) == id)
                .Select(p =>
                    new Category
                    {
                        Id = Convert.ToInt32(p.Attribute("Id").Value),
                        UserId = Convert.ToInt32(p.Element("UserId").Value),
                        Name = p.Element("Name").Value,
                        Description = p.Element("Description").Value
                    })
                 .First();
            if (tempCategory != null)
            {
                tempCategory.MarkOld();
                return tempCategory;
            }

            return null;
        }

        /// <summary>
        /// 插入一个分类
        /// </summary>
        /// <param name="category">Must be a valid Category object.</param>
        public override void InsertCategory(Category category)
        {
            LoadcategoryXml();
            int newId = Convert.ToInt32(categoryXml.Attribute("LastId").Value) + 1;
            XElement newCategory = new XElement("Category",
                new XAttribute("Id", newId),
                new XElement("UserId", category.UserId),
                new XElement("Name", category.Name),
                new XElement("Description", category.Description)
                );
            categoryXml.SetAttributeValue("LastId", newId);
            categoryXml.Add(newCategory);
            SaveToXml();
        }

        /// <summary>
        /// 更新一个分类
        /// </summary>
        /// <param name="category">Must be a valid Category object.</param>
        public override void UpdateCategory(Category category)
        {

            LoadcategoryXml();
            var tempCategory = categoryXml.Elements("Category")
                .Where(p => Convert.ToInt32(p.Attribute("Id").Value) == category.Id)
                .First();
            tempCategory.SetElementValue("Name", category.Name);
            tempCategory.SetElementValue("UserId", category.UserId);
            tempCategory.SetElementValue("Description", category.Description);

            SaveToXml();
            category.MarkOld();
        }

        /// <summary>
        /// 删除一个分类
        /// </summary>
        /// <param name="category">Must be a valid Category object.</param>
        public override void DeleteCategory(int id)
        {
            LoadcategoryXml();
            var tempCategory = categoryXml.Elements("Category")
                .Where(p => Convert.ToInt32(p.Attribute("Id").Value) == id)
                .First();
            tempCategory.Remove();

            SaveToXml();

        } 
        
        
        public override List<Category> FillCategories(int userId)
        {
            return null;
        }

        #region 暂时没用到的
        ///// <summary>
        ///// Fills an unsorted list of categories.
        ///// </summary>
        ///// <returns>A List&lt;Category&gt; of all Categories.</returns>
        //public override List<Category> FillCategories()
        //{

        //    string fileName = _Folder + "categories.xml";
        //    if (!File.Exists(fileName))
        //        return null;

        //    XmlDocument doc = new XmlDocument();
        //    doc.Load(fileName);
        //    List<Category> categories = new List<Category>();

        //    foreach (XmlNode node in doc.SelectNodes("categories/category"))
        //    {
        //        Category category = new Category();

        //        category.Id = new int(node.Attributes["id"].InnerText);
        //        category.Title = node.InnerText;
        //        if (node.Attributes["description"] != null)
        //            category.Description = node.Attributes["description"].InnerText;
        //        else
        //            category.Description = string.Empty;

        //        categories.Add(category);
        //        category.MarkOld();
        //    }

        //    return categories;
        //}
        #endregion

        #endregion
       
    }
}
