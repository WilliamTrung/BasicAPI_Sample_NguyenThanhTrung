using eStoreWeb.Models;
using NuGet.Protocol;
using System.Text.Json;

namespace eStoreWeb.Extension
{
    public static class SessionExtension
    {
        public static void SetLoginUser(this ISession session, Member member)
        {
            session.SetString("login-user", member.ToJson());
        }
        public static Member? GetLoginUser(this ISession session)
        {
            var member_json = session.GetString("login-user");
            if (member_json == null) { 
                return null;
            } else
            {
                var member = JsonSerializer.Deserialize<Member>(member_json);
                return member;
            }
        }
        public static void SetOrder(this ISession session, Order order)
        {
            session.SetString("order", order.ToJson());
        }
        public static Order? GetOrder(this ISession session)
        {
            var order_json = session.GetString("order");
            if(order_json == null)
            {
                return null;
            } else
            {
                var order = JsonSerializer.Deserialize<Order>(order_json);
                return order;
            }

        }
    }
}
