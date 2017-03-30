﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnowyImageCopy.ViewModels
{
	public class ItemObservableCollection<T> : ObservableCollection<T>
		where T : class, INotifyPropertyChanged, IComparable<T>
	{
		private readonly object _locker = new object();

		/// <summary>
		/// Inserts new item to correct position in order to minimize the necessity of sorting.
		/// </summary>
		/// <param name="item">New item to collection</param>
		public void Insert(T item)
		{
			lock (_locker)
			{
				int index = 0;

				for (int i = this.Count - 1; i >= 0; i--)
				{
					if (this[i].CompareTo(item) < 0)
					{
						index = i + 1;
						break;
					}
				}

				base.Insert(index, item);
			}
		}

		#region PropertyChanged event of item

		/// <summary>
		/// Adds/Removes an event handler for PropertyChanged event of each item.
		/// </summary>
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
				foreach (T item in e.OldItems)
					item.PropertyChanged -= OnItemPropertyChanged;

			if (e.NewItems != null) // e.NewItems seems not to become null.
				foreach (T item in e.NewItems)
					item.PropertyChanged += OnItemPropertyChanged;

			base.OnCollectionChanged(e);
		}

		/// <summary>
		/// Removes all event handlers for PropertyChanged event of all items.
		/// </summary>
		protected override void ClearItems()
		{
			if (this.Items != null)
				foreach (T item in this.Items)
					item.PropertyChanged -= OnItemPropertyChanged;

			base.ClearItems();
		}

		private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			ItemPropertyChangedSender = sender as T;
			ItemPropertyChangedEventArgs = e;

			base.OnPropertyChanged(new PropertyChangedEventArgs(nameof(ItemPropertyChangedSender)));
		}

		/// <summary>
		/// Item which fired a PropertyChanged event
		/// </summary>
		public T ItemPropertyChangedSender { get; private set; }

		/// <summary>
		/// PropertyChangedEventArgs of the PropertyChanged event
		/// </summary>
		public PropertyChangedEventArgs ItemPropertyChangedEventArgs { get; private set; }

		#endregion
	}
}