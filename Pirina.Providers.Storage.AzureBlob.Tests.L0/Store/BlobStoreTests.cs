using System;
using System.IO;
using System.Threading.Tasks;
using Pirina.Kernel.Serialisation;
using Pirina.Kernel.Storage;
using Pirina.Providers.Storage.AzureBlob.Store;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using NUnit.Framework;

namespace Pirina.Providers.Storage.AzureBlob.Tests.L0.Store
{
    [TestFixture]
    [Category("L0")]
    public class BlobStoreTests
    {
        private const string FileName = "Barry";
        private const string FolderName = "Von";
        private const int BlockSize = 64 * 1024;

        private Mock<IStorageConnection<CloudBlobContainer, Guid>> _mockConnection;
        private Mock<IJsonSerialiser> _mockSerialiser;
        private Mock<IBlobSizeCalculator> _mockCalculator;
        private Mock<CloudBlobContainer> _mockContainer;
        private Mock<CloudBlockBlob> _mockBlock;
        private MemoryStream _mockMemoryStream;
        private Uri _mockBlobUri;

        [SetUp]
        public void Init()
        {
            _mockBlobUri = new Uri("http://bogus/myaccount/blob");
            _mockConnection = new Mock<IStorageConnection<CloudBlobContainer, Guid>>();
            _mockSerialiser = new Mock<IJsonSerialiser>();
            _mockCalculator = new Mock<IBlobSizeCalculator>();
            _mockContainer = new Mock<CloudBlobContainer>(_mockBlobUri);
            _mockBlock = new Mock<CloudBlockBlob>(_mockBlobUri);
            _mockMemoryStream = new MemoryStream(new byte[] { 72, 101, 108, 108, 111, 32, 87, 111, 114, 108, 100, 33 });
        }

        [TestFixture]
        public class Constructor : BlobStoreTests
        {
            [Test]
            public void Null_Blob_Connection_Will_Throw_Argument_Null_Exception()
            {
                //Arrange

                //Act
                BlobStore TestDelegate() =>
                    new BlobStore(null, Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentNullException.With.Property("ParamName").EqualTo("storageConnection"));
            }

            [Test]
            public void Null_Json_Serialiser_Will_Throw_Argument_Null_Exception()
            {
                //Arrange

                //Act
                BlobStore TestDelegate() =>
                    new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(), null, Mock.Of<IBlobSizeCalculator>());

                //Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentNullException.With.Property("ParamName").EqualTo("jsonSerialiser"));
            }

            [Test]
            public void Null_Blob_Size_Calcualator_Will_Throw_Argument_Null_Exception()
            {
                //Arrange

                //Act
                BlobStore TestDelegate() => new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), null);

