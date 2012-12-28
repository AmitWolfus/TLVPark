using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using TLVPark.Model;

namespace TLVPark.Mapping
{
    public class ParkingStateUserType : IUserType
    {
        // Summary:
        //     Are objects of this type mutable?
        public bool IsMutable { get { return false; } }
        //
        // Summary:
        //     The type returned by NullSafeGet()
        public Type ReturnedType { get { return typeof(ParkingState); } }
        //
        // Summary:
        //     The SQL types for the columns mapped by this type.
        public SqlType[] SqlTypes { get { return new[] { SqlTypeFactory.Int32 }; } }

        // Summary:
        //     Reconstruct an object from the cacheable representation. At the very least
        //     this method should perform a deep copy if the type is mutable. (optional
        //     operation)
        //
        // Parameters:
        //   cached:
        //     the object to be cached
        //
        //   owner:
        //     the owner of the cached object
        //
        // Returns:
        //     a reconstructed object from the cachable representation
        public object Assemble(object cached, object owner)
        {
            return cached;
        }
        //
        // Summary:
        //     Return a deep copy of the persistent state, stopping at entities and at collections.
        //
        // Parameters:
        //   value:
        //     generally a collection element or entity field
        //
        // Returns:
        //     a copy
        public object DeepCopy(object value)
        {
            return value;
        }
        //
        // Summary:
        //     Transform the object into its cacheable representation. At the very least
        //     this method should perform a deep copy if the type is mutable. That may not
        //     be enough for some implementations, however; for example, associations must
        //     be cached as identifier values. (optional operation)
        //
        // Parameters:
        //   value:
        //     the object to be cached
        //
        // Returns:
        //     a cacheable representation of the object
        public object Disassemble(object value)
        {
            return value;
        }
        //
        // Summary:
        //     Compare two instances of the class mapped by this type for persistent "equality"
        //     ie. equality of persistent state
        //
        // Parameters:
        //   x:
        //
        //   y:
        public bool Equals(object x, object y)
        {
            return object.Equals(x, y);
        }
        //
        // Summary:
        //     Get a hashcode for the instance, consistent with persistence "equality"
        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }
        //
        // Summary:
        //     Retrieve an instance of the mapped class from a JDBC resultset.  Implementors
        //     should handle possibility of null values.
        //
        // Parameters:
        //   rs:
        //     a IDataReader
        //
        //   names:
        //     column names
        //
        //   owner:
        //     the containing entity
        //
        // Exceptions:
        //   NHibernate.HibernateException:
        //     HibernateException
        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            int state = (int)rs[names[0]];
            return (ParkingState)state;
        }
        //
        // Summary:
        //     Write an instance of the mapped class to a prepared statement.  Implementors
        //     should handle possibility of null values.  A multi-column type should be
        //     written to parameters starting from index.
        //
        // Parameters:
        //   cmd:
        //     a IDbCommand
        //
        //   value:
        //     the object to write
        //
        //   index:
        //     command parameter index
        //
        // Exceptions:
        //   NHibernate.HibernateException:
        //     HibernateException
        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            ((IDataParameter)cmd.Parameters[index]).Value = (int)value;
        }
        //
        // Summary:
        //     During merge, replace the existing (target) value in the entity we are merging
        //     to with a new (original) value from the detached entity we are merging. For
        //     immutable objects, or null values, it is safe to simply return the first
        //     parameter. For mutable objects, it is safe to return a copy of the first
        //     parameter. For objects with component values, it might make sense to recursively
        //     replace component values.
        //
        // Parameters:
        //   original:
        //     the value from the detached entity being merged
        //
        //   target:
        //     the value in the managed entity
        //
        //   owner:
        //     the managed entity
        //
        // Returns:
        //     the value to be merged
        public object Replace(object original, object target, object owner)
        {
            return original;
        }
    }
}