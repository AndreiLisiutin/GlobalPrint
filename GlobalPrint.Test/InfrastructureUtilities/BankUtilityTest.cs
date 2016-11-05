using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPrint.Infrastructure.BankUtility;
using GlobalPrint.Infrastructure.BankUtility.BankInfo;
using GlobalPrint.Infrastructure.BankUtility.BicInfo;

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
            IBankUtility bankUtility = new BankUtility();

            IBankInfo bankInfo = bankUtility.GetBankInfo(this._sberbankBic);

            Assert.IsNotNull(bankInfo);
            Assert.IsNotNull(bankInfo.ShortName);
            Assert.IsTrue(bankInfo.ShortName.ToUpper().Contains("СБЕРБАНК"));
        }

        /// <summary>
        /// Check, if bic bank utility can find Sberbank by it's bic.
        /// www.bik-info.ru
        /// </summary>
        [TestMethod]
        public void GetBankBicInfoBySberbankBic()
        {
            IBankUtility bankUtility = new BicInfoUtility();

            IBankInfo bankInfo = bankUtility.GetBankInfo(this._sberbankBic);

            Assert.IsNotNull(bankInfo);
            Assert.IsNotNull(bankInfo.ShortName);
            Assert.IsTrue(bankInfo.ShortName.ToUpper().Contains("СБЕРБАНК"));
            Assert.IsTrue(bankInfo.CorrespondentAccount != null);
        }
    }
}
