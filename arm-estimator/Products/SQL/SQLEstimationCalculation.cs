﻿using Azure.Core;

internal class SQLEstimationCalculation : BaseEstimation, IEstimationCalculation
{
    public SQLEstimationCalculation(RetailItem[] items, ResourceIdentifier id, WhatIfAfterBeforeChange change)
        : base(items, id, change)
    {
    }

    public IOrderedEnumerable<RetailItem> GetItems()
    {
        return this.items.OrderByDescending(_ => _.retailPrice);
    }

    public TotalCostSummary GetTotalCost(WhatIfChange[] changes, IDictionary<string, string>? usagePatterns)
    {
        double? estimatedCost = 0;
        var items = GetItems();
        var summary = new TotalCostSummary();

        foreach (var item in items)
        {
            double? cost = 0;

            if(item.meterName == "B DTU")
            {
                cost = item.retailPrice * 30;
            }
            else if (item.meterName == "S0 DTUs" || 
                item.meterName == "S1 DTUs" || 
                item.meterName == "S2 DTUs" || 
                item.meterName == "S3 DTUs" || 
                item.meterName == "S4 DTUs" ||
                item.meterName == "10 DTUs")
            {
                cost = item.retailPrice * (730 / 24d);
            }
            else
            {
                cost = item.retailPrice;
            }

            estimatedCost += cost;
            if (summary.DetailedCost.ContainsKey(item.meterName!))
            {
                summary.DetailedCost[item.meterName!] += cost;
            }
            else
            {
                summary.DetailedCost.Add(item.meterName!, cost);
            }
        }

        summary.TotalCost = estimatedCost.GetValueOrDefault();
        return summary;
    }
}
