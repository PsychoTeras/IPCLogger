using IPCLogger.Core.Resolvers.Base;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DocItemParamModel
    {

#region Private fields

        private IBaseResolver _valuesResolver;
        private string[] _constValues;

#endregion

#region Properties

        public string Name { get; }

        public string Description { get; }

        public string[] Values
        {
            get
            {
                return _valuesResolver != null
                    ? _valuesResolver.Resolve(Name)
                    : _constValues;
            }
        }

#endregion

#region Ctor

        public DocItemParamModel(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public DocItemParamModel(string value, string name, string description)
            : this(name, description)
        {
            _constValues = new[] { value };
        }

        public DocItemParamModel(string[] values, string name, string description)
            : this(name, description)
        {
            _constValues = values;
        }

        public DocItemParamModel(IBaseResolver valueResolver, string name, string description)
            : this(name, description)
        {
            _valuesResolver = valueResolver;
        }

#endregion

    }
}
