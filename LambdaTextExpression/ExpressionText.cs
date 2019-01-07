using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LambdaTextExpression
{
    /// <summary>
    /// Represents a text representation of an expression.
    /// </summary>
    public class ExpressionText
    {
        /// <summary>
        /// Get an instance of ExpressionText
        /// </summary>
        public ExpressionText()
        {
        }

        /// <summary>
        /// Get an instance of ExpressionText
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="parameters"></param>
        /// <param name="arguments"></param>
        public ExpressionText(
            string expression,
            Dictionary<string, string> parameters,
            Dictionary<string, object> arguments = null)
        {
            this.InternalExpression = expression;
            this.Arguments = arguments;
            this.Parameters = parameters;
        }

        /// <summary>
        /// The expression text
        /// </summary>
        public string Expression
        {
            get
            {
                var result = this.InternalExpression;

                if (this.Parameters != null)
                {
                    foreach (var p in this.Parameters)
                    {
                        result = result.Replace("[" + p.Key + "]", p.Value);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// The text expression with parameterized parameters and arguments
        /// </summary>
        public string InternalExpression { get; set; }

        /// <summary>
        /// Any literal arguments
        /// </summary>
        public Dictionary<string, object> Arguments { get; set; }

        /// <summary>
        /// List of expression parameters, they appear in the expression unreplaced
        /// preceeded by @
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, ParameterExpression> Parameters2 { get; set; }

        /// <summary>
        /// The result CLR type of the lambda expression call
        /// </summary>
        public string ClrType { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public ExpressionText SetParameterMapping(string parameter, string map)
        {
            this.Parameters = this.Parameters ?? new Dictionary<string, string>();
            this.Parameters[parameter] = map;
            return this;
        }

        /// <summary>
        /// Set the inner expression
        /// </summary>
        /// <returns></returns>
        public ExpressionText SetInternalExpression(string expression)
        {
            this.InternalExpression = expression;
            return this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ExpressionText AddArgument(string key, object value)
        {
            this.Arguments = this.Arguments ?? new Dictionary<string, object>();
            this.Arguments[key] = value;
            return this;
        }

        /// <summary>
        /// Merge in the parameters from the incoming exp and renames them if there are any collisions.
        ///
        /// Returns a new instance of exp with the parameters replaced.
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public ExpressionText MergeParameterMapping(ExpressionText exp)
        {
            // Para evitar que la expresión original quede manipulada la clonamos.
            // No usamos JsonClone clone por rendimiento
            // var expSource = ExtensionHelpers.JsonClone(exp);
            var expSource = new ExpressionText()
            {
                Parameters = new Dictionary<string, string>(exp.Parameters ?? new Dictionary<string, string>()),
                InternalExpression = exp.InternalExpression,
                Arguments = new Dictionary<string, object>(exp.Arguments ?? new Dictionary<string, object>()),
                ClrType = exp.ClrType
            };

            if (exp.Parameters == null)
            {
                return expSource;
            }

            this.Parameters = this.Parameters ?? new Dictionary<string, string>();

            foreach (var key in expSource.Parameters.Keys.ToList())
            {
                var keyName = this.FindFreeParameterPlaceholder(this, expSource, key);
                expSource.ReplaceParameterPlaceholder(key, keyName);
                this.Parameters[keyName] = expSource.Parameters[keyName];
            }

            return expSource;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="originalName"></param>
        /// <param name="newName"></param>
        public void ReplaceParameterPlaceholder(string originalName, string newName)
        {
            if (originalName == newName)
            {
                return;
            }

            this.InternalExpression = this.InternalExpression.Replace($"[{originalName}]", $"[{newName}]");
            this.Parameters[newName] = this.Parameters[originalName];
            this.Parameters.Remove(originalName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="exp1"></param>
        /// <param name="exp2"></param>
        /// <param name="proposal"></param>
        /// <param name="freeIfMatchingValue"></param>
        /// <returns></returns>
        public string FindFreeParameterPlaceholder(
            ExpressionText exp1,
            ExpressionText exp2,
            string proposal,
            bool freeIfMatchingValue = true)
        {
            int x = 0;
            string key = $"{proposal}";

            if (exp1.Parameters == null || !exp1.Parameters.ContainsKey(key))
            {
                return key;
            }

            while (true)
            {
                // Si la proyección es igual, podemos sobrescribir
                if (freeIfMatchingValue
                    && exp1.Parameters.ContainsKey(key)
                    && exp2.Parameters.ContainsKey(key)
                    && exp1.Parameters[key] == exp2.Parameters[key])
                {
                    return key;
                }

                if (exp1.Parameters == null || exp2.Parameters == null)
                {
                    return key;
                }

                if (!exp1.Parameters.ContainsKey(key) && !exp2.Parameters.ContainsKey(key))
                {
                    return key;
                }

                key = $"{proposal}_{x}";
                x++;
            }
        }
    }
}
