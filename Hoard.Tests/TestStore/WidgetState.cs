﻿using F3N.Hoard.State;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hoard.Tests.TestStore
{
    public class WidgetState : IStatefulCollectionItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public int StockQuantity { get; set; }

        public WidgetState(Guid id, string title, int stockQuantity = 0)
        {
            Id = id;
            Title = title;
            StockQuantity = stockQuantity;
        }
    }
}
