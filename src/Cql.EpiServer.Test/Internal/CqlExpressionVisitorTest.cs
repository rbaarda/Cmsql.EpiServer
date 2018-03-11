using System.Collections.Generic;
using System.Linq;
using Cql.EpiServer.Internal;
using Cql.Query;
using Cql.Query.Execution;
using EPiServer;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using FluentAssertions;
using Xunit;

namespace Cql.EpiServer.Test.Internal
{
    public class CqlExpressionVisitorTest
    {
        [Fact]
        public void Test_can_map_query_condition_to_property_criteria()
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
                new CqlExpressionVisitor(
                    new QueryConditionToPropertyCriteriaMapper(
                        new PropertyDataTypeResolver(new ContentType())), context);

            // Act
            cqlExpressionVisitor.VisitQueryCondition(condition);

            PropertyCriteriaCollection propertyCriteriaCollection = context.GetCriteria().Single();

            PropertyCriteria propertyCriteria = propertyCriteriaCollection.Single();

            // Assert
            propertyCriteria.Value.ShouldBeEquivalentTo(condition.Value);
            propertyCriteria.Condition.ShouldBeEquivalentTo(CompareCondition.GreaterThan);
            propertyCriteria.Name.ShouldBeEquivalentTo(condition.Identifier);
        }

        [Fact]
        public void Test_when_condition_is_null_criteria_should_be_empty()
        {
            // Arrange
            CqlExpressionVisitorContext context = new CqlExpressionVisitorContext();

            CqlExpressionVisitor cqlExpressionVisitor =
                new CqlExpressionVisitor(
                    new QueryConditionToPropertyCriteriaMapper(
                        new PropertyDataTypeResolver(new ContentType())), context);

            // Act
            cqlExpressionVisitor.VisitQueryCondition(null);

            IEnumerable<PropertyCriteriaCollection> propertyCriteriaCollection = context.GetCriteria();
            
            // Assert
            propertyCriteriaCollection.Should().BeEmpty();
        }

        [Fact]
        public void Test_when_condition_is_null_context_should_contain_error()
        {
            // Arrange
            CqlExpressionVisitorContext context = new CqlExpressionVisitorContext();

            CqlExpressionVisitor cqlExpressionVisitor =
                new CqlExpressionVisitor(
                    new QueryConditionToPropertyCriteriaMapper(
                        new PropertyDataTypeResolver(new ContentType())), context);

            // Act
            cqlExpressionVisitor.VisitQueryCondition(null);

            // Assert
            CqlQueryExecutionError error = context.Errors.Single();
            error.Message.ShouldBeEquivalentTo("Could not process malformed query condition.");
        }

        [Fact]
        public void Test_when_property_cannot_be_resolved_context_should_contain_error()
        {
            // Arrange
            CqlQueryCondition condition = new CqlQueryCondition
            {
                Identifier = "ThisPropertyCannotBeFound",
                Operator = EqualityOperator.GreaterThan,
                Value = "5"
            };

            CqlExpressionVisitorContext context = new CqlExpressionVisitorContext();

            CqlExpressionVisitor cqlExpressionVisitor =
                new CqlExpressionVisitor(
                    new QueryConditionToPropertyCriteriaMapper(
                        new PropertyDataTypeResolver(new ContentType())), context);

            // Act
            cqlExpressionVisitor.VisitQueryCondition(condition);
            
            // Assert
            CqlQueryExecutionError error = context.Errors.Single();
            error.Message.ShouldBeEquivalentTo("Could not find property 'ThisPropertyCannotBeFound'");
        }

        [Fact]
        public void Test_when_condition_cannot_be_mapped_criteria_should_be_empty()
        {
            // Arrange
            CqlQueryCondition condition = new CqlQueryCondition
            {
                Identifier = "ThisPropertyCannotBeFound",
                Operator = EqualityOperator.GreaterThan,
                Value = "5"
            };

            CqlExpressionVisitorContext context = new CqlExpressionVisitorContext();

            CqlExpressionVisitor cqlExpressionVisitor =
                new CqlExpressionVisitor(
                    new QueryConditionToPropertyCriteriaMapper(
                        new PropertyDataTypeResolver(new ContentType())), context);

            // Act
            cqlExpressionVisitor.VisitQueryCondition(condition);

            IEnumerable<PropertyCriteriaCollection> propertyCriteriaCollection = context.GetCriteria();

            // Assert
            propertyCriteriaCollection.Should().BeEmpty();
        }

