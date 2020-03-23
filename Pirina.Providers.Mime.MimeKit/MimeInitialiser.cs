using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Pirina.Common.Initialisation;
using Pirina.Kernel.DependencyResolver;

[assembly:InternalsVisibleTo("Glasswall.Providers.Mime.MimeKit.Tests.L0")]

namespace Pirina.Providers.Mime.MimeKit
{
    [ExcludeFromCodeCoverage]
    public class MimeInitialiser : Initialiser
    {
        public override byte Order => 0;

        protected override Task InitialiseInternal(IDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterType<MimeMessageParser>(Lifetime.Transient);
            dependencyResolver.RegisterType<MimeMessage>(Lifetime.Transient);
            
            return Task.CompletedTask;
        }
    }
}
