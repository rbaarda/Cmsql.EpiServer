using Cql.Query;
using Cql.Query.Execution;
using EPiServer;

namespace Cql.EpiServer.Internal
{
    internal class CqlExpressionVisitor : ICqlQueryExpressionVisitor
    {
        private readonly QueryConditionToPropertyCriteriaMapper _conditionToCriteriaMapper;

        protected readonly CqlExpressionVisitorContext Context;

        internal CqlExpressionVisitor(
            QueryConditionToPropertyCriteriaMapper conditionToCriteriaMapper,
            CqlExpressionVisitorContext context)
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
            CqlExpressionVisitor visitor = binaryExpression.Operator == ConditionalOperator.Or
                ? new CqlBinaryOrExpressionVisitor(_conditionToCriteriaMapper, Context)
                : new CqlExpressionVisitor(_conditionToCriteriaMapper, Context);

            binaryExpression.LeftExpression.Accept(visitor);
            binaryExpression.RightExpression.Accept(visitor);
        }
    }
}
