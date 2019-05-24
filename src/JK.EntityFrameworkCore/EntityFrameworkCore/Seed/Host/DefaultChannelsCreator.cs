using System.Collections.Generic;
using System.Linq;
using JK.Payments.Bacis;
using Microsoft.EntityFrameworkCore;

namespace JK.EntityFrameworkCore.Seed.Host
{
    public class DefaultChannelsCreator
    {
        public static List<Channel> InitialChannels => GetInitialChannels();
        private readonly JKDbContext _context;
        private static List<Channel> GetInitialChannels()
        {
            return new List<Channel>
            {
                new Channel { Name = "支付宝扫码", DisplayName = "支付宝", IsActive = true, ChannelCode = "ALIPAY" },
                new Channel { Name = "支付宝H5", DisplayName = "支付宝", IsActive = true, ChannelCode = "ALIPAYH5" },

                new Channel { Name = "微信扫码", DisplayName = "微信支付", IsActive = true, ChannelCode = "WECHAT" },
                new Channel { Name = "微信H5", DisplayName = "微信支付", IsActive = true, ChannelCode = "WECHATH5" },

                new Channel { Name = "QQ钱包扫码", DisplayName = "QQ钱包", IsActive = true, ChannelCode = "QPAY" },
                new Channel { Name = "QQ钱包H5", DisplayName = "QQ钱包", IsActive = true, ChannelCode = "QPAYH5" },

                new Channel { Name = "银联扫码", DisplayName = "银联支付", IsActive = true, ChannelCode = "UNIONPAY" },
                new Channel { Name = "银联H5", DisplayName = "银联支付", IsActive = true, ChannelCode = "UNIONPAYH5" },

                new Channel { Name = "京东扫码", DisplayName = "京东钱包", IsActive = true, ChannelCode = "JDPAY" },
                new Channel { Name = "京东H5", DisplayName = "京东钱包", IsActive = true, ChannelCode = "JDPAYH5" },

                new Channel { Name = "快捷支付", DisplayName = "快捷支付", IsActive = true, ChannelCode = "QUICKPAY", }
            };
        }
        public DefaultChannelsCreator(JKDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateChannels();
        }

        private void CreateChannels()
        {
            foreach (var paymentMethod in InitialChannels)
            {
                AddChannelIfNotExists(paymentMethod);
            }
        }
        private void AddChannelIfNotExists(Channel channel)
        {
            if (_context.Channels.IgnoreQueryFilters().Any(l => l.ChannelCode == channel.ChannelCode))
            {
                return;
            }

            _context.Channels.Add(channel);
            _context.SaveChanges();
        }
    }
}