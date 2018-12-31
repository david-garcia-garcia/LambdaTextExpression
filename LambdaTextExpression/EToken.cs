using System;
using System.Linq.Expressions;

namespace LambdaTextExpression
{
    /// <summary>
    /// Expression token
    /// </summary>
    public class EToken
    {
        /// <summary>
        /// Get an instance of EToken
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        /// <param name="clrType"></param>
        public EToken(
            string text,
            ExpressionType type,
            Type clrType)
        {
            this.Text = text;
            this.Type = type;
            this.ClrType = clrType;
        }

        /// <summary>
        /// The expression text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The expression type
        /// </summary>
        public ExpressionType Type { get; set; }

        /// <summary>
        /// The CLR type of the expression evaluation result, if applicable
        /// </summary>
        public Type ClrType { get; set; }

        /// <summary>
        /// Late argument binding through .ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Text;
        }
    }
}
