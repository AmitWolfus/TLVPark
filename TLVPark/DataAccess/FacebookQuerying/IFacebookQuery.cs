using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLVPark.DataAccess.FacebookQuerying
{
    public enum FacebookTable
    {
        Place
    }
    /// <summary>
    /// Represents a query to be run against the facebook FQL API
    /// </summary>
    public interface IFacebookQuery : IDisposable
    {
        /// <summary>
        /// Sets the columns that will be selected
        /// </summary>
        /// <param name="columns">The column names that will be selected</param>
        /// <returns>The facebook query with the columns saved in it</returns>
        IFacebookQuery Select(params string[] columns);

        /// <summary>
        /// Sets the table to query from
        /// </summary>
        /// <param name="table">The type of table that will be selected from</param>
        /// <returns>The query with the table set in it</returns>
        IFacebookQuery From(FacebookTable table);

        /// <summary>
        /// Sets the id that should be looked for
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IFacebookQuery WhereId(long id);

        /// <summary>
        /// Runs the FacebookQuery
        /// </summary>
        /// <returns>A dictionary containing the results, each with its column name</returns>
        Dictionary<string, string> Execute();
    }
}
