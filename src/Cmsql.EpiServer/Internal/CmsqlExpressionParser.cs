using Cmsql.Query;
using EPiServer.DataAbstraction;

namespace Cmsql.EpiServer.Internal
{
    internal class CmsqlExpressionParser
    {
        public CmsqlExpressionVisitorContext Parse(
            ContentType contentType,
            ICmsqlQueryExpression expression)
        {
            CmsqlExpressionVisitorContext context = new CmsqlExpressionVisitorContext();

            if (contentType == null || expression == null)
            {
                return context;
            }
            
            CmsqlExpressionVisitor visitor =
                new CmsqlExpressionVisitor(
                    new QueryConditionToPropertyCriteriaMapper(
                        new PropertyDataTypeResolver(contentType)), context);

            expression.Accept(visitor);

            return context;
        }
    }
}
