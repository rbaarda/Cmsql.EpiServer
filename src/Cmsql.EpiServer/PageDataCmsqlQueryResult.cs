using Cmsql.Query;
using EPiServer.Core;

namespace Cmsql.EpiServer
{
    public class PageDataCmsqlQueryResult : ICmsqlQueryResult
    {
        public PageData Page { get; }

        public PageDataCmsqlQueryResult(PageData pageData)
        {
            Page = pageData;
        }
    }
}
