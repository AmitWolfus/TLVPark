using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TLVPark.Model
{
    [DataContract]
    public enum BusinessType
    {
        Facebook = 0,
    }
    /// <summary>
    /// Represents a a business place linked to a facebook place
    /// </summary>
    [DataContract]
    public class Business
    {
        #region Properties

        /// <summary>
        /// Gets and sets the Facebook unique id of the business
        /// </summary>
        [DataMember]
        public virtual int FacebookId { get; set; }
        /// <summary>
        /// Gets the list of the recommended parkings for this business
        /// </summary>
        [DataMember]
        public virtual List<Parking> RecommendedParkings { get; private set; }
        
        [DataMember]
        public virtual BusinessType BusinessType { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new business object
        /// </summary>
        public Business()
        {
            RecommendedParkings = new List<Parking>();
        }

        #endregion
    }
}
