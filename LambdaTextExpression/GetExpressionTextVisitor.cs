using Dynamitey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace LambdaTextExpression
{
    /// <summary>
    /// Permite convertir una expresión lambda en su versión texto
    ///
    /// Utilities to deal with expressions. Inspiration taken from:
    /// https://github.com/aspnet/Mvc/blob/9c545aa343ccb1bf413888573c398fe56017d9ee/src/Microsoft.AspNet.Mvc.Core/Rendering/Expressions/ExpressionHelper.cs
    /// </summary>
    public class GetExpressionTextVisitor
    {
        /// <summary>
        /// The arguments for the expression
        /// </summary>
        protected Dictionary<string, object> Arguments;

        /// <summary>
        /// TODO: Review the usage of this. It was only added to give test coverage
        /// to the produced expressions. It must be removed because
        /// these cannot be serialzied and cannot replace Parameters. Figure out
        /// how to remove this and keep test coverage.
        /// </summary>
        protected Dictionary<string, ParameterExpression> TempParameters;

        /// <summary>
        /// Name and type of the parameters in the expression
        /// </summary>
        protected Dictionary<string, string> Parameters;

        /// <summary>
        /// Get an instance of GetExpressionTextVisitor
        /// </summary>
        public GetExpressionTextVisitor()
        {
        }

        /// <summary>
        /// Transform a lambda expression into it's text representation
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public ExpressionText GetExpressionText(
            Expression expression)
        {
            this.Arguments = new Dictionary<string, object>();
            this.Parameters = new Dictionary<string, string>();
            this.TempParameters = new Dictionary<string, ParameterExpression>();

            ExpressionText result = new ExpressionText();
            var exp = this.VisitExpression(expression, null);

            result.InternalExpression = exp.Text;
            result.Arguments = this.Arguments;
            result.Parameters = this.Parameters;
            result.Parameters2 = this.TempParameters;
            result.ClrType = exp.ClrType?.FullName;

            return result;
        }

        /// <summary>
        /// Visit an expression
        /// </summary>
        protected EToken VisitExpression(Expression exp, Expression parentExpression)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.ListInit:

                    // No soportamo esto porque la únicamanera de hacerlo sería si 
                    // el inicializador es materializable, lo que no cubriría todos los casos.
                    throw new NotSupportedException();

                case ExpressionType.Convert:

                    return this.VisitConvertExpression((UnaryExpression)exp, parentExpression);

                case ExpressionType.Not:

                    return this.VisitNotExpression((UnaryExpression)exp);

                case ExpressionType.Call:

                    return this.VisitMethodCallExpression((MethodCallExpression)exp);

                case ExpressionType.ArrayIndex:

                    return this.VisitArrayIndexExpression((BinaryExpression)exp);

                case ExpressionType.MemberAccess:

                    return this.VisitMemberAccessExpression((MemberExpression)exp);

                case ExpressionType.Parameter:

                    return this.VisitParameterExpression((ParameterExpression)exp, parentExpression);

                case ExpressionType.Lambda:

                    return this.VisitLambdaExpression((LambdaExpression)exp);

                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.Subtract:
                case ExpressionType.Add:
                case ExpressionType.Multiply:
                case ExpressionType.Modulo:
                case ExpressionType.ExclusiveOr:

                    return this.VisitBinaryExpression((BinaryExpression)exp, parentExpression);

                case ExpressionType.Constant:

                    return this.VisitConstantExpression((ConstantExpression)exp);

                case ExpressionType.New:

                    return this.VisitNewExpression((NewExpression)exp);

                case ExpressionType.MemberInit:

                    return this.VisitMemberInitExpression((MemberInitExpression)exp);

                case ExpressionType.Conditional:

                    return this.VisitConditionalExpression((ConditionalExpression)exp);

                case ExpressionType.NewArrayInit:

                    return this.VisitArrayInitExpression((ConstantExpression)exp);

                default:

                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Visit a lambda expression
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="parentExpression"></param>
        /// <returns></returns>
        protected EToken VisitConvertExpression(UnaryExpression exp, Expression parentExpression)
        {
            // Many of these CONVERT expressions are automatically infered by the language itself,
            // and having them here is too verbose, we need a way to only leave those that are required
            // by the final expression.

            Expression innerExp = exp;

            Type underlyingType = exp.Type.GetUnderlyingType();
            bool isNullable = false;

            // Expressions usually have useless implicit cast chains that can be simplified.
            while (innerExp.NodeType == ExpressionType.Convert && underlyingType == innerExp.Type.GetUnderlyingType())
            {
                exp = (UnaryExpression)innerExp;
                isNullable = exp.Type.IsNullable() || isNullable;
                innerExp = exp.Operand;
            }

            string typeConversion = underlyingType.Name;
            if (isNullable)
            {
                typeConversion += "?";
            }

            var innerExpression = this.VisitExpression(innerExp, exp);
            return new EToken(typeConversion + "(" + innerExpression.Text + ")", exp.NodeType, underlyingType);
        }

        /// <summary>
        /// Visit a lambda expression
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected EToken VisitNotExpression(UnaryExpression exp)
        {
            var innerExpression = this.VisitExpression(exp.Operand, exp);
            return new EToken("!(" + innerExpression.Text + ")", exp.NodeType, typeof(bool));
        }

        /// <summary>
        /// Visit a lambda expression
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected EToken VisitLambdaExpression(LambdaExpression exp)
        {
            foreach (var p in exp.Parameters)
            {
                this.TempParameters[p.Name] = p;
                this.Parameters[p.Name] = p.Name;
            }

            var expBody = this.VisitExpression(exp.Body, exp);
            return new EToken(expBody.Text, exp.NodeType, exp.ReturnType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected EToken VisitArrayInitExpression(ConstantExpression exp)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected EToken VisitConstantExpression(ConstantExpression exp)
        {
            return new EToken(
                this.AddArgument(exp.Value),
                exp.NodeType,
                exp.Type);
        }

        /// <summary>
        /// Visit a parameter expression
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="parentExpression"></param>
        /// <returns></returns>
        protected EToken VisitParameterExpression(ParameterExpression exp, Expression parentExpression)
        {
            if (!this.Parameters.ContainsKey(exp.Name))
            {
                this.TempParameters[exp.Name] = exp;
                this.Parameters[exp.Name] = exp.Name;
            }

            return new EToken("[" + exp.Name + "]", exp.NodeType, exp.Type);
        }

        /// <summary>
        /// Visit a binary expression
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="parentExpression"></param>
        /// <returns></returns>
        protected EToken VisitBinaryExpression(BinaryExpression exp, Expression parentExpression)
        {
            var left = this.VisitExpression(exp.Left, exp);
            var right = this.VisitExpression(exp.Right, exp);

            if (left.Type == ExpressionType.Constant && right.Type == ExpressionType.Constant)
            {
                var evaluationResult = Dynamic.InvokeBinaryOperator(
                    this.PopArgument(left.Text),
                    exp.NodeType,
                    this.PopArgument(right.Text));

                return new EToken(this.AddArgument(evaluationResult), ExpressionType.Constant, exp.Type);
            }

            bool changedOperatorType =
                new List<ExpressionType>() {
                    ExpressionType.And,
                    ExpressionType.Or,
                    ExpressionType.AndAlso,
                    ExpressionType.OrElse,
                    ExpressionType.Add,
                    ExpressionType.Subtract,
                }.Contains(exp.NodeType) &&
                parentExpression?.NodeType != exp.NodeType
                && parentExpression is BinaryExpression;

            string result = $"{left} {this.BinaryExpressionTemplate(exp.NodeType)} {right}";
            if (changedOperatorType)
            {
                result = $"({result})";
            }

            return new EToken(result, exp.NodeType, exp.Type);
        }

        /// <summary>
        /// Visit a conditional expression
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected EToken VisitConditionalExpression(ConditionalExpression exp)
        {
            var expTest = this.VisitExpression(exp.Test, exp);
            var expIfTrue = this.VisitExpression(exp.IfTrue, exp);
            var expIfFalse = this.VisitExpression(exp.IfFalse, exp);

            // We can evaluate this at compile time
            if (expTest.Type == ExpressionType.Constant)
            {
                var testValue = this.PopArgument(expTest.Text) as bool?;
                var ifTrueValue = this.PopArgument(expIfTrue.Text);
                var ifFalseValue = this.PopArgument(expIfFalse.Text);

                var finalValue = (testValue == true) ? ifTrueValue : ifFalseValue;
                return new EToken(this.AddArgument(finalValue), ExpressionType.Constant, exp.Type);
            }

            return new EToken($"iif({expTest}, {expIfTrue}, {expIfFalse})", exp.NodeType, exp.Type);
        }

        /// <summary>
        /// Visit a member access expression
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected EToken VisitMemberAccessExpression(MemberExpression exp)
        {
            EToken memberExp;

            if (exp.Expression == null)
            {
                // Método estáticos
                // TODO: El tipo ExpressionType.Unbox aquí es incorrecto, debería ser null
                memberExp = new EToken(exp.Type.FullName, ExpressionType.Unbox, null);
            }
            else
            {
                // Métodos en objetos
                memberExp = this.VisitExpression(exp.Expression, exp);
            }

            var memberName = exp.Member.Name;
            var memberType = this.GetUnderlyingType(exp.Member);

            if (memberExp.Type == ExpressionType.Constant)
            {
                // Nos permite evaluar sobre la marcha member accesors que SÍ podemos resolver
                if (memberExp.ClrType.IsNullable() && memberName == "Value")
                {
                    // No hacer nada, ya que la llamada a Value() - que es un unboxing de un struct -
                    // lo hace automáticamente el runtime de .Net
                }
                else
                {
                    this.Arguments[memberExp.Text] = Dynamic.InvokeGet(this.Arguments[memberExp.Text], memberName);
                }

                return new EToken(memberExp.Text, ExpressionType.Constant, memberType);
            }

            if (memberExp.Type == ExpressionType.Unbox)
            {
                var expCallResult = ((FieldInfo)exp.Member).GetValue(null);
                return new EToken(this.AddArgument(expCallResult), ExpressionType.Constant, memberType);
            }

            return new EToken($"{memberExp.Text}.{memberName}", exp.NodeType, memberType);
        }

        /// <summary>
        /// Visit a New() expression
        /// </summary>
        /// <param name="expNew"></param>
        /// <returns></returns>
        protected EToken VisitNewExpression(NewExpression expNew)
        {
            List<string> memberInits = new List<string>();

            for (int x = 0; x < expNew.Members.Count; x++)
            {
                var memberInfo = expNew.Members[x];
                var memberExpression = expNew.Arguments[x];

                memberInits.Add($"{this.VisitExpression(memberExpression, expNew)} as {memberInfo.Name}");
            }

            return new EToken($"new({memberInits.StringJoin(", ")})", expNew.NodeType, expNew.Type);
        }

        /// <summary>
        /// Visit a New() expression
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected EToken VisitMemberInitExpression(MemberInitExpression exp)
        {
            string typeName = exp.Type.FullName;

            if (typeName.Contains("<") ||
                exp.Type.GetCustomAttribute<CompilerGeneratedAttribute>() != null)
            {
                throw new Exception("Currently Dynamic.Linq.Core only support simple class names and nested classes. https://github.com/StefH/System.Linq.Dynamic.Core/pull/207");
            }

            List<string> memberInits = new List<string>();

            foreach (var binding in exp.Bindings)
            {
                var memberInfo = binding.Member;
                Expression memberExpression;

                switch (binding.BindingType)
                {
                    case MemberBindingType.Assignment:
                        memberExpression = ((MemberAssignment)binding).Expression;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                memberInits.Add($"{this.VisitExpression(memberExpression, exp)} as {memberInfo.Name}");
            }

            return new EToken($"new {exp.Type.FullName}({memberInits.StringJoin(", ")})", exp.NodeType, exp.Type);
        }

        /// <summary>
        /// Visit a method call expression
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected EToken VisitArrayIndexExpression(BinaryExpression exp)
        {
            var left = this.VisitExpression(exp.Left, exp);
            var right = this.VisitExpression(exp.Right, exp);

            if (left.Type == ExpressionType.Constant
                && right.Type == ExpressionType.Constant)
            {
                var resolvedValue = Dynamic.InvokeGetIndex(this.PopArgument(left.Text), this.PopArgument(right.Text));
                return new EToken(this.AddArgument(resolvedValue), ExpressionType.Constant, exp.Type);
            }

            string result = $"{left}[{right}]";
            return new EToken(result, exp.NodeType, exp.Type);
        }

        /// <summary>
        /// Visit a method call expression
        /// </summary>
        /// <param name="expCall"></param>
        /// <returns></returns>
        protected EToken VisitMethodCallExpression(MethodCallExpression expCall)
        {
            List<EToken> args = (from p in expCall.Arguments
                                 select this.VisitExpression(p, expCall)).ToList();

            EToken expCallTargetExpression;

            if (expCall.Object == null)
            {
                // Here we should use FullName of declaring type, but a bug in Dynamic.Linq.Core prevents
                // usage of full namespaces and uses shortcut aliases only.
                // TODO: El tipo ExpressionType.Unbox aquí es incorrecto, debería ser null
                expCallTargetExpression = new EToken(expCall.Method?.DeclaringType?.Name, ExpressionType.Unbox, null);
            }
            else
            {
                // Métodos en objetos
                expCallTargetExpression = this.VisitExpression(expCall.Object, expCall);
            }

            bool canMaterializeMethodCall = args.All((i) => i.Type == ExpressionType.Constant)
                && (expCallTargetExpression.Type == ExpressionType.Constant ||
                    expCallTargetExpression.Type == ExpressionType.Unbox);

            if (canMaterializeMethodCall)
            {
                List<object> methodCallUnboxedArgs = new List<object>();

                // Grab all arguments and remove them from the dictionary
                foreach (var arg in args)
                {
                    methodCallUnboxedArgs.Add(this.PopArgument(arg.Text));
                }

                object expCallResult;

                if (expCall.Object == null)
                {
                    expCallResult = Dynamic.InvokeMember(
                        InvokeContext.CreateStatic(expCall.Method?.DeclaringType),
                        expCall.Method.Name,
                        methodCallUnboxedArgs.ToArray());
                }
                else
                {
                    expCallResult = Dynamic.InvokeMember(
                        this.PopArgument(expCallTargetExpression.Text),
                        expCall.Method.Name,
                        methodCallUnboxedArgs.ToArray());
                }

                return this.CheckMaterializationResult(expCallResult, expCall.Method.ReturnType);
            }

            // If this method is LinqKit's Invoke() then we can do this ASAP because Dynamic.Linq.Core won't swallow complex method calls
            // We asume that the coder is looking for lazy evaluation
            if (expCall.Method.Name == "Invoke" && expCall.Method?.DeclaringType?.FullName.Contains("LinqKit") == true)
            {
                var targetOfInvoke = expCall.Arguments.First() as MethodCallExpression;

                var argumentsOfTargetInvoke = new List<object>();
                foreach (var arg in targetOfInvoke.Arguments)
                {
                    if (arg is ConstantExpression constant)
                    {
                        argumentsOfTargetInvoke.Add(constant.Value);
                    }
                }

                // Materialize it! Currently only works with static calls
                var materializedTargetOfInvoke = Dynamic.InvokeMember(
                    InvokeContext.CreateStatic(targetOfInvoke.Method?.DeclaringType),
                    targetOfInvoke.Method.Name,
                    argumentsOfTargetInvoke.ToArray()) as LambdaExpression;

                var result = this.VisitLambdaExpression(materializedTargetOfInvoke);

                // Replace the target source arguments
                for (int x = 0; x < materializedTargetOfInvoke.Parameters.Count; x++)
                {
                    var expArg = materializedTargetOfInvoke.Parameters[x];
                    var externalArg = args[x + 1];

                    this.Parameters.Remove(expArg.Name);
                    this.TempParameters.Remove(expArg.Name);
                    result.Text = result.Text.Replace($"[{expArg.Name}]", externalArg.Text);
                }

                return result;
            }

            return new EToken($"{expCallTargetExpression}.{expCall.Method.Name}({args.StringJoinObject(", ")})", expCall.NodeType, expCall.Method.ReturnType);
        }

        /// <summary>
        /// When something can be evaluated, and the evaluation result itself is a lambda or expression,
        /// let's try to reduce it even further!
        /// </summary>
        /// <returns></returns>
        protected EToken CheckMaterializationResult(object result, Type expectedType)
        {
            if (result == null)
            {
                return new EToken(this.AddArgument(result), ExpressionType.Constant, expectedType);
            }

            if (result is LambdaExpression lambda)
            {
                return this.VisitLambdaExpression(lambda);
            }

            return new EToken(this.AddArgument(result), ExpressionType.Constant, expectedType);
        }

        /// <summary>
        /// Get the text operator used for binary expressions
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string BinaryExpressionTemplate(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Equal:
                    return "==";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.And:
                    return "&&";
                case ExpressionType.AndAlso:
                    return "&&";
                case ExpressionType.Or:
                    return "||";
                case ExpressionType.OrElse:
                    return "||";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.Coalesce:
                    return "??";
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.Multiply:
                    return "*";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.ExclusiveOr:
                    return "^";
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected object PopArgument(string name)
        {
            var result = this.Arguments[name];
            this.Arguments.Remove(name);
            return result;
        }

        /// <summary>
        /// An argument to the expression
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        protected string AddArgument(object argument)
        {
            var name = "@p" + this.Arguments.Count;
            this.Arguments.Add(name, argument);
            return name;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        protected Type GetUnderlyingType(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                default:
                    throw new ArgumentException("Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo");
            }
        }
    }
}
