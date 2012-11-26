using System.Runtime.Serialization;

namespace TLVPark.Model
{
    /// <summary>
    /// Represents a public parking space
    /// </summary>
    [DataContract]
    public class Parking
    {
        #region Properties

        /// <summary>
        /// Represents the parking's ID in the TLV database
        /// </summary>
        [DataMember]
        public virtual int ID { get; set; }
        /// <summary>
        /// Gets and sets the name of the parking
        /// </summary>
        [DataMember]
        public virtual string Name { get; set; }
        /// <summary>
        /// Gets and sets the number of vehicles that the parking
        /// can contain
        /// </summary>
        [DataMember]
        public virtual int Capacity { get; set; }
        /// <summary>
        /// Gets and sets the geographical location of the parking
        /// </summary>
        [DataMember]
        public GeoPoint Location { get; set; }

        #endregion
    }
}
