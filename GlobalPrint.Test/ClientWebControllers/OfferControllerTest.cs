using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GlobalPrint.ClientWeb.Controllers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using Moq;
using GlobalPrint.ServerBusinessLogic._IBusinessLogicLayer.Units.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Offers;
using System.Collections.Generic;
using GlobalPrint.ClientWeb.Models.OfferController;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;

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
            var userOfferUnitMock = new Mock<IUserOfferUnit>();
            var offerUnitMock = new Mock<IOfferUnit>();

            #region Expected user offer

            var expectedUserOffer = new UserOfferExtended()
            {
                User = new User()
                {
                   Bik = 123456789,
                   UserID = 1,
                   UserName = this.UserName
                },
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
                    Text = "Bla bla bla." + Environment.NewLine + "Bla bla bla. Bik is {bik}.",
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

            userOfferUnitMock.Setup(a => a.GetLatestUserOfferByUserID(It.IsAny<int>(), It.IsAny<OfferTypeEnum>()))
                .Returns(expectedUserOffer);

            OfferController controller = GetController<OfferController>(new OfferController(userOfferUnitMock.Object, offerUnitMock.Object));

            ViewResult result = controller.Offer(OfferTypeEnum.UserOffer) as ViewResult;

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(OfferViewModel));
            Assert.IsNotNull(result.Model);

            OfferViewModel model = result.Model as OfferViewModel;
            Assert.IsTrue(model.Title == "Offer name № 123 от " + DateTime.Now.ToString("dd.MM.yyyy"));
            CollectionAssert.AreEqual(model.Paragraphs, new List<string>() { "Bla bla bla.", "Bla bla bla. Bik is 123456789." });
        }
    }
}
