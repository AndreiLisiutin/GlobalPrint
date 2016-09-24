using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPrint.ClientWeb.Controllers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using Moq;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Offers;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using GlobalPrint.ClientWeb;

namespace GlobalPrint.Test.ClientWebControllers
{
    /// <summary>
    /// Tests for offer MVC controller.
    /// </summary>
    [TestClass]
    public class OfferControllerTest : BaseControllerTest
    {
        /// <summary>
        /// Simple test of offer view.
        /// </summary>
        [TestMethod]
        public void OfferViewSimpleTest()
        {
            var mock = new Mock<IUserOfferUnit>();

            #region Expected object

            var expectedOffer = new UserOfferExtended()
            {
                LatestUserOffer = new UserOffer()
                {
                    ID = 1,
                    OfferDate = DateTime.Now,
                    UserID = 1,
                    OfferID = 1,
                    OfferNumber = "123"
                },
                Offer = new Offer()
                {
                    Name = "Offer name",
                    Text = "Bla bla bla." + Environment.NewLine + "Bla bla bla.",
                    OfferTypeID = 1,
                    CreatedOn = DateTime.Now,
                    ID = 1,
                    Version = "1"
                },
                OfferType = new OfferType()
                {
                    Name = "Bla.",
                    ID = 1
                }
            };

            #endregion

            mock.Setup(a => a.GetLatestUserOfferByUserID(It.IsAny<int>(), It.IsAny<OfferTypeEnum>()))
                .Returns(expectedOffer);

            OfferController controller = GetController<OfferController>(new OfferController(mock.Object));

            ViewResult result = controller.Offer(OfferTypeEnum.UserOffer) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewBag.OfferTitle == "Offer name № 123 от " + DateTime.Now.ToString("dd.MM.yyyy"));
            CollectionAssert.AreEqual(result.ViewBag.OfferParagraphs, new List<string>() { "Bla bla bla.", "Bla bla bla." });
        }
    }
}