                //Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentNullException.With.Property("ParamName").EqualTo("blobSizeCalculator"));
            }

            [Test]
            public void Can_Be_Constructed()
            {
                //Arrange

                //Act
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Assert
                Assert.That(store, Is.Not.Null);
            }
        }

        [TestFixture]
        public class AddAsyncMethod : BlobStoreTests
        {
            [Test]
            public void Null_Stream_Will_Throw_Argument_Null_Exception_3_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.AddAsync<object>(null, Guid.NewGuid(), FileName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentNullException.With.Property("ParamName").EqualTo("data"));
            }

            [Test]
            public void Empty_Transaction_Id_Will_Throw_Argument_Exception_3_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.AddAsync(_mockMemoryStream, Guid.Empty, FileName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("id cannot be Empty"));
            }

            [Test]
            public void Empty_File_Name_Will_Throw_Argument_Exception_3_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.AddAsync(_mockMemoryStream, Guid.NewGuid(), "");

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("key cannot be Null or Empty"));
            }

            [Test]
            public void Null_File_Name_Will_Throw_Argument_Exception_3_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.AddAsync(_mockMemoryStream, Guid.NewGuid(), null);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("key cannot be Null or Empty"));
            }

            [Test]
            public void Null_Stream_Will_Throw_Argument_Null_Exception_4_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.AddAsync<object>(null, Guid.NewGuid(), FileName, FolderName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentNullException.With.Property("ParamName").EqualTo("data"));
            }

            [Test]
            public void Empty_Transaction_Id_Will_Throw_Argument_Exception_4_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.AddAsync(_mockMemoryStream, Guid.Empty, FileName, FolderName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("id cannot be Empty"));
            }

            [Test]
            public void Empty_File_Name_Will_Throw_Argument_Exception_4_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.AddAsync(_mockMemoryStream, Guid.NewGuid(), "", FolderName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("key cannot be Null or Empty"));
            }

            [Test]
            public void Null_File_Name_Will_Throw_Argument_Exception_4_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.AddAsync(_mockMemoryStream, Guid.NewGuid(), null, FolderName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("key cannot be Null or Empty"));
            }

            [Test]
            public void Empty_Folder_Name_Will_Throw_Argument_Exception()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.AddAsync(_mockMemoryStream, Guid.NewGuid(), FileName, "");

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("objectName cannot be Null or Empty"));
            }

            [Test]
            public void Null_Folder_Name_Will_Throw_Argument_Exception()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.AddAsync(_mockMemoryStream, Guid.NewGuid(), FileName, null);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("objectName cannot be Null or Empty"));
            }

            [Test]
            public async Task Can_Upload_Stream_To_Blob()
            {
                //Arrange
                _mockConnection.Setup(x => x.GetObjectAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(_mockContainer.Object));
                _mockContainer.Setup(x => x.GetBlockBlobReference(It.IsAny<string>())).Returns(_mockBlock.Object);
                _mockCalculator.Setup(x => x.GetBlockSize(It.IsAny<long>())).Returns(Task.FromResult(BlockSize));
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                await store.AddAsync(_mockMemoryStream, Guid.NewGuid(), FileName);

                //Assert
                _mockBlock.Verify(x => x.UploadFromStreamAsync(_mockMemoryStream), Times.Once);
            }

            [Test]
            public async Task Can_Upload_Object_To_Blob()
            {
                //Arrange
                const string uploadObject = "Hello World!";
                _mockConnection.Setup(x => x.GetObjectAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(_mockContainer.Object));
                _mockContainer.Setup(x => x.GetBlockBlobReference(It.IsAny<string>())).Returns(_mockBlock.Object);
                _mockCalculator.Setup(x => x.GetBlockSize(It.IsAny<long>())).Returns(Task.FromResult(BlockSize));
                _mockSerialiser.Setup(x => x.SerialiseToJson(It.IsAny<object>()))
                    .Returns(Task.FromResult(uploadObject));
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                await store.AddAsync(uploadObject, Guid.NewGuid(), FileName);

                //Assert
                _mockBlock.Verify(x => x.UploadFromStreamAsync(It.IsAny<Stream>()), Times.Once);
            }

            [Test]
            public async Task Can_Upload_Stream_To_Blob_With_Folder_Name()
            {
                //Arrange
                _mockConnection.Setup(x => x.GetObjectAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(_mockContainer.Object));
                _mockContainer.Setup(x => x.GetBlockBlobReference(It.IsAny<string>())).Returns(_mockBlock.Object);
                _mockCalculator.Setup(x => x.GetBlockSize(It.IsAny<long>())).Returns(Task.FromResult(BlockSize));
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                await store.AddAsync(_mockMemoryStream, Guid.NewGuid(), FileName, FolderName);

                //Assert
                _mockBlock.Verify(x => x.UploadFromStreamAsync(_mockMemoryStream), Times.Once);
            }

            [Test]
            public async Task When_Uploading_To_Blob_The_Size_Will_Be_Calculated()
            {
                //Arrange
                _mockConnection.Setup(x => x.GetObjectAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(_mockContainer.Object));
                _mockContainer.Setup(x => x.GetBlockBlobReference(It.IsAny<string>())).Returns(_mockBlock.Object);
                _mockCalculator.Setup(x => x.GetBlockSize(It.IsAny<long>())).Returns(Task.FromResult(BlockSize));
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                await store.AddAsync(_mockMemoryStream, Guid.NewGuid(), FileName);

                //Assert
                _mockCalculator.Verify(x => x.GetBlockSize(It.IsAny<long>()), Times.Once);
            }

            [Test]
            public void Exception_Thrown_During_Upload_Will_Bubble_Up()
            {
                //Arrange
                const string exceptionMessage = "Test Exception";
                _mockConnection.Setup(x => x.GetObjectAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(_mockContainer.Object));
                _mockContainer.Setup(x => x.GetBlockBlobReference(It.IsAny<string>())).Returns(_mockBlock.Object);
                _mockCalculator.Setup(x => x.GetBlockSize(It.IsAny<long>())).Returns(Task.FromResult(BlockSize));
                _mockBlock.Setup(x => x.UploadFromStreamAsync(It.IsAny<Stream>()))
                    .Throws(new Exception(exceptionMessage));
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                async Task TestDelegate() => await store.AddAsync(_mockMemoryStream, Guid.NewGuid(), FileName);

                //Assert
                Assert.That(TestDelegate, Throws.Exception.With.Message.EqualTo(exceptionMessage));
            }
        }

        [TestFixture]
        public class GetAsyncMethod : BlobStoreTests
        {
            [Test]
            public void Empty_Transaction_Id_Will_Throw_Argument_Exception_2_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.GetAsync<object>(Guid.Empty, FileName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("id cannot be Empty"));
            }

            [Test]
            public void Empty_File_Name_Will_Throw_Argument_Exception_2_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.GetAsync<object>(Guid.NewGuid(), "");

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("key cannot be Null or Empty"));
            }

            [Test]
            public void Null_File_Name_Will_Throw_Argument_Exception_2_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.GetAsync<object>(Guid.NewGuid(), null);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("key cannot be Null or Empty"));
            }

            [Test]
            public void Empty_Transaction_Id_Will_Throw_Argument_Exception_3_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.GetAsync<object>(Guid.Empty, FileName, FolderName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("id cannot be Empty"));
            }

            [Test]
            public void Empty_File_Name_Will_Throw_Argument_Exception_3_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.GetAsync<object>(Guid.NewGuid(), "", FolderName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("key cannot be Null or Empty"));
            }

            [Test]
            public void Null_File_Name_Will_Throw_Argument_Exception_3_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.GetAsync<object>(Guid.NewGuid(), null, FolderName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("key cannot be Null or Empty"));
            }

            [Test]
            public void Empty_Folder_Name_Will_Throw_Argument_Exception()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.GetAsync<object>(Guid.NewGuid(), FileName, "");

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("objectName cannot be Null or Empty"));
            }

            [Test]
            public void Null_Folder_Name_Will_Throw_Argument_Exception()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.GetAsync<object>(Guid.NewGuid(), FileName, null);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("objectName cannot be Null or Empty"));
            }

            [Test]
            public async Task Can_Download_Object_From_Blob()
            {
                //Arrange
                _mockConnection.Setup(x => x.GetObjectAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(_mockContainer.Object));
                _mockContainer.Setup(x => x.GetBlockBlobReference(It.IsAny<string>())).Returns(_mockBlock.Object);
                _mockSerialiser.Setup(x => x.Deserialise(It.IsAny<Stream>()))
                    .Returns(Task.FromResult("Hello World!" as object));
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                var result = await store.GetAsync<object>(Guid.NewGuid(), FileName);

                //Assert
                _mockBlock.Verify(x => x.DownloadToStreamAsync(It.IsAny<Stream>()), Times.Once);
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public async Task Can_Download_Object_From_Blob_With_Folder_Name()
            {
                //Arrange
                _mockConnection.Setup(x => x.GetObjectAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(_mockContainer.Object));
                _mockContainer.Setup(x => x.GetBlockBlobReference(It.IsAny<string>())).Returns(_mockBlock.Object);
                _mockSerialiser.Setup(x => x.Deserialise(It.IsAny<Stream>()))
                    .Returns(Task.FromResult("Hello World!" as object));
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                var result = await store.GetAsync<object>(Guid.NewGuid(), FileName, FolderName);

                //Assert
                _mockBlock.Verify(x => x.DownloadToStreamAsync(It.IsAny<Stream>()), Times.Once);
                Assert.That(result, Is.Not.Null);
            }

            [Test]
            public async Task When_Downloading_Stream_From_Blob_Blob_Size_Will_Not_Be_Calculated()
            {
                //Arrange
                _mockConnection.Setup(x => x.GetObjectAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(_mockContainer.Object));
                _mockContainer.Setup(x => x.GetBlockBlobReference(It.IsAny<string>())).Returns(_mockBlock.Object);
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                await store.GetAsync<object>(Guid.NewGuid(), FileName);

                //Assert
                _mockCalculator.Verify(x => x.GetBlockSize(It.IsAny<long>()), Times.Never);
            }

            [Test]
            public void Exception_Thrown_During_Download_Will_Bubble_Up()
            {
                //Arrange
                const string exceptionMessage = "Test Exception";
                _mockConnection.Setup(x => x.GetObjectAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(_mockContainer.Object));
                _mockContainer.Setup(x => x.GetBlockBlobReference(It.IsAny<string>())).Returns(_mockBlock.Object);
                _mockBlock.Setup(x => x.DownloadToStreamAsync(It.IsAny<Stream>()))
                    .Throws(new Exception(exceptionMessage));
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                async Task TestDelegate() => await store.GetAsync<object>(Guid.NewGuid(), FileName);

                //Assert
                Assert.That(TestDelegate, Throws.Exception.With.Message.EqualTo(exceptionMessage));
            }
        }

        [TestFixture]
        public class RemoveFileAsyncMethod : BlobStoreTests
        {
            [Test]
            public void Empty_Transaction_Id_Will_Throw_Argument_Exception_2_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.RemoveAsync(Guid.Empty, FileName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("id cannot be Empty"));
            }

            [Test]
            public void Empty_File_Name_Will_Throw_Argument_Exception_2_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.RemoveAsync(Guid.NewGuid(), "");

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("key cannot be Null or Empty"));
            }

            [Test]
            public void Null_File_Name_Will_Throw_Argument_Exception_2_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.RemoveAsync(Guid.NewGuid(), null);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("key cannot be Null or Empty"));
            }

            [Test]
            public void Empty_Transaction_Id_Will_Throw_Argument_Exception_3_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.RemoveAsync(Guid.Empty, FileName, FolderName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("id cannot be Empty"));
            }

            [Test]
            public void Empty_File_Name_Will_Throw_Argument_Exception_3_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.RemoveAsync(Guid.NewGuid(), "", FolderName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("key cannot be Null or Empty"));
            }

            [Test]
            public void Null_File_Name_Will_Throw_Argument_Exception_3_Arguments()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.RemoveAsync(Guid.NewGuid(), null, FolderName);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("key cannot be Null or Empty"));
            }

            [Test]
            public void Empty_Folder_Name_Will_Throw_Argument_Exception()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.RemoveAsync(Guid.NewGuid(), FileName, "");

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("objectName cannot be Null or Empty"));
            }

            [Test]
            public void Null_Folder_Name_Will_Throw_Argument_Exception()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.RemoveAsync(Guid.NewGuid(), FileName, null);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("objectName cannot be Null or Empty"));
            }

            [Test]
            public async Task Can_Remove_File_From_Block()
            {
                //Arrange
                _mockConnection.Setup(x => x.GetObjectAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(_mockContainer.Object));
                _mockContainer.Setup(x => x.GetBlockBlobReference(It.IsAny<string>())).Returns(_mockBlock.Object);
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                await store.RemoveAsync(Guid.NewGuid(), FileName);

                //Assert
                _mockBlock.Verify(x => x.DeleteIfExistsAsync(), Times.Once);
            }

            [Test] public async Task Can_Remove_File_From_Block_With_Folder_Name()
            {
                //Arrange
                _mockConnection.Setup(x => x.GetObjectAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(_mockContainer.Object));
                _mockContainer.Setup(x => x.GetBlockBlobReference(It.IsAny<string>())).Returns(_mockBlock.Object);
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                await store.RemoveAsync(Guid.NewGuid(), FileName, FolderName);

                //Assert
                _mockBlock.Verify(x => x.DeleteIfExistsAsync(), Times.Once);
            }

            [Test]
            public void Exception_Thrown_During_Delete_Will_Bubble_Up()
            {
                //Arrange
                const string exceptionMessage = "Test Exception";
                _mockConnection.Setup(x => x.GetObjectAsync(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(_mockContainer.Object));
                _mockContainer.Setup(x => x.GetBlockBlobReference(It.IsAny<string>())).Returns(_mockBlock.Object);
                _mockBlock.Setup(x => x.DeleteIfExistsAsync())
                    .Throws(new Exception(exceptionMessage));
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                async Task TestDelegate() => await store.RemoveAsync(Guid.NewGuid(), FileName);

                //Assert
                Assert.That(TestDelegate, Throws.Exception.With.Message.EqualTo(exceptionMessage));
            }
        }

        [TestFixture]
        public class RemoveAllAsyncMethod : BlobStoreTests
        {
            [Test]
            public void Empty_Transaction_Id_Will_Throw_Argument_Exception()
            {
                //Arrange
                var store = new BlobStore(Mock.Of<IStorageConnection<CloudBlobContainer, Guid>>(),
                    Mock.Of<IJsonSerialiser>(), Mock.Of<IBlobSizeCalculator>());

                //Act
                async Task TestDelegate() => await store.RemoveAllAsync(Guid.Empty);

                //Assert
                Assert.That(TestDelegate, Throws.ArgumentException.With.Message.EqualTo("transactionId cannot be Empty"));
            }

            [Test]
            public async Task Can_Remove_Container_From_Blob_Storage()
            {
                //Arrange
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                await store.RemoveAllAsync(Guid.NewGuid());

                //Assert
                _mockConnection.Verify(x => x.RemoveObjectAsync(It.IsAny<Guid>()), Times.Once);
            }

            [Test]
            public void Exception_Thrown_During_Delete_Will_Bubble_Up()
            {
                //Arrange
                const string exceptionMessage = "Test Exception";
                _mockConnection.Setup(x => x.RemoveObjectAsync(It.IsAny<Guid>()))
                    .Throws(new Exception(exceptionMessage));
                var store = new BlobStore(_mockConnection.Object, _mockSerialiser.Object, _mockCalculator.Object);

                //Act
                async Task TestDelegate() => await store.RemoveAllAsync(Guid.NewGuid());

                //Assert
                Assert.That(TestDelegate, Throws.Exception.With.Message.EqualTo(exceptionMessage));
            }
        }
    }
}
