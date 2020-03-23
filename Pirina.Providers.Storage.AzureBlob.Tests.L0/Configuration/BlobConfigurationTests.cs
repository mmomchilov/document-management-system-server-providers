using Pirina.Providers.Storage.AzureBlob.Configuration;
using NUnit.Framework;

namespace Pirina.Providers.Storage.AzureBlob.Tests.L0.Configuration
{
    [TestFixture]
    [Category("L0")]
    public class BlobConfigurationTests
    {
        private const string ConnectionString = "Barry";
        private const string ObjectName = "Von";
        private const string Key = "Barriton";

        [TestFixture]
        public class Constructor : BlobConfigurationTests
        {
            [Test]
            public void Null_Connection_String_Will_Throw_An_Argument_Exception()
            {
                //Arrange

                //Act
                BlobConfiguration TestDelegate() => new BlobConfiguration(null);

                //Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentException.With.Message.EqualTo("connectionString cannot be Null or Empty"));
            }

            [Test]
            public void Empty_Connection_String_Will_Throw_An_Argument_Exception()
            {
                //Arrange

                //Act
                BlobConfiguration TestDelegate() => new BlobConfiguration("");

                //Assert
                Assert.That(TestDelegate,
                    Throws.ArgumentException.With.Message.EqualTo("connectionString cannot be Null or Empty"));
            }

            [Test]
            public void Can_Be_Constructed_With_1_Argument()
            {
                //Arrange

                //Act
                var config = new BlobConfiguration(ConnectionString);

                //Assert
                Assert.That(config, Is.Not.Null);
            }

            [Test]
            public void Can_Be_Constructed_With_2_Arguments()
            {
                //Arrange

                //Act
                var config = new BlobConfiguration(ConnectionString, ObjectName);

                //Assert
                Assert.That(config, Is.Not.Null);
            }

            [Test]
            public void Can_Be_Constructed_With_3_Arguments()
            {
                //Arrange

                //Act
                var config = new BlobConfiguration(ConnectionString, ObjectName, Key);

                //Assert
                Assert.That(config, Is.Not.Null);
            }
        }

        [TestFixture]
        public class ConnectionStringProperty : BlobConfigurationTests
        {
            [Test]
            public void Can_Get_Value()
            {
                //Arrange
                var config = new BlobConfiguration(ConnectionString);

                //Act
                var result = config.ConnectionString;

                //Assert
                Assert.AreEqual(ConnectionString, result);
            }
        }

        [TestFixture]
        public class ObjectNameProperty : BlobConfigurationTests
        {
            [Test]
            public void Default_Value_Is_Null()
            {
                //Arrange
                var config = new BlobConfiguration(ConnectionString);

                //Act
                var result = config.ObjectName;

                //Assert
                Assert.That(result, Is.Null);
            }

            [Test]
            public void Can_Get_Value()
            {
                //Arrange
                var config = new BlobConfiguration(ConnectionString, ObjectName);

                //Act
                var result = config.ObjectName;

                //Assert
                Assert.AreEqual(ObjectName, result);
            }
        }

        [TestFixture]
        public class KeyProperty : BlobConfigurationTests
        {
            [Test]
            public void Default_Value_Is_Null()
            {
                //Arrange
                var config = new BlobConfiguration(ConnectionString);

                //Act
                var result = config.Key;

                //Assert
                Assert.That(result, Is.Null);
            }

            [Test]
            public void Can_Get_Value()
            {
                //Arrange
                var config = new BlobConfiguration(ConnectionString, key: Key);

                //Act
                var result = config.Key;

                //Assert
                Assert.AreEqual(Key, result);
            }
        }
    }
}
