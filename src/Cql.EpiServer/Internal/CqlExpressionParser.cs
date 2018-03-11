using Cql.Query;
using EPiServer.DataAbstraction;

namespace Cql.EpiServer.Internal
{
    internal class CqlExpressionParser
    {
        public CqlExpressionVisitorContext Parse(
            ContentType contentType,
            ICqlQueryExpression expression)
        {
            CqlExpressionVisitorContext context = new CqlExpressionVisitorContext();

            if (contentType == null || expression == null)
            {
                return context;
            }
            
            CqlExpressionVisitor visitor =
                new CqlExpressionVisitor(
                    new QueryConditionToPropertyCriteriaMapper(
                        new PropertyDataTypeResolver(contentType)), context);

            expression.Accept(visitor);

            return context;
        }
    }
}
