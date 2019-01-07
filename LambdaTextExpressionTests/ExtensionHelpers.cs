using LambdaTextExpression;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace LambdaTextExpressionTests
{
    public static class ExtensionHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        public static void AssertExpTextCanBeConvertedToLambda<TResult>(
            this ExpressionText expText)
        {
            ParsingConfig pc = ParsingConfig.Default;
            pc.AllowNewToEvaluateAnyType = true;
            pc.CustomTypeProvider = new CustomTypeProvider();

            // Build the parameters...
            List<ParameterExpression> parameters = new List<ParameterExpression>();

            foreach (var p in expText.Parameters2.Values)
            {
                parameters.Add(p);
            }

            DynamicExpressionParser.ParseLambda(
                pc,
                true,
                parameters.ToArray(),
                typeof(TResult),
                expText.Expression,
                new object[] { expText.Arguments });
        }
    }
}
