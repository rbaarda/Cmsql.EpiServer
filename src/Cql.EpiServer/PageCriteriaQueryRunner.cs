using System.Collections.Generic;
using System.Linq;
using Cql.EpiServer.Internal;
using Cql.Query;
using Cql.Query.Execution;
using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;

namespace Cql.EpiServer
{
    public class PageCriteriaQueryRunner : ICqlQueryRunner
    {
        private readonly IPageCriteriaQueryService _pageCriteriaQueryService;
        private readonly IContentTypeRepository _contentTypeRepository;

        public PageCriteriaQueryRunner(
            IPageCriteriaQueryService pageCriteriaQueryService,
            IContentTypeRepository contentTypeRepository)
        {
            _pageCriteriaQueryService = pageCriteriaQueryService;
            _contentTypeRepository = contentTypeRepository;
        }

        public CqlQueryExecutionResult ExecuteQueries(IEnumerable<CqlQuery> queries)
        {
            List<CqlQueryExecutionError> errors = new List<CqlQueryExecutionError>();

            List<PageData> result = new List<PageData>();
            foreach (CqlQuery query in queries)
            {
                ContentType contentType = _contentTypeRepository.Load(query.ContentType);
                if (contentType == null)
                {
                    errors.Add(new CqlQueryExecutionError($"Couldn't load content-type '{query.ContentType}'."));
                    continue;
                }

                CqlExpressionParser expressionParser = new CqlExpressionParser();
                CqlExpressionVisitorContext visitorContext = expressionParser.Parse(contentType, query.Criteria);
                if (visitorContext.Errors.Any())
                {
                    errors.AddRange(visitorContext.Errors);
                    continue;
                }

                PageReference searchStartNodeRef = GetStartSearchFromNode(query.StartNode);
                if (PageReference.IsNullOrEmpty(searchStartNodeRef))
                {
                    errors.Add(new CqlQueryExecutionError($"Couldn't process start node '{query.StartNode}'."));
                    continue;
                }

                foreach (PropertyCriteriaCollection propertyCriteriaCollection in visitorContext.GetCriteria())
                {
                    PageDataCollection foundPages = _pageCriteriaQueryService.FindPagesWithCriteria(
                        searchStartNodeRef,
                        propertyCriteriaCollection);
                    if (foundPages != null && foundPages.Any())
                    {
                        result.AddRange(foundPages);
                    }
                }
            }

            IEnumerable<ICqlQueryResult> pageDataCqlQueryResults =
                result.Select(p => new PageDataCqlQueryResult(p)).ToList();

            return new CqlQueryExecutionResult(pageDataCqlQueryResults, errors);
        }

        private PageReference GetStartSearchFromNode(CqlQueryStartNode startNode)
        {
            switch (startNode.StartNodeType)
            {
                case CqlQueryStartNodeType.Start:
                    return ContentReference.StartPage;
                case CqlQueryStartNodeType.Root:
                    return ContentReference.RootPage;
                case CqlQueryStartNodeType.Id:
                    if (int.TryParse(startNode.StartNodeId, out int rootNodeId))
                    {
                        return new PageReference(rootNodeId);
                    }
                    break;
            }
            return PageReference.EmptyReference;
        }
    }
}
