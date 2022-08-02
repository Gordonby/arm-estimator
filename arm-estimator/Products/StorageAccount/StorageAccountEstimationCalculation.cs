﻿using Azure.Core;

internal class StorageAccountEstimationCalculation : BaseEstimation, IEstimationCalculation
{
    public StorageAccountEstimationCalculation(RetailItem[] items, ResourceIdentifier id, WhatIfAfterBeforeChange change)
        : base(items, id, change)
    {
    }

    public IOrderedEnumerable<RetailItem> GetItems()
    {
        var consumptionMetrics = this.items.Where(_ => _.type != "Reservation");

        return this.items.Where(_ => _.type != "Reservation").OrderByDescending(_ => _.retailPrice);
    }

    public double GetTotalCost(WhatIfChange[] changes)
    {
        double? estimatedCost;
        var items = GetItems();
        

        estimatedCost = items.Select(_ => _.retailPrice).Sum();

        return estimatedCost == null ? 0 : (double)estimatedCost;
    }
}
