﻿using Castle.Core;
using Castle.Core.Internal;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Security;
using System;
using System.Linq;

namespace SSD.DependencyInjection
{
    [TestClass]
    public class SecurityConfigurationInstallerTest
    {
        private SecurityConfigurationInstaller Target { get; set; }
        private IWindsorContainer Container { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new SecurityConfigurationInstaller();
            Container = new WindsorContainer().Install(Target);
        }

        [TestMethod]
        public void GivenNoContainer_WhenIInstall_ThenThrowArgumentNullException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Install(null, new DefaultConfigurationStore()));
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenRegistrationsExist_AndAllHandlersRegistered()
        {
            var allHandlers = Container.GetAllHandlers();
            var actualHandlers = Container.GetHandlersFor(typeof(ISecurityConfiguration));
            Assert.AreNotEqual(0, allHandlers.Length);
            CollectionAssert.AreEqual(allHandlers, actualHandlers);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllHandlersHaveExpectedLifestyle()
        {
            var invalidHandlers = Container.GetHandlersFor(typeof(ISecurityConfiguration))
                .Where(handler => handler.ComponentModel.LifestyleType != LifestyleType.Singleton)
                .ToArray();
            Assert.AreEqual(0, invalidHandlers.Length);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenImplementationsRegistered()
        {
            var allTypes = typeof(ISecurityConfiguration).Assembly.GetPublicClasses(c => c.Is<ISecurityConfiguration>());
            var actualTypes = Container.GetImplementationTypesFor(typeof(ISecurityConfiguration));
            CollectionAssert.AreEqual(allTypes, actualTypes);
        }

        [TestMethod]
        public void GivenRegistrationsInstalled_ThenAllRegistrationsAreNamedAfterTheirType()
        {
            var invalidHandlers = Container.GetHandlersFor(typeof(ISecurityConfiguration))
                .Where(handler => handler.ComponentModel.Name != handler.ComponentModel.Implementation.FullName)
                .ToArray();
            Assert.AreEqual(0, invalidHandlers.Length);
        }
    }
}
