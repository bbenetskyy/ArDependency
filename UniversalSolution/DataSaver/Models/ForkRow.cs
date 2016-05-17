﻿namespace DataSaver.Models
{
    /// <summary>
    ///Represents strongly named DataRow class.
    ///</summary>
    public class ForkRow
    {
        /// <summary>
        /// Id stored in DB, ONLY auto increment so please NOT fill it
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Sport Type
        /// </summary>
        public string Sport { get; set; }

        /// <summary>
        /// Default place for Team Names, time of match and other details
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Details for first type of Fork 
        /// </summary>
        public string TypeFirst { get; set; }

        /// <summary>
        /// Value for first coef of Fork
        /// </summary>
        public string CoefFirst { get; set; }

        /// <summary>
        /// Details for second type of Fork 
        /// </summary>
        public string TypeSecond { get; set; }

        /// <summary>
        /// Value for second coef of Fork
        /// </summary>
        public string CoefSecond { get; set; }

        /// <summary>
        /// Game time
        /// </summary>
        public string MatchDateTime { get; set; }

        /// <summary>
        /// First Bookmaker Name
        /// </summary>
        public string BookmakerFirst { get; set; }

        /// <summary>
        /// Second Bookmaker Name
        /// </summary>
        public string BookmakerSecond { get; set; }
    }
}