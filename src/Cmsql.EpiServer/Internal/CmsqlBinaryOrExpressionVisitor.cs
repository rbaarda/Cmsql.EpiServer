using Cql.Query;

namespace Cmsql.EpiServer.Internal
{
    internal class CmsqlBinaryOrExpressionVisitor : CmsqlExpressionVisitor
    {
        internal CmsqlBinaryOrExpressionVisitor(
            QueryConditionToPropertyCriteriaMapper conditionToCriteriaMapper,
            CmsqlExpressionVisitorContext context)
            : base(conditionToCriteriaMapper, context)
        {
        }

        public override void VisitQueryCondition(CqlQueryCondition condition)
        {
            Context.PushNewPropertyCriteriaCollection();

            base.VisitQueryCondition(condition);
        }

        public override void VisitQueryExpression(CqlQueryBinaryExpression binaryExpression)
        {
            if (binaryExpression.Operator == ConditionalOperator.And)
            {
                Context.PushNewPropertyCriteriaCollection();
            }

            base.VisitQueryExpression(binaryExpression);
        }
    }
}
