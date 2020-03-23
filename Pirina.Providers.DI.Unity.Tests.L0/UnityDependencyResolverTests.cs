using System;
using System.Linq;
using Pirina.Kernel.Data;
using Pirina.Kernel.DependencyResolver;
using Pirina.Providers.DI.Unity;
using NUnit.Framework;

namespace UnityTests
{
    internal interface ITestInterface { }
	internal interface IGenericInterface<T> { }
	internal class BaseClass : IHasID<Guid>
	{

		public Guid Id
		{
			get { throw new NotImplementedException(); }
		}
	}

	internal class Derived : BaseClass, ITestInterface
	{

	}

	internal class TestClassWithProperty
	{
		public Derived TestProperty { get; set; }
		public ITestInterface TestAbstractProperty { get; set; }
	}

	internal class Derived1 : BaseClass, ITestInterface
	{

	}

	internal class DependentClass
	{
		public ITestInterface Dependency { get; private set; }
		public DependentClass(ITestInterface dependency)
		{
			this.Dependency = dependency;
		}
	}

	internal class DependentClass1
	{
		public BaseClass Dependency { get; private set; }
		public DependentClass1(BaseClass dependency)
		{
			this.Dependency = dependency;
		}
	}

	internal class SingletonClass { }

	internal class DependentClass2
	{
		public BaseClass Dependency { get; private set; }
		public Derived Dependency1 { get; private set; }
		
		public DependentClass2() 
		{ 
		}

		public DependentClass2(BaseClass dependency)
		{
			this.Dependency = dependency;
		}

		public DependentClass2(BaseClass dependency, Derived dependency1)
		{
			this.Dependency = dependency;
			this.Dependency1 = dependency1;
		}
	}

	internal class BaseGeneric<T> : IGenericInterface<T> { }

	internal class DeriveGeneric : BaseGeneric<Derived> { }

	[TestFixture, Category("DIUnity")]
	public class UnityDependencyResolverTests
	{
		#region contains tests
		
		[Test]
		public void IsRegister_concrete_class_not_registered()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			
			//ACT
			var isRegistered = container.Contains<Derived>();
			
			//ASSERT
			Assert.IsFalse(isRegistered);
		}

		[Test]
		public void IsRegister_concrete_class_registered()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();

			//ACT
			container.RegisterType<Derived>(Lifetime.Transient);
			var isRegistered = container.Contains<Derived>();

