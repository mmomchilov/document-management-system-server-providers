using System;
using System.Threading.Tasks;
using Pirina.Kernel.Storage;
using Pirina.Providers.Storage.AzureBlob.Connection;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using NUnit.Framework;

namespace Pirina.Providers.Storage.AzureBlob.Tests.L0.Connection
{
    [TestFixture]
    [Category("L0")]
    public class BlobConnectionTests
    {
        private Mock<IStorageConnectionManager<CloudBlobClient>> _mockManager;
        private Uri _mockBlobUri;
        private Mock<CloudBlobClient> _mockClient;
        private Mock<CloudBlobContainer> _mockContainer;

        [SetUp]
        public void Init()
        {
            _mockManager = new Mock<IStorageConnectionManager<CloudBlobClient>>();
            _mockBlobUri = new Uri("http://bogus/myaccount/blob");
            _mockClient = new Mock<CloudBlobClient>(_mockBlobUri);
            _mockContainer = new Mock<CloudBlobContainer>(_mockBlobUri);
        }
        
        [TestFixture]
        public class Constructor : BlobConnectionTests
        {
            [Test]
            public void Null_Blob_Connection_Manager_Throws_Argument_Null_Exception()
            {
                //Arrange

                //Act
                BlobConnection TestDelegate() => new BlobConnection(null);

                //Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentNullException.With.Property("ParamName").EqualTo("storageConnectionManager"));
            }

            [Test]
            public void Can_Be_Constructed()
            {
                //Arrange

                //Act
                var connection = new BlobConnection(Mock.Of<IStorageConnectionManager<CloudBlobClient>>());

                //Assert
                Assert.That(connection, Is.Not.Null);
            }
        }

        [TestFixture]
        public class GetContainerAsyncMethod : BlobConnectionTests
        {
            [Test]
            public void Transaction_Id_Cannot_Be_Empty()
            {
                //Arrange
                var connection = new BlobConnection(Mock.Of<IStorageConnectionManager<CloudBlobClient>>());

                //Act
                async Task<CloudBlobContainer> TestDelegate() => await connection.GetObjectAsync(Guid.Empty);

                //Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentException.With.Message.EqualTo("Id cannot be Empty"));
            }

            [Test]
            public async Task Can_Get_Container()
            {
                //Arrange
                _mockManager.Setup(x => x.GetStorageClient()).Returns(Task.FromResult(_mockClient.Object));
                _mockClient.Setup(x => x.GetContainerReference(It.IsAny<string>())).Returns(_mockContainer.Object);
                _mockContainer.Setup(x => x.CreateIfNotExistsAsync()).Returns(Task.FromResult(true));
                var connection = new BlobConnection(_mockManager.Object);

                //Act
                var result = await connection.GetObjectAsync(Guid.NewGuid());

                //Assert
                Assert.That(result, Is.EqualTo(_mockContainer.Object));
            }

            [Test]
            public void Exception_Thrown_In_Connection_Manager_Will_Bubble_Up()
            {
                //Arrange
                const string exceptionMessage = "Test Exception";
                _mockManager.Setup(x => x.GetStorageClient())
                    .Throws(new Exception(exceptionMessage));
                var connection = new BlobConnection(_mockManager.Object);

                //Act
                async Task<CloudBlobContainer> TestDelegate() => await connection.GetObjectAsync(Guid.NewGuid());

                //Assert
                Assert.That(TestDelegate, Throws.Exception.With.Message.EqualTo(exceptionMessage));
            }

            [Test]
            public void Exception_Thrown_In_Cloud_Blob_Client_Will_Bubble_Up()
            {
                //Arrange
                const string exceptionMessage = "Test Exception";
                _mockManager.Setup(x => x.GetStorageClient()).Returns(Task.FromResult(_mockClient.Object));
                _mockClient.Setup(x => x.GetContainerReference(It.IsAny<string>()))
                    .Throws(new Exception(exceptionMessage));
                var connection = new BlobConnection(_mockManager.Object);

                //Act
                async Task<CloudBlobContainer> TestDelegate() => await connection.GetObjectAsync(Guid.NewGuid());

                //Assert
                Assert.That(TestDelegate, Throws.Exception.With.Message.EqualTo(exceptionMessage));
            }

