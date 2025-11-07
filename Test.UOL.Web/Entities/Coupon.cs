using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Test.UOL.Web.Enums;

namespace Test.UOL.Web.Entities
{
    public class Coupon
    {
        public string? Key { get; set; }
        public decimal? Value { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ECouponType Type { get; set; }
    }
}
