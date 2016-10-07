using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Users;
using GlobalPrint.Configuration.DI;
using System.Web.Configuration;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Users;
using GlobalPrint.Infrastructure.EmailUtility;
using GlobalPrint.Infrastructure.LogUtility;
using Moq;
using System.Net.Mail;

namespace GlobalPrint.Test.ServerBusinessLogicUnits
{
    [TestClass]
    public class UserUnitTest : BaseTest
    {
        private IUserUnit _userUnit;
        private readonly int _activityCheckerThreshold;
        private readonly int _activityCheckerCallInterval;

        public UserUnitTest()
        {
            _activityCheckerThreshold = Int32.Parse(WebConfigurationManager.AppSettings["ActivityCheckerThreshold"]);
            _activityCheckerCallInterval = Int32.Parse(WebConfigurationManager.AppSettings["ActivityCheckerCallInterval"]);            
            _userUnit = new UserUnit(new Lazy<IEmailUtility>(() => GetEmailMoq().Object));
        }

        [TestMethod]
        public void GetInactiveUsersTest()
        {
            // Update user activity on (threshold + callInterval / 2)
            TimeSpan testLastUserActivity = TimeSpan.FromMinutes(_activityCheckerThreshold + _activityCheckerCallInterval / 2);
            _userUnit.UpdateUserActivity(CurrentUserID, DateTime.Now.Subtract(testLastUserActivity));

            // Get inactive users
            var inactiveUserList = _userUnit.GetInactiveUsers(TimeSpan.FromMinutes(_activityCheckerThreshold), TimeSpan.FromMinutes(_activityCheckerCallInterval));

            // Inactive users list should be non empty
            Assert.IsTrue(inactiveUserList != null && inactiveUserList.Count >= 0);

            // Inactive users list should contain Bob Tester
            Assert.IsTrue(inactiveUserList.FindAll(u => u.PrinterOperator.ID == CurrentUserID).Count == 1);
        }
    }
}
