using IPCLogger.ConfigurationService.Entities.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IPCLogger.ConfigurationService.Entities
{
    public enum PageType
    {
        Applications,
        Loggers,
        LoggerSettings,
        Users,        
    }

    public class PageModel : IEnumerable<PageModel>
    {
        private object _model;

        public string PageName;
        public PageType PageType;
        public string PagePath;
        public string Caption;

        public object Model
        {
            get
            {
                if (_model is JToken jToken)
                {
                    Type objType = Type.GetType(ModelType);
                    _model = JsonConvert.DeserializeObject(jToken.ToString(), objType);
                }
                return _model;
            }
            set
            {
                if (!(value is JToken))
                {
                    ModelType = value?.GetType().AssemblyQualifiedName;
                }
                _model = value;
            }
        }
        public string ModelType;

        public PageModel PreviousPageModel;

        private PageModel(PageType pageType, string pageName, string pagePath, string caption, 
            object model, PageModel previousPageModel)
        {
            PageType = pageType;
            PageName = pageName ?? pageType.ToString();
            Caption = caption ?? pageType.ToString();
            PagePath = pagePath ?? $"/{pageType.ToString().ToLower()}";
            Model = model;
            PreviousPageModel = previousPageModel;
        }

        private static PageModel GetPageModel(PageType pageType, object model)
        {
            return GetPageModel(pageType, null, null, null, model, null);
        }

        private static PageModel GetPageModel(PageType pageType, string pagePath, string caption, object model, PageModel previousPageModel)
        {
            return GetPageModel(pageType, null, pagePath, caption, model, previousPageModel);
        }

        private static PageModel GetPageModel(PageType pageType, string pageName, string pagePath, string caption,
            object model, PageModel previousPageModel)
        {
            pagePath = pagePath ?? $"/{pageType.ToString().ToLower()}";
            if (previousPageModel != null && previousPageModel.PagePath.Equals(pagePath, StringComparison.InvariantCultureIgnoreCase))
            {
                return previousPageModel;
            }

            return new PageModel(pageType, pageName, pagePath, caption, model, previousPageModel);
        }

        public static PageModel Applications(List<ApplicationModel> applications)
        {
            return GetPageModel(PageType.Applications, applications);
        }

        public static PageModel Loggers(ApplicationModel application, List<DeclaredLoggerModel> declaredLoggers, 
            PageModel previousPageModel)
        {
            string pagePath = $"/applications/{application.Id}";
            return GetPageModel(PageType.Loggers, pagePath, application.ToString(), declaredLoggers, previousPageModel);
        }

        public static PageModel AddLogger(int applicationId, DeclaredLoggerModel logger, PageModel previousPageModel)
        {
            string pagePath = $"/applications/{applicationId}/loggers/{logger.Id}";
            return GetPageModel(PageType.LoggerSettings, pagePath, logger.ToString(), logger, previousPageModel);
        }

        public static PageModel LoggerSettings(int applicationId, DeclaredLoggerModel logger, PageModel previousPageModel)
        {
            string pagePath = $"/applications/{applicationId}/loggers/{logger.Id}/settings";
            return GetPageModel(PageType.LoggerSettings, pagePath, logger.ToString(), logger, previousPageModel);
        }

        public static PageModel Users(List<UserModel> users)
        {
            return GetPageModel(PageType.Users, users);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<PageModel> GetEnumerator()
        {
            PageModel pageModel = this;
            while (pageModel != null)
            {
                yield return pageModel;
                pageModel = pageModel.PreviousPageModel;
            }
        }

        public IEnumerable<PageModel> Reverse()
        {
            return this.Reverse<PageModel>().ToArray();
        }
    }
}
