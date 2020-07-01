using IPCLogger.Resolvers.Base;
using System.Collections.Generic;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DocItemModel
    {

#region Private fields

        private IResolver _objectIdResolver;
        private object _constObjectId;

#endregion

#region Properties

        public object ObjectId  //Ex: logger class, snippet name
        {
            get
            {
                return _objectIdResolver != null
                    ? _objectIdResolver.Resolve(ObjectSubId)
                    : _constObjectId;
            }
        }

        public string ObjectSubId { get; } //Ex: logger property, snippet type

        public string DisplayName { get; }

        public string Description { get; }

        public List<DocItemParamModel> Params { get; }

#endregion

#region Ctor

        public DocItemModel(string objectId, string objectSubId, string displayName, string description)
        {
            _constObjectId = objectId;
            ObjectSubId = objectSubId;
            DisplayName = displayName;
            Description = description;
            Params = new List<DocItemParamModel>();
        }

        public DocItemModel(IResolver objectIdResolver, string objectSubId, string displayName, string description)
            : this((string)null, objectSubId, displayName, description)
        {
            _objectIdResolver = objectIdResolver;
        }

#endregion

    }
}
