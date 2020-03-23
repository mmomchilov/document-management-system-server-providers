using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Glasswall.Providers.Storage.AzureBlob.Store
{
    public class BlobSizeCalculator : IBlobSizeCalculator
    {
        private const int Multiplier = 1024;
        private const int Kb64 = 64 * Multiplier;
        private const int Kb128 = 128 * Multiplier;
        private const int Kb256 = 256 * Multiplier;
        private const int Kb512 = 512 * Multiplier;
        private const int Mb1 = Multiplier * Multiplier;
        private const int Mb2 = 2 * Multiplier * Multiplier;
        private const int Mb4 = 4 * Multiplier * Multiplier;

        public Task<int> GetBlockSize(long streamLength)
        {
            if (streamLength < Kb64)
                return Task.FromResult(Kb64);

            if (streamLength >= Kb64 && streamLength < Kb128)
                return Task.FromResult(Kb128);

            if (streamLength >= Kb128 && streamLength < Kb256)
                return Task.FromResult(Kb256);

            if (streamLength >= Kb256 && streamLength < Kb512)
                return Task.FromResult(Kb512);

            if (streamLength >= Kb512 && streamLength < Mb1)
                return Task.FromResult(Mb1);

            if (streamLength >= Mb1 && streamLength < Mb2)
                return Task.FromResult(Mb2);

            return Task.FromResult(Mb4);
        }
    }
}
