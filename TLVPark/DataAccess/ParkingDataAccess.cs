using System;
using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Criterion;
using TLVPark.Mapping;
using TLVPark.Model;

namespace TLVPark.DataAccess
{

    /// <summary>
    /// Defines the data access layer
    /// </summary>
    public class ParkingDataAccess : IParkingDataAccess
    {
        #region Data Members

        private static readonly ISessionFactory _sessionFactory;
        private static readonly Lazy<IDictionary<int, Parking>> _parkingsCache;
        private bool _isDisposed;

        #endregion

        #region Properties

        private static IDictionary<int, Parking> ParkingCache
        {
            get { return _parkingsCache.Value; }
        }

        #endregion

        #region Constructors

        static ParkingDataAccess()
        {
            // Configure the session factory
            _sessionFactory =
                Fluently.Configure()
                        .Database(SQLiteConfiguration.Standard.UsingFile(@"C:\BuizParkDB.db"))
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ParkingMap>())
                        .BuildSessionFactory();
            _parkingsCache = new Lazy<IDictionary<int, Parking>>(GetAllParkings);
        }

        #endregion

        #region IParkingDataAccess Members

        public IEnumerable<Parking> GetParkingsByGeo(GeoPoint point, double radius)
        {
            var list = new List<Parking>();
            // Go over all the parkings and calculate the distance
            foreach (var parking in ParkingCache.Values)
            {
                var distance = parking.Location.Distance(point);
                if (distance <= radius)
                {
                    list.Add(parking);
                }
            }
            return list;
        }

        public IEnumerable<Parking> GetParkingByBusiness(int businessId)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    // Get the business that corresponds to the given id
                    var business =
                        session.CreateCriteria<Business>()
                               .Add(Restrictions.IdEq(businessId))
                               .List<Business>()
                               .FirstOrDefault();
                    // TODO: Add more complex logic
                    // Return the recommended parkings for the business or an empty list if there are non available
                    return business == null ? new List<Parking>() :
                        business.RecommendedParkings.Select(
                            parking =>
                            new Parking
                            {
                                Capacity = parking.Capacity,
                                ID = parking.ID,
                                Name = parking.Name,
                                Location = (GeoPoint)parking.Location.Clone()
                            });

                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
        }

        #endregion

        #region Other Members

        public static IDictionary<int, Parking> GetAllParkings()
        {
            var dictionary = new Dictionary<int, Parking>();
            using (var session = _sessionFactory.OpenSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    // Select all the parkings
                    foreach (var parking in session.CreateCriteria<Parking>().List<Parking>())
                    {
                        // Create a stateless version of the parking
                        var statelessParking = new Parking
                        {
                            Capacity = parking.Capacity,
                            ID = parking.ID,
                            Location = (GeoPoint)parking.Location.Clone(),
                            Name = parking.Name
                        };
                        dictionary.Add(statelessParking.ID, statelessParking);
                    }
                }
            }
            return dictionary;
        }

        #endregion
    }
}