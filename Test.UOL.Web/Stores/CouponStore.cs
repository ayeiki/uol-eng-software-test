using Newtonsoft.Json;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;

namespace Test.UOL.Web.Stores
{
    public class CouponStore : ICouponStore
    {
        private IEnumerable<Coupon> _coupons { get; set; } = Enumerable.Empty<Coupon>();

        public CouponStore()
        {
            LoadCoupons();
        }
        public IEnumerable<Coupon> GetCoupons()
        {
            return _coupons;
        }

        public Coupon? GetCouponByKey(string key)
        {
            return _coupons.FirstOrDefault(x => x.Key == key);
        }
        private void LoadCoupons()
        {
            string jsonString = File.ReadAllText("files/cupom.json");
            if (string.IsNullOrEmpty(jsonString)) return;
            
           
            CouponFile? couponFile = JsonConvert.DeserializeObject<CouponFile?>(jsonString);
            if (couponFile != null)
            {
                _coupons = couponFile.Coupons;
            }
        }        
    }

    public class CouponFile
    {
        public IEnumerable<Coupon> Coupons { get; set; } = [];
    }
}