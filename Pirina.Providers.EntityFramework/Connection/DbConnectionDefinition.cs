//  ----------------------------------------------------------------------- 
//   <copyright file="DbConnectionDefinition.cs" company="Glasswall Solutions Ltd.">
//       Glasswall Solutions Ltd.
//   </copyright>
//  ----------------------------------------------------------------------- 

namespace Pirina.Providers.EntityFramework.Connection
{
    using Kernel.Data.Connection;

    /// <summary>
    ///     Define database properties from which connection string could be built
    /// </summary>
    public class DbConnectionDefinition : IDbConnectionDefinition
    {
        #region Methods

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("DataSource: {0}, Database: {1}, UserName: {2}", DataSource, Database, UserName);
        }

        #endregion

        #region properties

        /// <summary>
        ///     Gets the database name.
        /// </summary>
        /// <value>
        ///     The database.
        /// </value>
        public string Database { get; internal set; }

        /// <summary>
        ///     Gets the data source.
        /// </summary>
        /// <value>
        ///     The data source.
        /// </value>
        public string DataSource { get; internal set; }

        /// <summary>
        ///     Gets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; internal set; }

        /// <summary>
        ///     Gets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; internal set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is integrated connection.
        /// </summary>
        /// <value><c>true</c> if this instance is integrated connection; otherwise, <c>false</c>.</value>
        public bool IntegratedSecurity => string.IsNullOrWhiteSpace(UserName) && string.IsNullOrWhiteSpace(Password);

        #endregion
    }
}