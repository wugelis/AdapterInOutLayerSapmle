using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Tests
{
    [TestClass()]
    public class AccountTests
    {
        [TestMethod()]
        public void Test_CheckIsAID()
        {
            // Arrange
            Account account = new Account();
            account.SetAccountId("F123456789");
            bool expected = true;
            bool actual;
            
            // Act
            actual = account.CheckIsAID();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}