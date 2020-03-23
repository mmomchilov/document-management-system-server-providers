using System.Threading.Tasks;
using Pirina.Providers.Storage.AzureBlob.Store;
using NUnit.Framework;

namespace Pirina.Providers.Storage.AzureBlob.Tests.L0.Store
{
    [TestFixture]
    [Category("L0")]
    public class BlobSizeCalculatorTests
    {
        private const int Kb64 = 64 * 1024;
        private const int Kb128 = 128 * 1024;
        private const int Kb256 = 256 * 1024;
        private const int Kb512 = 512 * 1024;
        private const int Mb1 = 1024 * 1024;
        private const int Mb2 = 2 * 1024 * 1024;
        private const int Mb4 = 4 * 1024 * 1024;

        [TestFixture]
        public class Constructor : BlobSizeCalculatorTests
        {
            [Test]
            public void Can_Be_Constructed()
            {
                //Arrange

                //Act
                var calculator = new BlobSizeCalculator();

                //Assert
                Assert.That(calculator, Is.Not.Null);
            }
        }

        [TestFixture]
        public class GetBlockSizeMethod : BlobSizeCalculatorTests
        {
            [Test]
            public async Task Stream_Length_Less_That_64_Kb_Will_Return_64_Kb_In_Bytes()
            {
                //Arrange
                const long length = 12 * 1024;
                var calculator = new BlobSizeCalculator();

                //Act
                var result = await calculator.GetBlockSize(length);

                //Assert
                Assert.AreEqual(Kb64, result);
            }

            [Test]
            public async Task Stream_Length_Between_64_Kb_And_128_Kb_Will_Return_128_Kb_In_Bytes()
            {
                //Arrange
                const long length = 85 * 1024;
                var calculator = new BlobSizeCalculator();

                //Act
                var result = await calculator.GetBlockSize(length);

                //Assert
                Assert.AreEqual(Kb128, result);
            }

            [Test]
            public async Task Stream_Length_Between_128_Kb_And_256_Kb_Will_Return_256_Kb_In_Bytes()
            {
                //Arrange
                const long length = 167 * 1024;
                var calculator = new BlobSizeCalculator();

                //Act
                var result = await calculator.GetBlockSize(length);

                //Assert
                Assert.AreEqual(Kb256, result);
            }

            [Test]
            public async Task Stream_Length_Between_256_Kb_And_512_Kb_Will_Return_512_Kb_In_Bytes()
            {
                //Arrange
                const long length = 333 * 1024;
                var calculator = new BlobSizeCalculator();

                //Act
                var result = await calculator.GetBlockSize(length);

                //Assert
                Assert.AreEqual(Kb512, result);
            }

            [Test]
            public async Task Stream_Length_Between_512_Kb_And_1_Mb_Will_Return_1_Mb_In_Bytes()
            {
                //Arrange
                const long length = 666 * 1024;
                var calculator = new BlobSizeCalculator();

                //Act
                var result = await calculator.GetBlockSize(length);

                //Assert
                Assert.AreEqual(Mb1, result);
            }

            [Test]
            public async Task Stream_Length_Between_1_Mb_And_2_Mb_Will_Return_2_Mb_In_Bytes()
            {
                //Arrange
                const long length = 1234 * 1024;
                var calculator = new BlobSizeCalculator();

                //Act
                var result = await calculator.GetBlockSize(length);

                //Assert
                Assert.AreEqual(Mb2, result);
            }

            [Test]
            public async Task Stream_Length_Greater_Than_2_Mb_Will_Return_4_Mb_In_Bytes()
            {
                //Arrange
                const long length = 2345 * 1024;
                var calculator = new BlobSizeCalculator();

                //Act
                var result = await calculator.GetBlockSize(length);

                //Assert
                Assert.AreEqual(Mb4, result);
            }
        }
    }
}