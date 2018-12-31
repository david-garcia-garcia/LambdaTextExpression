using LambdaTextExpression;
using LambdaTextExpressionTests.Models;
using System;
using System.Linq;
using Xunit;

namespace LambdaTextExpressionTests
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            GetExpressionTextVisitor expVisitor = new GetExpressionTextVisitor();

            var a = 1;

            Guid? nullableValue = Guid.NewGuid();

            // Binary materialization
            Assert.Equal(
                $"new {typeof(CORE_USER).FullName}(@p0 as name)",
                expVisitor.GetExpressionText(
                    LambdaBuilder.Lambda((CORE_PERSON i) =>
                        new CORE_USER() { name = "dtoname" })).Expression);

            //// Binary materialization
            //Assert.Equal(
            //    "company.fk_sabentis_businessgroup == @p0",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((SABENTIS_COMPANY company) =>
            //            company.fk_sabentis_businessgroup == nullableValue.Value)).Expression);

            // Binary materialization
            Assert.Equal(
                "@p0",
                expVisitor.GetExpressionText(
                    LambdaBuilder.Lambda((CORE_PERSON i) =>
                        (a + 2) == 3 ? "yes" : "no")).Expression);

            // Binary materialization
            Assert.Equal(
                "@p0",
                expVisitor.GetExpressionText(
                    LambdaBuilder.Lambda((CORE_PERSON i) =>
                        1 + 2)).Expression);

            // Binary not materialized
            Assert.Equal(
                "i.deletedAt + @p0",
                expVisitor.GetExpressionText(
                    LambdaBuilder.Lambda((CORE_PERSON i) =>
                        i.deletedAt + 2)).Expression);

            string[] strings = { "a", "b" };

            // Array accesor
            Assert.Equal(
                "@p0",
                expVisitor.GetExpressionText(
                    LambdaBuilder.Lambda((CORE_PERSON i) =>
                        strings[0])).Expression);

            // Array accesor II
            Assert.Equal(
                "@p0[i.changedAt]",
                expVisitor.GetExpressionText(
                    LambdaBuilder.Lambda((CORE_PERSON i) =>
                        strings[(int)i.changedAt])).Expression);

            // SQL Functions
            Assert.Equal(
                "rootprojection.DBFIELD.field.firstname == rootprojection2.DBFIELD.firstname",
                expVisitor.GetExpressionText(
                        LambdaBuilder.Lambda((CORE_PERSON i, CORE_PERSON i2) => i.firstname == i2.firstname))
                    .SetParameterMapping("i", "rootprojection.DBFIELD.field")
                    .SetParameterMapping("i2", "rootprojection2.DBFIELD")
                    .Expression);

            // SQL Functions
            //Assert.Equal(
            //    "DbFunctions.Like(i.firstname, @p0)",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((CORE_PERSON i) =>
            //            DbFunctions.Like(i.firstname, "thename"))).Expression);

            // Negate
            Assert.Equal(
                "!(i.deletedAt == @p0)",
                expVisitor.GetExpressionText(
                    LambdaBuilder.Lambda((CORE_PERSON i) =>
                        !(i.deletedAt == 25))).Expression);

            // Bitwise
            Assert.Equal(
                "i.deletedAt ^ @p0",
                expVisitor.GetExpressionText(
                    LambdaBuilder.Lambda((CORE_PERSON i) =>
                        i.deletedAt ^ 3)).Expression);

            // Sumar, multiplicar y restar
            Assert.Equal(
                "(i.deletedAt * @p0 - @p1) + @p2",
                expVisitor.GetExpressionText(
                    LambdaBuilder.Lambda((CORE_PERSON i) =>
                        (i.deletedAt * 5) - 5 + 6)).Expression);

            // Módulo
            Assert.Equal(
                "i.deletedAt % @p0",
                expVisitor.GetExpressionText(
                    LambdaBuilder.Lambda((CORE_PERSON i) =>
                        i.deletedAt % 5)).Expression);

            string[] testList = new string[] { "op1", "op2" };

            // This expression is fully calculable at compile time, and
            // should reduce to a constant expression
            Assert.Equal(
                "@p0",
                expVisitor.GetExpressionText(
                    LambdaBuilder.Lambda((CORE_PERSON i) =>
                        testList.ToList().Contains("op1"))).Expression);

            Assert.Equal(
                "i.deletedAt != @p0",
                expVisitor.GetExpressionText(
                    LambdaBuilder.Lambda((CORE_PERSON i) =>
                        i.deletedAt != null)).Expression);

            var sampleObject = new
            {
                prop1 = new
                {
                    prop2 = "My string",

                    number = 58
                }
            };

            //Assert.Equal(
            //    "i.SABENTIS_ADDRESS.roadname.Contains(@p0)",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((CORE_PERSON i) =>
            //            i.SABENTIS_ADDRESS.roadname.Contains(sampleObject.prop1.prop2))).Expression);

            Assert.Equal(
                "i.deletedAt > @p0",
                expVisitor.GetExpressionText(
                    LambdaBuilder.Lambda((CORE_PERSON i) =>
                        i.deletedAt > sampleObject.prop1.number)).Expression);

            //Assert.Equal(
            //    "new(i.SABENTIS_ADDRESS.SABENTIS_REGION.SABENTIS_REGIONTYPE.description as description)",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((CORE_PERSON i) =>
            //            new
            //            {
            //                i.SABENTIS_ADDRESS.SABENTIS_REGION.SABENTIS_REGIONTYPE.description
            //            })).Expression);

            //Assert.Equal(
            //    "new(i.SABENTIS_ADDRESS.SABENTIS_REGION.SABENTIS_REGIONTYPE.description as description2)",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((CORE_PERSON i) =>
            //            new
            //            {
            //                description2 = i.SABENTIS_ADDRESS.SABENTIS_REGION.SABENTIS_REGIONTYPE.description
            //            })).Expression);

            //Assert.Equal(
            //    "i.SABENTIS_ADDRESS.SABENTIS_REGION.SABENTIS_REGIONTYPE.description",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((CORE_PERSON i) =>
            //            i.SABENTIS_ADDRESS.SABENTIS_REGION.SABENTIS_REGIONTYPE.description)).Expression);

            //Assert.Equal(
            //    "i.SABENTIS_ADDRESS.altitude == @p0",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((CORE_PERSON i) =>
            //            i.SABENTIS_ADDRESS.altitude == 2)).Expression);

            //Assert.Equal(
            //    "i.SABENTIS_ADDRESS.altitude > @p0",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((CORE_PERSON i) =>
            //            i.SABENTIS_ADDRESS.altitude > 25)).Expression);

            //Assert.Equal(
            //    "assignment.deletedAt == @p0 && assignment.quala == @p1 && assignment.QUALA_IDENTIFIEDPROBLEM.QUALA_PROBLEM.QUALA_PROBLEMTYPE.QUALA_PROBLEMCATEGORY.code == @p2 && assignment.QUALA_IDENTIFIEDPROBLEM.status == @p3",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((QUALA_ASSIGNATION assignment) =>
            //            assignment.deletedAt == null &&
            //            assignment.quala == (byte)QualaTypesEnums.HIRA
            //            && assignment.QUALA_IDENTIFIEDPROBLEM.QUALA_PROBLEM.QUALA_PROBLEMTYPE.QUALA_PROBLEMCATEGORY.code == ProblemCategoryEnum.ProblemCategorySecurity
            //            && assignment.QUALA_IDENTIFIEDPROBLEM.status == (byte)QualaProblemStatusEnums.Identified)).Expression);

            //Assert.Equal(
            //    "i.SABENTIS_ADDRESS.altitude > @p0 || (i.SABENTIS_ADDRESS.altitude == @p1 && i.SABENTIS_ADDRESS.altitude == @p2)",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((CORE_PERSON i) =>
            //            i.SABENTIS_ADDRESS.altitude > 25
            //            || (i.SABENTIS_ADDRESS.altitude == 0 && i.SABENTIS_ADDRESS.altitude == 1))).Expression);

            //Assert.Equal(
            //    "i.SABENTIS_ADDRESS.altitude > @p0 || (@p1 && i.SABENTIS_ADDRESS.altitude == @p2 && i.SABENTIS_ADDRESS.altitude == @p3)",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((CORE_PERSON i) =>
            //            i.SABENTIS_ADDRESS.altitude > 25
            //            || (true && (i.SABENTIS_ADDRESS.altitude == 0 && i.SABENTIS_ADDRESS.altitude == 1)))).Expression);

            //Assert.Equal(
            //    "i.SABENTIS_ADDRESS.roadname.Contains(@p0)",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((CORE_PERSON i) =>
            //            i.SABENTIS_ADDRESS.roadname.Contains("my list"))).Expression);

            //var localExpressionValue = "literal";

            //// By default .Net wraps literal local values in objects in lambda expressions
            //Assert.Equal(
            //    "i.SABENTIS_ADDRESS.roadname.Contains(@p0)",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((CORE_PERSON i) =>
            //            i.SABENTIS_ADDRESS.roadname.Contains(localExpressionValue))).Expression);

            //Assert.Equal(
            //    "i.SABENTIS_ADDRESS.roadname ?? @p0",
            //    expVisitor.GetExpressionText(
            //        LambdaBuilder.Lambda((CORE_PERSON i) =>
            //            i.SABENTIS_ADDRESS.roadname ?? "myliteralroadname")).Expression);
        }
    }
}
