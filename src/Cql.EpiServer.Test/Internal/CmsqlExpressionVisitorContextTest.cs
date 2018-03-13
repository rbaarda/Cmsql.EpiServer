using System.Collections.Generic;
using System.Linq;
using EPiServer;
using FluentAssertions;
using Xunit;

namespace Cmsql.EpiServer.Test.Internal
{
    public class CmsqlExpressionVisitorContextTest
    {
        [Fact]
        public void Test_can_add_criteria_when_no_criteria_collection_has_been_pushed_yet()
        {
            CmsqlExpressionVisitorContext context = new CmsqlExpressionVisitorContext();

            context.AddPropertyCriteria(new PropertyCriteria());

            IEnumerable<PropertyCriteriaCollection> criteria = context.GetCriteria().ToList();
            criteria.Should().HaveCount(1);
            criteria.Single().Should().HaveCount(1);
        }

        [Fact]
        public void Test_can_add_criteria_when_criteria_collection_has_been_pushed()
        {
            CmsqlExpressionVisitorContext context = new CmsqlExpressionVisitorContext();
            
            context.AddPropertyCriteria(new PropertyCriteria());

            IEnumerable<PropertyCriteriaCollection> criteria = context.GetCriteria().ToList();
            criteria.Should().HaveCount(1);
            criteria.Single().Should().HaveCount(1);
        }
    }
}
