using IPCLogger.Core.Resolvers.Base;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DocItemModel
    {
        private List<DocItemParamModel> _params;
        private IBaseResolver _relObjectIdResolver;
        private object _constRelObjectId;

        public object RelObjectId  //Ex: snippet class, snippet name
        {
            get
            {
                return _relObjectIdResolver != null
                    ? _relObjectIdResolver.Resolve(Type)
                    : _constRelObjectId;
            }
        }

        public string Type { get; } //Ex: snippet type

        public string Name { get; }

        public string Description { get; }

        public IReadOnlyList<DocItemParamModel> Params
        {
            get { return _params; }
        }

        public DocItemModel(string relObjectId, string type, string name, string description)
        {
            _constRelObjectId = relObjectId;
            Type = type;
            Name = name;
            Description = description;
            _params = new List<DocItemParamModel>();
        }

        public DocItemModel(IBaseResolver resolver, string type, string name, string description)
            : this((string)null, type, name, description)
        {
            _relObjectIdResolver = resolver;
        }
    }
}
