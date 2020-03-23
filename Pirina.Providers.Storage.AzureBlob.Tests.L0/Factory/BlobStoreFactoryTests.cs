using System;
using System.Threading.Tasks;
using Pirina.Kernel.DependencyResolver;
using Pirina.Kernel.Storage;
using Pirina.Providers.Storage.AzureBlob.Configuration;
using Pirina.Providers.Storage.AzureBlob.Factories;
using Moq;
using NUnit.Framework;

namespace Pirina.Providers.Storage.AzureBlob.Tests.L0.Factory
{
    [TestFixture]
    [Category("L0")]
    public class BlobStoreFactoryTests
    {
        private Mock<IDependencyResolver> _mockResolver;
        private Mock<IStorageConfiguration> _mockConfiguration;
        private Mock<IStorage<Guid>> _mockStore;

        [SetUp]
        public void Init()
        {
            _mockResolver = new Mock<IDependencyResolver>();
            _mockConfiguration = new Mock<IStorageConfiguration>();
            _mockStore = new Mock<IStorage<Guid>>();
        }

        [TestFixture]
        public class Constructor : BlobStoreFactoryTests
        {
            [Test]
            public void Null_Dependency_Resolver_Will_Throw_Argument_Null_Exception()
            {
                //Arrange

                //Act
                BlobStoreFactory TestDelegate() => new BlobStoreFactory(null);

                //Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentNullException.With.Property("ParamName").EqualTo("dependencyResolver"));
            }

            [Test]
            public void Can_Be_Constructed()
            {
                //Arrange

                //Act
                var factory = new BlobStoreFactory(Mock.Of<IDependencyResolver>());

                //Assert
                Assert.That(factory, Is.Not.Null);
            }
        }

        [TestFixture]
        public class GetBlobStoreMethod : BlobStoreFactoryTests
        {
            [Test]
            public void Null_Blob_Configuration_Will_Throw_Argument_Null_Exception()
            {
                //Arrange
                var factory = new BlobStoreFactory(Mock.Of<IDependencyResolver>());

                //Act
                async Task<IStorage<Guid>> TestDelegate() => await factory.GetStorage<string>(null);

                //Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentNullException.With.Property("ParamName").EqualTo("connection"));
            }

            [Test]
            public void Invalid_Type_Provided_Blob_Configuration_Will_Throw_Argument_Exception()
            {
                //Arrange
                var factory = new BlobStoreFactory(Mock.Of<IDependencyResolver>());

                //Act
                async Task<IStorage<Guid>> TestDelegate() => await factory.GetStorage<int>(0);

                //Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentException.With.Property("ParamName").EqualTo("connection"));
            }

            [Test]
            public async Task Can_Register_Factory_Method_To_Create_Configuration()
            {
                //Arrange
                IStorageConfiguration configuration = null;
                _mockResolver.Setup(x => x.Resolve<IStorage<Guid>>()).Returns(_mockStore.Object);
                _mockResolver
                    .Setup(x => x.RegisterFactory<IStorageConfiguration>(It.IsAny<Func<IStorageConfiguration>>(),
                        Lifetime.Transient)).Callback<Func<IStorageConfiguration>, Lifetime>((a, b) => configuration = a());
                var factory = new BlobStoreFactory(_mockResolver.Object);

                //Act
                var result = await factory.GetStorage("Barry");

                //Assert
                Assert.That(configuration, Is.Not.Null);
            }

            [Test]
            public async Task Can_Resolve_Blob_Store()
            {
                //Arrange
                _mockResolver.Setup(x => x.Resolve<IStorage<Guid>>()).Returns(_mockStore.Object);
                var factory = new BlobStoreFactory(_mockResolver.Object);

                //Act
                var result = await factory.GetStorage("Barry");

                //Assert
                Assert.AreEqual(_mockStore.Object, result);
            }

            [Test]
            public async Task Blob_Store_Is_Only_Resolved_Once()
            {
                //Arrange
                _mockResolver.Setup(x => x.Resolve<IStorage<Guid>>()).Returns(_mockStore.Object);
                var factory = new BlobStoreFactory(_mockResolver.Object);

                //Act
                await factory.GetStorage("SomeConnectionString");
                await factory.GetStorage("SomeConnectionString");

                //Assert
                _mockResolver.Verify(x => x.Resolve<IStorage<Guid>>(), Times.Once);
            }

            [Test]
            public void Exception_When_Resolving_Blob_Store_Will_Bubble_Up()
            {
                //Arrange
                const string exceptionMessage = "Test Exception";
                _mockResolver.Setup(x => x.Resolve<IStorage<Guid>>()).Throws(new Exception(exceptionMessage));
                var factory = new BlobStoreFactory(_mockResolver.Object);

                //Act
                async Task<IStorage<Guid>> TestDelegate() => await factory.GetStorage("Barry");

                //Assert
                Assert.That(TestDelegate, Throws.Exception.With.Message.EqualTo(exceptionMessage));
            }

            [Test]
            public void Exception_When_Registering_Configuration_Will_Bubble_Up()
            {
                //Arrange
                const string exceptionMessage = "Test Exception";
                _mockResolver.Setup(x => x.RegisterFactory<IStorageConfiguration>(It.IsAny<Func<IStorageConfiguration>>(), Lifetime.Transient))
                    .Throws(new Exception(exceptionMessage));
                var factory = new BlobStoreFactory(_mockResolver.Object);

                //Act
                async Task<IStorage<Guid>> TestDelegate() => await factory.GetStorage("Barry");

                //Assert
                Assert.That(TestDelegate, Throws.Exception.With.Message.EqualTo(exceptionMessage));
            }
        }
    }
}
