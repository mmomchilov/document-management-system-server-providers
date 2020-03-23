namespace UnityTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Pirina.Kernel.DependencyResolver;
    using Pirina.Providers.DI.Unity;
    using NUnit.Framework;

    [TestFixture, Category("DIUnity")]
    public class UnityDependencyResolverConcurrencyTests
	{
		#region contains tests
		
		[Test(Description="This test simulates concurrent access to Unity container which causes invalid operation exception. Test should pass if no exception is thrown.")]
		public void ConcurrentWriteReadTest()
		{
			var tasks = new List<Task>();
			//ARRANGE
			var container = new UnityDependencyResolver();
			
			//ACT
			for (var i = 0; i < 100; i++ )
			{
				var task = Task.Factory.StartNew(() => container.RegisterType<Derived>(Lifetime.Transient));
				tasks.Add(task);
			}

			for (var i = 0; i < 100; i++)
			{
				var task = Task.Factory.StartNew(() => 
				{ 
					var isRegistered = container.Contains<Derived>();
					Assert.True(isRegistered);
				});
				tasks.Add(task);
			}
		}

		#endregion
	}
}