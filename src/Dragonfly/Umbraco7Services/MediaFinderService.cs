namespace Dragonfly.Umbraco7Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Web;

    public class MediaFinderService
    {
        private UmbracoHelper _umbHelper;
        //private IEnumerable<IPublishedContent> _allMediaFlat;
        private IEnumerable<IPublishedContent> _mediaAtRoot;

        /// <summary>
        /// Service to retrieve Media Nodes via various search methods
        /// </summary>
        /// <param name="umbHelper">UmbracoHelper passed-in</param>
        public MediaFinderService(UmbracoHelper UmbHelper)
        {
            var emptyList = new List<IPublishedContent>();

            CtorMediaFinderService(UmbHelper, emptyList);
        }

        /// <summary>
        /// Service to retrieve Media Nodes via various search methods
        /// </summary>
        /// <param name="umbHelper">UmbracoHelper passed-in</param>
        public MediaFinderService(UmbracoHelper UmbHelper, IEnumerable<IPublishedContent> MediaAtRoot)
        {
            CtorMediaFinderService(UmbHelper, MediaAtRoot);
        }

        private void CtorMediaFinderService(UmbracoHelper UmbHelper, IEnumerable<IPublishedContent> MediaAtRoot)
        {
            if (UmbHelper != null)
            {
                _umbHelper = UmbHelper;
            }
            else
            {
                //TODO create custom exception
                var msg = "Unable to Initialize 'MediaFinderService' - UmbracoHelper is NULL.";
                throw new NullReferenceException(msg);
            }

            _mediaAtRoot = MediaAtRoot;
        }

        public bool HasRootMedia()
        {
            return _mediaAtRoot.Any();
        }

        #region Get By Name

        /// <summary>
        /// Lookup Media Image by Node Name
        /// </summary>
        /// <param name="ImageMediaName">Name to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetImageByName(string ImageMediaName, int StartNodeId = 0)
        {
            return GetMediaByName(ImageMediaName, StartNodeId, Constants.Conventions.MediaTypes.Image);
        }

        /// <summary>
        /// Lookup Media Folder by Node Name
        /// </summary>
        /// <param name="FolderName">Name to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetFolderByName(string FolderName, int StartNodeId = 0)
        {
            return GetMediaByName(FolderName, StartNodeId, Constants.Conventions.MediaTypes.Folder);
        }

        /// <summary>
        /// Lookup Media File by Node Name
        /// </summary>
        /// <param name="FileMediaName">Name to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetFileByName(string FileMediaName, int StartNodeId = 0)
        {
            return GetMediaByName(FileMediaName, StartNodeId, Constants.Conventions.MediaTypes.File);
        }

        /// <summary>
        /// Lookup Media Node by Node Name
        /// </summary>
        /// <param name="MediaName">Name to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <param name="MediaTypeAlias">Alias of MediaType to return</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetMediaByName(string MediaName, int StartNodeId = 0, string MediaTypeAlias = "")
        {
            var allMediaList = new List<IPublishedContent>();

            if (StartNodeId > 0)
            {
                var startMedia = _umbHelper.TypedMedia(StartNodeId);
                allMediaList.AddRange(FindDescendantsByName(startMedia, MediaName));
            }
            else
            {
                var rootMedia = GetInitMediaAtRoot().ToList();

                if (rootMedia.Any())
                {
                    foreach (var mediaRoot in rootMedia)
                    {
                        allMediaList.AddRange(FindDescendantsByName(mediaRoot, MediaName));
                    }
                }
            }

            if (MediaTypeAlias != "")
            {
                var limitedMediaList = allMediaList.Where(n => n.DocumentTypeAlias == MediaTypeAlias);
                return limitedMediaList;
            }
            else
            {
                return allMediaList;
            }

        }

        private IEnumerable<IPublishedContent> FindDescendantsByName(IPublishedContent StartMedia, string MediaName)
        {
            //var mediaList = new List<IPublishedContent>();

            return StartMedia.DescendantsOrSelf().Where(n => n.Name == MediaName);
        }

        #endregion

        #region Get By File Path

        /// <summary>
        /// Lookup Media Image by File Path
        /// </summary>
        /// <param name="MediaFilePath">File Path to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetImageByFilePath(string MediaFilePath, int StartNodeId = 0)
        {
            return GetMediaByFilePath(MediaFilePath, StartNodeId, Constants.Conventions.MediaTypes.Image);
        }

        /// <summary>
        /// Lookup Media File by File Path
        /// </summary>
        /// <param name="MediaFilePath">File Path to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetFileByFilePath(string MediaFilePath, int StartNodeId = 0)
        {
            return GetMediaByFilePath(MediaFilePath, StartNodeId, Constants.Conventions.MediaTypes.File);
        }

        /// <summary>
        /// Lookup Media Node by File Path
        /// </summary>
        /// <param name="MediaFilePath">File Path to search for</param>
        /// <param name="StartNodeId">ID of MediaNode to limit search to descendants</param>
        /// <param name="MediaTypeAlias">Alias of MediaType to return</param>
        /// <returns></returns>
        public IEnumerable<IPublishedContent> GetMediaByFilePath(string MediaFilePath, int StartNodeId = 0, string MediaTypeAlias = "")
        {
            var allMediaList = new List<IPublishedContent>();

            if (StartNodeId > 0)
            {
                var startMedia = _umbHelper.TypedMedia(StartNodeId);
                allMediaList.AddRange(FindDescendantsByFilePath(startMedia, MediaFilePath));
            }
            else
            {
                var rootMedia = GetInitMediaAtRoot().ToList();

                if (rootMedia.Any())
                {
                    foreach (var mediaRoot in rootMedia)
                    {
                        allMediaList.AddRange(FindDescendantsByFilePath(mediaRoot, MediaFilePath));
                    }
                }
            }

            if (MediaTypeAlias != "")
            {
                var limitedMediaList = allMediaList.Where(n => n.DocumentTypeAlias == MediaTypeAlias);
                return limitedMediaList;
            }
            else
            {
                return allMediaList;
            }

        }

        private IEnumerable<IPublishedContent> FindDescendantsByFilePath(IPublishedContent StartMedia, string MediaPath)
        {
            //var mediaList = new List<IPublishedContent>();

            return StartMedia.DescendantsOrSelf().Where(n => n.GetPropertyValue<string>("umbracoFile") == MediaPath);
        }

        #endregion

        #region GetInit

        private IEnumerable<IPublishedContent> GetInitMediaAtRoot()
        {
            if (!_mediaAtRoot.Any())
            {
                _mediaAtRoot = _umbHelper.TypedMediaAtRoot();

                if (!_mediaAtRoot.Any())
                {
                    //TODO create custom exception
                    var msg ="Unable to Initialize Media List - No Media at Root Found. Please Rebuild your Internal Examine Index";
                    throw new NullReferenceException(msg);
                }
            }

            return _mediaAtRoot;
        }

        #endregion

        #region Exceptions
        //TODO create custom Exceptions


        #endregion

    }

}
