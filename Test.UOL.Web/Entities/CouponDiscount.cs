using System.Text.Json.Serialization;
using Test.UOL.Web.Entities.Enums;
using Test.UOL.Web.Entities.Helpers;

namespace Test.UOL.Web.Entities;

public class CouponDiscount
{
    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("value")]
    [JsonConverter(typeof(DecimalJsonConverter))]
    public decimal Value { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public CouponDiscountType Type { get; set; }
}