            [Test]
            public void Exception_Thrown_In_Cloud_Blob_Container_Will_Bubble_Up()
            {
                //Arrange
                const string exceptionMessage = "Test Exception";
                _mockManager.Setup(x => x.GetStorageClient()).Returns(Task.FromResult(_mockClient.Object));
                _mockClient.Setup(x => x.GetContainerReference(It.IsAny<string>())).Returns(_mockContainer.Object);
                _mockContainer.Setup(x => x.CreateIfNotExistsAsync()).Throws(new Exception(exceptionMessage));
                var connection = new BlobConnection(_mockManager.Object);

                //Act
                async Task<CloudBlobContainer> TestDelegate() => await connection.GetObjectAsync(Guid.NewGuid());

                //Assert
                Assert.That(TestDelegate, Throws.Exception.With.Message.EqualTo(exceptionMessage));
            }
        }

        [TestFixture]
        public class RemoveContainerAsyncMethod : BlobConnectionTests
        {
            [Test]
            public void Transaction_Id_Cannot_Be_Emtpy()
            {
                //Arrange
                var connection = new BlobConnection(Mock.Of<IStorageConnectionManager<CloudBlobClient>>());

                //Act
                async Task TestDelegate() => await connection.RemoveObjectAsync(Guid.Empty);

                //Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentException.With.Message.EqualTo("Id cannot be Empty"));
            }

            [Test]
            public void Can_Remove_Container()
            {
                //Arrange
                _mockManager.Setup(x => x.GetStorageClient()).Returns(Task.FromResult(_mockClient.Object));
                _mockClient.Setup(x => x.GetContainerReference(It.IsAny<string>())).Returns(_mockContainer.Object);
                _mockContainer.Setup(x => x.DeleteIfExistsAsync()).Returns(Task.FromResult(true));
                var connection = new BlobConnection(_mockManager.Object);

                //Act
                async Task TestDelegate() => await connection.RemoveObjectAsync(Guid.NewGuid());

                //Assert
                Assert.That(TestDelegate, Throws.Nothing);
            }

            [Test]
            public void Exception_Thrown_In_Connection_Manager_Will_Bubble_Up()
            {
                //Arrange
                const string exceptionMessage = "Test Exception";
                _mockManager.Setup(x => x.GetStorageClient())
                    .Throws(new Exception(exceptionMessage));
                var connection = new BlobConnection(_mockManager.Object);

                //Act
                async Task TestDelegate() => await connection.RemoveObjectAsync(Guid.NewGuid());

                //Assert
                Assert.That(TestDelegate, Throws.Exception.With.Message.EqualTo(exceptionMessage));
            }

            [Test]
            public void Exception_Thrown_In_Cloud_Blob_Client_Will_Bubble_Up()
            {
                //Arrange
                const string exceptionMessage = "Test Exception";
                _mockManager.Setup(x => x.GetStorageClient()).Returns(Task.FromResult(_mockClient.Object));
                _mockClient.Setup(x => x.GetContainerReference(It.IsAny<string>()))
                    .Throws(new Exception(exceptionMessage));
                var connection = new BlobConnection(_mockManager.Object);

                //Act
                async Task TestDelegate() => await connection.RemoveObjectAsync(Guid.NewGuid());

                //Assert
                Assert.That(TestDelegate, Throws.Exception.With.Message.EqualTo(exceptionMessage));
            }

            [Test]
            public void Exception_Thrown_In_Cloud_Blob_Container_Will_Bubble_Up()
            {
                //Arrange
                const string exceptionMessage = "Test Exception";
                _mockManager.Setup(x => x.GetStorageClient()).Returns(Task.FromResult(_mockClient.Object));
                _mockClient.Setup(x => x.GetContainerReference(It.IsAny<string>())).Returns(_mockContainer.Object);
                _mockContainer.Setup(x => x.DeleteIfExistsAsync()).Throws(new Exception(exceptionMessage));
                var connection = new BlobConnection(_mockManager.Object);

                //Act
                async Task TestDelegate() => await connection.RemoveObjectAsync(Guid.NewGuid());

                //Assert
                Assert.That(TestDelegate, Throws.Exception.With.Message.EqualTo(exceptionMessage));
            }
        }
    }
}
