using JK.Payments.Bacis;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
namespace JK.EntityFrameworkCore.Seed.Host
{
    public class DefaultBanksCreator
    {
        private readonly JKDbContext _context;
        public DefaultBanksCreator(JKDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateBanks();
        }

        private void CreateBanks()
        {
            foreach (var bank in InitialBanks)
            {
                AddBankIfNotExists(bank);
            }
        }
        public static List<Bank> InitialBanks => GetInitialBanks();

        public static List<Bank> GetInitialBanks()
        {
            return new List<Bank>
            {
                new Bank { Name = "农业银行", BankCode = "ABC", OrderNumber = 1 },
                new Bank { Name = "工商银行", BankCode = "ICBC", OrderNumber = 2 },
                new Bank { Name = "建设银行", BankCode = "CCB", OrderNumber = 3 },
                new Bank { Name = "交通银行", BankCode = "BCOM", OrderNumber = 4 },
                new Bank { Name = "中国银行", BankCode = "BOC", OrderNumber = 5 },
                new Bank { Name = "招商银行", BankCode = "CMB", OrderNumber = 6 },
                new Bank { Name = "民生银行", BankCode = "CMBC", OrderNumber = 7 },
                new Bank { Name = "光大银行", BankCode = "CEBB", OrderNumber = 8 },
                new Bank { Name = "中国邮政", BankCode = "PSBC", OrderNumber = 9 },
                new Bank { Name = "平安银行", BankCode = "SPABANK", OrderNumber = 10 },
                new Bank { Name = "中信银行", BankCode = "ECITIC", OrderNumber = 11 },
                new Bank { Name = "广东发展银行", BankCode = "GDB", OrderNumber = 12 },
                new Bank { Name = "华夏银行", BankCode = "HXB", OrderNumber = 13 },
                new Bank { Name = "浦发银行", BankCode = "SPDB", OrderNumber = 14 },
                new Bank { Name = "兴业银行", BankCode = "CIB", OrderNumber = 15 },
                new Bank { Name = "东亚银行", BankCode = "BEA", OrderNumber = 16 },
                new Bank { Name = "河北银行", BankCode = "BHB", OrderNumber = 17 },
                new Bank { Name = "徽商银行", BankCode = "HSBANK", OrderNumber = 18 }
            };
        }

        private void AddBankIfNotExists(Bank bank)
        {
            if (_context.Banks.IgnoreQueryFilters().Any(l => l.BankCode == bank.BankCode))
            {
                return;
            }

            _context.Banks.Add(bank);
            _context.SaveChanges();
        }
    }
}
