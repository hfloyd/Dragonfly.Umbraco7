namespace Dragonfly.Umbraco7Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Models;
    using Umbraco.Core.Services;
    using Umbraco.Web;
    using HtmlAgilityPack;

    /// <summary>
    /// Development-related helpers (For working with Node Paths, Templates, DataTypes, Udis, etc.)
    /// </summary>
    public static class Development
    {
        private const string ThisClassName = "Dragonfly.Umbraco7Helpers.Development";

        private static IFileService umbFileService = ApplicationContext.Current.Services.FileService;
        private static IContentService umbContentService = ApplicationContext.Current.Services.ContentService;
        private static IMediaService umbMediaService = ApplicationContext.Current.Services.MediaService;

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

            var xPathExpr = String.Format("root/{0}[@id={1}]//{2}", SiteRootDocTypeAlias, SiteRootNodeId, DoctypeAlias);

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

        /// <summary>
        /// Return a list of Prevalues for a given DataType by Name
        /// </summary>
        /// <param name="DataTypeName"></param>
        /// <returns></returns>
        public static IEnumerable<PreValue> GetPrevaluesForDataType(string DataTypeName)
        {
            IEnumerable<PreValue> toReturn = new List<PreValue>();

            IDataTypeDefinition dataType = ApplicationContext.Current.Services.DataTypeService.GetDataTypeDefinitionByName(DataTypeName);

            if (dataType == null)
            {
                return toReturn;
            }

            PreValueCollection preValues = ApplicationContext.Current.Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dataType.Id);

            if (preValues == null)
            {
                return toReturn;
            }

            IDictionary<string, PreValue> tempDictionary = preValues.FormatAsDictionary();

            toReturn = tempDictionary.Select(n => n.Value);

            return toReturn;
        }

        #region Node Paths

        /// <summary>
        /// Gets a string-representation of the full Path to a Content Node in the Content Tree
        /// </summary>
        /// <param name="UmbContentNode">Content Node to get a Path for</param>
        /// <param name="Separator">string used to separate the parts of the path (default is  ») </param>
        /// <returns></returns>
        public static string NodePath(IPublishedContent UmbContentNode, string Separator = " » ")
        {
            string nodePathString = String.Empty;

            try
            {
                string pathIdsCSV = UmbContentNode.Path;
                nodePathString = NodePathFromPathIdsCSV(pathIdsCSV, Separator);
            }
            catch (Exception ex)
            {
                var functionName = String.Format("{0}.NodePath", ThisClassName);
                var errMsg = String.Format(
                    "ERROR in {0} for node #{1} ({2}). [{3}]",
                    functionName,
                    UmbContentNode.Id.ToString(),
                    UmbContentNode.Name,
                    ex.Message);
                LogHelper.Error<string>(errMsg, ex);

                var returnMsg = String.Format("Unable to generate node path. (ERROR:{0})", ex.Message);
                return returnMsg;
            }

            return nodePathString;
        }

        /// <summary>
        /// Gets a string-representation of the full Path to a Content Node in the Content Tree
        /// </summary>
        /// <param name="UmbContentNode">Content Node to get a Path for</param>
        /// <param name="Separator">string used to separate the parts of the path (default is  ») </param>
        /// <returns></returns>
        public static string NodePath(IContent UmbContentNode, string Separator = " » ")
        {
            string nodePathString = String.Empty;

            try
            {
                string pathIdsCSV = UmbContentNode.Path;
                nodePathString = NodePathFromPathIdsCSV(pathIdsCSV, Separator);
            }
            catch (Exception ex)
            {
                var functionName = String.Format("{0}.NodePath", ThisClassName);
                var errMsg = String.Format(
                    "ERROR in {0} for node #{1} ({2}). [{3}]",
                    functionName,
                    UmbContentNode.Id.ToString(),
                    UmbContentNode.Name,
                    ex.Message);
                LogHelper.Error<string>(errMsg, ex);

                var returnMsg = String.Format("Unable to generate node path. (ERROR:{0})", ex.Message);
                return returnMsg;
            }

            return nodePathString;
        }

        /// <summary>
        /// Gets a string-representation of the full Path to a Content Node in the Content Tree
        /// </summary>
        /// <param name="PathIdsCSV">CSV string of path ids to parse</param>
        /// <param name="Separator">string used to separate the parts of the path (default is  ») </param>
        /// <returns></returns>
        private static string NodePathFromPathIdsCSV(string PathIdsCSV, string Separator = " » ")
        {
            string NodePathString = String.Empty;

            string[] PathIdsArray = PathIdsCSV.Split(',');

            foreach (var sId in PathIdsArray)
            {
                if (sId != "-1")
                {
                    IContent GetNode = umbContentService.GetById(Convert.ToInt32(sId));
                    string NodeName = GetNode.Name;
                    NodePathString = String.Concat(NodePathString, Separator, NodeName);
                }
            }

            return NodePathString.TrimStart(Separator);

        }

        /// <summary>
        /// Gets a string-representation of the full Path to a Media Node in the Media Tree
        /// </summary>
        /// <param name="UmbMediaNode">Media Node to get a Path for</param>
        /// <param name="Separator">string used to separate the parts of the path (default is  ») </param>
        /// <returns></returns>
        public static string MediaPath(IPublishedContent UmbMediaNode, string Separator = " » ")
        {

            string nodePathString = String.Empty;

            try
            {
                string pathIdsCSV = UmbMediaNode.Path;
                nodePathString = MediaNodePathFromPathIdsCSV(pathIdsCSV, Separator);
            }
            catch (Exception ex)
            {
                var functionName = String.Format("{0}.NodePath", ThisClassName);
                var errMsg = String.Format(
                    "ERROR in {0} for node #{1} ({2}). [{3}]",
                    functionName,
                    UmbMediaNode.Id.ToString(),
                    UmbMediaNode.Name,
                    ex.Message);
                LogHelper.Error<string>(errMsg, ex);

                var returnMsg = String.Format("Unable to generate node path. (ERROR:{0})", ex.Message);
                return returnMsg;
            }

            return nodePathString;
        }

        /// <summary>
        /// Gets a string-representation of the full Path to a Media Node in the Media Tree
        /// </summary>
        /// <param name="PathIdsCSV">CSV string of path ids to parse</param>
        /// <param name="Separator">string used to separate the parts of the path (default is  ») </param>
        /// <returns></returns>
        private static string MediaNodePathFromPathIdsCSV(string PathIdsCSV, string Separator = " » ")
        {
            string NodePathString = String.Empty;

            string[] PathIdsArray = PathIdsCSV.Split(',');

            foreach (var sId in PathIdsArray)
            {
                if (sId != "-1")
                {
                    IMedia GetNode = umbMediaService.GetById(Convert.ToInt32(sId));
                    string NodeName = GetNode.Name;
                    NodePathString = String.Concat(NodePathString, Separator, NodeName);
                }
            }

            return NodePathString.TrimStart(Separator);
        }

        #endregion

        #region Udi

        /// <summary>
        /// Converts a list of published content to a comma-separated string of UDI values suitable for using with the content service
        /// </summary>
        /// <param name="IPubsEnum">A collection of IPublishedContent</param>
        /// <param name="UdiType">UDI Type to use (document, media, etc) (use 'Umbraco.Core.Constants.UdiEntityType.' to specify)
        /// If excluded, will try to use the DocTypeAlias to determine the UDI Type</param>
        /// <returns>A CSV string of UID values eg. umb://document/56c0f0ef0ac74b58ae1cce16db1476af,umb://document/5cbac9249ffa4f5ab4f5e0db1599a75b</returns>
        public static string ToUdiCsv(this IEnumerable<IPublishedContent> IPubsEnum, string UdiType = "")
        {
            var list = new List<string>();
            if (IPubsEnum != null)
            {
                foreach (var publishedContent in IPubsEnum)
                {
                    if (publishedContent != null)
                    {
                        var udi = ToUdiString(publishedContent, UdiType);
                        list.Add(udi.ToString());
                    }
                }
            }
            return String.Join(",", list);
        }

        /// <summary>
        /// Converts an IPublishedContent to a UDI string suitable for using with the content service
        /// </summary>
        /// <param name="IPub">IPublishedContent</param>
        /// <param name="UdiType">UDI Type to use (document, media, etc) (use 'Umbraco.Core.Constants.UdiEntityType.' to specify)
        /// If excluded, will try to use the DocTypeAlias to determine the UDI Type</param>
        /// <returns></returns>
        public static string ToUdiString(this IPublishedContent IPub, string UdiType = "")
        {
            if (IPub != null)
            {
                var udiType = UdiType != "" ? UdiType : GetUdiType(IPub);
                var udi = Umbraco.Core.Udi.Create(udiType, IPub.GetKey());
                return udi.ToString();
            }
            else
            {
                return "";
            }
        }

        private static string GetUdiType(IPublishedContent PublishedContent)
        {
            var udiType = Umbraco.Core.Constants.UdiEntityType.Document;

            //if it's a known (default) media or member type, use that, otherwise, document is assumed
            switch (PublishedContent.DocumentTypeAlias)
            {
                case Constants.Conventions.MediaTypes.Image:
                    udiType = Umbraco.Core.Constants.UdiEntityType.Media;
                    break;
                case Constants.Conventions.MediaTypes.File:
                    udiType = Umbraco.Core.Constants.UdiEntityType.Media;
                    break;
                case Constants.Conventions.MediaTypes.Folder:
                    udiType = Umbraco.Core.Constants.UdiEntityType.Media;
                    break;
                case Constants.Conventions.MemberTypes.DefaultAlias:
                    udiType = Umbraco.Core.Constants.UdiEntityType.Member;
                    break;
            }

            return udiType;
        }

        #endregion

        #region Html
        /// <summary>
        /// Validates string as html
        /// </summary>
        /// <param name="OriginalHtml"></param>
        /// <returns>True if valid HTML, False if Invalid</returns>
        public static bool HtmlIsValid(this string OriginalHtml)
        {
            IEnumerable<HtmlParseError> validationErrors;
            return HtmlIsValid(OriginalHtml, out validationErrors);
        }

        /// <summary>
        /// Validates string as html, returns errors
        /// </summary>
        /// <param name="OriginalHtml"></param>
        /// <param name="ValidationErrors">Variable of type IEnumerable&lt;HtmlParseError&gt;</param>
        /// <returns></returns>
        public static bool HtmlIsValid(this string OriginalHtml, out IEnumerable<HtmlParseError> ValidationErrors)
        {
            if (!OriginalHtml.IsNullOrWhiteSpace())
            {
                HtmlDocument doc = new HtmlDocument();

                doc.LoadHtml(OriginalHtml);

                if (doc.ParseErrors.Any())
                {
                    //Invalid HTML
                    ValidationErrors = doc.ParseErrors;
                    return false;
                }
            }
            ValidationErrors = new List<HtmlParseError>();
            return true;
        }

        /// <summary>
        /// Removes all &lt;script&gt; tags from HTML
        /// </summary>
        /// <param name="OriginalHtml"></param>
        /// <param name="ReplaceWith">optional - text or HTML to replace the script tag with</param>
        /// <returns></returns>
        public static string StripScripts(this string OriginalHtml, string ReplaceWith = "")
        {
            if (!OriginalHtml.IsNullOrWhiteSpace())
            {
                HtmlDocument doc = new HtmlDocument()
                {
                    OptionWriteEmptyNodes = true
                };

                doc.LoadHtml(OriginalHtml);

                var scriptNodes = doc.DocumentNode.SelectNodes("//script");
                if (scriptNodes != null)
                {
                    var badNodes = scriptNodes.ToArray();
                    if (ReplaceWith != "")
                    {
                        try
                        {
                            foreach (var node in badNodes)
                            {
                                HtmlNode replacementNode = doc.CreateTextNode(ReplaceWith);
                                node.ParentNode.ReplaceChild(replacementNode, node);
                            }
                        }
                        catch (Exception e)
                        {
                            LogHelper.Error<string>($"Error in Development.StripScripts() for HTML: /n/r {OriginalHtml}", e);
                            throw;
                        }
                    }
                    else
                    {
                        //Just remove
                        foreach (var node in badNodes)
                        {
                            node.Remove();
                        }
                    }

                    return doc.DocumentNode.InnerHtml.ToString();
                }
                else
                {
                    //No scripts, just return original
                    return OriginalHtml;
                }
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// Removes all &lt;iframe&gt; tags from HTML
        /// </summary>
        /// <param name="OriginalHtml"></param>
        /// <param name="ReplaceWith">optional - text or HTML to replace the script tag with</param>
        /// <returns></returns>
        public static string StripIframes(this string OriginalHtml, string ReplaceWith = "")
        {
            if (!OriginalHtml.IsNullOrWhiteSpace())
            {
                HtmlDocument doc = new HtmlDocument()
                {
                    OptionWriteEmptyNodes = true
                };

                doc.LoadHtml(OriginalHtml);

                var badNodes = doc.DocumentNode.SelectNodes("//iframe");
                if (badNodes != null)
                {
                    if (ReplaceWith != "")
                    {
                        try
                        {
                            foreach (var node in badNodes)
                            {
                                HtmlNode replacementNode = doc.CreateTextNode(ReplaceWith);
                                node.ParentNode.ReplaceChild(replacementNode, node);
                            }
                        }
                        catch (Exception e)
                        {
                            LogHelper.Error<string>($"Error in Development.StripScripts() for HTML: /n/r {OriginalHtml}", e);
                            throw;
                        }
                    }
                    else
                    {
                        //Just remove
                        foreach (var node in badNodes)
                        {
                            node.Remove();
                        }
                    }
                    return doc.DocumentNode.InnerHtml.ToString();
                }
                else
                {
                    //Nothing to remove, just return original
                    return OriginalHtml;
                }
            }
            else
            {
                return "";
            }
        }


        /// <summary>
        /// Convert HTML to an AMP-compatible version of HTML
        /// </summary>
        /// <param name="originalHtml"></param>
        /// <returns></returns>
        public static HtmlString ConvertToAmpHtml(this IHtmlString originalHtml)
        {
            //TODO Check against AMP documentation

            string originalGridHtmlStr = originalHtml.ToString();

            //Initial basic AMP replacements
            originalGridHtmlStr = originalGridHtmlStr.Replace("<img ", "<amp-img layout=\"responsive\" width=\"320\" height=\"200\" style=\"width:100%;height:auto\" ");
            originalGridHtmlStr = originalGridHtmlStr.Replace("<iframe ", "<amp-iframe layout=\"responsive\" sandbox=\"allow-scripts allow-same-origin allow-popups\" ");

            //more in-depth replacements/fixes
            var htmlDoc = new HtmlDocument()
            {
                OptionWriteEmptyNodes = true
            };
            htmlDoc.LoadHtml(originalGridHtmlStr);
            var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("html/body");
            bool isBodyContentOnly = bodyNode == null;

            //AMP-IMG fixes
            var imageTags = htmlDoc.DocumentNode.SelectNodes("//amp-img");
            if (imageTags != null)
            {
                foreach (var image in imageTags.ToList())
                {
                    image.Attributes.Remove("displaymode");
                }
            }

            //AMP-IFRAME fixes
            var iframeTags = htmlDoc.DocumentNode.SelectNodes("//amp-iframe");
            if (iframeTags != null)
            {
                foreach (var iframe in iframeTags.ToList())
                {

                    iframe.Attributes.Remove("mozallowfullscreen");
                    iframe.Attributes.Remove("webkitallowfullscreen");
                    iframe.Attributes.Remove("gesture");

                    iframe.Attributes.Remove("allowfullscreen");
                    iframe.SetAttributeValue("allowfullscreen", "");

                    //default: width="640" height="360"
                    var width = iframe.GetAttributeValue("width", string.Empty);
                    if (string.IsNullOrEmpty(width))
                    {
                        iframe.Attributes.Remove("width");
                        iframe.SetAttributeValue("width", "640");
                    }
                    var height = iframe.GetAttributeValue("height", string.Empty);
                    if (string.IsNullOrEmpty(height))
                    {
                        iframe.Attributes.Remove("height");
                        iframe.SetAttributeValue("height", "360");
                    }
                }
            }

            //SCRIPT fixes
            var scriptTags = htmlDoc.DocumentNode.SelectNodes("//script");
            if (scriptTags != null)
            {
                foreach (var script in scriptTags.ToList())
                {
                    //if it is an inline script, remove it
                    var src = script.GetAttributeValue("src", string.Empty);
                    if (string.IsNullOrEmpty(src))
                    {
                        script.Remove();
                    }
                }
            }

            //STYLE tag fixes
            //if it is in the BODY, remove it
            if (isBodyContentOnly)
            {
                var styleTags = htmlDoc.DocumentNode.SelectNodes("//style");
                if (styleTags != null)
                {
                    foreach (var tag in styleTags.ToList())
                    {
                        tag.Remove();
                    }
                }
            }
            else
            {
                var styleTags = bodyNode.SelectNodes("//style");
                if (styleTags != null)
                {
                    foreach (var tag in styleTags.ToList())
                    {
                        tag.Remove();
                    }
                }
            }

            //Fixes to A tags
            var aTags = htmlDoc.DocumentNode.SelectNodes("//a");
            if (aTags != null)
            {
                foreach (var tag in aTags.ToList())
                {
                    tag.Attributes.Remove("onclick");


                }
            }


            //Remove ALL style attributes from ALL Tags
            var allTagsWithAttributes = htmlDoc.DocumentNode.Descendants()
                .Where(x => x.NodeType == HtmlNodeType.Element && x.Attributes.Any()).ToList();

            foreach (var tag in allTagsWithAttributes.ToList())
            {
                tag.Attributes.Remove("style");
            }



            return new HtmlString(htmlDoc.DocumentNode.OuterHtml);
        }

        #endregion
    }

}
