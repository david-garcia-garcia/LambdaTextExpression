using System;
using System.Linq.Expressions;

namespace LambdaTextExpression
{
    /// <summary>
    /// Used to build inline lambdas easily, because inline typed lambdas
    /// won't work with C# as it won't infere types automatically.
    /// <see cref="https://stackoverflow.com/questions/4761516/c-sharp-inline-lambda-evaluation"/>
    /// </summary>
    public static class LambdaBuilder
    {
        /// <summary>
        /// Create lambda
        /// </summary>
        public static Expression<Func<TResult>> Lambda<TResult>(Expression<Func<TResult>> func)
        {
            return func;
        }

        /// <summary>
        /// Create lambda
        /// </summary>
        public static Expression<Func<T, TResult>> Lambda<T, TResult>(Expression<Func<T, TResult>> func)
        {
            return func;
        }

        /// <summary>
        /// Create lambda
        /// </summary>
        public static Expression<Func<T1, T2, TResult>> Lambda<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> func)
        {
            return func;
        }

        /// <summary>
        /// Create lambda
        /// </summary>
        public static Expression<Func<T1, T2, T3, TResult>> Lambda<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> func)
        {
            return func;
        }

        /// <summary>
        /// Create lambda
        /// </summary>
        public static Expression<Func<T1, T2, T3, T4, TResult>> Lambda<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> func)
        {
            return func;
        }

        /// <summary>
        /// Create lambda
        /// </summary>
        public static Expression<Func<T1, T2, T3, T4, T5, TResult>> Lambda<T1, T2, T3, T4, T5, TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> func)
        {
            return func;
        }

        /// <summary>
        /// Create lambda
        /// </summary>
        public static Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> Lambda<T1, T2, T3, T4, T5, T6, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> func)
        {
            return func;
        }
    }
}
