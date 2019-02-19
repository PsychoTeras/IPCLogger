using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.Core.Snippets.Base;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class SnippetsInfoModel
    {
        public Dictionary<SnippetType, List<BaseSnippet>> Snippets { get; set; }

        public Dictionary<SnippetType, string> SnippetIcon { get; set; }

        public DocsService DocsService { get; set; }

        public SnippetsInfoModel(Dictionary<SnippetType, List<BaseSnippet>> snippets,
            DocsService docsService)
        {
            Snippets = snippets;
            DocsService = docsService;

            SnippetIcon = new Dictionary<SnippetType, string>
            {
                {SnippetType.Template, "dollar"},
                {SnippetType.Pattern, "percent"},
                {SnippetType.Storage, "hashtag"},
                {SnippetType.AutoKey, "chevron-up"}
            };
        }
    }
}
