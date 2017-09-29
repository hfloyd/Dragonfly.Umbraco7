namespace Dragonfly.Umbraco7Helpers
{
    using System;
    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web;

    public static class Development
    {
        private const string ThisClassName = "Dragonfly.Umbraco7Helpers.Development";

        private static IFileService umbFileService = ApplicationContext.Current.Services.FileService;
        private static IContentService umbContentService = ApplicationContext.Current.Services.ContentService;

        /// <summary>
        /// Get the Alias of a template from its ID. If the Id is null or zero, "NONE" will be returned.
        /// </summary>
        /// <param name="TemplateId">
        /// The template id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetTemplateAlias(int? TemplateId)
        {
            //var umbFileService = ApplicationContext.Current.Services.FileService;
            string TemplateAlias;

            if (TemplateId == 0 | TemplateId == null)
            {
                TemplateAlias = "NONE";
            }
            else
            {
                var LookupTemplate = umbFileService.GetTemplate(Convert.ToInt32(TemplateId));
                TemplateAlias = LookupTemplate.Alias;
            }

            return TemplateAlias;
        }

        public static string NodePath(IPublishedContent UmbContentNode, string Separator = " » ")
        {
            string nodePathString = string.Empty;

            try
            {
                string pathIdsCSV = UmbContentNode.Path;
                nodePathString = NodePathFromPathIdsCSV(pathIdsCSV, Separator);
            }
            catch (Exception ex)
            {
                var functionName = string.Format("{0}.NodePath", ThisClassName);
                var errMsg = string.Format(
                    "ERROR in {0} for node #{1} ({2}). [{3}]",
                    functionName,
                    UmbContentNode.Id.ToString(),
                    UmbContentNode.Name,
                    ex.Message);
                LogHelper.Error<string>(errMsg, ex);

                var returnMsg = string.Format("Unable to generate node path. (ERROR:{0})", ex.Message);
                return returnMsg;
            }

            return nodePathString;
        }

        public static string NodePath(IContent UmbContentNode, string Separator = " » ")
        {
            string nodePathString = string.Empty;

            try
            {
                string pathIdsCSV = UmbContentNode.Path;
                nodePathString = NodePathFromPathIdsCSV(pathIdsCSV, Separator);
            }
            catch (Exception ex)
            {
                var functionName = string.Format("{0}.NodePath", ThisClassName);
                var errMsg = string.Format(
                    "ERROR in {0} for node #{1} ({2}). [{3}]",
                    functionName,
                    UmbContentNode.Id.ToString(),
                    UmbContentNode.Name,
                    ex.Message);
                LogHelper.Error<string>(errMsg, ex);

                var returnMsg = string.Format("Unable to generate node path. (ERROR:{0})", ex.Message);
                return returnMsg;
            }

            return nodePathString;
        }

        private static string NodePathFromPathIdsCSV(string PathIdsCSV, string Separator = " » ")
        {
            string NodePathString = string.Empty;

            string[] PathIdsArray = PathIdsCSV.Split(',');

            foreach (var sId in PathIdsArray)
            {
                if (sId != "-1")
                {
                    IContent GetNode = umbContentService.GetById(Convert.ToInt32(sId));
                    string NodeName = GetNode.Name;
                    NodePathString = string.Concat(NodePathString, Separator, NodeName);
                }
            }

            return NodePathString.TrimStart(Separator);

        }

        /// <summary>
        /// Lookup a descendant page in the site by its DocType
        /// </summary>
        /// <param name="SiteRootNodeId">ex: model.Site.Id</param>
        /// <param name="DoctypeAlias">Name of the Doctype to serach for</param>
        /// <param name="SiteRootDocTypeAlias">default="Homepage"</param>
        /// <returns>An IPublishedContent of the node, or NULL if not found. You can then cast to a strongly-typed model for the DocType (ex: new ContactUsPage(contactPage))</returns>
        public static IPublishedContent GetSitePage(int SiteRootNodeId, string DoctypeAlias, string SiteRootDocTypeAlias = "Homepage")
        {
            var umbracoHelper = new UmbracoHelper(UmbracoContext.Current);

            var xPathExpr = string.Format("root/{0}[@id={1}]//{2}", SiteRootDocTypeAlias, SiteRootNodeId, DoctypeAlias);

            var page = umbracoHelper.ContentSingleAtXPath(xPathExpr);
            if (page.Id > 0)
            {
                return page as IPublishedContent;
            }
            else
            {
                return null;
            }
        }
    }
}
