using BusinessObject.Models;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace eStoreAPI
{
    public class StartupExtension
    {
        public static IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Category>("Categories");
            builder.EntitySet<Product>("Products");
            builder.EntitySet<Member>("Members");
            builder.EntitySet<Order>("Orders");
            //builder.EntitySet<OrderDetail>("OrderDetails");
            return builder.GetEdmModel();
        }
    }
}
