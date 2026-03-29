using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp1.Models;
using ConsoleApp1.Services;
using ConsoleApp1.Debugging;
using System;
using System.Collections.Generic;

namespace ConsoleApp1.Tests;

[TestClass]
public class PurchaseServiceTests
{
    private PurchaseService CreateService() => new(new DebugLogger(enabled: false));

    [TestMethod]
    public void CalculatePurchase_ExampleFromTask_ReturnsCorrectResult()
    {
        var batches = new List<Batch>
        {
            new Batch(30, 8, 'A'),  
            new Batch(50, 12, 'B'), 
            new Batch(40, 14, 'A'), 
            new Batch(30, 60, 'B')   
        };
        int budget = 1000;
        var service = CreateService();

        var result = service.CalculatePurchase(batches, budget);

        Assert.AreEqual(6, result.ItemsBPurchased);
        Assert.AreEqual(20, result.RemainingMoney);
    }

    [TestMethod]
    public void CalculatePurchase_NoItemsA_PurchasesOnlyB()
    {
        var batches = new List<Batch>
    {
        new Batch(50, 10, 'B'),  
        new Batch(30, 20, 'B')   
    };
        int budget = 500;
        var service = CreateService();

        var result = service.CalculatePurchase(batches, budget);

        Assert.AreEqual(16, result.ItemsBPurchased);
        Assert.AreEqual(20, result.RemainingMoney);
    }

    [TestMethod]
    public void CalculatePurchase_BudgetZero_ThrowsException()
    {
        var batches = new List<Batch> { new Batch(10, 5, 'A') };  
        var service = CreateService();

        Assert.ThrowsException<ArgumentException>(() => service.CalculatePurchase(batches, 0));
    }

    [TestMethod]
    public void CalculatePurchase_EmptyBatches_ThrowsException()
    {
        var batches = new List<Batch>();
        var service = CreateService();

        Assert.ThrowsException<ArgumentException>(() => service.CalculatePurchase(batches, 1000));
    }

    [TestMethod]
    public void CalculatePurchase_BatchWithInvalidType_ThrowsException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Batch(10, 5, 'C'));
    }

    [TestMethod]
    public void CalculatePurchase_BatchWithZeroPrice_ThrowsException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Batch(0, 5, 'A'));
    }

    [TestMethod]
    public void CalculatePurchase_BatchWithZeroQuantity_ThrowsException()
    {
        Assert.ThrowsException<ArgumentException>(() => new Batch(10, 0, 'A'));
    }

    [TestMethod]
    public void CalculatePurchase_InsufficientBudgetForA_PurchasesOnlyAffordableB()
    {
        var batches = new List<Batch>
        {
            new Batch(100, 5, 'A'), 
            new Batch(20, 10, 'B') 
        };
        int budget = 150; 

        var service = CreateService();

        var result = service.CalculatePurchase(batches, budget);

        Assert.AreEqual(7, result.ItemsBPurchased);
        Assert.AreEqual(10, result.RemainingMoney);
    }
}