using System;
using Cql.EpiServer.Internal;
using Cql.Query;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.Filters;
using FluentAssertions;
using Xunit;

namespace Cql.EpiServer.Test.Internal
{
    public class QueryConditionToPropertyCriteriaMapperTest
    {
        [Fact]
        public void Test_can_map_query_condition_to_property_criteria()
        {
            // Arrange
            ContentType contentType = new ContentType
            {
                PropertyDefinitions =
                {
                    new PropertyDefinition
                    {
                        Name = "FooBar",
                        Type = new PropertyDefinitionType
                        {
                            DataType = PropertyDataType.Number
                        }
                    }
                }
            };

            CqlQueryCondition condition = new CqlQueryCondition
            {
                Identifier = "FooBar",
                Operator = EqualityOperator.GreaterThan,
                Value = "5"
            };

            QueryConditionToPropertyCriteriaMapper mapper = new QueryConditionToPropertyCriteriaMapper(
                new PropertyDataTypeResolver(contentType));

            // Act
            bool isMapSuccessfull = mapper.TryMap(condition, out PropertyCriteria criteria);

            // Assert
            isMapSuccessfull.Should().BeTrue();
            criteria.Name.ShouldBeEquivalentTo(condition.Identifier);
            criteria.Condition.ShouldBeEquivalentTo(CompareCondition.GreaterThan);
            criteria.Value.ShouldBeEquivalentTo(condition.Value);
            criteria.Type.ShouldBeEquivalentTo(PropertyDataType.Number);
        }

        [Fact]
        public void Test_can_map_query_condition_with_meta_data_property_to_property_criteria()
        {
            // Arrange
            CqlQueryCondition condition = new CqlQueryCondition
            {
                Identifier = MetaDataProperties.PageName,
                Operator = EqualityOperator.GreaterThan,
                Value = "5"
            };

            QueryConditionToPropertyCriteriaMapper mapper = new QueryConditionToPropertyCriteriaMapper(
                new PropertyDataTypeResolver(new ContentType()));

            // Act
            bool isMapSuccessfull = mapper.TryMap(condition, out PropertyCriteria criteria);

            // Assert
            isMapSuccessfull.Should().BeTrue();
            criteria.Name.ShouldBeEquivalentTo(condition.Identifier);
            criteria.Condition.ShouldBeEquivalentTo(CompareCondition.GreaterThan);
            criteria.Value.ShouldBeEquivalentTo(condition.Value);
            criteria.Type.ShouldBeEquivalentTo(PropertyDataType.String);
        }

        [Fact]
        public void Test_when_condition_is_null_mapping_should_return_false()
        {
            // Arrange
            QueryConditionToPropertyCriteriaMapper mapper = new QueryConditionToPropertyCriteriaMapper(
                new PropertyDataTypeResolver(new ContentType()));

            // Act
            bool isMapSuccessfull = mapper.TryMap(null, out PropertyCriteria criteria);

            // Assert
            isMapSuccessfull.Should().BeFalse();
            criteria.Should().BeNull();
        }

        [Fact]
        public void Test_when_property_is_unkown_mapping_should_return_false()
        {
            // Arrange
            CqlQueryCondition condition = new CqlQueryCondition
            {
                Identifier = "This is some unknown property",
                Operator = EqualityOperator.GreaterThan,
                Value = "5"
            };

            QueryConditionToPropertyCriteriaMapper mapper = new QueryConditionToPropertyCriteriaMapper(
                new PropertyDataTypeResolver(new ContentType()));

            // Act
            bool isMapSuccessfull = mapper.TryMap(condition, out PropertyCriteria criteria);

            // Assert
            isMapSuccessfull.Should().BeFalse();
            criteria.Should().BeNull();
        }

        [Theory]
        [InlineData(EqualityOperator.GreaterThan, CompareCondition.GreaterThan)]
        [InlineData(EqualityOperator.Equals, CompareCondition.Equal)]
        [InlineData(EqualityOperator.LessThan, CompareCondition.LessThan)]
        [InlineData(EqualityOperator.NotEquals, CompareCondition.NotEqual)]
        public void Test_can_map_equality_operator_to_compare_condition(EqualityOperator operatr, CompareCondition condition)
        {
            QueryConditionToPropertyCriteriaMapper mapper =
                new QueryConditionToPropertyCriteriaMapper(
                    new PropertyDataTypeResolver(new ContentType()));

            CompareCondition mappedCondition = mapper.MapEqualityOperatorToCompareCondition(operatr);

            mappedCondition.ShouldBeEquivalentTo(condition);
        }

        [Fact]
        public void Test_when_mapping_unknown_equality_operator_throw()
        {
            QueryConditionToPropertyCriteriaMapper mapper =
                new QueryConditionToPropertyCriteriaMapper(
                    new PropertyDataTypeResolver(new ContentType()));

            mapper.Invoking(m => m.MapEqualityOperatorToCompareCondition(EqualityOperator.None))
                .ShouldThrow<InvalidOperationException>();
        }
    }
}
