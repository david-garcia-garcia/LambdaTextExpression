using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Dynamic.Core.Validation;
using System.Reflection;

namespace LambdaTextExpressionTests
{
    public class CustomTypeProvider : AbstractDynamicLinqCustomTypeProvider, IDynamicLinkCustomTypeProvider
    {
        /// <inheritdoc cref="M:System.Linq.Dynamic.Core.CustomTypeProviders.IDynamicLinkCustomTypeProvider.GetCustomTypes" />
        public virtual HashSet<Type> GetCustomTypes()
        {
            return new HashSet<Type>(this.FindTypesMarkedWithDynamicLinqTypeAttribute((IEnumerable<Assembly>)
                AppDomain.CurrentDomain.GetAssemblies()));
        }

        /// <inheritdoc cref="M:System.Linq.Dynamic.Core.CustomTypeProviders.IDynamicLinkCustomTypeProvider.ResolveType(System.String)" />
        public Type ResolveType(string typeName)
        {
            return Type.GetType(typeName);
        }
    }
}
