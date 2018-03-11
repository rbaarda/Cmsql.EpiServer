using System.Collections.Generic;
using System.Linq;
using Cql.EpiServer.Internal;
using EPiServer;
using FluentAssertions;
using Xunit;

namespace Cql.EpiServer.Test.Internal
{
    public class CqlExpressionVisitorContextTest
    {
        [Fact]
        public void Test_can_add_criteria_when_no_criteria_collection_has_been_pushed_yet()
        {
            CqlExpressionVisitorContext context = new CqlExpressionVisitorContext();

            context.AddPropertyCriteria(new PropertyCriteria());

            IEnumerable<PropertyCriteriaCollection> criteria = context.GetCriteria().ToList();
            criteria.Should().HaveCount(1);
            criteria.Single().Should().HaveCount(1);
        }

        [Fact]
        public void Test_can_add_criteria_when_criteria_collection_has_been_pushed()
        {
            CqlExpressionVisitorContext context = new CqlExpressionVisitorContext();
            
            context.AddPropertyCriteria(new PropertyCriteria());

            IEnumerable<PropertyCriteriaCollection> criteria = context.GetCriteria().ToList();
            criteria.Should().HaveCount(1);
            criteria.Single().Should().HaveCount(1);
        }
    }
}
