using GlobalPrint.Infrastructure.CommonUtils;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.DataContext;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Orders;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Printers;
using GlobalPrint.ServerBusinessLogic._IDataAccessLayer.Repository.Users;
using GlobalPrint.ServerBusinessLogic.Models.Business.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Business.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Orders;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Printers;
using GlobalPrint.ServerBusinessLogic.Models.Domain.Users;
using GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace GlobalPrint.ServerBusinessLogic.BusinessLogicLayer.Units.Printers
{
    public class PrintServicesUnit : BaseUnit
    {
        [DebuggerStepThrough]
        public PrintServicesUnit()
            : base()
        {
        }

        /// <summary> Get print services of the system by its identifier.
        /// </summary>
        /// <param name="printServiceID">Identifier of the print service.</param>
        /// <returns>Found print service or null if it not exists.</returns>
        public PrintServiceExtended GetPrintServiceByID(int printServiceID)
        {
            return this.GetPrintServices(e => e.PrintService.ID == printServiceID)
                .FirstOrDefault();
        }

        /// <summary> Get print services of the system by its identifier.
        /// </summary>
        /// <param name="printServiceID">Identifier of the print service.</param>
        /// <param name="context">Existing data context.</param>
        /// <returns>Found print service or null if it not exists.</returns>
        public PrintServiceExtended GetPrintServiceByID(int printServiceID, IDataContext context)
        {
            return this.GetPrintServices(e => e.PrintService.ID == printServiceID, context)
                .FirstOrDefault();
        }

        /// <summary>
        /// Get print services of the system by certain criteria.
        /// </summary>
        /// <param name="predicate">Criteria for print services.</param>
        /// <returns></returns>
        public IEnumerable<PrintServiceExtended> GetPrintServices(Expression<Func<PrintServiceExtended, bool>> predicate = null)
        {
            using (IDataContext context = this.Context())
            {
                return this.GetPrintServices(predicate, context);
            }
        }

        /// <summary>
        /// Get print services of the system by certain criteria. Requires data context.
        /// </summary>
        /// <param name="predicate">Criteria for print services.</param>
        /// <param name="context">Existing data context.</param>
        /// <returns>List of matching print services.</returns>
        public IEnumerable<PrintServiceExtended> GetPrintServices(Expression<Func<PrintServiceExtended, bool>> predicate, IDataContext context)
        {
            Argument.NotNull(context, "Контекст данных пустой.");

            IQueryable<PrintServiceExtended> services = this.PrintServices(context);
            if (predicate != null)
            {
                services = services.Where(predicate);
            }
            return services.ToList();
        }

        /// <summary> Get printer services by certain condition.
        /// </summary>
        /// <param name="predicate">Filtering predicate.</param>
        /// <returns></returns>
        public IEnumerable<PrinterServiceExtended> GetPrinterServices(Expression<Func<PrinterServiceExtended, bool>> predicate = null)
        {
            using (IDataContext context = this.Context())
            {
                IQueryable<PrinterServiceExtended> services = this.PrinterServices(context);
                if (predicate != null)
                {
                    services = services.Where(predicate);
                }
                return services.ToList();
            }
        }

        /// <summary> Get printer services of certain printer.
        /// </summary>
        /// <param name="printerID">Identifier of the printer.</param>
        /// <returns></returns>
        public IEnumerable<PrinterServiceExtended> GetPrinterServices(int printerID)
        {
            return GetPrinterServices(e => e.PrinterService.PrinterID == printerID);
        }

        /// <summary> Create Queryable for print services with all the text values for IDs.
        /// </summary>
        /// <param name="context">Data context with connection to the data source.</param>
        /// <returns></returns>
        public IQueryable<PrintServiceExtended> PrintServices(IDataContext context)
        {
            IPrintServiceRepository printServiceRepo = this.Repository<IPrintServiceRepository>(context);
            IPrintSizePrintTypeRepository printSizePrintTypeRepo = this.Repository<IPrintSizePrintTypeRepository>(context);
            IPrintSizeRepository printSizeRepo = this.Repository<IPrintSizeRepository>(context);
            IPrintTypeRepository printTypeRepo = this.Repository<IPrintTypeRepository>(context);

            IQueryable<PrintServiceExtended> queryableServices =
                from service in printServiceRepo.GetAll()
                join sizeType in printSizePrintTypeRepo.GetAll() on service.PrintSizePrintTypeID equals sizeType.PrintSizePrintTypeID
                join size in printSizeRepo.GetAll() on sizeType.PrintSizeID equals size.ID
                join type in printTypeRepo.GetAll() on sizeType.PrintTypeID equals type.ID
                select new PrintServiceExtended() { PrintService = service, PrintSizePrintType = sizeType, PrintSize = size, PrintType = type };

            return queryableServices;
        }

        /// <summary> Create Queryable for printER services with all the text values for IDs. 
        /// Printer services are print services in terms of a certain printer. 
        /// It holds prices for the print service provided by the printer.
        /// ACHTUNG! Not PrintServices!
        /// </summary>
        /// <param name="context">Data context with connection to the data source.</param>
        /// <returns></returns>
        public IQueryable<PrinterServiceExtended> PrinterServices(IDataContext context)
        {
            IPrinterServiceRepository printerServiceRepo = this.Repository<IPrinterServiceRepository>(context);

            IQueryable<PrinterServiceExtended> queryableServices =
                from printerService in printerServiceRepo.GetAll()
                join service in this.PrintServices(context) on printerService.PrintServiceID equals service.PrintService.ID
                select new PrinterServiceExtended() { PrintService = service, PrinterService = printerService };

            return queryableServices;
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
