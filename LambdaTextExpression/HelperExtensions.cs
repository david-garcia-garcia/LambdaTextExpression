using System;
using System.Collections.Generic;
using System.Linq;

namespace LambdaTextExpression
{
    internal static class HelperExtensions
    {
        /// <summary>
        /// Join elements in a string array
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static string StringJoin(
            this IEnumerable<string> source,
            string separator,
            Func<string, string> transform = null)
        {
            if (source == null)
            {
                return string.Empty;
            }

            if (transform != null)
            {
                source = source.Select(transform);
            }

            return string.Join(separator, source);
        }

        /// <summary>
        /// Join elements in a string array
        /// </summary>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string StringJoinObject<T>(this IEnumerable<T> source, string separator)
        {
            return source.AsQueryable().Select((i) => Convert.ToString(i)).StringJoin(separator);
        }

        /// <summary>
        /// Determined whether the specific type is nullable value type.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>true if the type is nullable value type otherwise false</returns>
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Retrieves underlaying value type of the nullable value type.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>The underlaying value type.</returns>
        public static Type GetNullableValueType(this Type type)
        {
            return type.GetGenericArguments().Single();
        }

        /// <summary>
        /// Get the actual or underlying type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetUnderlyingType(this Type type)
        {
            if (!IsNullable(type))
            {
                return type;
            }

            return GetNullableValueType(type);
        }
    }
}
