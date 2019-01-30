using IPCLogger.Core.Resolvers.Base;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DocItemModel
    {
        private List<DocItemParamModel> _params;
        private IBaseResolver _relObjectSubIdResolver;
        private string _constRelObjectSubId;

        public string RelObjectId { get; } //Ex: snippet class, logger class

        public string RelObjectSubId  //Ex: snippet name, logger param name
        {
            get
            {
                return _relObjectSubIdResolver != null
                    ? _relObjectSubIdResolver.Resolve(RelObjectId).ToString()
                    : _constRelObjectSubId;
            }
        }

        public string Type { get; } //Ex: snippet type

        public string Name { get; }

        public string Description { get; }

        public IReadOnlyList<DocItemParamModel> Params
        {
            get { return _params; }
        }

        public DocItemModel(string relObjectId, string relObjectSubId, string type,
            string name, string description)
        {
            RelObjectId = relObjectId;
            _constRelObjectSubId = relObjectSubId;
            Type = type;
            Name = name;
            Description = description;
            _params = new List<DocItemParamModel>();
        }

        public DocItemModel(string relObjectId, string type, string name, string description)
            : this(relObjectId, null, type, name, description)
        {
        }

        public void SetRelObjectSubIdResolver(IBaseResolver resolver)
        {
            _relObjectSubIdResolver = resolver;
        }
    }
}
