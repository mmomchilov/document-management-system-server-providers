namespace Pirina.Providers.Storage.AzureCosmosDatabase
{
    public interface ICosmosDbConfiguration
    {
        string DatabaseId { get; }
        string EndPointUri { get; }
        string PrimaryKey { get; }
    }
}
