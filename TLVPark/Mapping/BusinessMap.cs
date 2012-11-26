using FluentNHibernate.Mapping;
using TLVPark.Model;

namespace TLVPark.Mapping
{
    public class BusinessMap : ClassMap<Business>
    {
        public BusinessMap()
        {
            Id(x => x.FacebookId);
            HasMany(x => x.RecommendedParkings).Cascade.All();
            Map(x => x.BusinessType);
        }
    }
}
