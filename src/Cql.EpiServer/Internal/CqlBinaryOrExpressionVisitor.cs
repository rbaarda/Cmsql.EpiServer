using Cql.Query;

namespace Cql.EpiServer.Internal
{
    internal class CqlBinaryOrExpressionVisitor : CqlExpressionVisitor
    {
        internal CqlBinaryOrExpressionVisitor(
            QueryConditionToPropertyCriteriaMapper conditionToCriteriaMapper,
            CqlExpressionVisitorContext context)
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
