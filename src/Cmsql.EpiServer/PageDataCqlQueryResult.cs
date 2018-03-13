using Cql.Query;
using EPiServer.Core;

namespace Cmsql.EpiServer
{
    public class PageDataCqlQueryResult : ICqlQueryResult
    {
        public PageData Page { get; }

        public PageDataCqlQueryResult(PageData pageData)
        {
            Page = pageData;
        }
    }
}
