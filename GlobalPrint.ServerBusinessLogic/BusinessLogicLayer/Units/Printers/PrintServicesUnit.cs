using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Models.Domain.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers
{
    public class PrintServicesUnit : BaseUnit
    {
        public PrintServicesUnit()
            : base()
        {
        }
        
        /// <summary> Get all possible print services of the system.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PrintServiceExtended> GetPrintServices()
        {
            using (IDataContext context = this.Context())
            {
                IPrintServiceRepository printServiceRepo = this.Repository<IPrintServiceRepository>(context);
                IPrintSizePrintTypeRepository printSizePrintTypeRepo = this.Repository<IPrintSizePrintTypeRepository>(context);
                IPrintSizeRepository printSizeRepo = this.Repository<IPrintSizeRepository>(context);
                IPrintTypeRepository printTypeRepo = this.Repository<IPrintTypeRepository>(context);

                var queryableServices = 
                    from service in printServiceRepo.GetAll()
                    join sizeType in printSizePrintTypeRepo.GetAll() on service.PrintSizePrintTypeID equals sizeType.PrintSizePrintTypeID
                    join size in printSizeRepo.GetAll() on sizeType.PrintSizeID equals size.PrintSizeID
                    join type in printTypeRepo.GetAll() on sizeType.PrintTypeID equals type.PrintTypeID
                    select new { service = service, sizeType = sizeType, size = size, type = type };


                return queryableServices.ToList().Select(e => new PrintServiceExtended(e.service, e.sizeType, e.size, e.type));
            }
        }
        
        public IEnumerable<PrintSize> GetPrintSizes()
        {
            using (IDataContext context = this.Context())
            {
                IPrintSizeRepository printSizeRepo = this.Repository<IPrintSizeRepository>(context);
                return printSizeRepo.GetAll().ToList();
            }
        }
        public IEnumerable<PrintType> GetPrintTypes()
        {
            using (IDataContext context = this.Context())
            {
                IPrintTypeRepository printTypeRepo = this.Repository<IPrintTypeRepository>(context);
                return printTypeRepo.GetAll().ToList();
            }
        }
        
    }

}
