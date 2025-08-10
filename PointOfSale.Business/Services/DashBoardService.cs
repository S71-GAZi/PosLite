using Microsoft.EntityFrameworkCore;
using PointOfSale.Business.Contracts;
using PointOfSale.Data.Repository;
using PointOfSale.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale.Business.Services
{
    public class DashBoardService : IDashBoardService
    {
        private readonly ISaleRepository _repositorySale;
        private readonly IGenericRepository<DetailSale> _repositoryDetailSale;
        private readonly IGenericRepository<Category> _repositoryCategory;
        private readonly IGenericRepository<Product> _repositoryProduct;
        private DateTime StartDate = DateTime.Now;

        public DashBoardService(
            ISaleRepository repositorySale,
            IGenericRepository<DetailSale> repositoryDetailSale,
            IGenericRepository<Category> repositoryCategory,
            IGenericRepository<Product> repositoryProduct
            )
        {

            _repositorySale = repositorySale;
            _repositoryDetailSale = repositoryDetailSale;
            _repositoryCategory = repositoryCategory;
            _repositoryProduct = repositoryProduct;

            StartDate = StartDate.AddDays(-7);

        }
        public async Task<int> TotalSalesLastWeek()
        {
            try
            {
                IQueryable<Sale> query = await _repositorySale.Query(v => v.RegistrationDate.Value.Date >= StartDate.Date);
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> TotalIncomeLastWeek()
        {
            try
            {
                IQueryable<Sale> query = await _repositorySale.Query(v => v.RegistrationDate.Value.Date >= StartDate.Date);

                decimal resultado = query
                    .Select(v => v.Total)
                    .Sum(v => v.Value);

                return Convert.ToString(resultado, new CultureInfo("es-PE"));
            }
            catch
            {
                throw;
            }
        }

        //public async Task<TempDashBoard> ProftLossInformation()
        //{
        //    try
        //    {
        //        TempDashBoard obj = new TempDashBoard();
        //        var today = DateTime.Today;
        //        var currentDayOfWeek = (int)today.DayOfWeek;
        //        var startOfWeek = today.AddDays(-currentDayOfWeek + (currentDayOfWeek == 0 ? -6 : 1)); // Monday
        //        var endOfWeek = startOfWeek.AddDays(6); // Sunday

        //        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        //        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        //        var startOfYear = new DateTime(today.Year, 1, 1);
        //        var endOfYear = new DateTime(today.Year, 12, 31);


        //        IQueryable<Sale> querysale = (IQueryable<Sale>)await _repositorySale.Query(null);
        //        IQueryable<Product> queryProduct = (IQueryable<Product>)await _repositoryProduct.Query(null);

        //        decimal totalSaleWeekly = querysale
        //            .Where(x => x.RegistrationDate >= startOfWeek && x.RegistrationDate <= endOfWeek)
        //            .Select(v => v.Total)
        //            .Sum(v => v.Value);

        //        decimal totalSaleMonthly = querysale
        //            .Where(x => x.RegistrationDate >= startOfMonth && x.RegistrationDate <= endOfMonth)
        //            .Select(v => v.Total)
        //            .Sum(v => v.Value);

        //        decimal totalSaleYearly = querysale
        //            .Where(x => x.RegistrationDate >= startOfYear && x.RegistrationDate <= endOfYear)
        //            .Select(v => v.Total)
        //            .Sum(v => v.Value);
        //        decimal totalSale = querysale
        //            .Select(v => v.Total)
        //            .Sum(v => v.Value);



        //        decimal totalPurchaseWeekly = queryProduct
        //           .Where(x => x.RegistrationDate >= startOfWeek && x.RegistrationDate <= endOfWeek)
        //           .Select(v => v.PurchasePrice * v.Quantity)
        //           .Sum(v => v.Value);

        //        decimal totalPurchaseMonthly = queryProduct
        //            .Where(x => x.RegistrationDate >= startOfMonth && x.RegistrationDate <= endOfMonth)
        //            .Select(v => v.PurchasePrice * v.Quantity)
        //            .Sum(v => v.Value);

        //        decimal totalPurchaseYearly = queryProduct
        //            .Where(x => x.RegistrationDate >= startOfYear && x.RegistrationDate <= endOfYear)
        //            .Select(v => v.PurchasePrice * v.Quantity)
        //            .Sum(v => v.Value);
        //        decimal totalPurchase = queryProduct
        //            .Where(x => x.RegistrationDate >= startOfYear && x.RegistrationDate <= endOfYear)
        //            .Select(v => v.PurchasePrice * v.Quantity)
        //            .Sum(v => v.Value);

        //        obj.WeeklyProfites = totalSaleWeekly - totalPurchaseWeekly;
        //        obj.TotalProfites = totalSaleMonthly - totalPurchaseMonthly;
        //        obj.TotalProfites = totalSaleYearly - totalPurchaseYearly;
        //        obj.TotalProfites = totalSale - totalPurchase;
        //        return obj;
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public async Task<TempDashBoard> ProftLossInformation()
        {
            try
            {
                TempDashBoard obj = new TempDashBoard();
                var today = DateTime.Today;

                // Calculate how many days to subtract to get to the previous Saturday
                var daysSinceSaturday = (7 + (int)today.DayOfWeek - (int)DayOfWeek.Saturday) % 7;

                var startOfWeek = today.AddDays(-daysSinceSaturday); // Saturday
                var endOfWeek = startOfWeek.AddDays(6);              // Friday


                var startOfMonth = new DateTime(today.Year, today.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var startOfYear = new DateTime(today.Year, 1, 1);
                var endOfYear = new DateTime(today.Year, 12, 31);


                IQueryable<Sale> querysale = (IQueryable<Sale>)await _repositorySale.Query(null);
                IQueryable<Product> queryProduct = (IQueryable<Product>)await _repositoryProduct.Query(null);

                decimal totalSaleWeekly = querysale
                    .Where(x => x.RegistrationDate >= startOfWeek && x.RegistrationDate <= endOfWeek)
                    .Select(v => v.Total)
                    .Sum(v => v.Value);

                decimal totalSaleMonthly = querysale
                    .Where(x => x.RegistrationDate >= startOfMonth && x.RegistrationDate <= endOfMonth)
                    .Select(v => v.Total)
                    .Sum(v => v.Value);

                decimal totalSaleYearly = querysale
                    .Where(x => x.RegistrationDate >= startOfYear && x.RegistrationDate <= endOfYear)
                    .Select(v => v.Total)
                    .Sum(v => v.Value);
                decimal totalSale = querysale
                    .Select(v => v.Total)
                    .Sum(v => v.Value);



                decimal totalPurchaseWeekly = queryProduct
                   .Where(x => x.RegistrationDate >= startOfWeek && x.RegistrationDate <= endOfWeek)
                   .Select(v => v.PurchasePrice * v.Quantity)
                   .Sum(v => v.Value);

                decimal totalPurchaseMonthly = queryProduct
                    .Where(x => x.RegistrationDate >= startOfMonth && x.RegistrationDate <= endOfMonth)
                    .Select(v => v.PurchasePrice * v.Quantity)
                    .Sum(v => v.Value);

                decimal totalPurchaseYearly = queryProduct
                    .Where(x => x.RegistrationDate >= startOfYear && x.RegistrationDate <= endOfYear)
                    .Select(v => v.PurchasePrice * v.Quantity)
                    .Sum(v => v.Value);
                decimal totalPurchase = queryProduct
                    .Where(x => x.RegistrationDate >= startOfYear && x.RegistrationDate <= endOfYear)
                    .Select(v => v.PurchasePrice * v.Quantity)
                    .Sum(v => v.Value);

                obj.WeeklyProfites = totalSaleWeekly - totalPurchaseWeekly;
                obj.MonthlyProfites = totalSaleMonthly - totalPurchaseMonthly;
                obj.YearlyProfites = totalSaleYearly - totalPurchaseYearly;
                obj.TotalProfites = totalSale - totalPurchase;
                return obj;
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> TotalProducts()
        {
            try
            {
                IQueryable<Product> query = await _repositoryProduct.Query();
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }
        public async Task<int> TotalCategories()
        {
            try
            {
                IQueryable<Category> query = await _repositoryCategory.Query();
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }
        public async Task<Dictionary<string, int>> SalesLastWeek()
        {
            try
            {
                IQueryable<Sale> query = await _repositorySale.Query(v => v.RegistrationDate.Value.Date >= StartDate.Date);

                Dictionary<string, int> resultado = query
                    .GroupBy(v => v.RegistrationDate.Value.Date).OrderByDescending(g => g.Key)
                    .Select(dv => new { date = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                    .ToDictionary(keySelector: r => r.date, elementSelector: r => r.total);

                return resultado;

            }
            catch
            {
                throw;
            }
        }
        public async Task<Dictionary<string, int>> ProductsTopLastWeek()
        {
            try
            {
                IQueryable<DetailSale> query = await _repositoryDetailSale.Query();

                Dictionary<string, int> resultado = query
                    .Include(v => v.IdSaleNavigation)
                    .Where(dv => dv.IdSaleNavigation.RegistrationDate.Value.Date >= StartDate)
                    .GroupBy(dv => dv.DescriptionProduct).OrderByDescending(g => g.Count())
                    .Select(dv => new { product = dv.Key, total = dv.Count() }).Take(4)
                    .ToDictionary(keySelector: r => r.product, elementSelector: r => r.total);

                return resultado;
            }
            catch
            {
                throw;
            }
        }


    }
}
