using IPCLogger.Core.Resolvers.Base;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DocItemParamModel
    {

#region Private fields

        private IResolver _valuesResolver;
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
                    ? _valuesResolver.AsArray(Name)
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

        public DocItemParamModel(IResolver valueResolver, string name, string description)
            : this(name, description)
        {
            _valuesResolver = valueResolver;
        }

#endregion

    }
}
