using IPCLogger.ConfigurationService.CoreServices;
using IPCLogger.Core.Snippets.Base;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class SnippetsInfoModel
    {
        public Dictionary<SnippetType, List<BaseSnippet>> Snippets { get; set; }

        public DocsService DocsService { get; set; }

        public SnippetsInfoModel(Dictionary<SnippetType, List<BaseSnippet>> snippets, DocsService docsService)
        {
            Snippets = snippets;
            DocsService = docsService;
        }
    }
}