			//ASSERT
			Assert.IsTrue(isRegistered);
		}

		#endregion

		#region resolve single instance tests

		[Test]
		public void Register_non_generic_concrete_resolve_concrete()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterType<Derived>(Lifetime.Transient);
			var resolved = container.Resolve<Derived>();
			//ASSERT
			Assert.IsInstanceOf<Derived>(resolved);
		}

		[Test]
		public void Try_Resolve_concrete__not_register()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			
			//ACT
			Derived result;
			var isResolved = container.TryResolve<Derived>(out result);

			//ASSERT
			Assert.IsInstanceOf<Derived>(result);
			Assert.IsTrue(isResolved);
		}

		[Test]
		public void Try_Resolve_concrete__with_dependency_not_register()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();

			//ACT
			DependentClass result;
			var isResolved = container.TryResolve<DependentClass>(out result);

			//ASSERT
			Assert.Null(result);
			Assert.IsFalse(isResolved);
		}

		[Test]
		public void ResolvePropertyDependency()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterType<Derived>(Lifetime.Transient);
			container.RegisterType<TestClassWithProperty>(Lifetime.Transient);
			var resolved = container.Resolve<TestClassWithProperty>();
			//ASSERT
			Assert.IsInstanceOf<TestClassWithProperty>(resolved);
			Assert.IsInstanceOf<Derived>(resolved.TestProperty);
			Assert.IsInstanceOf<Derived>(resolved.TestAbstractProperty);
		}

		[Test]
		public void Register_non_generic_concrete_resolve_interface()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterType<Derived>(Lifetime.Transient);
			var resolved = container.Resolve<ITestInterface>();
			//ASSERT
			Assert.IsInstanceOf<Derived>(resolved);
		}

		[Test]
		public void Register_no_concrete_resolve_interface_should_throw_an_execption()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			
			//ASSERT
			Assert.Throws<InvalidOperationException>(() => container.Resolve<ITestInterface>());
		}

		[Test]
		public void Register_non_generic_2_concrete_resolve_interface_should_throw_an_execption()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterType<Derived>(Lifetime.Transient);
			container.RegisterType<Derived1>(Lifetime.Transient);
			
			//ASSERT
			Assert.Throws<InvalidOperationException>(() => container.Resolve<ITestInterface>());
		}

		[Test]
		public void Register_non_generic_resolve_with_dependency()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterType<Derived>(Lifetime.Transient);
			container.RegisterType<Derived1>(Lifetime.Transient);
			//ACT
			var resolved = container.Resolve<DependentClass>();
			//ASSERT
			Assert.IsInstanceOf<DependentClass>(resolved);
			Assert.IsInstanceOf<Derived>(resolved.Dependency);
		}

		[Test]
		public void Register_non_generic_resolve_with_dependency_base_class()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterType<Derived>(Lifetime.Transient);
			container.RegisterType<Derived1>(Lifetime.Transient);
			//ACT
			var resolved = container.Resolve<DependentClass1>();
			//ASSERT
			Assert.IsInstanceOf<DependentClass1>(resolved);
			Assert.IsInstanceOf<Derived>(resolved.Dependency);
		}

		[Test]
		public void Register_non_generic_resolve_with_dependency_base_class_and_longest_constructor()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterType<Derived>(Lifetime.Transient);
			container.RegisterType<Derived1>(Lifetime.Transient);
			//ACT
			var resolved = container.Resolve<DependentClass2>();
			//ASSERT
			Assert.IsInstanceOf<DependentClass2>(resolved);
			Assert.IsInstanceOf<BaseClass>(resolved.Dependency);
			Assert.IsInstanceOf<Derived>(resolved.Dependency1);
		}

		[Test]
		public void Register_non_generic_resolve_all_for_interface()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterType<Derived>(Lifetime.Transient);
			container.RegisterType<Derived1>(Lifetime.Transient);
			var resolved = container.ResolveAll<ITestInterface>();
			//ASSERT
			Assert.AreEqual(2, resolved.Count());
		}

		[Test]
		public void Register_non_generic_resolve_all_for_baseClass()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterType<Derived>(Lifetime.Transient);
			container.RegisterType<Derived1>(Lifetime.Transient);
			var resolved = container.ResolveAll<BaseClass>();
			//ASSERT
			Assert.AreEqual(2, resolved.Count());
		}

		[Test]
		public void Register_generic_without_factory()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterType(typeof(DeriveGeneric), Lifetime.Transient);

			var resolved = container.Resolve<IGenericInterface<Derived>>();
			//ASSERT
			Assert.IsInstanceOf<DeriveGeneric>(resolved);
		}

		[Test]
		public void Register_generic_definition()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterType(typeof(BaseGeneric<>), Lifetime.Transient);
			
			var resolved = container.Resolve<IGenericInterface<Derived>>();
			//ASSERT
			Assert.IsInstanceOf<BaseGeneric<Derived>>(resolved);
		}

		[Test]
		public void Resolve_generic_interface_implementation()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterType(typeof(BaseGeneric<>), Lifetime.Transient);

			var resolved = container.Resolve<BaseGeneric<Derived>>();
			//ASSERT
			Assert.IsInstanceOf<BaseGeneric<Derived>>(resolved);
		}

		[Test]
		public void Register_generic_with_factory_not_abstract()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterFactory(typeof(BaseGeneric<>), t =>
			{
				var genericArgument = t.GetGenericArguments()[0];
				var typeToCreate = typeof(BaseGeneric<>).MakeGenericType(genericArgument);
				return Activator.CreateInstance(typeToCreate);
			}, Lifetime.Transient);
			
			var resolved = container.Resolve<BaseGeneric<Derived>>();
			//ASSERT
			Assert.IsInstanceOf<BaseGeneric<Derived>>(resolved);
		}

		[Test]
		public void Register_generic_with_factory_abstract()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterFactory(typeof(IGenericInterface<>), t =>
			{
				var genericArgument = t.GetGenericArguments()[0];
				var typeToCreate = typeof(BaseGeneric<>).MakeGenericType(genericArgument);
				return Activator.CreateInstance(typeToCreate);
			}, Lifetime.Transient);

			var resolved = container.Resolve<IGenericInterface<Derived>>();
			//ASSERT
			Assert.IsInstanceOf<BaseGeneric<Derived>>(resolved);
		}

		[Test]
		public void Register_generic_with_factory1()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			container.RegisterFactory<Derived>(t =>
			{
				return new Derived();
			}, Lifetime.Transient);

			var resolved = container.Resolve<Derived>();
			//ASSERT
			Assert.IsInstanceOf<Derived>(resolved);
		}

		[Test]
		public void Child_container_resolve_singlenton_test()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			var singlenton = new SingletonClass();

			container.RegisterInstance<SingletonClass>(singlenton, Lifetime.Singleton);

			var childContainer = container.CreateChildContainer();

			var resolvedFomParent = container.Resolve<SingletonClass>();
			var resolvedFomChild = childContainer.Resolve<SingletonClass>();
			//ASSERT
			Assert.AreSame(singlenton, resolvedFomParent);
			Assert.AreSame(singlenton, resolvedFomChild);
		}

		[Test]
		public void Child_container_resolve_singlenton_test_dispose_Child()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			var singlenton = new SingletonClass();

			container.RegisterInstance<SingletonClass>(singlenton, Lifetime.Singleton);

			var childContainer = container.CreateChildContainer();

			var resolvedFomParent = container.Resolve<SingletonClass>();
			var resolvedFomChild = childContainer.Resolve<SingletonClass>();
			childContainer.Dispose();
			var resolvedFomParentAfterDisposed = container.Resolve<SingletonClass>();
			//ASSERT
			Assert.AreSame(singlenton, resolvedFomParent);
			Assert.AreSame(singlenton, resolvedFomChild);
			Assert.AreSame(singlenton, resolvedFomParentAfterDisposed);
		}

		[Test]
		public void Child_container_resolve_singlenton_test_dispose_Child_should_throw_disposed_exception()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			var singlenton = new SingletonClass();

			container.RegisterInstance<SingletonClass>(singlenton, Lifetime.Singleton);

			var childContainer = container.CreateChildContainer();

			var resolvedFomParent = container.Resolve<SingletonClass>();
			var resolvedFomChild = childContainer.Resolve<SingletonClass>();
			childContainer.Dispose();
			var resolvedFomParentAfterDisposed = container.Resolve<SingletonClass>();
			//ASSERT
			Assert.AreSame(singlenton, resolvedFomParent);
			Assert.AreSame(singlenton, resolvedFomChild);
			Assert.AreSame(singlenton, resolvedFomParentAfterDisposed);
			Assert.Throws<ObjectDisposedException>(() => childContainer.Resolve<SingletonClass>());
		}

		[Test]
		public void Child_container_resolve_type_test()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT
			
			container.RegisterType<Derived>(Lifetime.Transient);

			var childContainer = container.CreateChildContainer();

			childContainer.RegisterType<Derived1>(Lifetime.Transient);
			//ACT
			var resolvedFomParent = container.Resolve<Derived>();
			var resolvedFomChild = childContainer.Resolve<Derived1>();
			//ASSERT
			Assert.IsInstanceOf<Derived>(resolvedFomParent);
			Assert.IsInstanceOf<Derived1>(resolvedFomChild);
		}

		[Test]
		public void Child_container_resolve_all_type_test()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT

			container.RegisterType<Derived>(Lifetime.Transient);

			var childContainer = container.CreateChildContainer();

			childContainer.RegisterType<Derived1>(Lifetime.Transient);
			//ACT
			var resolvedFomParent = container.ResolveAll<ITestInterface>();
			var resolvedFomChild = childContainer.ResolveAll<ITestInterface>();
			//ASSERT
			Assert.AreEqual(1, resolvedFomParent.Count());
			Assert.AreEqual(2, resolvedFomChild.Count());
		}

		[Test]
		public void Child_container_resolve_all_type_after_child_dispose_test()
		{
			//ARRANGE
			var container = new UnityDependencyResolver();
			//ACT

			container.RegisterType<Derived>(Lifetime.Transient);

			var childContainer = container.CreateChildContainer();

			childContainer.RegisterType<Derived1>(Lifetime.Transient);
			//ACT
			var resolvedFomParent = container.ResolveAll<ITestInterface>();
			var resolvedFomChild = childContainer.ResolveAll<ITestInterface>();

			childContainer.Dispose();
			var resolvedFomParentAfterDisposed = container.ResolveAll<ITestInterface>();
			//ASSERT
			Assert.AreEqual(1, resolvedFomParent.Count());
			Assert.AreEqual(2, resolvedFomChild.Count());
			Assert.AreEqual(1, resolvedFomParent.Count());
			Assert.Throws<ObjectDisposedException>(() => childContainer.Resolve<ITestInterface>());
		}

		#endregion
	}
}