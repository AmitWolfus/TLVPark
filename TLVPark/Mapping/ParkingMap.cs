using FluentNHibernate.Mapping;
using TLVPark.Model;

namespace TLVPark.Mapping
{
    public class ParkingMap : ClassMap<Parking>
    {
        public ParkingMap()
        {
            Table("Parkings");
            Id(x => x.ID);
            Map(x => x.Name).Length(40).Not.Nullable();
            Map(x => x.Capacity);
            Map(x => x.Latitude);
            Map(x => x.Longtitude);
            Map(x => x.StreetName);
            Map(x => x.HouseNumber).Length(10);
            Map(x => x.CurrentState).CustomType<ParkingStateUserType>();
            HasManyToMany(x => x.Businesses).AsBag().Inverse();
        }
    }
}
