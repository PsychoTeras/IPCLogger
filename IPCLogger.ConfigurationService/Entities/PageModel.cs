using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Entities
{
    public enum PageType
    {
        Loggers,
        Logger,
        Users,        
    }

    [JsonObject]
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

        public PageModel() { }

        public PageModel(PageType pageType, string pageName, string pagePath, string caption, 
            object model, PageModel previousPageModel)
        {
            PageType = pageType;
            PageName = pageName ?? pageType.ToString();
            Caption = caption ?? pageType.ToString();
            PagePath = pagePath ?? $"/{pageType.ToString().ToLower()}";
            Model = model;
            PreviousPageModel = previousPageModel;
        }

        public PageModel(PageType pageType, object model) : 
            this(pageType, null, null, null, model, null) { }

        public PageModel(PageType pageType, object model, string pagePath, PageModel previousPageModel) :
            this(pageType, null, pagePath, null, model, previousPageModel)
        { }

        public static PageModel Loggers(object model)
        {
            return new PageModel(PageType.Loggers, model);
        }

        public static PageModel Logger(object model, int loggerId, PageModel previousPageModel)
        {
            string pagePath = $"/loggers/{loggerId}";
            return new PageModel(PageType.Logger, model, pagePath, previousPageModel);
        }

        public static PageModel Users(object model)
        {
            return new PageModel(PageType.Users, model);
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
