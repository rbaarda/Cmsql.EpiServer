using Cql.Query;
using Cql.Query.Execution;
using EPiServer;

namespace Cmsql.EpiServer.Internal
{
    internal class CmsqlExpressionVisitor : ICqlQueryExpressionVisitor
    {
        private readonly QueryConditionToPropertyCriteriaMapper _conditionToCriteriaMapper;

        protected readonly CmsqlExpressionVisitorContext Context;

        internal CmsqlExpressionVisitor(
            QueryConditionToPropertyCriteriaMapper conditionToCriteriaMapper,
            CmsqlExpressionVisitorContext context)
        {
            _conditionToCriteriaMapper = conditionToCriteriaMapper;
            Context = context;
        }

        public virtual void VisitQueryCondition(CqlQueryCondition condition)
        {
            if (condition == null)
            {
                Context.Errors.Add(new CqlQueryExecutionError("Could not process malformed query condition."));
                return;
            }

            if (_conditionToCriteriaMapper.TryMap(condition, out PropertyCriteria criteria))
            {
                Context.AddPropertyCriteria(criteria);
            }
            else
            {
                Context.Errors.Add(new CqlQueryExecutionError($"Could not find property '{condition.Identifier}'"));
            }
        }

        public virtual void VisitQueryExpression(CqlQueryBinaryExpression binaryExpression)
        {
            CmsqlExpressionVisitor visitor = binaryExpression.Operator == ConditionalOperator.Or
                ? new CmsqlBinaryOrExpressionVisitor(_conditionToCriteriaMapper, Context)
                : new CmsqlExpressionVisitor(_conditionToCriteriaMapper, Context);

            binaryExpression.LeftExpression.Accept(visitor);
            binaryExpression.RightExpression.Accept(visitor);
        }
    }
}
