using IPCLogger.Core.Resolvers.Base;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DocItemModel
    {

#region Private fields

        private List<DocItemParamModel> _params;
        private IBaseResolver _relObjectIdResolver;
        private object _constRelObjectId;

#endregion

#region Properties

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

        public string DisplayName { get; }

        public string Description { get; }

        public IReadOnlyList<DocItemParamModel> Params
        {
            get { return _params; }
        }

#endregion

#region Ctor

        public DocItemModel(string relObjectId, string type, string displayName, string description)
        {
            _constRelObjectId = relObjectId;
            Type = type;
            DisplayName = displayName;
            Description = description;
            _params = new List<DocItemParamModel>();
        }

        public DocItemModel(IBaseResolver resolver, string type, string displayName, string description)
            : this((string)null, type, displayName, description)
        {
            _relObjectIdResolver = resolver;
        }

#endregion

    }
}
