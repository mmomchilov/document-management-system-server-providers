using System.Threading.Tasks;
using Pirina.Kernel.Storage;
using Pirina.Providers.Storage.AzureBlob.Configuration;
using Pirina.Providers.Storage.AzureBlob.Connection;
using Pirina.Providers.Storage.AzureBlob.Exceptions;
using Microsoft.WindowsAzure.Storage;
using Moq;
using NUnit.Framework;

namespace Pirina.Providers.Storage.AzureBlob.Tests.L0.Connection
{
    [TestFixture]
    [Category("L0")]
    public class BlobConnectionManagerTests
    {
        private Mock<IStorageConfiguration> _mockConnectionString;

        [SetUp]
        public void Init()
        {
            _mockConnectionString = new Mock<IStorageConfiguration>();
        }

        [TestFixture]
        public class Constructor : BlobConnectionManagerTests
        {
            [Test]
            public void Null_Blob_Configuration_Factory_Will_Throw_Argument_Null_Exception()
            {
                //Arrange

                //Act
                BlobConnectionManager TestDelegate() => new BlobConnectionManager(null);

                //Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentNullException.With.Property("ParamName")
                        .EqualTo("storageConfigurationFactory"));
            }

            [Test]
            public void Null_Blob_Configuration_Will_Throw_Argument_Null_Exception()
            {
                //Arrange

                //Act
                BlobConnectionManager TestDelegate() => new BlobConnectionManager(() => null);

                //Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentNullException.With.Property("ParamName")
                        .EqualTo("storageConfigurationFactory"));
            }

            [Test]
            public void Can_Be_Constructed()
            {
                //Arrange

                //Act
                var manager = new BlobConnectionManager(() => Mock.Of<IStorageConfiguration>());

                //Assert
                Assert.That(manager, Is.Not.Null);
            }
        }

        [TestFixture]
        public class GetCloudBlobClientMethod : BlobConnectionManagerTests
        {
            [Test]
            public void Can_Get_Cloud_Blob_Client()
            {
                //Arrange
                _mockConnectionString.Setup(x => x.ConnectionString).Returns("UseDevelopmentStorage=true");
                var manager = new BlobConnectionManager(() => _mockConnectionString.Object);
                var expectedAccount = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();

                //Act
                var result = manager.GetStorageClient().GetAwaiter().GetResult();

                //Assert
                Assert.That(result.BaseUri, Is.EqualTo(expectedAccount.BaseUri));
            }

            [Test]
            public void Failure_To_Parse_Connection_String_Throws_Blob_Storage_Connection_Error()
            {
                //Arrange
                const string connectionString = "Barry";
                _mockConnectionString.Setup(x => x.ConnectionString).Returns(connectionString);
                var manager = new BlobConnectionManager(() => _mockConnectionString.Object);

                //Act
                async Task TestDelegate() => await manager.GetStorageClient();

                //Assert
                Assert.That(TestDelegate,
                    Throws.TypeOf<BlobStorageConnectionError>().With.Message.Contain(connectionString));
            }
        }
    }
}
