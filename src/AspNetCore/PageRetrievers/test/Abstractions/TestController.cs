using System.Threading.Tasks;
using BizStream.Extensions.Kentico.Xperience.AspNetCore.PageRetrievers.Tests.Abstractions;
using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;

[assembly: RegisterPageRoute( TestNode.CLASS_NAME, typeof( TestController ) )]

namespace BizStream.Extensions.Kentico.Xperience.AspNetCore.PageRetrievers.Tests.Abstractions
{

    public class TestController : Controller
    {

        [HttpGet( "" )]
        public async Task<IActionResult> Index( )
            => Content( "Hello, World!" );

        public IActionResult Test( [FromServices] IPageDataContextRetriever contextRetriever )
        {
            var context = contextRetriever.Retrieve<TestNode>();
            return Content( context.Page.DocumentName );
        }

    }

}
