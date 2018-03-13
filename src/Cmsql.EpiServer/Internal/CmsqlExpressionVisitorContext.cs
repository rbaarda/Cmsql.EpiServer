using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cmsql.Query.Execution;
using EPiServer;

namespace Cmsql.EpiServer.Internal
{
    internal class CmsqlExpressionVisitorContext
    {
        private readonly Stack<PropertyCriteriaCollection> _propertyCriteriaCollectionStack;

        internal IList<CmsqlQueryExecutionError> Errors { get; }

        internal CmsqlExpressionVisitorContext()
        {
            _propertyCriteriaCollectionStack = new Stack<PropertyCriteriaCollection>();

            Errors = new List<CmsqlQueryExecutionError>();
        }

        internal void AddPropertyCriteria(PropertyCriteria propertyCriteria)
        {
            Debug.Assert(propertyCriteria != null);

            if (!_propertyCriteriaCollectionStack.Any())
            {
                PushNewPropertyCriteriaCollection();
            }

            _propertyCriteriaCollectionStack.Peek().Add(propertyCriteria);
        }

        internal void PushNewPropertyCriteriaCollection()
        {
            _propertyCriteriaCollectionStack.Push(new PropertyCriteriaCollection());
        }

        internal IEnumerable<PropertyCriteriaCollection> GetCriteria()
        {
            return _propertyCriteriaCollectionStack;
        }
    }
}
