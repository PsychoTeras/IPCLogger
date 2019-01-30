using IPCLogger.Core.Resolvers.Base;
using System;

namespace IPCLogger.ConfigurationService.Entities.Models
{
    public class DocItemParamModel
    {
        private IBaseResolver _valueResolver;

        private object _constValue;

        public string Name { get; }

        public string Description { get; }

        public Type ParamType { get; }

        public object Value
        {
            get
            {
                return _valueResolver != null
                    ? _valueResolver.Resolve(Name)
                    : _constValue;
            }
        }

        public DocItemParamModel(object value, string name, string descritption, string paramType)
        {
            Name = name;
            Description = descritption;
            ParamType = Type.GetType(paramType, false, true);
            if (ParamType == null)
            {
                ParamType = typeof(string);
            }
            _constValue = value;
        }

        public DocItemParamModel(IBaseResolver resolver, string name, string descritption, string paramType)
            : this((object)null, name, descritption, paramType)
        {
            _valueResolver = resolver;
        }
    }
}
