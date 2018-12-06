using System.Collections.Generic;
using System.Linq;

namespace Dragonfly.Umbraco7Services
{
    using Umbraco.Core;
    using Umbraco.Core.Models;
    using Umbraco.Web;

    public class MediaFinderService
    {
        private UmbracoHelper _umbHelper;
        //private IEnumerable<IPublishedContent> _allMediaFlat;
        private IEnumerable<IPublishedContent> _mediaAtRoot;

        public MediaFinderService(UmbracoHelper umbHelper)
        {
            _umbHelper = umbHelper;
        }

        #region Get By Name

        public IEnumerable<IPublishedContent> GetImageByName(string ImageMediaName, int StartNodeId = 0)
        {
            return GetMediaByName(ImageMediaName, StartNodeId, Constants.Conventions.MediaTypes.Image);
        }

        public IEnumerable<IPublishedContent> GetFolderByName(string FolderName, int StartNodeId = 0)
        {
            return GetMediaByName(FolderName, StartNodeId, Constants.Conventions.MediaTypes.Folder);
        }

        public IEnumerable<IPublishedContent> GetFileByName(string FileMediaName, int StartNodeId = 0)
        {
            return GetMediaByName(FileMediaName, StartNodeId, Constants.Conventions.MediaTypes.File);
        }

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

            if (MediaTypeAlias!= "")
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

        public IEnumerable<IPublishedContent> GetImageByFilePath(string MediaFilePath, int StartNodeId = 0)
        {
            return GetMediaByFilePath(MediaFilePath, StartNodeId, Constants.Conventions.MediaTypes.Image);
        }

        public IEnumerable<IPublishedContent> GetFileByFilePath(string MediaFilePath, int StartNodeId = 0)
        {
            return GetMediaByFilePath(MediaFilePath, StartNodeId, Constants.Conventions.MediaTypes.File);
        }

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

        IEnumerable<IPublishedContent> GetInitMediaAtRoot()
        {
            if (!_mediaAtRoot.Any())
            {
                _mediaAtRoot = _umbHelper.TypedMediaAtRoot();
            }

            return _mediaAtRoot;
        }

        #endregion

    }

}