        [Fact]
        public void Test_can_map_binary_orexpression_to_two_criteria_collections()
        {
            // Arrange
            CqlQueryBinaryExpression orExpression = new CqlQueryBinaryExpression
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
                    Operator = EqualityOperator.Equals,
                    Value = "10"
                }
            };

            CqlExpressionVisitorContext context = new CqlExpressionVisitorContext();

            CqlExpressionVisitor cqlExpressionVisitor =
                new CqlExpressionVisitor(
                    new QueryConditionToPropertyCriteriaMapper(
                        new PropertyDataTypeResolver(new ContentType())), context);

            // Act
            cqlExpressionVisitor.VisitQueryExpression(orExpression);

            IEnumerable<PropertyCriteriaCollection> propertyCriteriaCollection = context.GetCriteria();

            // Assert
            propertyCriteriaCollection.Should().HaveCount(2);
        }

        [Fact]
        public void Test_can_map_binary_andexpression_to_one_criteria_collections()
        {
            // Arrange
            CqlQueryBinaryExpression orExpression = new CqlQueryBinaryExpression
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
                    Operator = EqualityOperator.Equals,
                    Value = "10"
                }
            };

            CqlExpressionVisitorContext context = new CqlExpressionVisitorContext();

            CqlExpressionVisitor cqlExpressionVisitor =
                new CqlExpressionVisitor(
                    new QueryConditionToPropertyCriteriaMapper(
                        new PropertyDataTypeResolver(new ContentType())), context);

            // Act
            cqlExpressionVisitor.VisitQueryExpression(orExpression);

            IEnumerable<PropertyCriteriaCollection> propertyCriteriaCollection = context.GetCriteria();

            // Assert
            propertyCriteriaCollection.Should().HaveCount(1);
        }

        [Fact]
        public void Test_can_map_nested_expressions()
        {
            // Arrange
            CqlQueryBinaryExpression expressions = new CqlQueryBinaryExpression
            {
                Operator = ConditionalOperator.Or,
                LeftExpression = new CqlQueryBinaryExpression
                {
                    Operator = ConditionalOperator.Or,
                    LeftExpression = new CqlQueryBinaryExpression
                    {
                        Operator = ConditionalOperator.Or,
                        LeftExpression = new CqlQueryCondition
                        {
                            Identifier = "PageName",
                            Operator = EqualityOperator.Equals,
                            Value = "1"
                        },
                        RightExpression = new CqlQueryBinaryExpression
                        {
                            Operator = ConditionalOperator.Or,
                            LeftExpression = new CqlQueryCondition
                            {
                                Identifier = "PageName",
                                Operator = EqualityOperator.Equals,
                                Value = "2"
                            },
                            RightExpression = new CqlQueryCondition
                            {
                                Identifier = "PageName",
                                Operator = EqualityOperator.Equals,
                                Value = "3"
                            }
                        }
                    },
                    RightExpression = new CqlQueryBinaryExpression
                    {
                        Operator = ConditionalOperator.Or,
                        LeftExpression = new CqlQueryCondition
                        {
                            Identifier = "PageName",
                            Operator = EqualityOperator.Equals,
                            Value = "4"
                        },
                        RightExpression = new CqlQueryCondition
                        {
                            Identifier = "PageName",
                            Operator = EqualityOperator.Equals,
                            Value = "5"
                        }
                    }
                },
                RightExpression = new CqlQueryBinaryExpression
                {
                    Operator = ConditionalOperator.And,
                    RightExpression = new CqlQueryCondition
                    {
                        Identifier = "PageName",
                        Operator = EqualityOperator.Equals,
                        Value = "6"
                    },
                    LeftExpression = new CqlQueryCondition
                    {
                        Identifier = "PageName",
                        Operator = EqualityOperator.Equals,
                        Value = "7"
                    }
                }
            };

            CqlExpressionVisitorContext context = new CqlExpressionVisitorContext();

            CqlExpressionVisitor cqlExpressionVisitor =
                new CqlExpressionVisitor(
                    new QueryConditionToPropertyCriteriaMapper(
                        new PropertyDataTypeResolver(new ContentType())), context);

            // Act
            cqlExpressionVisitor.VisitQueryExpression(expressions);

            IEnumerable<PropertyCriteriaCollection> propertyCriteriaCollection = context.GetCriteria();

            // Assert
            propertyCriteriaCollection.Should().HaveCount(6);
        }
    }
}
