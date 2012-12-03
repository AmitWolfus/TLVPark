using FluentNHibernate.Mapping;
using TLVPark.Model;

namespace TLVPark.Mapping
{
    public class BusinessMap : ClassMap<Business>
    {
        public BusinessMap()
        {
            Table("Businesses");
            Id(x => x.FacebookId).GeneratedBy.Assigned();
            HasManyToMany(x => x.RecommendedParkings).AsBag().Cascade.All();
            Map(x => x.BusinessType);
        }
    }
}
