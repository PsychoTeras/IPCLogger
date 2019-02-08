﻿using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.ConfigurationService.Web.modules.common;
using IPCLogger.Core.Common;
using IPCLogger.Core.Resolvers;
using IPCLogger.Core.Resolvers.Base;
using Nancy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IPCLogger.ConfigurationService.CoreServices
{
    public class DocsService
    {

#region Constants

        private const string FOLDER_LOGGERS = "loggers";
        private const string FOLDER_SNIPPETS = "snippets";

#endregion

#region Private fields

        private static readonly Lazy<DocsService> _instance = new Lazy<DocsService>(() => new DocsService());

        private List<DocItemModel> _docLoggers;
        private List<DocItemModel> _docSnippets;


#endregion

#region Properties

        public static DocsService Instance { get { return _instance.Value; } }

#endregion

#region Ctor

        private DocsService()
        {
            DefaultRootPathProvider pathProvider = new DefaultRootPathProvider();
            string docsPath = BootstrapperCommon.StaticContentsConventions.
                First(kv => kv.Key == "docs").
                Value.Substring(1).
                Replace("/", "\\");
            docsPath = Path.Combine(pathProvider.GetRootPath(), docsPath);
            Initialize(docsPath);
        }

#endregion

#region Initialization methods

        private DocItemParamModel DocItemModelParamFromJObject(JToken jParam)
        {
            DocItemParamModel model = null;

            string name = jParam["Name"]?.Value<string>();
            string description = jParam["Description"]?.Value<string>();

            string value = jParam["Value"]?.Value<string>();
            string valueResolver = jParam["ValueResolver"]?.Value<string>();
            string[] values = jParam["Values"]?.Values<string>().ToArray();

            if (!string.IsNullOrEmpty(value))
            {
                model = new DocItemParamModel(value, name, description);
            }
            else if (!string.IsNullOrEmpty(valueResolver))
            {
                IBaseResolver resolver = RFactory.Get(valueResolver);
                if (resolver != null)
                {
                    model = new DocItemParamModel(resolver, name, description);
                }
                else
                {
                    string msg = "ValueResolver is wrong";
                    throw new Exception(msg);
                }
            }
            else if (values != null)
            {
                model = new DocItemParamModel(values, name, description);
            }
            else
            {
                model = new DocItemParamModel(name, description);
            }

            return model;
        }

        private void ReadDocItemModelParamsFromJObject(DocItemModel model, JToken jDoc)
        {
            IJEnumerable<JToken> jParams = jDoc["Params"]?.AsJEnumerable();
            if (jParams == null) return;
            
            foreach (JToken jParam in jParams)
            {
                DocItemParamModel modelParam = DocItemModelParamFromJObject(jParam);
                if (modelParam != null)
                {
                    model.Params.Add(modelParam);
                }
            }
        }

        private DocItemModel DocItemModelFromJObject(JToken jDoc)
        {
            try
            {
                DocItemModel model = null;

                string objectSubId = jDoc["ObjectSubId"]?.Value<string>();
                string displayName = jDoc["DisplayName"]?.Value<string>();
                string description = jDoc["Description"]?.Value<string>();

                string objectIdResolver = jDoc["ObjectIdResolver"]?.Value<string>();
                if (!string.IsNullOrEmpty(objectIdResolver))
                {
                    IBaseResolver resolver = RFactory.Get(objectIdResolver);
                    if (resolver != null)
                    {
                        model = new DocItemModel(resolver, objectSubId, displayName, description);
                    }
                }
                else
                {
                    string objectId = jDoc["ObjectId"]?.Value<string>();
                    if (!string.IsNullOrWhiteSpace(objectId))
                    {
                        model = new DocItemModel(objectId, objectSubId, displayName, description);
                    }
                }

                if (model != null)
                {
                    ReadDocItemModelParamsFromJObject(model, jDoc);
                    return model;
                }

                string msg = "Doc record doesn\'t have ObjectId or ObjectIdResolver is wrong";
                throw new Exception(msg);
            }
            catch
            {
                return null;
            }
        }

        private void ReadDocItems(string filePath, List<DocItemModel> docsSnippet)
        {
            JArray jDocs;
            try
            {
                using (StreamReader file = File.OpenText(filePath))
                {
                    using (JsonReader reader = new JsonTextReader(file))
                    {
                        jDocs = (JArray)JToken.ReadFrom(reader);
                    }
                }
            }
            catch
            {
                jDocs = null;
            }

            if (jDocs != null)
            {
                foreach (JToken jDoc in jDocs.Children())
                {
                    DocItemModel model = DocItemModelFromJObject(jDoc);
                    if (model != null)
                    {
                        docsSnippet.Add(model);
                    }
                }
            }
        }

        private void ReadDocsItems(string docsPath, List<DocItemModel> docsSnippet)
        {
            if (!Directory.Exists(docsPath)) return;

            string[] docsFiles = Directory.GetFiles(docsPath, "*.json");
            foreach (string docsFile in docsFiles)
            {
                ReadDocItems(docsFile, docsSnippet);
            }
        }

        private void Initialize(string docsPath)
        {
            _docLoggers = new List<DocItemModel>();
            _docSnippets = new List<DocItemModel>();

            //Read docs for loggers
            string loggersPath = Path.Combine(docsPath, FOLDER_LOGGERS);
            ReadDocsItems(loggersPath, _docLoggers);

            //Read docs for snippets
            string snippetsPath = Path.Combine(docsPath, FOLDER_SNIPPETS);
            ReadDocsItems(snippetsPath, _docSnippets);
        }

#endregion

#region Logger methods

        public DocItemModel GetLoggerDoc(string loggerType, string propertyName)
        {
            return _docLoggers.FirstOrDefault
            (
                d => d.ObjectId.ToString() == loggerType && d.ObjectSubId == propertyName
            ) ??
            _docLoggers.FirstOrDefault
            (
                d => d.ObjectId.ToString() == Constants.ApplicableForAllMark && d.ObjectSubId == propertyName
            );
        }

#endregion

    }
}
