using System;
using System.Collections.Generic;
using System.Text;

namespace Pirina.Providers.Microsoft.Dependency.Tests.L0.MockData
{
    internal interface IHasID<T>
    {
        T Id { get; }
    }
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
}
