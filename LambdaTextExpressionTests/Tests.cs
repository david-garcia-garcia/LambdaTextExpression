using LambdaTextExpression;
using LambdaTextExpressionTests.Models;
using System;
using System.Data.Entity;
using System.Linq;
using Xunit;

namespace LambdaTextExpressionTests
{
    public class Tests
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expText"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected void AssertHasArgument(string name, object value, ExpressionText expText)
        {
            var item = expText.Arguments[name];
            Assert.Equal(value, item);
        }

        [Fact]
        public void TestBinaryMaterialization()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            var exp = LambdaBuilder.Lambda((CORE_PERSON i) =>
                new CORE_USER() { name = "dtoname" });

            var expText = expVisitor.GetExpressionText(exp);
            expText.AssertExpTextCanBeConvertedToLambda<CORE_USER>();

            Assert.Equal($"new {typeof(CORE_USER).FullName}(@p0 as name)", expText.Expression);

            this.AssertHasArgument("@p0", "dtoname", expText);
        }

        [Fact]
        public void TestBinaryMaterializationWithNullable()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            Guid? nullableValue = Guid.NewGuid();

            // Nullable has some special handling in .Net
            var exp = LambdaBuilder.Lambda((CORE_PERSON person) =>
                person.fk_core_user == nullableValue.Value);

            ExpressionText expText;

            expText = expVisitor.GetExpressionText(exp);
            expText.AssertExpTextCanBeConvertedToLambda<bool>();

            Assert.Equal($"person.fk_core_user == @p0", expText.Expression);

            this.AssertHasArgument("@p0", nullableValue.Value, expText);

            nullableValue = null;

            expText = expVisitor.GetExpressionText(exp);
            expText.AssertExpTextCanBeConvertedToLambda<bool>();

            Assert.Equal($"person.fk_core_user == @p0", expText.Expression);

            this.AssertHasArgument("@p0", null, expText);
        }

        [Fact]
        public void TestBinaryMaterializationSimple()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            int a = 1;

            var exp = LambdaBuilder.Lambda((CORE_PERSON i) =>
                (a + 2) == 3 ? "yes" : "no");

            var expText = expVisitor.GetExpressionText(exp);
            expText.AssertExpTextCanBeConvertedToLambda<string>();

            // What happens here is that the expression can be 100% evaluated
            // so it gets reduced to a constant result

            Assert.Equal("@p0", expText.Expression);

            this.AssertHasArgument("@p0", "yes", expText);
        }

        [Fact]
        public void TestBinaryMaterializationSimple2()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            var exp = LambdaBuilder.Lambda((CORE_PERSON person) =>
                person.deletedAt + (2 + 3));

            var expText = expVisitor.GetExpressionText(exp);
            expText.AssertExpTextCanBeConvertedToLambda<long?>();

            Assert.Equal("person.deletedAt + @p0", expText.Expression);

            this.AssertHasArgument("@p0", 5, expText);
        }

        [Fact]
        public void TestArrayAccessor()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            string[] strings = { "a", "b" };

            ExpressionText expText = null;

            expText = expVisitor.GetExpressionText(LambdaBuilder.Lambda((CORE_PERSON i) => strings[0]));
            expText.AssertExpTextCanBeConvertedToLambda<string>();

            Assert.Equal("@p0", expText.Expression);

            this.AssertHasArgument("@p0", "a", expText);

            expText = expVisitor.GetExpressionText(LambdaBuilder.Lambda((CORE_PERSON i) => strings[(int)i.changedAt]));
            expText.AssertExpTextCanBeConvertedToLambda<string>();

            Assert.Equal("@p0[Int32(i.changedAt)]", expText.Expression);

            this.AssertHasArgument("@p0", strings, expText);
        }

        [Fact]
        public void TestParameterRenaming()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            ExpressionText expText = null;

            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i, CORE_PERSON i2) => i.firstname == i2.firstname));

            expText.AssertExpTextCanBeConvertedToLambda<bool>();

            Assert.Equal("i.firstname == i2.firstname", expText.Expression);
            Assert.Empty(expText.Arguments);

            // Rename arguments
            expText.SetParameterMapping("i", "rootprojection.DBFIELD.field");
            expText.SetParameterMapping("i2", "rootprojection2.DBFIELD");

            Assert.Equal("rootprojection.DBFIELD.field.firstname == rootprojection2.DBFIELD.firstname", expText.Expression);
            Assert.Empty(expText.Arguments);
        }

        [Fact]
        public void TestStaticFunctionCall()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            ExpressionText expText = null;

            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i) =>
                                DbFunctions.Like(i.firstname, "thename")));

            expText.AssertExpTextCanBeConvertedToLambda<bool>();

            Assert.Equal("DbFunctions.Like(i.firstname, @p0)", expText.Expression);
            this.AssertHasArgument("@p0", "thename", expText);
        }

        [Fact]
        public void TestNegate()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            ExpressionText expText = null;

            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i) =>
                    !(i.deletedAt == 25)));

            expText.AssertExpTextCanBeConvertedToLambda<bool>();

            Assert.Equal("!(i.deletedAt == @p0)", expText.Expression);
            this.AssertHasArgument("@p0", 25, expText);
        }

        [Fact]
        public void TestBitwise()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            ExpressionText expText = null;

            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i) =>
                    i.deletedAt ^ 3));

            // TODO: Seems not supported by Dynamic.Linq.Core
            // expText.AssertExpTextCanBeConvertedToLambda<long?>();

            Assert.Equal("i.deletedAt ^ @p0", expText.Expression);
            this.AssertHasArgument("@p0", 3, expText);
        }

        [Fact]
        public void TestAddSubstractMultiply()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            ExpressionText expText = null;

            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i) =>
                    (i.deletedAt * 5) - 5 + 6));

            expText.AssertExpTextCanBeConvertedToLambda<long?>();

            Assert.Equal("(i.deletedAt * @p0 - @p1) + @p2", expText.Expression);
            this.AssertHasArgument("@p0", 5, expText);
            this.AssertHasArgument("@p1", 5, expText);
            this.AssertHasArgument("@p2", 6, expText);
        }

        [Fact]
        public void TestModulus()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            ExpressionText expText = null;

            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i) =>
                    i.deletedAt % 5));

            expText.AssertExpTextCanBeConvertedToLambda<long?>();

            Assert.Equal("i.deletedAt % @p0", expText.Expression);
            this.AssertHasArgument("@p0", 5, expText);
        }

        [Fact]
        public void TestMaterializationAndMethodCall()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            string[] testList = new string[] { "op1", "op2" };

            ExpressionText expText = null;

            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i) =>
                    testList.ToList().Contains("op1")));

            expText.AssertExpTextCanBeConvertedToLambda<bool>();

            Assert.Equal("@p0", expText.Expression);
            this.AssertHasArgument("@p0", true, expText);
        }

        [Fact]
        public void TestObjectAccessor()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            var sampleObject = new
            {
                prop1 = new
                {
                    prop2 = "My string",
                    number = 58
                }
            };

            ExpressionText expText = null;

            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i) =>
                                i.CORE_USER.mail.Contains(sampleObject.prop1.prop2)));

            expText.AssertExpTextCanBeConvertedToLambda<bool>();

            Assert.Equal("@p0", expText.Expression);
            this.AssertHasArgument("@p0", true, expText);
        }

        [Fact]
        public void TestNullCompare()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            ExpressionText expText = null;

            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i) =>
                    i.deletedAt != null));

            expText.AssertExpTextCanBeConvertedToLambda<bool>();

            Assert.Equal("i.deletedAt != @p0", expText.Expression);
            this.AssertHasArgument("@p0", null, expText);
        }

        [Fact]
        public void TestStringLiteralInExpression()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            ExpressionText expText = null;

            var localExpressionValue = "literal";

            // By default .Net wraps literal local values in objects in lambda expressions
            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i) =>
                                i.CORE_USER.mail.Contains(localExpressionValue)));

            expText.AssertExpTextCanBeConvertedToLambda<bool>();

            Assert.Equal("i.CORE_USER.mail.Contains(@p0)", expText.Expression);
            this.AssertHasArgument("@p0", localExpressionValue, expText);
        }

        [Fact]
        public void TestNullCoalesce()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            ExpressionText expText = null;

            // By default .Net wraps literal local values in objects in lambda expressions
            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i) =>
                                i.CORE_USER.mail ?? "myliteralroadname"));

            expText.AssertExpTextCanBeConvertedToLambda<string>();

            Assert.Equal("i.CORE_USER.mail ?? @p0", expText.Expression);
            this.AssertHasArgument("@p0", "myliteralroadname", expText);
        }

        [Fact]
        public void TestObjectCreate()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            ExpressionText expText = null;

            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i) =>
                                new
                                {
                                    i.CORE_USER.mail
                                }));

            expText.AssertExpTextCanBeConvertedToLambda<object>();

            Assert.Equal("new(i.CORE_USER.mail as mail)", expText.Expression);

            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i) =>
                    new
                    {
                        mail2 = i.CORE_USER.mail
                    }));

            expText.AssertExpTextCanBeConvertedToLambda<object>();

            Assert.Equal("new(i.CORE_USER.mail as mail2)", expText.Expression);
        }

        [Fact]
        public void TestPropertySelector()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            ExpressionText expText = null;

            expText = expVisitor.GetExpressionText(
                LambdaBuilder.Lambda((CORE_PERSON i) => i.CORE_USER.mail));

            expText.AssertExpTextCanBeConvertedToLambda<string>();

            Assert.Equal("i.CORE_USER.mail", expText.Expression);
        }  
    }
}
