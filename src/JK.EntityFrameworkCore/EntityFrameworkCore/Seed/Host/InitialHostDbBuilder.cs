﻿namespace JK.EntityFrameworkCore.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly JKDbContext _context;

        public InitialHostDbBuilder(JKDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();
            new DefaultChannelsCreator(_context).Create();
            new DefaultBanksCreator(_context).Create();
            _context.SaveChanges();
        }
    }
}
