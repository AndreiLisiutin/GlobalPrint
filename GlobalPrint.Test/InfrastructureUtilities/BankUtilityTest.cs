using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPrint.Infrastructure.BankUtility;

namespace GlobalPrint.Test.InfrastructureUtilities
{
    [TestClass]
    public class BankUtilityTest
    {
        private readonly string _sberbankBic = "044525225";

        /// <summary>
        /// Check, if bank utility can find Sberbank by it's bic.
        /// </summary>
        [TestMethod]
        public void GetBankInfoBySberbankBic()
        {
            BankUtility bankUtility = new BankUtility();

            BankInfo bankInfo = bankUtility.GetBankInfo(this._sberbankBic);

            Assert.IsNotNull(bankInfo);
            Assert.IsNotNull(bankInfo.OrgName);
            Assert.IsTrue(bankInfo.OrgName.ToUpper().Contains("СБЕРБАНК"));
        }
    }
}
