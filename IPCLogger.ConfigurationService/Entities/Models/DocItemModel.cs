using IPCLogger.Core.Resolvers.Base;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DocItemModel
    {

#region Private fields

        private IBaseResolver _objectIdResolver;
        private object _constObjectId;

#endregion

#region Properties

        public object ObjectId  //Ex: snippet class, snippet name
        {
            get
            {
                return _objectIdResolver != null
                    ? _objectIdResolver.Resolve(Type)
                    : _constObjectId;
            }
        }

        public string Type { get; } //Ex: snippet type

        public string DisplayName { get; }

        public string Description { get; }

        public List<DocItemParamModel> Params { get; }

#endregion

#region Ctor

        public DocItemModel(string relObjectId, string type, string displayName, string description)
        {
            _constObjectId = relObjectId;
            Type = type;
            DisplayName = displayName;
            Description = description;
            Params = new List<DocItemParamModel>();
        }

        public DocItemModel(IBaseResolver objectIdResolver, string type, string displayName, string description)
            : this((string)null, type, displayName, description)
        {
            _objectIdResolver = objectIdResolver;
        }

#endregion

    }
}
