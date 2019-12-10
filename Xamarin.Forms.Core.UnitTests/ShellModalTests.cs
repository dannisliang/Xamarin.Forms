﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class ShellModalTests : ShellTestBase
	{
		[Test]
		public async Task BasicModalBehaviorTest()
		{
			Shell shell = new Shell();
			shell.Items.Add(CreateShellItem());

			await shell.GoToAsync("ModalTestPage");

			var navStack = shell.Items[0].Items[0].Navigation;

			Assert.AreEqual(1, navStack.ModalStack.Count);
			Assert.AreEqual(typeof(ModalTestPage), navStack.ModalStack[0].Navigation.NavigationStack[0].GetType());
		}


		[Test]
		public async Task ModalPopsWhenSwitchingShellItem()
		{
			Shell shell = new Shell();
			shell.Items.Add(CreateShellItem());
			shell.Items.Add(CreateShellItem(shellItemRoute: "NewRoute"));

			// pushes modal onto visible shell section
			await shell.GoToAsync("ModalTestPage");

			// Navigates to different Shell Item
			await shell.GoToAsync("///NewRoute");

			var navStack = shell.Items[0].Items[0].Navigation;
			Assert.AreEqual(0, navStack.ModalStack.Count);
		}

		[Test]
		public async Task ModalPopsWhenSwitchingShellSection()
		{
			Shell shell = new Shell();
			shell.Items.Add(CreateShellItem());
			shell.Items[0].Items.Add(CreateShellSection(shellSectionRoute: "NewRoute"));

			// pushes modal onto visible shell section
			await shell.GoToAsync("ModalTestPage");

			// Navigates to different Shell Item
			await shell.GoToAsync("///NewRoute");

			var navStack = shell.Items[0].Items[0].Navigation;
			Assert.AreEqual(0, navStack.ModalStack.Count);
		}

		[Test]
		public async Task ModalPopsWhenSwitchingShellContent()
		{
			Shell shell = new Shell();
			shell.Items.Add(CreateShellItem());
			shell.Items[0].Items[0].Items.Add(CreateShellContent(shellContentRoute: "NewRoute"));

			// pushes modal onto visible shell section
			await shell.GoToAsync("ModalTestPage");

			// Navigates to different Shell Item
			await shell.GoToAsync("///NewRoute");

			var navStack = shell.Items[0].Items[0].Navigation;
			Assert.AreEqual(0, navStack.ModalStack.Count);
		}

		[Test]
		public async Task ModalPopsWhenNavigatingWithoutModalRoute()
		{
			Shell shell = new Shell();
			shell.Items.Add(CreateShellItem(shellItemRoute: "NewRoute"));

			// pushes modal onto visible shell section
			await shell.GoToAsync("ModalTestPage");

			// Navigates to different Shell Item
			await shell.GoToAsync("///NewRoute");

			var navStack = shell.Items[0].Items[0].Navigation;
			Assert.AreEqual(0, navStack.ModalStack.Count);
		}


		[Test]
		public async Task ModalPopsWhenNavigatingToNewModalRoute()
		{
			Shell shell = new Shell();
			shell.Items.Add(CreateShellItem(shellItemRoute: "NewRoute"));

			// pushes modal onto visible shell section
			await shell.GoToAsync("ModalTestPage");

			// Navigates to different Shell Item
			await shell.GoToAsync("///NewRoute/ModalTestPage2");

			var navStack = shell.Items[0].Items[0].Navigation;
			Assert.AreEqual(1, navStack.ModalStack.Count);
			Assert.AreEqual(typeof(ModalTestPage2), navStack.ModalStack[0].Navigation.NavigationStack[0].GetType());
		}

		[Test]
		public async Task PagesPushToModalStack()
		{
			Routing.RegisterRoute("ContentPage", typeof(ContentPage));
			Shell shell = new Shell();
			shell.Items.Add(CreateShellItem(shellItemRoute: "NewRoute", shellSectionRoute:"Section", shellContentRoute:"Content"));

			await shell.GoToAsync("ModalTestPage/ContentPage");

			var navStack = shell.Items[0].Items[0].Navigation;
			Assert.AreEqual(typeof(ModalTestPage), navStack.ModalStack[0].Navigation.NavigationStack[0].GetType());
			Assert.AreEqual(typeof(ContentPage), navStack.ModalStack[0].Navigation.NavigationStack[1].GetType());

			Assert.AreEqual("//NewRoute/Section/Content/ModalTestPage/ContentPage", shell.CurrentState.Location.ToString());
		}

		[Test]
		public async Task MultipleModalStacks()
		{
			Routing.RegisterRoute("ContentPage", typeof(ContentPage));
			Shell shell = new Shell();
			shell.Items.Add(CreateShellItem(shellItemRoute: "NewRoute", shellSectionRoute: "Section", shellContentRoute: "Content"));

			await shell.GoToAsync("ModalTestPage/ModalTestPage2/ContentPage");

			var navStack = shell.Items[0].Items[0].Navigation;
			Assert.AreEqual(typeof(ModalTestPage), navStack.ModalStack[0].Navigation.NavigationStack[0].GetType());
			Assert.AreEqual(typeof(ModalTestPage2), navStack.ModalStack[1].Navigation.NavigationStack[0].GetType());
			Assert.AreEqual(typeof(ContentPage), navStack.ModalStack[1].Navigation.NavigationStack[1].GetType());

			Assert.AreEqual("//NewRoute/Section/Content/ModalTestPage/ModalTestPage2/ContentPage", shell.CurrentState.Location.ToString());
		}

		[Test]
		public async Task MultipleModalStacksWithContentPageAlreadyPushed()
		{
			Routing.RegisterRoute("ContentPage", typeof(ContentPage));
			Shell shell = new Shell();
			shell.Items.Add(CreateShellItem(shellItemRoute: "NewRoute", shellSectionRoute: "Section", shellContentRoute: "Content"));

			await shell.GoToAsync("ContentPage/ModalTestPage/ContentPage/ModalTestPage2/ContentPage");
			Assert.AreEqual("//NewRoute/Section/Content/ContentPage/ModalTestPage/ContentPage/ModalTestPage2/ContentPage", shell.CurrentState.Location.ToString());
		}


		[Test]
		public async Task SwitchingModalStackAbsoluteNavigation()
		{
			Routing.RegisterRoute("ContentPage", typeof(ContentPage));
			Shell shell = new Shell();
			shell.Items.Add(CreateShellItem(shellItemRoute: "NewRoute", shellSectionRoute: "Section", shellContentRoute: "Content"));

			await shell.GoToAsync("ModalTestPage/ContentPage/ModalTestPage2/ContentPage");
			await shell.GoToAsync("//NewRoute/ModalTestPage2/ContentPage");

			Assert.AreEqual("//NewRoute/Section/Content/ModalTestPage2/ContentPage", shell.CurrentState.Location.ToString());
		}

		[Test]
		public async Task PushingNonNavigationPage()
		{
			Shell shell = new Shell();
			Routing.RegisterRoute("ContentPage", typeof(ContentPage));
			shell.Items.Add(CreateShellItem(shellItemRoute: "NewRoute", shellSectionRoute: "Section", shellContentRoute: "Content"));

			await shell.GoToAsync("//NewRoute/SomeCustomPage/ModalTestPage/ContentPage");

			Assert.AreEqual("//NewRoute/Section/Content/SomeCustomPage/ModalTestPage/ContentPage", shell.CurrentState.Location.ToString());
		}


		[Test]
		public async Task PushingMultipleVersionsOfTheModalRoute()
		{
			Shell shell = new Shell();
			Routing.RegisterRoute("ContentPage", typeof(ContentPage));
			shell.Items.Add(CreateShellItem(shellItemRoute: "NewRoute", shellSectionRoute: "Section", shellContentRoute: "Content"));

			await shell.GoToAsync("ModalTestPage");
			await shell.GoToAsync("ModalTestPage");

			Assert.AreEqual("//NewRoute/Section/Content/ModalTestPage/ModalTestPage", shell.CurrentState.Location.ToString());
		}

		[Test]
		public async Task PushingContentPageToNonNavigationPageThrowsException()
		{
			Shell shell = new Shell();
			Routing.RegisterRoute("ContentPage", typeof(ContentPage));
			shell.Items.Add(CreateShellItem(shellItemRoute: "NewRoute", shellSectionRoute: "Section", shellContentRoute: "Content"));

			bool invalidOperationThrown = true;
			Device.PlatformServices = new MockPlatformServices(invokeOnMainThread: (action) =>
			{
				try
				{
					action();
				}
				catch(InvalidOperationException) 
				{
					invalidOperationThrown = true;
				}
			});

			Assert.IsTrue(invalidOperationThrown);
		}

		public class ModalTestPage : ContentPage
		{
			public ModalTestPage()
			{
				Shell.SetModalBehavior(this, new ModalBehavior() { Modal = true });
			}
		}

		public class ModalTestPage2 : ContentPage
		{
			public ModalTestPage2()
			{
				Shell.SetModalBehavior(this, new ModalBehavior() { Modal = true });
			}
		}

		public class SomeCustomPage : Page
		{
			public SomeCustomPage()
			{
				Shell.SetModalBehavior(this, new ModalBehavior() { Modal = true });
			}
		}

		public override void Setup()
		{
			base.Setup();
			Routing.RegisterRoute("ModalTestPage", typeof(ModalTestPage));
			Routing.RegisterRoute("ModalTestPage2", typeof(ModalTestPage2));
			Routing.RegisterRoute("SomeCustomPage", typeof(SomeCustomPage));
		}
	}
}
