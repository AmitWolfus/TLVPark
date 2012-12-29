using System;
using System.Collections.Generic;
using System.Configuration;
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
            var conn = ConfigurationManager.ConnectionStrings["Prod"];
            // Configure the session factory
            _sessionFactory =
                Fluently.Configure()
                        .Database(MySQLConfiguration.Standard.ConnectionString(conn.ConnectionString))
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ParkingMap>())
                        .BuildSessionFactory();
            _parkingsCache = new Lazy<IDictionary<int, Parking>>(GetAllParkings);
        }

        #endregion

        #region IParkingDataAccess Members

        public IEnumerable<Parking> GetParkingsByGeo(GeoPoint point, double radius)
        {
            return GetParkingsByGeo(point, radius, -1);
        }

        public IEnumerable<Parking> GetParkingsByGeo(GeoPoint point, double radius, int parkingsToTake)
        {
            var parkings = new List<DistanceParking>();

            // Go over all the parkings and calculate the distance
            foreach (var parking in ParkingCache.Values)
            {
                var location = new GeoPoint(parking.Longtitude, parking.Latitude);
                var distance = location.Distance(point);
                if (distance <= radius)
                {
                    parkings.Add(new DistanceParking(){Distance = distance, Parking = parking});
                }
            }

            if (parkingsToTake < 0)
            {
                return parkings.Select(dist => dist.Parking);
            }
            else
            {
                parkings.Sort(new DistanceComparer());
                return parkings.Take(parkingsToTake > parkings.Count ? parkings.Count : parkingsToTake).Select(dist => dist.Parking);
            }
        }

        public void AddBusiness(Business business)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    session.Save(business);
                    trans.Commit();
                }
                session.Close();
            }
        }

        public IEnumerable<Parking> GetParkingByBusiness(long businessId)
        {
            IEnumerable<Parking> recommended;
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
                    // Return the recommended parkings for the business or an empty list if there are non available
                    recommended =  business == null ? new List<Parking>() :
                        business.RecommendedParkings.Select(
                            parking =>
                            new Parking
                            {
                                Capacity = parking.Capacity,
                                ID = parking.ID,
                                Name = parking.Name,
                                Longtitude = parking.Longtitude,
                                Latitude = parking.Latitude,
                                StreetName = parking.StreetName,
                                HouseNumber = parking.HouseNumber,
                                CurrentState = parking.CurrentState,
                            }).ToList();

                }
                session.Close();
            }
            return recommended;
        }

        public void SetStatusForParking(int id, ParkingState state)
        {
            if (!_parkingsCache.Value.ContainsKey(id))
            {
                throw new KeyNotFoundException(string.Format("Parking with id {0} doesn't exist", id));
            }
            // Get the parking
            var parking = _parkingsCache.Value[id];
            // Change the state of the parking
            lock (parking)
            {
                parking.CurrentState = state;
            }
            using (var session = _sessionFactory.OpenSession())
            {
                using (var trans = session.BeginTransaction())
                {
                    var dbPark = session.CreateCriteria<Parking>().Add(Restrictions.IdEq(id)).List<Parking>().FirstOrDefault();
                    dbPark.CurrentState = state;
                    session.SaveOrUpdate(dbPark);
                    trans.Commit();
                }
                session.Close();
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
                            Latitude = parking.Latitude,
                            Longtitude = parking.Longtitude,
                            Name = parking.Name,
                            StreetName = parking.StreetName,
                            HouseNumber = parking.HouseNumber,
                            CurrentState = parking.CurrentState,
                        };
                        dictionary.Add(statelessParking.ID, statelessParking);
                    }
                }
            }
            return dictionary;
        }

        #endregion

        #region Distance Comparer

        private struct DistanceParking
        {
            public double Distance { get; set; }
            public Parking Parking { get; set; }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is DistanceParking && Equals((DistanceParking) obj);
            }
            public bool Equals(DistanceParking other)
            {
                return Parking.ID == other.Parking.ID;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Distance.GetHashCode() * 397) ^ (Parking != null ? Parking.GetHashCode() : 0);
                }
            }
        }

        private class DistanceComparer : IComparer<DistanceParking>
        {
            public int Compare(DistanceParking x, DistanceParking y)
            {
                return x.Distance == y.Distance ? 0 : x.Distance < y.Distance ? -1 : 1;
            }
        }

        #endregion
    }
}