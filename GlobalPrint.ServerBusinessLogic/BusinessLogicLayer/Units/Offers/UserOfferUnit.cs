using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Offers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Offers;
using System;
using System.Diagnostics;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Offers
{
    /// <summary>
    /// User offer business logic unit.
    /// </summary>
    public class UserOfferUnit : BaseUnit
    {
        [DebuggerStepThrough]
        public UserOfferUnit()
            : base()
        {
        }

        /// <summary>
        /// Save user offer. Calls <see cref="CreateUserOffer"/> or <see cref="EditUserOffer"/>
        /// </summary>
        /// <param name="model">User offer object to save.</param>
        /// <returns>Saved user offer with correct ID.</returns>
        public UserOffer SaveUserOffer(UserOffer model)
        {
            bool isEdit = (model?.ID ?? 0) > 0;
            if (isEdit)
            {
                return this.EditUserOffer(model);
            }
            else
            {
                return this.CreateUserOffer(model);
            }
        }

        /// <summary>
        /// Perform validation checks of model. Usually used before saving method.
        /// </summary>
        /// <param name="model">Object to check.</param>
        private void _ValidateUserOffer(UserOffer model)
        {
            Argument.NotNull(model, "Договор оферты не может быть пустым.");
            bool isEdit = (model?.ID ?? 0) > 0;
            Argument.Positive(model.UserID, "Пользователь не может быть пустым.");
            Argument.Positive(model.OfferID, "Договор оферты не может быть пустым.");
            Argument.Require(model.OfferDate != default(DateTime), "Дата договора оферты не может быть пустой.");
            if (isEdit)
            {
                Argument.NotNullOrWhiteSpace(model.OfferNumber, "Номер договора оферты не может быть пустым.");
            }
        }

        /// <summary>
        /// Insert new user ofer.
        /// </summary>
        /// <param name="model">User offer model.</param>
        /// <returns>Saved user offer with correct ID.</returns>
        public UserOffer CreateUserOffer(UserOffer model)
        {
            this._ValidateUserOffer(model);

            using (IDataContext context = this.Context())
            {
                IUserOfferRepository userOfferRepo = this.Repository<IUserOfferRepository>(context);

                userOfferRepo.Insert(model);
                context.Save();
                
                return model;
            }
        }

        /// <summary>
        /// Create new user offer from registration
        /// </summary>
        /// <param name="userID">User identidier.</param>
        /// <param name="offerTypeID">Offer type identifier.</param>
        /// <returns>Created user offer with correct ID.</returns>
        public UserOffer CreateUserOffer(int userID, OfferTypeEnum offerTypeID)
        {
            using (IDataContext context = this.Context())
            {
                IUserOfferRepository userOfferRepo = this.Repository<IUserOfferRepository>(context);
                OfferUnit offerUnit = new OfferUnit();

                Offer latestOffer = offerUnit.GetLatestOfferByType(OfferTypeEnum.UserOffer, context);
                if (latestOffer == null)
                {
                    throw new Exception("Не найден договор оферты пользователя.");
                }

                UserOffer model = new UserOffer()
                {
                    UserID = userID,
                    OfferID = latestOffer.ID,
                    OfferDate = DateTime.Now,
                    OfferNumber = null
                };

                context.BeginTransaction();
                try
                {
                    // Insert new offer
                    this._ValidateUserOffer(model);
                    userOfferRepo.Insert(model);
                    context.Save();

                    // Update offer number
                    model.OfferNumber = model.ID.ToString();
                    this._ValidateUserOffer(model);
                    userOfferRepo.Update(model);
                    context.Save();

                    context.CommitTransaction();
                }
                catch (Exception)
                {
                    context.RollbackTransaction();
                    throw;
                }

                return model;
            }
        }

        /// <summary>
        /// Edit existing user ofer.
        /// </summary>
        /// <param name="model">User offer model.</param>
        /// <returns>Saved user offer with correct ID.</returns>
        public UserOffer EditUserOffer(UserOffer model)
        {
            this._ValidateUserOffer(model);

            using (IDataContext context = this.Context())
            {
                IUserOfferRepository userOfferRepo = this.Repository<IUserOfferRepository>(context);

                userOfferRepo.Update(model);
                context.Save();

                return model;
            }
        }

        /// <summary>
        /// Delete all offers of specified user
        /// </summary>
        /// <param name="userID">User Identifier</param>
        public void DeleteUserOfferByUserID(int userID)
        {
            using (IDataContext context = this.Context())
            {
                IUserOfferRepository userOfferRepo = this.Repository<IUserOfferRepository>(context);
                var userOfferList = userOfferRepo.Get(e => e.UserID == userID);
                userOfferRepo.Delete(userOfferList);
            }
        }
    }
}
