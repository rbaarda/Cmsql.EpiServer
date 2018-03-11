using System.Collections.Generic;
using Cql.EpiServer.Internal;
using Cql.Query;
using EPiServer;
using EPiServer.DataAbstraction;
using FluentAssertions;
using Xunit;

namespace Cql.EpiServer.Test.Internal
{
    public class CqlBinaryOrExpressionVisitorTest
    {
        [Fact]
        public void Test_when_visit_query_condition_push_new_criteria_collection()
        {
            // Arrange
            CqlQueryCondition condition = new CqlQueryCondition
            {
                Identifier = "PageName",
                Operator = EqualityOperator.GreaterThan,
                Value = "5"
            };

            CqlExpressionVisitorContext context = new CqlExpressionVisitorContext();

            CqlExpressionVisitor cqlExpressionVisitor =
                new CqlBinaryOrExpressionVisitor(
                    new QueryConditionToPropertyCriteriaMapper(
                        new PropertyDataTypeResolver(new ContentType())), context);

            // Act
            cqlExpressionVisitor.VisitQueryCondition(condition);

            IEnumerable<PropertyCriteriaCollection> propertyCriteriaCollection = context.GetCriteria();

            // Assert
            propertyCriteriaCollection.Should().HaveCount(1);
        }

        [Fact]
        public void Test_when_orexpression_visitor_visits_andexpression_with_conditions_push_one_new_criteria_collection()
        {
            // Arrange
            CqlQueryBinaryExpression expression = new CqlQueryBinaryExpression
            {
                Operator = ConditionalOperator.And,
                LeftExpression = new CqlQueryCondition
                {
                    Identifier = "PageName",
                    Operator = EqualityOperator.GreaterThan,
                    Value = "5"
                },
                RightExpression = new CqlQueryCondition
                {
                    Identifier = "PageName",
                    Operator = EqualityOperator.GreaterThan,
                    Value = "5"
                }
            };

            CqlExpressionVisitorContext context = new CqlExpressionVisitorContext();

            CqlExpressionVisitor cqlExpressionVisitor =
                new CqlBinaryOrExpressionVisitor(
                    new QueryConditionToPropertyCriteriaMapper(
                        new PropertyDataTypeResolver(new ContentType())), context);

            // Act
            cqlExpressionVisitor.VisitQueryExpression(expression);

            IEnumerable<PropertyCriteriaCollection> propertyCriteriaCollection = context.GetCriteria();

            // Assert
            propertyCriteriaCollection.Should().HaveCount(1);
        }

        [Fact]
        public void Test_when_orexpression_visitor_visits_orexpression_push_criteria_collection_for_every_condition()
        {
            // Arrange
            CqlQueryBinaryExpression expression = new CqlQueryBinaryExpression
            {
                Operator = ConditionalOperator.Or,
                LeftExpression = new CqlQueryCondition
                {
                    Identifier = "PageName",
                    Operator = EqualityOperator.GreaterThan,
                    Value = "5"
                },
                RightExpression = new CqlQueryCondition
                {
                    Identifier = "PageName",
                    Operator = EqualityOperator.GreaterThan,
                    Value = "5"
                }
            };

            CqlExpressionVisitorContext context = new CqlExpressionVisitorContext();

            CqlExpressionVisitor cqlExpressionVisitor =
                new CqlBinaryOrExpressionVisitor(
                    new QueryConditionToPropertyCriteriaMapper(
                        new PropertyDataTypeResolver(new ContentType())), context);

            // Act
            cqlExpressionVisitor.VisitQueryExpression(expression);

            IEnumerable<PropertyCriteriaCollection> propertyCriteriaCollection = context.GetCriteria();

            // Assert
            propertyCriteriaCollection.Should().HaveCount(2);
        }
    }
}
