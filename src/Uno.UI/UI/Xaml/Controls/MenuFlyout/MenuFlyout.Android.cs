﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Android.Views;
using Android.Widget;
using Uno.Client;
using Uno.Extensions;
using Uno.UI;
using Uno.UI.Controls;

namespace Windows.UI.Xaml.Controls
{
	public partial class MenuFlyout
	{
		private PopupMenu _menu;

		internal protected override void Open()
		{
			_menu = new PopupMenu(ContextHelper.Current, GetActualTarget());

			_menu.MenuItemClick += OnMenuItemClick;
			_menu.DismissEvent += OnDismiss;

			Items
				.OfType<MenuFlyoutItem>()
				.Where(item => item.Visibility == Visibility.Visible)
				.ForEach((index, item) =>
				{
					item.ApplyCompiledBindings();

					// Using index as ID
					_menu.Menu.Add(0, index, index, item.Text);
				});

			_menu.Show();
		}

		internal protected override void Close()
		{
			_menu?.Dismiss();
			if (_menu != null)
			{
				_menu.MenuItemClick -= OnMenuItemClick;
				_menu.DismissEvent -= OnDismiss;
			}
		}

		private void OnDismiss(object sender, PopupMenu.DismissEventArgs e)
		{
			Hide(canCancel: false);
		}

		private void OnMenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs e)
		{
			var items = Items.OfType<MenuFlyoutItem>().Where(i => i.Visibility == Visibility.Visible).ToArray();
			var item = items[e.Item.ItemId];

			item.InvokeClick();
		}

		private View GetActualTarget()
		{
			if (Target is AppBarButton appBarButton
			&& ((Android.Views.View)Target).Parent == null // View.Parent (IViewParent) is the visual parent (hidden using `new` modifier).
			&& Target.Parent is CommandBar commandBar // FrameworkElement.Parent (DependencyObject) is the logical parent.
			&& commandBar.FindViewById(appBarButton.GetHashCode()) is View actionMenuItemView)
			{
				// This AppBarButton doesn't exist in the visual tree and is likely rendered natively as an ActionMenuItemView.
				return actionMenuItemView;
			}

			return Target;
		}
	}
}
