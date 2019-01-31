using IPCLogger.ConfigurationService.Entities.Models;
using IPCLogger.Core.Resolvers;
using IPCLogger.Core.Resolvers.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace IPCLogger.ConfigurationService.CoreServices
{
    public class DocsService
    {

#region Constants

        private const string FOLDER_SNIPPETS = "snippets";

#endregion

#region Private fields

        private List<DocItemModel> _docSnippets;

#endregion

#region Properties

        
#endregion

#region Ctor

        public DocsService(string docsPath)
        {
            Initialize(docsPath);
        }

#endregion

#region Initialization methods

        private DocItemModel DocItemModelFromJObject(JObject jDoc)
        {
            try
            {
                string type = jDoc["Type"]?.Value<string>();
                string displayName = jDoc["DisplayName"]?.Value<string>();
                string description = jDoc["Description"]?.Value<string>();

                string relObjectIdResolver = jDoc["RelObjectIdResolver"].Value<string>();
                if (!string.IsNullOrEmpty(relObjectIdResolver))
                {
                    IBaseResolver resolver = RFactory.Get(relObjectIdResolver);
                    if (resolver != null)
                    {
                        return new DocItemModel(resolver, type, displayName, description);
                    }
                }

                string relObjectId = jDoc["RelObjectId"].Value<string>();
                if (!string.IsNullOrWhiteSpace(relObjectId))
                {
                    return new DocItemModel(relObjectId, type, displayName, description);
                }

                string msg = $"Doc record doesn't have RelObjectId or RelObjectIdResolver is wrong";
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
                foreach (JObject jDoc in jDocs.Children())
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
            string[] docsFiles = Directory.GetFiles(docsPath, "*.json");
            foreach (string docsFile in docsFiles)
            {
                ReadDocItems(docsFile, docsSnippet);
            }
        }

        private void Initialize(string docsPath)
        {
            _docSnippets = new List<DocItemModel>();

            if (!Directory.Exists(docsPath)) return;

            //Read docs for snippets
            string snippetsPath = Path.Combine(docsPath, FOLDER_SNIPPETS);
            ReadDocsItems(snippetsPath, _docSnippets);
        }

#endregion

    }
}
